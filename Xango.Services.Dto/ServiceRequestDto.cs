using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Services.Dto
{
    public class ServiceRequestDto
    {
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceAction { get; set; } = string.Empty;
        public string ServiceRequestData { get; set; } = string.Empty;
        public string ServiceRequestAction  { get; set; } = string.Empty;

    }
}
