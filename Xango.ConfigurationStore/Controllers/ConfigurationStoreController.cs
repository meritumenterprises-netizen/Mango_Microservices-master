using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Xango.Services.Dto;

namespace Xango.ConfigurationStore.Controllers
{
    [Route("api/configuration")]
    [ApiController]
    public class ConfigurationStoreController : Controller
    {
        private ResponseDto _response = null;

        public ConfigurationStoreController()
        {
            _response = new ResponseDto();
        }

        [HttpPost("RegisterService")]
        public async Task<IActionResult> RegisterService()
        {
            return Ok();
        }

        [HttpGet("ListServices")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseDto> ListServices()
        {
            List<ServiceDto> services = new List<ServiceDto>()
            {
                new ServiceDto() { ServiceName = "ÄuthenticationApi", ServiceUrl = "https://localhost:7002" },
                new ServiceDto() { ServiceName = "ProductApi", ServiceUrl = "https://localhost:7000" },
                new ServiceDto() { ServiceName = "OrderApi", ServiceUrl = "https://localhost:7004" },
                new ServiceDto() { ServiceName = "ShoppingCartApi", ServiceUrl = "https://localhost:7003" },
                new ServiceDto() { ServiceName = "CouponApi", ServiceUrl = "https://localhost:7004" },
                new ServiceDto() { ServiceName = "ConfigurationServiceApi", ServiceUrl = "https://localhost:7211" },
                new ServiceDto() { ServiceName = "MessagingApi", ServiceUrl = "https://localhost:7008" }
            };
            _response.IsSuccess = true;
            _response.Message = "Ok";
            _response.Result = JsonConvert.SerializeObject(services);
            return _response;
        }

        [HttpGet("GetServiceMethods/{servicename}")]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any)]
        public async Task<ResponseDto> GetServiceMethods(string servicename)
        {
            List<ServiceActionDto> list = new List<ServiceActionDto>();
            switch (servicename.ToLower())
            {
                case "authenticationapi":
                    list.Add(new ServiceActionDto() { ActionName = "Register", ActionMethod = "https://localhost:7002/api/auth/register", HttpMethod = "POST", IsAuthorized = false });
                    list.Add(new ServiceActionDto() { ActionName = "Login", ActionMethod = "https://localhost:7002/api/auth/login", HttpMethod = "POST", IsAuthorized = false });
                    list.Add(new ServiceActionDto() { ActionName = "GetUser", ActionMethod = "https://localhost:7002/api/auth/GetUser/{email}", HttpMethod = "GET", IsAuthorized = false });
                    list.Add(new ServiceActionDto() { ActionName = "AssignRole", ActionMethod = "https://localhost:7002/api/auth/AssignRole", HttpMethod = "POST", IsAuthorized = false });
                    list.Add(new ServiceActionDto() { ActionName = "CurrentUser", ActionMethod = "https://localhost:7002/api/auth/CurrentUser", HttpMethod = "GET", IsAuthorized = false });
                    break;
                case "productapi":
                    list.Add(new ServiceActionDto() { ActionName = "GetProducts", ActionMethod = "https://localhost:7000/api/product", HttpMethod = "GET", IsAuthorized = false });
                    list.Add(new ServiceActionDto() { ActionName = "GetProduct", ActionMethod = "https://localhost:7000/api/product/{id}", HttpMethod = "GET", IsAuthorized = false });
                    list.Add(new ServiceActionDto() { ActionName = "CreateProduct", ActionMethod = "https://localhost:7000/api/product", HttpMethod = "POST", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "UpdateProduct", ActionMethod = "https://localhost:7000/api/product", HttpMethod = "PUT", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "DeleteProduct", ActionMethod = "https://localhost:7000/api/product/{id}", HttpMethod = "DELETE", IsAuthorized = true });
                    break;
                case "orderapi":
                    list.Add(new ServiceActionDto() { ActionName = "GetOrders", ActionMethod = "https://localhost:7004/api/order", HttpMethod = "GET", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "GetOrder", ActionMethod = "https://localhost:7004/api/order/{id}", HttpMethod = "GET", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "CreateOrder", ActionMethod = "https://localhost:7004/api/order", HttpMethod = "POST", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "UpdateOrder", ActionMethod = "https://localhost:7004/api/order", HttpMethod = "PUT", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "DeleteOrder", ActionMethod = "https://localhost:7004/api/order/{id}", HttpMethod = "DELETE", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "CancelOrder", ActionMethod = "https://localhost:7004/api/order/CancelOrder/{id:int}", HttpMethod = "GET", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "CreateStripeSession", ActionMethod = "https://localhost:7004/api/order/CreateStripeSession/{userId}", HttpMethod = "POST", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "ValidateStripeSession", ActionMethod = "https://localhost:7004/api/order/ValidateStripeSession", HttpMethod = "POST", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "UpdateOrderStatus", ActionMethod = "https://localhost:7004/api/order,UpdateOrderStatus/{orderId:int}", HttpMethod = "POST", IsAuthorized = true });
                    list.Add(new ServiceActionDto() { ActionName = "ProductUsedInOrders", ActionMethod = "https://localhost:7004/api/order/ProductUsedInOrders/{id}", HttpMethod = "GET", IsAuthorized = true });                    
                    break;
                case "shoppingcartapi":
                    list.Add(new ServiceActionDto() { ActionName = "GetCart", ActionMethod = "https://localhost:7003/api/shoppingcart/{userId}", HttpMethod = "GET" });
                    list.Add(new ServiceActionDto() { ActionName = "AddToCart", ActionMethod = "https://localhost:7003/api/shoppingcart", HttpMethod = "POST" });
                    list.Add(new ServiceActionDto() { ActionName = "UpdateCart", ActionMethod = "https://localhost:7003/api/shoppingcart", HttpMethod = "PUT" });
                    list.Add(new ServiceActionDto() { ActionName = "RemoveFromCart", ActionMethod = "https://localhost:7003/api/shoppingcart/{cartId}", HttpMethod = "DELETE" });
                    list.Add(new ServiceActionDto() { ActionName = "ClearCart", ActionMethod = "https://localhost:7003/api/shoppingcart/clear/{userId}", HttpMethod = "DELETE" });
                    list.Add(new ServiceActionDto() { ActionName = "ApplyCoupon", ActionMethod = "https://localhost:7003/api/shoppingcart/ApplyCoupon", HttpMethod = "POST" });
                    list.Add(new ServiceActionDto() { ActionName = "RemoveCoupon", ActionMethod = "https://localhost:7003/api/shoppingcart/RemoveCoupon/{userId}", HttpMethod = "DELETE" });
                    break;
                case "couponapi":
                    list.Add(new ServiceActionDto() { ActionName = "GetCoupons", ActionMethod = "https://localhost:7004/api/coupon", HttpMethod = "GET" });
                    list.Add(new ServiceActionDto() { ActionName = "GetCoupon", ActionMethod = "https://localhost:7004/api/coupon/{id}", HttpMethod = "GET" });
                    list.Add(new ServiceActionDto() { ActionName = "GetCouponByCode", ActionMethod = "https://localhost:7004/api/coupon/GetCouponByCode/{couponCode}", HttpMethod = "GET" });
                    list.Add(new ServiceActionDto() { ActionName = "CreateCoupon", ActionMethod = "https://localhost:7004/api/coupon", HttpMethod = "POST" });  
                    list.Add(new ServiceActionDto() { ActionName = "UpdateCoupon", ActionMethod = "https://localhost:7004/api/coupon", HttpMethod = "PUT" });   
                    break;
                case "configurationserviceapi":
                    list.Add(new ServiceActionDto() { ActionName = "ListServices", ActionMethod = "https://localhost:7211/api/configuration/ListServices", HttpMethod = "GET" });
                    list.Add(new ServiceActionDto() { ActionName = "GetServiceMethods", ActionMethod = "https://localhost:7211/api/configuration/GetServiceMethods/{servicename}", HttpMethod = "GET" });
                    list.Add(new ServiceActionDto() { ActionName = "RegisterService", ActionMethod = "https://localhost:7211/api/configuration/RegisterService", HttpMethod = "POST" });
                    break;
                case "messagingapi":
                    list.Add(new ServiceActionDto() { ActionName = "PublishMessage", ActionMethod = "https://localhost:7008/api/messaging", HttpMethod = "POST" });
                    break;
                default:
                    _response.IsSuccess = false;
                    _response.Message = "Service not found";
                    return _response;
            }
            _response.IsSuccess = true;
            _response.Message = "OK";
            _response.Result = JsonConvert.SerializeObject(list);
            return _response;
        }
    }
}
