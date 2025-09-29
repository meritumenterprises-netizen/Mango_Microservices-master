using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Xango.Services.Interfaces;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor ctx)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = ctx;
        }

        public async Task<ResponseDto?> GetUser(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                var response = new ResponseDto()
                {
                    IsSuccess = true,
                    Result = DtoConverter.ToJson<UserDto>(new UserDto() { ID = user.Id, Name = user.Name, Email = user.Email, PhoneNumber = user.PhoneNumber })
                };
                return response;
            }
            return new ResponseDto()
            {
                IsSuccess = false,
                Message = $"User {email} not found",
                Result = JsonConvert.SerializeObject(
                    new UserDto()
                    {
                        ID = "",
                        Name = "",
                        Email = "",
                        PhoneNumber = ""
                    })
            };
        }

        public async Task<ResponseDto> Login(LoginRequestDto loginRequestDto)
        {
            var responseDto = new ResponseDto();
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.UserName.ToLower() == loginRequestDto.UserName.ToLower());

            bool isValid = await _userManager.CheckPasswordAsync(user, loginRequestDto.Password);

            if (user == null || isValid == false)
            {
                responseDto.IsSuccess = false;
                responseDto.Result = JsonConvert.SerializeObject(new LoginResponseDto() { User = null, Token = "" });
                return responseDto;
            }

            //if user was found , Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            UserDto userDTO = new()
            {
                Email = user.Email,
                ID = user.Id,
                Name = user.Name,
                PhoneNumber = user.PhoneNumber
            };

            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                User = userDTO,
                Token = token
            };
            responseDto.Result = JsonConvert.SerializeObject(loginResponseDto);

            return responseDto;
        }

        public async Task<ResponseDto?> Register(RegistrationRequestDto registrationRequestDto)
        {
            ApplicationUser user = new()
            {
                UserName = registrationRequestDto.Email,
                Email = registrationRequestDto.Email,
                NormalizedEmail = registrationRequestDto.Email.ToUpper(),
                Name = registrationRequestDto.Name,
                PhoneNumber = registrationRequestDto.PhoneNumber
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registrationRequestDto.Password);
                if (result.Succeeded)
                {
                    var userToReturn = _db.ApplicationUsers.First(u => u.UserName == registrationRequestDto.Email);

                    UserDto userDto = new()
                    {
                        Email = userToReturn.Email,
                        ID = userToReturn.Id,
                        Name = userToReturn.Name,
                        PhoneNumber = userToReturn.PhoneNumber
                    };

                    var responseDto = await AssignRole(new RegistrationRequestDto() { Email = user.Email, Role = registrationRequestDto.Role });
                    return responseDto;

                }
                else
                {
                    return ResponseProducer.ErrorResponse(message: $"User {user.Email} could not be created");
                }

            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(message: ex.Message, stackTrace: ex.StackTrace);
            }
            return ResponseProducer.OkResponse();
        }

        public async Task<ResponseDto?> AssignRole(RegistrationRequestDto registrationRequestDto)
        {
            var responseDto = new ResponseDto();
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == registrationRequestDto.Email.ToLower());
            if (user != null)
            {
                var roleExists = await _roleManager.RoleExistsAsync(registrationRequestDto.Role);
                if (!roleExists)
                {
                    await _roleManager.CreateAsync(new IdentityRole(registrationRequestDto.Role));
                }
                await _userManager.AddToRoleAsync(user, registrationRequestDto.Role);
                responseDto.IsSuccess = true;
                responseDto.Result = DtoConverter.ToJson<ResponseDto>(responseDto);
                return responseDto;
            }
            responseDto.IsSuccess = false;
            responseDto.Message = "User does not exist";
            return responseDto;
        }

        public async Task<ResponseDto?> Logout()
        {
            return new ResponseDto();
        }
        
    }

}
