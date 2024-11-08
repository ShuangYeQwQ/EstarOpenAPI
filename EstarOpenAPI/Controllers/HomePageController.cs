
using Application.RequestModel;
using EstarOpenAPI.Application.Interfaces;
using EstarOpenAPI.Application.RequestModel.HomePage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EstarOpenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomePageController : ControllerBase
    {
        

        private readonly ILogger<HomePageController> _logger;
        private readonly IHomePageService _homepageService;

        public HomePageController(ILogger<HomePageController> logger, IHomePageService homePageService)
        {
            _logger = logger;
            _homepageService = homePageService;
        }
        //[Authorize]
        [HttpPost("register")]
        public async Task<IActionResult> RegisterAsync([FromBody] common_req<Signup_req> signup_req)
        {
            return Ok(await _homepageService.RegisterAsync(signup_req));
        }
        [HttpPost("sendverificationemail")]
        public async Task<IActionResult> SendVerificationEmailAsync([FromBody] common_req<Signup_req> signup_req)
        {
            return Ok(await _homepageService.SendVerificationEmailAsync(signup_req));
        }
        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] common_req<Signup_req> signup_req)
        {
            return Ok(await _homepageService.LoginAsync(signup_req));
        }
    }
}
