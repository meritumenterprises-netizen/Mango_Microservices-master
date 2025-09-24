using Microsoft.AspNetCore.Mvc;

namespace Xango.ConfigurationStore.Controllers
{
    public class ConfigurationStoreController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
