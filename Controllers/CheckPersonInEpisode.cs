using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Net.Http;
using System.Threading.Tasks;

namespace API_assignment.Controllers
{

    [ApiController]
    [Route("api/v1/check-person")]
    public class CheckPersonController : ControllerBase
    {
        private readonly string _baseUrl = "https://rickandmortyapi.com/api/";

        [HttpPost]
        public async Task<IActionResult> CheckPersonInEpisode([FromBody] CheckPersonRequest request)
        {
            var characterUrl = $"{_baseUrl}/character/?name={request.PersonName}";
            var httpClient = new HttpClient();
            var characterResponse = await httpClient.GetAsync(characterUrl);

            if (!characterResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var characterContent = await characterResponse.Content.ReadAsStringAsync();
            var characterJson = JsonDocument.Parse(characterContent);
            var characterResults = characterJson.RootElement.GetProperty("results");

            if (characterResults.GetArrayLength() == 0)
            {
                return NotFound();
            }

            var characterId = characterResults[0].GetProperty("id").GetInt32();
            var episodeUrl = $"{_baseUrl}/episode/?name={request.EpisodeName}";
            var episodeResponse = await httpClient.GetAsync(episodeUrl);

            if (!episodeResponse.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var episodeContent = await episodeResponse.Content.ReadAsStringAsync();
            var episodeJson = JsonDocument.Parse(episodeContent);
            var episodeResults = episodeJson.RootElement.GetProperty("results");

            if (episodeResults.GetArrayLength() == 0)
            {
                return NotFound();
            }

            var episodeCharacters = episodeResults[0].GetProperty("characters");

            foreach (var character in episodeCharacters.EnumerateArray())
            {
                var url = character.GetString();

                if (url.Contains(characterId.ToString()))
                {
                    return Ok(true);
                }
            }

            return Ok(false);
        }
    }
    public class CheckPersonRequest
    {
        public string? PersonName { get; set; }
        public string? EpisodeName { get; set; }
    }
}




