using System.Text.Json;
using OpenAI.Chat;
using VolunteerMatch.Application.Dtos;

namespace VolunteerMatch.Infrastructure.Ai
{
    public class OpenAiMatchingClient // : IAiMatchingClient
    {
        private readonly ChatClient _chatClient;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            WriteIndented = true,
            PropertyNameCaseInsensitive = true
        };

        public OpenAiMatchingClient(IConfiguration configuration)
        {
            var apiKey = configuration["OpenAI:ApiKey"];
            var model = configuration["OpenAI:Model"] ?? "gpt-4o-mini";

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new InvalidOperationException("OpenAI API key is not configured.");
            }

            _chatClient = new ChatClient(model: model, apiKey: apiKey);
        }

        public async Task<AiBatchResponseDto> GetMatchedEventIdsForVolunteerAsync(
            AiBatchRequestDto request,
            CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Volunteer);
            ArgumentNullException.ThrowIfNull(request.Events);
            if (request.Events.Count == 0)
            {
                return new AiBatchResponseDto();
            } 
                

            var prompt = CreatePromptForVolunteer(request);
            var messages = new List<ChatMessage>
                {
                    new SystemChatMessage(
                        "You are a volunteer-event matching assistant. " +
                        "Return only valid JSON. Do not include markdown, explanation, comments, or extra text."),

                    new UserChatMessage(prompt)
                };
            var response = await _chatClient.CompleteChatAsync(
                messages,
                cancellationToken: cancellationToken);

            var content = response.Value.Content[0].Text;

            if (string.IsNullOrWhiteSpace(content))
            {
                return new AiBatchResponseDto();
            }

            var result = JsonSerializer.Deserialize<AiBatchResponseDto>(
                content,
                JsonOptions);

            if (result is null)
            {
                return new AiBatchResponseDto();
            }

            result.MatchedEventIds ??= new List<Guid>();

            var providedEventIds = request.Events
                .Select(e => e.EventId)
                .ToHashSet();

            result.MatchedEventIds = result.MatchedEventIds
                .Where(providedEventIds.Contains)
                .Distinct()
                .ToList();

            return result;
        }

        private static string CreatePromptForVolunteer(AiBatchRequestDto request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Volunteer);
            ArgumentNullException.ThrowIfNull(request.Events);

            var volunteerJson = JsonSerializer.Serialize(request.Volunteer, JsonOptions);
            var eventsJson = JsonSerializer.Serialize(request.Events, JsonOptions);

            return $$"""
You are a volunteer-event matching assistant.

Your task:
Compare the volunteer with each event using only the provided fields.
Return only the IDs of events that are strong matches for the volunteer.

Volunteer JSON:
{{volunteerJson}}

Events JSON:
{{eventsJson}}

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
- Do not return an event only because the interests match.
- Return an event only when the volunteer's skills reasonably fit the event requirements.
- Do not invent skills, interests, requirements, tags, or event IDs.
- Do not return event IDs that were not provided in Events JSON.
- If the provided information is missing, vague, or weak, do not include that event.
- If no events match, return an empty array.
- Return only valid JSON.
- Do not include markdown, explanation, comments, or extra text.

Required JSON response format:
{
  "matchedEventIds": [
    "event-id-here"
  ]
}
""";
        }
    }
}