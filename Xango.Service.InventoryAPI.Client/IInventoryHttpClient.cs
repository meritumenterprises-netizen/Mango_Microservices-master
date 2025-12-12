using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xango.Services.Interfaces;

namespace Xango.Service.InventoryAPI.Client
{
    public interface IInventoryttpClient : IInventoryService, ISetToken
    {
    }
}
