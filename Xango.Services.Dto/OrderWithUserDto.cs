using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xango.Models.Dto
{
    public class OrderWithUserDto
    {
        public UserDto User { get; set; } 
        public OrderHeaderDto OrderHeader { get; set; }
    }
}
