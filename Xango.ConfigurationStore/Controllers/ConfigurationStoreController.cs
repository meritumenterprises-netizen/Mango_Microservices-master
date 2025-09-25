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

        [HttpPost("RegisterService")]
        public async Task<IActionResult> RegisterService()
        {
            return Ok();
        }

        [HttpGet("ListServices")]
        public async Task<ResponseDto> ListServices()
        {
            _response.IsSuccess = true;
            _response.Message = "All is alright";
            return _response;
        }

        [HttpGet("GetServiceMethods/{servicename}")]
        public async Task<ResponseDto> GetServiceMethods(string servicename)
        {
            _response.IsSuccess = true;
            _response.Message = "All is alright";
            return _response;
        }
    }
}
