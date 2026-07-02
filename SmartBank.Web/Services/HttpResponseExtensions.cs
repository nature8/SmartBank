using System.Text.Json;

namespace SmartBank.Web.Services
{
    public static class HttpResponseExtensions
    {
        /// <summary>
        /// The backend services return errors in a few different shapes:
        ///   - BadRequest(ModelState)                 -> { "Field": ["error1","error2"], ... }
        ///   - Unauthorized(new { Message = "..." })  -> { "message": "..." }
        ///   - NotFound("some string")                -> "some string"
        ///   - BadRequest("some string")               -> "some string"
        /// This tries each shape and falls back to the raw body / status code.
        /// </summary>
        public static async Task<string> ReadErrorMessageAsync(this HttpResponseMessage response)
        {
            var body = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(body))
            {
                return $"Request failed with status code {(int)response.StatusCode} ({response.ReasonPhrase}).";
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("message", out var msgProp) ||
                        root.TryGetProperty("Message", out msgProp))
                    {
                        return msgProp.GetString() ?? body;
                    }

                    if (root.TryGetProperty("title", out var titleProp))
                    {
                        var messages = new List<string> { titleProp.GetString() ?? "Validation error." };

                        if (root.TryGetProperty("errors", out var errorsProp) &&
                            errorsProp.ValueKind == JsonValueKind.Object)
                        {
                            foreach (var field in errorsProp.EnumerateObject())
                            {
                                foreach (var err in field.Value.EnumerateArray())
                                {
                                    messages.Add(err.GetString() ?? string.Empty);
                                }
                            }
                        }

                        return string.Join(" ", messages.Where(m => !string.IsNullOrWhiteSpace(m)));
                    }

                    // Raw ModelState dictionary: { "Field": ["error1"] }
                    var fieldErrors = new List<string>();
                    foreach (var field in root.EnumerateObject())
                    {
                        if (field.Value.ValueKind == JsonValueKind.Array)
                        {
                            foreach (var err in field.Value.EnumerateArray())
                            {
                                fieldErrors.Add(err.GetString() ?? string.Empty);
                            }
                        }
                    }

                    if (fieldErrors.Count > 0)
                    {
                        return string.Join(" ", fieldErrors.Where(m => !string.IsNullOrWhiteSpace(m)));
                    }
                }
                else if (root.ValueKind == JsonValueKind.String)
                {
                    return root.GetString() ?? body;
                }
            }
            catch (JsonException)
            {
                // Not JSON - treat as plain text (e.g. NotFound("Account not found."))
                return body.Trim('"');
            }

            return body;
        }
    }
}
