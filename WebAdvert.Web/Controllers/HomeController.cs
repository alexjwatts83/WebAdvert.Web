using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Home;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _imageBucket;
        private readonly IFileUploader _fileUploader;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IFileUploader fileUploader)
        {
            _logger = logger;
            _configuration = configuration;
            _fileUploader = fileUploader;
            _imageBucket = _configuration.GetValue<string>("ImageBucket");
        }

        // TODO: figure out why it always goes back to the login page even when logged it
        //[Authorize]
        public async Task<IActionResult> Index()
        {
            var doesImageBucketExists = await _fileUploader.CheckHealthAsync();

            var model = new HomeViewModel()
            {
                ImageBucket = _imageBucket,
                DoesImageBucketExists = doesImageBucketExists
            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
