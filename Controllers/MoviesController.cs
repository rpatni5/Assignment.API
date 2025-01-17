using Assignment.Common;
using Assignment.Contracts.Services;
using Assignments.Utilities.Models;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/[controller]/v1")]
    public class MoviesController : CustomApiControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IMovieService _movieService;
        private readonly string _apiKey;


        public MoviesController(ILogger<MoviesController> logger, IMovieService movieService, IConfiguration configuration) : base(logger)
        {
            _configuration = configuration;
            _movieService = movieService;
            _apiKey = _configuration.GetSection("AppKey").Value;

        }


        [HttpGet]
        public async Task<IActionResult> GetMovies([FromQuery(Name = "s")] string? title, [FromQuery(Name = "apikey")] string apiKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(apiKey) || apiKey != _apiKey)
                {
                    return Unauthorized(new { Message = "Invalid or missing API key." });
                }

                var baseUrl = $"{Request.Scheme}://{Request.Host}";

                var movies = await _movieService.GetMovies(title, baseUrl);

                return HandleResponse(movies);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

    }
}
