using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Dto
{
    public class ServiceDto
    {
        public string ServiceName { get; set; } = string.Empty; 
        public string ServiceBase { get; set; } = string.Empty; 
        public string ServiceEndpoint { get; set; } = string.Empty;
        public string ServiceHttpMethod { get; set; } = string.Empty;   

    }
}
