using Microsoft.AspNetCore.Mvc;
using url_shortening_apis.Models;
using url_shortening_apis.Service;

namespace url_shortening_apis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlShortenerController : Controller
    {

        private readonly IUrlShorteningService _urlShorteningService;
        public UrlShortenerController(IUrlShorteningService urlShorteningService)
        {
            _urlShorteningService = urlShorteningService;
        }


        [HttpPost]
        public IActionResult ShortenUrl([FromBody] UrlModel sampleURL)
        {

            if (sampleURL == null)
                return BadRequest("Bad user data");

            if (string.IsNullOrEmpty(sampleURL.OriginalUrl))
                return BadRequest("Invalid OriginalUrl");
            try
            {
                string shortUrl = _urlShorteningService.ShortenUrl(sampleURL).Result;

                if (!string.IsNullOrEmpty(shortUrl))
                    return Ok(shortUrl);
                else
                    return BadRequest("Bad user data");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("api/{hashCode}")]
        public IActionResult GetFullUrl(string hashCode)
        {
            if (string.IsNullOrEmpty(hashCode))
                return BadRequest("Bad user input");

            string FullUrl = _urlShorteningService.GetOriginalUrl(hashCode).Result;

            if (!string.IsNullOrEmpty(FullUrl))
                return Ok(FullUrl);
            else
                return BadRequest("Input short url was not found");
        }

    }
}
