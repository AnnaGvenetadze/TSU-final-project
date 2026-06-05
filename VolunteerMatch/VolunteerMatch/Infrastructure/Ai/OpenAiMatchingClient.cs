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

            var prompt = AiPromptBuilder.CreatePromptForVolunteer(
                request,
                JsonOptions);

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



        public async Task<AiBatchResponseForEventDto> GetMatchedVolunteerIdsForEventAsync(
            AiBatchRequestForEventDto request,
            CancellationToken cancellationToken = default)
        {
            ValidateRequest(request);

            if (request.Volunteers.Count == 0)
            {
                return new AiBatchResponseForEventDto();
            }

            var prompt = AiPromptBuilder.CreatePromptForEvent(
                request,
                JsonOptions);

            var responseContent = await SendRequestAsync(
                prompt,
                cancellationToken);

            var result = DeserializeResponseForEvent(responseContent);

            EnsureValidMatchedVolunteerIds(result, request.Volunteers);

            return result;
        }



        private static void ValidateRequest(AiBatchRequestForEventDto request)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Event);
            ArgumentNullException.ThrowIfNull(request.Volunteers);
        }



        private static AiBatchResponseForEventDto DeserializeResponseForEvent(
            string? content)
        {
            if (string.IsNullOrWhiteSpace(content))
            {
                throw new AiMatchingException("AI მეჩინგის პასუხი ცარიელია.");
            }

            try
            {
                var result = JsonSerializer.Deserialize<AiBatchResponseForEventDto>(
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



        // AI-ის დაბრუნებული matchedVolunteerIds ნამდვილად იყო თუ არა
        // request-ში გაგზავნილ Volunteers სიაში.
        // თუ AI-მ გამოგონილი ID დააბრუნა, პროგრამას არ ვაგდებთ;
        // ვლოგავთ warning-ს და ვტოვებთ მხოლოდ ნამდვილ IDs-ს.
        private void EnsureValidMatchedVolunteerIds(
            AiBatchResponseForEventDto result,
            List<AiVolunteerInfoForEventDto> providedVolunteers)
        {
            if (result.MatchedVolunteerIds is null)
            {
                throw new AiMatchingException(
                    "AI მეჩინგის პასუხში matchedVolunteerIds ველი არასწორია.");
            }

            var providedVolunteerIds = providedVolunteers
                .Select(volunteer => volunteer.VolunteerId)
                .ToHashSet();

            var invalidVolunteerIds = result.MatchedVolunteerIds
                .Where(volunteerId => !providedVolunteerIds.Contains(volunteerId))
                .Distinct()
                .ToList();

            if (invalidVolunteerIds.Count != 0)
            {
                _logger.LogWarning(
                    "AI-მ დააბრუნა ისეთი VolunteerId-ები, რომლებიც მოთხოვნაში არ იყო გაგზავნილი. InvalidVolunteerIds: {InvalidVolunteerIds}",
                    string.Join(", ", invalidVolunteerIds));
            }

            result.MatchedVolunteerIds = result.MatchedVolunteerIds
                .Where(providedVolunteerIds.Contains)
                .Distinct()
                .ToList();
        }
    }
}