using System.Text.Json;

namespace ChatBotPjn.Models
{
  public  record ChatResponse(string text, JsonElement raw);
}
