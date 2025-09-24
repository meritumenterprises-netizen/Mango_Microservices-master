using Microsoft.AspNetCore.Mvc;
using Xango.Services.Dto;

namespace Xango.ConfigurationStore.Controllers
{
    [Route("api/configuration")]
    [ApiController]
    public class ConfigurationStoreController : Controller
    {
        private ResponseDto _response = null;

        public ConfigurationStoreController()
        {
            _response = new ResponseDto();
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("GetInfo")]
        public ResponseDto GetInfo()
        {
            _response.IsSuccess = true;
            _response.Message = "All is alright";
            return _response;
        }

    }
}
