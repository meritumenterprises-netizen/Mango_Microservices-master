using AutoMapper;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Web;
using Xango.Models.Dto;
using Xango.Services.Interfaces;

namespace Mango.Services.AuthAPI.Controllers
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
            var responseDto = JsonConvert.DeserializeObject<ResponseDto>(_authService.Register(model).Result.Result.ToString());
            var errorMessage = responseDto.Message;
            if (!string.IsNullOrEmpty(errorMessage))
            {
                _response.IsSuccess = false;
                _response.Message = errorMessage;
                return BadRequest(_response);
            }
            var userDto = _authService.GetUser(model.Email).Result;
            _response.IsSuccess = true;
            _response.Message = "Registration successful";
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var response = await _authService.Login(model);
            var loginResponseDto = (LoginResponseDto)response.Result; 
            var userDto = loginResponseDto.User;

            if (userDto == null || userDto.Email != model.UserName)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return NotFound(_response);
            }
            var user = _authService.GetUser(userDto.Email).Result;  
            _response.Result = JsonConvert.SerializeObject(userDto);
            _response.IsSuccess = true;
            return Ok(_response);

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
            var responseDto = await _authService.AssignRole(registrationDto);
            var assignRoleSuccessful = responseDto.IsSuccess;
            if (!assignRoleSuccessful)
            {
                _response.IsSuccess = false;
                _response.Message = "Error encountered";
                return BadRequest(_response);
            }
            return Ok(_response);

        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto model)
        {
            _response.IsSuccess = true;
            _response.Message = "Logout successful";
            return Ok(_response);
        }

        [HttpGet("GetUser/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            var responseDto = await _authService.GetUser(email);
            var loginResponseDto = (LoginResponseDto)responseDto.Result;
            if (loginResponseDto== null || loginResponseDto.User.Email.ToLower() != email.ToLower())
            {
                _response.IsSuccess = false;
                _response.Message = "User not found";
                return NotFound(_response);
            }
            _response.IsSuccess = true;
            _response.Result = JsonConvert.SerializeObject(loginResponseDto.User);
            return Ok(_response);
        }
    }
}
