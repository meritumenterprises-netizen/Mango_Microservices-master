using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json;
using Xango.Models.Dto;
using Xango.Services.Dto;
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
            var result = await _authService.Register(model);
            if (result.IsSuccess == false)
            {
                var response = new ResponseDto()
                {
                    IsSuccess = false,
                    Message = "User registration failed",
                    Result = ""
                };
                return BadRequest(response);
            }
            _response.IsSuccess = true;
            _response.Message = "Registration successful";
            _response.Result = result.Result;
            return Ok(_response);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var responseDto = await _authService.Login(model);
            LoginResponseDto loginResponse = DtoConverter.ToDto<LoginResponseDto>(responseDto);
            if (loginResponse.User == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Username or password is incorrect";
                return BadRequest(_response);
            }
            _response.Result = DtoConverter.ToJson(loginResponse);
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

        [HttpGet("GetUser/{email}")]
        public async Task<IActionResult> GetUser(string email)
        {
            var responseDto = await _authService.GetUser(email);
            var userDto = DtoConverter.ToDto<UserDto>(responseDto);
            if (userDto == null || userDto.Email.ToLower() != email.ToLower())
            {
                _response.IsSuccess = false;
                _response.Message = "User not found";
                return NotFound(_response);
            }
            _response.IsSuccess = true;
            _response.Result = DtoConverter.ToJson<UserDto>(userDto);
            return Ok(_response);
        }
    }
}
