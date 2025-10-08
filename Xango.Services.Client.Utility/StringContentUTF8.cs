using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;

namespace Xango.Services.Client.Utility
{
    public static class StringContentUTF8
    {
        public static StringContent AsJsonString<T>(T dto)
            where T : class, new()
        {
            return new StringContent(DtoConverter.ToJson(dto), Encoding.UTF8, "application/json");
        }
    }
}
