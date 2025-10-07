using AutoMapper;
using Microsoft.AspNetCore.Mvc;

using Xango.Models.Dto;
using Xango.Services.AuthAPI.Service.IService;
using Xango.Services.Client.Utility;

namespace Xango.Services.AuthAPI.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthAPIController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        protected ResponseDto _response;
        public AuthAPIController(IAuthService authService, IConfiguration configuration, IMapper mapper)
        {
            _authService = authService;
            _configuration = configuration;
            _mapper = mapper;
            _response = new();
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationRequestDto model)
        {
            var result = await _authService.Register(model);
            if (string.IsNullOrEmpty(result))
            {
                return BadRequest(ResponseProducer.ErrorResponse("User registration failed"));
            }
            _response.IsSuccess = true;
            _response.Message = "Registration successful";
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto model)
        {
            var loginResponse = await _authService.Login(model);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            //_response.Result = DtoConverter.ToJson<LoginResponseDto>(loginResponse);
            _response.Result = loginResponse;
            return Ok(_response);
        }

        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] RegistrationRequestDto model)
        {
            var registrationDto = new RegistrationRequestDto
            {
                Email = model.Email,
                Password = model.Password,
                Role = model.Role
            };
            var result = await _authService.AssignRole(model.Email,model.Role.ToUpper());
            if (!result)
            {
                return BadRequest(ResponseProducer.ErrorResponse("Error encountered"));
            }
            return Ok(_response);

        }

        [HttpGet("GetUser/{email}")] 
        public async Task<IActionResult> GetUser(string email)
        {
            var userDto = await _authService.GetUser(email);
            if (userDto == null || userDto.Email.ToLower() != email.ToLower())
            {
                return NotFound(ResponseProducer.ErrorResponse("User not found"));
            }
            return Ok(ResponseProducer.OkResponse(result: userDto));
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserById(string id)
        {
            var userDto = await _authService.GetUserById(id);
            if (userDto == null || userDto.Id != id)
            {
                return NotFound(ResponseProducer.ErrorResponse("User not found"));
            }
            return Ok(ResponseProducer.OkResponse(result: userDto));
        }
    }
}
