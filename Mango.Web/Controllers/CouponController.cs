using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Service.CouponAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Interfaces;
using Xango.Services.Server.Utility;
using Xango.Web.Extensions;

namespace Xango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponHttpClient _couponClient;
        private readonly ITokenProvider _tokenProvider;
		public CouponController(ICouponHttpClient couponClient, ITokenProvider tokenProvider)
		{
			_couponClient = couponClient;
			_tokenProvider = tokenProvider;
		}


		public async Task<IActionResult> CouponIndex()
        {
            List<CouponDto>? list = new();
            ResponseDto? response = await _couponClient.GetAllCoupons();

            if (response != null && response.IsSuccess)
            {
                list = DtoConverter.ToDto<List<CouponDto>>(response);
            }
            else
            {
                TempData["error"] = response?.Message + "<p/>" + response?.StackTrace;
            }

            return View(list);
        }

        public async Task<IActionResult> CouponCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CouponCreate(CouponDto model)
        {
            if (ModelState.IsValid)
            {
                this.SetClientToken(_couponClient, _tokenProvider);
                ResponseDto? response = await _couponClient.CreateCoupons(model);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Coupon created successfully";
                    return RedirectToAction(nameof(CouponIndex));
                }
                else
                {
                    TempData["error"] = response?.Message;
                }
            }
            return View(model);
        }

        public async Task<IActionResult> CouponDelete(int couponId)
        {
            this.SetClientToken(_couponClient, _tokenProvider);
			ResponseDto? response = await _couponClient.DeleteCoupons(couponId);

            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Coupon successfully deleted";
                return RedirectToAction(nameof(CouponIndex)); 
            }
            else
            {
                TempData["error"] = response?.Message;
            }
            return NotFound();
        }
    }
}
