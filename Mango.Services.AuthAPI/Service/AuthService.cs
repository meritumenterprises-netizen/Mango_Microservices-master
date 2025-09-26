using Mango.Services.AuthAPI.Data;
using Mango.Services.AuthAPI.Models;
using Mango.Services.AuthAPI.Service.IService;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using Xango.Models.Dto;
using Xango.Services.Interfaces;

namespace Mango.Services.AuthAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(AppDbContext db, IJwtTokenGenerator jwtTokenGenerator,
            UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _db = db;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ResponseDto?> GetUser(string email)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == email.ToLower());
            if (user != null)
            {
                var response = new ResponseDto()
                {
                    IsSuccess = true,
                    Result = JsonConvert.SerializeObject(new UserDto() { ID = user.Id, Name = user.Name, Email = user.Email, PhoneNumber = user.PhoneNumber })
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
                    return new ResponseDto() { IsSuccess = false, Message = result.Errors.FirstOrDefault().Description };
                }

            }
            catch (Exception ex)
            {
                return new ResponseDto() { IsSuccess = false, Message = ex.Message};
            }
            return new ResponseDto() { IsSuccess = false };
        }

        public async Task<ResponseDto?> Logout(LogoutRequestDto logoutRequestDto)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto?> AssignRole(RegistrationRequestDto registrationRequestDto)
        {
            var responseDto = new ResponseDto();
            var user = _db.ApplicationUsers.FirstOrDefault(u => u.Email.ToLower() == registrationRequestDto.Email.ToLower());
            if (user != null)
            {
                if (!_roleManager.RoleExistsAsync(registrationRequestDto.Role).GetAwaiter().GetResult())
                {
                    //create role if it does not exist
                    _roleManager.CreateAsync(new IdentityRole(registrationRequestDto.Role)).GetAwaiter().GetResult();
                }
                _userManager.AddToRoleAsync(user,registrationRequestDto.Role);
                responseDto.IsSuccess = true;
                return responseDto;
            }
            responseDto.IsSuccess = false;
            responseDto.Message = "User does not exist";
            return responseDto;
        }
    }
}
