using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Server.Utility;
using Xango.Services.Utility;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Newtonsoft.Json;

namespace Xango.Service.AuthenticationAPI.Client
{
    public class AuthenticationHttpClient : IAuthenticationHttpClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly string _baseUri;
        private readonly ITokenProvider _tokenProvider;
        private readonly IHttpContextAccessor _contextAccessor;

        public AuthenticationHttpClient(IHttpClientFactory httpClientFactory, IConfiguration configuration, ITokenProvider tokenProvider, IHttpContextAccessor contextAccessor  )
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _baseUri = Environment.GetEnvironmentVariable("AuthenticaionAPI");
            _tokenProvider = tokenProvider;
            _contextAccessor = contextAccessor;
        }
        public async Task<ResponseDto?> AssignRole(RegistrationRequestDto registrationRequestDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Authentication");

            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("/api/auth/AssignRole", StringContentUTF8.AsJsonString<RegistrationRequestDto>(registrationRequestDto));
            if (response != null & response.Content != null)
            {
                var responseDto = DtoConverter.ToDto<ResponseDto>(new ResponseDto() { IsSuccess = true, Result = response.Content.ReadAsStringAsync().Result });
                return ResponseProducer.OkResponse(responseDto.Result);
            }
            return ResponseProducer.ErrorResponse("Could not log in the user");

        }

        public async Task<ResponseDto> GetUser(string email)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Authentication");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/auth/GetUser/" + email);
            if (response != null & response.Content != null)
            {
                var responseDto = DtoConverter.ToDto<ResponseDto>(await response.Content.ReadAsStringAsync());
                return ResponseProducer.OkResponse(responseDto.Result);
            }
            return ResponseProducer.ErrorResponse("Could not log in the user");
        }

        public async Task<ResponseDto?> Login(LoginRequestDto loginRequestDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Authentication");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("/api/auth/login", StringContentUTF8.AsJsonString<LoginRequestDto>(loginRequestDto));
            if (response.StatusCode != System.Net.HttpStatusCode.BadRequest & response.Content != null)
            {
                var responseDto = DtoConverter.ToDto<ResponseDto>(new ResponseDto() { IsSuccess = true, Result = response.Content.ReadAsStringAsync().Result });    
                return ResponseProducer.OkResponse(responseDto.Result);
            }
            return ResponseProducer.ErrorResponse("Could not log in the user");

        }

        public async Task<ResponseDto?> Register(RegistrationRequestDto registrationRequestDto)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Authentication");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.PostAsync("/api/auth/register", StringContentUTF8.AsJsonString<RegistrationRequestDto>(registrationRequestDto));
            if (response != null & response.StatusCode != System.Net.HttpStatusCode.BadRequest & response.Content != null)
            {
                var responseDto1 = DtoConverter.ToDto<ResponseDto>(new ResponseDto() { IsSuccess = true, Result = response.Content.ReadAsStringAsync().Result });
                return ResponseProducer.OkResponse(responseDto1.Result);
            }
            var responseError = DtoConverter.ToDto<ResponseDto>(await response.Content.ReadAsStringAsync());
            return ResponseProducer.ErrorResponse(responseError.Message);
        }

        public async Task<ResponseDto?> Logout()
        {
            await _contextAccessor.HttpContext.SignOutAsync();
            _tokenProvider.ClearToken();
            return ResponseProducer.OkResponse();
        }

        public async Task<ResponseDto> GetUserById(string id)
        {
            var client = _httpClientFactory.NewClientNoSslErrors("Authentication");
            client.BaseAddress = new Uri(_baseUri);
            var token = _tokenProvider.GetToken();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync("/api/auth/GetUserById/" + id);
            if (response != null & response.Content != null)
            {
                var responseDto = DtoConverter.ToDto<ResponseDto>(await response.Content.ReadAsStringAsync());
                return ResponseProducer.OkResponse(responseDto.Result);
            }
            return ResponseProducer.ErrorResponse("Could not log in the user");
        }
    }
}
