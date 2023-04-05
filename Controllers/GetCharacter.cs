using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace API_assignment.Controllers
{
    [ApiController]
    [Route("api/v1/person")]
    public class CheckCharacter : ControllerBase
    {
        private readonly string _baseUrl = "https://rickandmortyapi.com/api/";

        [HttpGet]
        public async Task<IActionResult> GetCharacter(string name)
        {
            var characterUrl = $"{_baseUrl}/character/?name={name}";
            var httpClient = new HttpClient();
            var characterResponse = await httpClient.GetAsync(characterUrl);

            if (!characterResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var characterContent = await characterResponse.Content.ReadAsStringAsync();
            var characterJson = JsonDocument.Parse(characterContent);
            var results = characterJson.RootElement.GetProperty("results")[0];

            var id = results.GetProperty("id").GetInt32();
            var dimension = results.GetProperty("name").GetString();
            var status = results.GetProperty("status").GetString();
            var species = results.GetProperty("species").GetString();
            var type = results.GetProperty("type").GetString();
            var gender = results.GetProperty("gender").GetString();
            var origin = results.GetProperty("origin");
            var originName = origin.GetProperty("name").GetString();
            var originUrl = origin.GetProperty("url").GetString();

            var result = new
            {
                id,
                dimension,
                status,
                species,
                type,
                gender,
                origin = new { dimension = originName, url = originUrl }
            };

            return Ok(result);
        }
    }
}
