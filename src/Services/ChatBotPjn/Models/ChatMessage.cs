using System.Text.Json.Serialization;

namespace ChatBotPjn.Models
{
    public record ChatMessage([property: JsonPropertyName("role")] string Role,
                       [property: JsonPropertyName("content")] string Content);

}
