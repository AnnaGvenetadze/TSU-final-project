using System.Text.Json;
using OpenAI.Chat;
using VolunteerMatch.Application.Dtos;
using VolunteerMatch.Application.Exceptions;
using VolunteerMatch.Application.Interfaces;

namespace VolunteerMatch.Infrastructure.Ai
{
    public class OpenAiMatchingClient : IAiMatchingClient
    {
        private readonly ChatClient _chatClient;
        private readonly ILogger<OpenAiMatchingClient> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public OpenAiMatchingClient(
            IConfiguration configuration,
            ILogger<OpenAiMatchingClient> logger)
        {
            _logger = logger;

            var apiKey = configuration["OpenAI:ApiKey"];
            var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new AiMatchingException(
                    "OpenAI API გასაღები არაა კონფიგურირებული.");
            }

            _chatClient = new ChatClient(model: model, apiKey: apiKey);
        }

        public async Task<AiBatchResponseDto> GetMatchedEventIdsForVolunteerAsync(
            AiBatchRequestDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateRequest(request);

            if (request.Events.Count == 0)
            {
                return new AiBatchResponseDto();
            }

            var prompt = CreatePromptForVolunteer(request);

            var responseContent = await SendRequestAsync(
                prompt,
                cancellationToken);

            var result = DeserializeResponse(responseContent);

            EnsureValidMatchedEventIds(result, request.Events);

            return result;
        }

        private static void ValidateRequest(AiBatchRequestDto request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Volunteer);
            ArgumentNullException.ThrowIfNull(request.Events);
        }

        private async Task<string?> SendRequestAsync(
            string prompt,
            CancellationToken cancellationToken)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(prompt);

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(
                    "You are a volunteer-event matching assistant. Return only valid JSON. Do not include markdown, explanation, comments, or extra text."),
                new UserChatMessage(prompt)
            };

            try
            {
                var response = await _chatClient.CompleteChatAsync(
                    messages,
                    cancellationToken: cancellationToken);

                return response.Value.Content.FirstOrDefault()?.Text;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new AiMatchingException(
                    "AI მეჩინგის მოთხოვნის გაგზავნისას მოხდა შეცდომა.", ex);
            }
        }

        private static AiBatchResponseDto DeserializeResponse(string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new AiMatchingException("AI მეჩინგის პასუხი ცარიელია.");
            }

            try
            {
                var result = JsonSerializer.Deserialize<AiBatchResponseDto>(
                    content,
                    JsonOptions);

                if (result is null)
                {
                    throw new AiMatchingException(
                        "AI მეჩინგის პასუხი არასწორი ფორმატით დაბრუნდა.");
                }

                return result;
            }
            catch (JsonException ex)
            {
                throw new AiMatchingException(
                    "AI მეჩინგის პასუხი ვერ დაიპარსა.", ex);
            }
        }

        // AI-ის დაბრუნებული matchedEventIds ნამდვილად იყო თუ არა
        // request-ში გაგზავნილ Events სიაში.
        // თუ AI-მ გამოგონილი ID დააბრუნა, პროგრამას არ ვაგდებთ;
        // ვლოგავთ warning-ს და ვტოვებთ მხოლოდ ნამდვილ IDs-ს.
        private void EnsureValidMatchedEventIds(
            AiBatchResponseDto result,
            List<AiEventInfoDto> providedEvents)
        {
            if (result.MatchedEventIds is null)
            {
                throw new AiMatchingException(
                    "AI მეჩინგის პასუხში matchedEventIds ველი არასწორია.");
            }

            var providedEventIds = providedEvents
                .Select(e => e.EventId)
                .ToHashSet();

            var invalidEventIds = result.MatchedEventIds
                .Where(eventId => !providedEventIds.Contains(eventId))
                .Distinct()
                .ToList();

            if (invalidEventIds.Count != 0)
            {
                _logger.LogWarning(
                    "AI-მ დააბრუნა ისეთი EventId-ები, რომლებიც მოთხოვნაში არ იყო გაგზავნილი. InvalidEventIds: {InvalidEventIds}",
                    string.Join(", ", invalidEventIds));
            }

            result.MatchedEventIds = result.MatchedEventIds
                .Where(providedEventIds.Contains)
                .Distinct()
                .ToList();
        }

        private static string CreatePromptForVolunteer(AiBatchRequestDto request)
        {
            ArgumentNullException.ThrowIfNull(request);

            var volunteerJson = JsonSerializer.Serialize(
                request.Volunteer, JsonOptions);

            var eventsJson = JsonSerializer.Serialize(
                request.Events, JsonOptions);

            return $$"""
You are a volunteer-event matching assistant.

Your task:
Compare one volunteer with each event using only the provided fields.
Return only the IDs of events that are strong matches for the volunteer.

Volunteer JSON:
{{volunteerJson}}

Events JSON:
{{eventsJson}}

Context:
- These events were already pre-filtered by shared volunteer interests/tags.
- You must still be strict and return only events that are truly strong matches.

Matching rules:
- Internally score each event from 0 to 100.
- Return an event ID only if its score is 70 or higher.
- 0-39 means weak match.
- 40-69 means possible but not strong enough.
- 70-84 means good match.
- 85-100 means excellent match.

Important rules:
- Skills and event requirements are the most important factor.
- Interests are secondary, but they should match the event main theme or tags.
- If volunteer skills do not reasonably satisfy the event requirements, do not return that event even if interests or tags match.
- Do not return an event only because the interests match.
- Do not invent skills, interests, requirements, tags, or event IDs.
- Return only exact eventId values from the Events JSON.
- Do not return event IDs that were not provided in Events JSON.
- If the provided information is missing, vague, or weak, do not include that event.
- If no events match, return an empty array.
- Return only valid JSON.
- Do not include markdown, explanation, comments, or extra text.

Required JSON response format:
{
  "matchedEventIds": [
    "00000000-0000-0000-0000-000000000000"
  ]
}
""";
        }
    }
}