using Xango.Models.Dto;
using Newtonsoft.Json;
namespace Xango.Services.Client.Utility
{
    public static class DtoConverter
    {
        public static T ToDto<T>(ResponseDto response)
            where T : class, new()
        {
            if (response== null)
                return default;

#pragma warning disable CS8603 // Possible null reference return.
            return JsonConvert.DeserializeObject<T>(Convert.ToString(response.Result));
#pragma warning restore CS8603 // Possible null reference return.
        }

        public static string ToJson<T>(T model)
            where T : class, new()
        {
            if (model == null)
            {
                return "";
            }
            return JsonConvert.SerializeObject(model);
        }
    }
}
