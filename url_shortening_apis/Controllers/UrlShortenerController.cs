﻿using Microsoft.AspNetCore.Mvc;
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
            if (sampleURL == null || !ModelState.IsValid)
                return BadRequest("Bad user data");

            if (string.IsNullOrEmpty(sampleURL.LongUrl))
                return BadRequest("Invalid OriginalUrl");

            /// we can't set past dates
            if (sampleURL.CreatedDate < DateTime.UtcNow.AddMinutes(-1))
                return BadRequest("Created Date can't be in the past");

            try
            {
                string shortUrl = _urlShorteningService.ShortenUrl(sampleURL).Result;

                if (!string.IsNullOrEmpty(shortUrl))
                {
                    return Ok(shortUrl);
                }
                else
                {
                    return StatusCode(500, "Failed to generate short URL");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("api/{Short_ID}")]
        public IActionResult GetFullUrl(string Short_ID)
        {
            if (string.IsNullOrEmpty(Short_ID))
                return BadRequest("Bad user input");

            try
            {
                var FullUrl = _urlShorteningService.GetOriginalUrl(Short_ID).Result;

                if (FullUrl == null)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, "Url was expired or not found");
                }

                if (!string.IsNullOrEmpty(FullUrl.LongUrl))
                {
                    //if called in browser it will redirtect
                    return Redirect($"{FullUrl.LongUrl}");
                }

                /// bad or invalid data
                return StatusCode(StatusCodes.Status400BadRequest, "Invalid URL");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

    }
}
