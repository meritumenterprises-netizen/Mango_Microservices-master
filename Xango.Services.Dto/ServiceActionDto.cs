using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Dto
{
    public class ServiceActionDto
    {
        public string ActionName { get; set; } = string.Empty;
        public string ActionMethod { get; set; } = string.Empty;
        public string HttpMethod { get; set; } = string.Empty;
        public bool IsAuthorized { get; set; } = false; 
    }
}
