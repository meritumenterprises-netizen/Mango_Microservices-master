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
                return BadRequest(ResponseProducer.ErrorResponse("User registration failed"));
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
                return BadRequest(ResponseProducer.ErrorResponse("Username or password is incorrect"));
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
                return BadRequest(ResponseProducer.ErrorResponse("Error encountered"));
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
                return NotFound(ResponseProducer.ErrorResponse("User not found"));
            }
            return Ok(ResponseProducer.OkResponse(result: userDto));
        }
    }
}
