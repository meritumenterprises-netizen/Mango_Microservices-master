using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Service.ProductAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Interfaces;

namespace Xango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductHttpClient _productHttpClient;
        public ProductController(IProductHttpClient productHttpClient)
        {
            _productHttpClient = productHttpClient; 
        }


        public async Task<IActionResult> ProductIndex()
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

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productHttpClient.CreateProducts(model);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product created successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> DeleteProduct(int productId)
        {
            //var response = await _productService.DeleteProduct(productId);
            var response = await _productHttpClient.DeleteProduct(productId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Product deleted successfully";
                return RedirectToAction(nameof(ProductIndex));
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        public async Task<IActionResult> ProductEdit(int productId)
        {
            ResponseDto? response = await _productHttpClient.GetProductById(productId);

            if (response != null && response.IsSuccess)
            {
                ProductDto? model = DtoConverter.ToDto<ProductDto>(response);
                return View(nameof(ProductEdit), model);
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                ResponseDto? response = await _productHttpClient.UpdateProducts(productDto);

                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Product updated successfully";
                    return RedirectToAction(nameof(ProductIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(productDto);
        }

    }
}
