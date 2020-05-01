using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WebAdvert.Web.Models;
using WebAdvert.Web.Models.Home;
using WebAdvert.Web.ServiceClients;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _imageBucket;
        private readonly IFileUploader _fileUploader;
        public ISearchApiClient SearchApiClient { get; }
        public IMapper Mapper { get; }
        public IAdvertApiClient ApiClient { get; }

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration configuration,
            IFileUploader fileUploader,
            ISearchApiClient searchApiClient,
            IMapper mapper,
            IAdvertApiClient apiClient)
        {
            _logger = logger;
            _configuration = configuration;
            _fileUploader = fileUploader;
            _imageBucket = _configuration.GetValue<string>("ImageBucket");
            SearchApiClient = searchApiClient;
            Mapper = mapper;
            ApiClient = apiClient;
        }

        [ResponseCache(Duration = 60)]
        public async Task<IActionResult> Index()
        {
            var allAds = await ApiClient.GetAllAsync().ConfigureAwait(false);
            var allViewModels = allAds.Select(x => Mapper.Map<IndexViewModel>(x));

            return View(allViewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Search(string keyword)
        {
            var viewModel = new List<SearchViewModel>();

            var searchResult = await SearchApiClient.Search(keyword).ConfigureAwait(false);
            searchResult.ForEach(advertDoc =>
            {
                var viewModelItem = Mapper.Map<SearchViewModel>(advertDoc);
                viewModel.Add(viewModelItem);
            });

            return View("Search", viewModel);
        }

        // TODO: figure out why it always goes back to the login page even when logged it
        //[Authorize]
        [Route("check")]
        public async Task<IActionResult> CheckBuckert()
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
