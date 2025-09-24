using AutoMapper;
using Mango.MessageBus;
using Mango.Services.AuthAPI.Models.Dto;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Web;
using Xango.Services.AuthAPI.Models.Dto;
using Xango.Services.Dto;

namespace Mango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IMessageBus _messageBus;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;
        private readonly ConnectionMultiplexer _redis = null;
        private readonly IDatabase _db = null;

        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService, IMessageBus messageBus, IConfiguration configuration, IMapper mapper)
        {
            _authService = authService;
            _configuration = configuration;
            _messageBus = messageBus;
            _mapper = mapper;
            _response = new();
            _redis = ConnectionMultiplexer.Connect("localhost");
            _db = _redis.GetDatabase();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {

            var errorMessage = await _authService.Register(model);
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            await _messageBus.PublishMessage(model.Email, _configuration.GetValue<string>("TopicAndQueueNames:RegisterUserQueue"));
            _response.IsSuccess = true;
            _response.Message = "Registration successful";
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = loginResponse;
            _response.IsSuccess = true;
            await _messageBus.PublishMessage(model.UserName, _configuration.GetValue<string>("TopicAndQueueNames:LoginUserQueue"));
            return Ok(_response);

            return Ok(_response);

        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var assignRoleSuccessful = await _authService.AssignRole(model.Email, model.Role.ToUpper());
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            await _messageBus.PublishMessage(model.Name + ":" + model.Role, _configuration.GetValue<string>("TopicAndQueueNames:AssignRolesQueue"));
            return Ok(_response);

        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto model)
        {
            _response.IsSuccess = true;
            _response.Message = "Logout successful";
            _db.StringSet(model.UserName, "");
            await _messageBus.PublishMessage(model.UserName, _configuration.GetValue<string>("TopicAndQueueNames:LogoutUserQueue"));
            return Ok(_response);
        }

        [HttpPost("SaveUser")]
        public async Task<ResponseDto> SaveUser(string email, [FromBody] UserDto user)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                _db.StringSet(email, JsonConvert.SerializeObject(user));
                response.IsSuccess = true;
                response.Message = "Success";
                response.Result = user;

            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Result = "";
            }
            return response;
        }

        [HttpPost("GetUser")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseDto> GetUser([FromBody] string email)
        {
            ResponseDto response = new ResponseDto();
            try
            {
                string value = _db.StringGet(email);
                response.IsSuccess = true;
                response.Message = "Success";
                response.Result = value;
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                response.Result = "";
            }
            return response;
        }
    }
}
