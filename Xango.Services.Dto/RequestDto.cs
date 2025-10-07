using System.Net.Mime;
using Xango.Services.Dto;
using ContentType = Xango.Services.Dto.ContentType;


namespace Xango.Models.Dto
{
    public class RequestDto
    {
        public ApiType ApiType { get; set; } = ApiType.GET;
        public string Url { get; set; }
        public object Data { get; set; }
        public string AccessToken { get; set; }

        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
