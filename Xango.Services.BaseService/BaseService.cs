using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Security.AccessControl;
using System.Text;
using Xango.Models.Dto;
using Xango.Services.Dto;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ContentType = Xango.Models.Dto.ContentType;
using Microsoft.AspNetCore.Http;
//using Xango.Web.BaseService;
using Xango.Services.Token;


//using Xango.Services.Token;

//namespace Xango.Web.BaseService
//{
//    public class BaseService : IBaseService
//    {
//        private readonly IHttpClientFactory _httpClientFactory;
//        private readonly ITokenProvider _tokenProvider;
        
//        public BaseService(IHttpClientFactory httpClientFactory, ITokenProvider tokenProvider)
//        {
//            _httpClientFactory = httpClientFactory;
//            _tokenProvider = tokenProvider;
//        }

//        public async Task<ResponseDto?> SendAsync(RequestDto requestDto, bool withBearer = true)
//        {
//            try
//            {
//                HttpClient client = _httpClientFactory.CreateClient("MangoAPI");
//                HttpRequestMessage message = new();
//                if (requestDto.ContentType == ContentType.MultipartFormData)
//                {
//                    message.Headers.Add("Accept", "*/*");
//                }
//                else
//                {
//                    message.Headers.Add("Accept", "application/json");
//                }
//                //token
//                if (withBearer)
//                {
//                    var token = _tokenProvider.GetToken();
//                    if (token != null)
//                    {
//                        message.Headers.Add("Authorization", $"Bearer {token}");
//                    }
//                }

//                message.RequestUri = new Uri(requestDto.Url);

//                if (requestDto.ContentType == ContentType.MultipartFormData)
//                {
//                    var content = new MultipartFormDataContent();

//                    foreach (var prop in requestDto.Data.GetType().GetProperties())
//                    {
//                        var value = prop.GetValue(requestDto.Data);
//                        if (value is FormFile)
//                        {
//                            var file = (FormFile)value;
//                            if (file != null)
//                            {
//                                content.Add(new StreamContent(file.OpenReadStream()), prop.Name, file.FileName);
//                            }
//                        }
//                        else
//                        {
//                            content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
//                        }
//                    }
//                    message.Content = content;
//                }
//                else
//                {
//                    if (requestDto.Data != null)
//                    {
//                        message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
//                    }
//                }
//                HttpResponseMessage? apiResponse = null;

//                switch (requestDto.ApiType)
//                {
//                    case ApiType.POST:
//                        message.Method = HttpMethod.Post;
//                        break;
//                    case ApiType.DELETE:
//                        message.Method = HttpMethod.Delete;
//                        break;
//                    case ApiType.PUT:
//                        message.Method = HttpMethod.Put;
//                        break;
//                    default:
//                        message.Method = HttpMethod.Get;
//                        break;
//                }

//                apiResponse = client.SendAsync(message).Result;

//                switch (apiResponse.StatusCode)
//                {
//                    case HttpStatusCode.NotFound:
//                        return ResponseProducer.ErrorResponse("Not found");
//                    case HttpStatusCode.Forbidden:
//                        return ResponseProducer.ErrorResponse("Access denied");
//                    case HttpStatusCode.Unauthorized:
//                        return ResponseProducer.ErrorResponse("Unauthorized");
//                    case HttpStatusCode.InternalServerError:
//                        return ResponseProducer.ErrorResponse("Internal Server Error"); 
//                    default:
//                        var apiContent = apiResponse.Content.ReadAsStringAsync();
//                        ResponseDto? apiResponseDto = DtoConverter.ToDto<ResponseDto?>(new ResponseDto() { Result = apiContent });
//                        return apiResponseDto;
//                }
//            }
//            catch (Exception ex)
//            {
//                return ResponseProducer.ErrorResponse(ex.Message, ex.StackTrace);
//            }
//        }
//    }
//}
