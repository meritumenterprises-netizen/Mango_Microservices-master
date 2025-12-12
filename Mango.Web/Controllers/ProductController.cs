using Abp.IO.Extensions;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Service.ProductAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Services.Server.Utility;
using Xango.Web.Extensions;

namespace Xango.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductHttpClient _productHttpClient;
        private readonly IMapper _mapper;
        private readonly ITokenProvider _tokenProvider;
		public ProductController(IProductHttpClient productHttpClient, IMapper mapper, ITokenProvider tokenProvider)
        {
            _productHttpClient = productHttpClient;
            _mapper = mapper;
            _tokenProvider = tokenProvider;
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

		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        [HttpPost]
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> ProductCreate(ProductDto model)
        {
            if (ModelState.IsValid)
            {
                if (model.Image != null)
                {
                    var ms = new MemoryStream();
                    model.Image.CopyTo(ms);
                    ms.Position = 0;
                    model.Base64Image = Convert.ToBase64String(ms.GetAllBytes());
                    model.Image = null;
                }
				this.SetClientToken(_productHttpClient, _tokenProvider);
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

		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> DeleteProduct(int productId)
        {
			this.SetClientToken(_productHttpClient, _tokenProvider);
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

		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> ProductEdit(int productId)
        {
			this.SetClientToken(_productHttpClient, _tokenProvider);
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
		[Authorize(Roles = "ADMIN")]
		public async Task<IActionResult> ProductEdit(ProductDto productDto)
        {
            if (ModelState.IsValid)
            {
                if (productDto.Image != null)
                {
                    var ms = new MemoryStream();
                    productDto.Image.CopyTo(ms);
                    ms.Position = 0;
                    productDto.Base64Image = Convert.ToBase64String(ms.GetAllBytes());
                    productDto.Image = null;
                }
				this.SetClientToken(_productHttpClient, _tokenProvider);
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
