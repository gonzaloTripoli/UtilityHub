using System.Text.Json.Serialization;

namespace ChatBotPjn.Models
{
   public record ChatRequest(
        [property: JsonPropertyName("messages")] List<ChatMessage> Messages,
        [property: JsonPropertyName("model")] string? Model = null,
        [property: JsonPropertyName("temperature")] float? Temperature = 0.2f,
        [property: JsonPropertyName("max_tokens")] int? MaxTokens = 800
    );
}
