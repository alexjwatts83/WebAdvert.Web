using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAdvert.Web.Models.AdvertManagement;
using WebAdvert.Web.Services;

namespace WebAdvert.Web.Controllers
{
    public class AdvertManagementController : Controller
    {
        private readonly IFileUploader _fileUploader;
        //private readonly IAdvertApiClient _advertApiClient;
        //private readonly IMapper _mapper;

        public AdvertManagementController(
            IFileUploader fileUploader
            //, IAdvertApiClient advertApiClient, IMapper mapper
            )
        {
            _fileUploader = fileUploader;
            //_advertApiClient = advertApiClient;
            //_mapper = mapper;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new CreateAdvertViewModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateAdvertViewModel model, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                //var createAdvertModel = _mapper.Map<CreateAdvertModel>(model);
                //createAdvertModel.UserName = User.Identity.Name;

                //var apiCallResponse = await _advertApiClient.CreateAsync(createAdvertModel).ConfigureAwait(false);
                var id = Guid.NewGuid().ToString();

                //bool isOkToConfirmAd = true;
                string filePath = string.Empty;
                if (imageFile != null)
                {
                    var fileName = !string.IsNullOrEmpty(imageFile.FileName) ? Path.GetFileName(imageFile.FileName) : id;
                    filePath = $"{id}/{fileName}";

                    try
                    {
                        using (var readStream = imageFile.OpenReadStream())
                        {
                            var result = await _fileUploader.UploadFileAsync(filePath, readStream)
                                .ConfigureAwait(false);
                            if (!result)
                                throw new Exception(
                                    "Could not upload the image to file repository. Please see the logs for details.");
                        }
                    }
                    catch (Exception e)
                    {
                        //isOkToConfirmAd = false;
                        //var confirmModel = new ConfirmAdvertRequest()
                        //{
                        //    Id = id,
                        //    FilePath = filePath,
                        //    Status = AdvertStatus.Pending
                        //};
                        //await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                        Console.WriteLine(e);
                    }


                }

                //if (isOkToConfirmAd)
                //{
                //    var confirmModel = new ConfirmAdvertRequest()
                //    {
                //        Id = id,
                //        FilePath = filePath,
                //        Status = AdvertStatus.Active
                //    };
                //    await _advertApiClient.ConfirmAsync(confirmModel).ConfigureAwait(false);
                //}

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }
    }
}