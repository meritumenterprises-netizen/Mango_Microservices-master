using AutoMapper;
using Mango.Services.AuthAPI.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json;
using Xango.Models.Dto;
using Xango.Services.Interfaces;
using Xunit;

namespace AuthServiceTest
{
    public class AuthApiUnitTests
    {
        [Fact]
        public async Task LoginUser_Success()
        {
            var mockService = new Mock<IAuthService>();
            mockService.Setup(service => service.Login(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(new ResponseDto
                {
                    IsSuccess = true,
                    Result = new LoginResponseDto
                    {
                        Token = "dummy_token",
                        User = new UserDto() { ID = "f8a32fe6-9866-4f9f-9062-36a2680ae020", Name = "Piotr Łuczak", PhoneNumber = "783 622 481", Email = "pluczak99@gmail.com" }
                    }
                });
            IMapper mapper = Xango.Services.AuthAPI.MappingConfig.RegisterMaps().CreateMapper();
            var controller = new AuthAPIController(mockService.Object, null, mapper);
            var result = await controller.Login(new LoginRequestDto { UserName = "pluczak99@gmail.com", Password = "Password1!" });
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.IsType<UserDto>(JsonConvert.DeserializeObject<UserDto>(((ResponseDto)okResult.Value).Result.ToString()));
            var user = JsonConvert.DeserializeObject<UserDto>(((ResponseDto)okResult.Value).Result.ToString());
            Assert.Equal("Piotr Łuczak", user.Name);
            Assert.Equal("pluczak99@gmail.com", user.Email);
        }

        [Fact]
        public async Task LooutUser_Success()
        {
            var mockService = new Mock<IAuthService>();
            mockService.Setup(service => service.Login(It.IsAny<LoginRequestDto>()))
                .ReturnsAsync(new ResponseDto
                {
                    IsSuccess = true,
                    Result = new LoginResponseDto
                    {
                        Token = "dummy_token",
                        User = new UserDto() { ID = "f8a32fe6-9866-4f9f-9062-36a2680ae020", Name = "Piotr Łuczak", PhoneNumber = "783 622 481", Email = "pluczak99@gmail.com" }
                    }
                });
            IMapper mapper = Xango.Services.AuthAPI.MappingConfig.RegisterMaps().CreateMapper();
            var controller = new AuthAPIController(mockService.Object, null, mapper);
            var result = await controller.Login(new LoginRequestDto { UserName = "pluczak99@gmail.com", Password = "Password1!" });
            var okResult = await controller.Logout(new LogoutRequestDto() { UserName = "pluczak99@gmail.com" });
            var logoutResponseDto = (ResponseDto)((OkObjectResult)okResult).Value;
            Assert.IsType<OkObjectResult>(okResult);
            Assert.True(logoutResponseDto.IsSuccess);
            Assert.Equal("Logout successful", logoutResponseDto.Message);
        }
    }
}
