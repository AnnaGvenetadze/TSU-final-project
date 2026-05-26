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
            var model = configuration["OpenAI:Model"] ?? "gpt-5.4-mini";

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

                var responseText = response.Value.Content.FirstOrDefault()?.Text;

                _logger.LogInformation(
                    "OpenAI raw matching response: {ResponseText}",
                    responseText);

                return responseText;
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "OpenAI request failed while generating volunteer matches.");

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
Return event IDs of every event that is a strong match for the volunteer.

Volunteer JSON:
{{volunteerJson}}

Events JSON:
{{eventsJson}}

Context:
- These events were already pre-filtered by shared volunteer interests/tags.
- Shared tags only mean the event is a candidate.
- Shared tags do not automatically mean the event is a strong match.
- You must still evaluate whether the volunteer's skills fit the event requirements.

Matching rules:
- Internally score each event from 0 to 100.
- Evaluate each event independently against the volunteer.
- Do not compare events to each other.
- Do not select an event just because it is better than the other provided events.
- The score of an event must be based only on that event's fit with the volunteer, not on the quality of the other events in the list.
- The score of an event should stay consistent whether the event is evaluated alone or together with other events.
- If an event would score below 70 by itself, do not return it even if all other events are weaker.
- Return every event ID whose score is 70 or higher.
- Do not limit the answer to only the best few events.
- 0-39 means weak match.
- 40-69 means possible but not strong enough.
- 70-84 means good match.
- 85-100 means excellent match.

Scoring priorities:
- Event requirements and volunteer skills are the most important factor.
- Volunteer interests and event tags are secondary.
- A strong match usually requires at least one clear skill match between volunteer skills and event requirements.
- Prefer events where the requirements explicitly mention the volunteer's skills, a close synonym, or a practical task that clearly uses those skills.
- If requirements and skills conflict, requirements must win over tags and interests.
- Shared tags can increase confidence only after skill fit is confirmed.
- Shared tags must never compensate for missing required skills.
- Generic skills such as teamwork, communication, responsibility, motivation, or willingness to help are supportive only.
- A generic skill alone must not make an event a strong match.
- For a strong match, the volunteer should satisfy the event's main required skill area, not only generic supporting skills.

Skill matching guidance:
- Treat a skill as matching a requirement when the requirement asks for the same skill, a close synonym, or a practical task that clearly uses that skill.
- Do not require exact word-for-word equality. Georgian wording may vary.
- If a volunteer skill is a general ability and the event requirement describes a practical task that clearly needs that ability, it can count as a match.
- If the event requires a specific professional skill, tool, certification, or domain expertise, the volunteer must explicitly have that skill or a very close equivalent.
- Do not infer specialized skills only from interests or tags.

Rejection rules:
- Do not return an event only because its tags or interests match.
- If the event requires specialized skills that are not present in the volunteer's skills, do not return it.
- If the event requirements ask for specific professional, academic, technical, medical, legal, design, research, analytical, statistical, or tool-based skills, the volunteer must explicitly have those skills or a very close equivalent in Volunteer Skills.
- Do not infer specialized skills from interests, tags, main theme, or general motivation.
- If the event requires research methodology, statistics, academic writing, data analysis, design tools, programming, medical knowledge, legal knowledge, or other specialized expertise, reject it unless those skills are explicitly present in Volunteer Skills.
- If the requirements are vague, generic, or only say that any help/free time is enough, do not treat it as a strong match.
- If the requirements do not clearly need the volunteer's skills, do not return the event.
- If the event's main requirement is event organization, registration, participant coordination, technical support, design, research, programming, legal work, or another specific work area, the volunteer must explicitly have that specific skill or a very close equivalent.
- Do not return an event where the only matching skill is generic teamwork or communication.
- Do not invent skills, interests, requirements, tags, or event IDs.

Event ID rules:
- Return only exact eventId values from the Events JSON.
- Do not return event IDs that were not provided in Events JSON.
- Do not return event titles in matchedEventIds.
- If no events match, return an empty array.

Output rules:
- Return only valid JSON.
- Do not include markdown, explanation, comments, scores, or extra text.

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