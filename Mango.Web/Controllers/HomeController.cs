using Abp.Web.Mvc.Models;
using IdentityModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Xango.Models.Dto;
using Xango.Service.ProductAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Interfaces;


namespace Xango.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICartService _cartService;
        private readonly IProductHttpClient _productHttpClient;
        public HomeController(ICartService cartService, IProductHttpClient productHttpClient)
        {
            _cartService = cartService;
            _productHttpClient = productHttpClient;
        }


        public async Task<IActionResult> Index()
        {
            List<ProductDto>? list = new();

            ResponseDto? response = await _productHttpClient.GetAllProducts();

            if (response != null && response.IsSuccess)
            {
                list = DtoConverter.ToDto<List<ProductDto>>(response);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(list);
        }

        [Authorize]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            ProductDto? model = new();

            ResponseDto? response = await _productHttpClient.GetProductById(productId);

            if (response != null && response.IsSuccess)
            {
                model = DtoConverter.ToDto<ProductDto>(response);
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(model);
        }


        [Authorize]
        [HttpPost]
        [ActionName("ProductDetails")]
        public async Task<IActionResult> ProductDetails(ProductDto productDto)
        {
            CartDto cartDto = new CartDto()
            {
                CartHeader = new CartHeaderDto
                {
                    UserId = User.Claims.Where(u => u.Type == JwtClaimTypes.Subject)?.FirstOrDefault()?.Value
                }
            };

            CartDetailsDto cartDetails = new CartDetailsDto()
            {
                Count = productDto.Count,
                ProductId = productDto.ProductId,
            };

            List<CartDetailsDto> cartDetailsDtos = new() { cartDetails };
            cartDto.CartDetails = cartDetailsDtos;

            ResponseDto? response = await _cartService.UpsertCart(cartDto);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Item has been added to the Shopping Cart";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["error"] = response?.Message;
            }

            return View(productDto);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(model: new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}