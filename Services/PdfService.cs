using System.Net.Http;
using System.Text;
using System.Text.Json;
using UglyToad.PdfPig;

namespace Back_HR.Services
{
    public class PdfService
    {
        public string ExtractTextFromPdf(string filePath)
        {
            var text = new StringBuilder();
            using var document = PdfDocument.Open(filePath);
            foreach (var page in document.GetPages())
            {
                text.AppendLine(page.Text);
            }
            return text.ToString();
        }
        public async Task<float[]> GetEmbedding(string text)
        {
            var request = new
            {
                model = "nomic-embed-text",
                prompt = text
            };

            var response = await _httpClient.PostAsJsonAsync("api/embeddings", request);
            var json = await response.Content.ReadFromJsonAsync<JsonDocument>();
            return json.RootElement.GetProperty("embedding").EnumerateArray().Select(x => x.GetSingle()).ToArray();
        }

    }

}
