using Microsoft.AspNetCore.Mvc;
using Xango.Models.Dto;
using Xango.Service.CouponAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Interfaces;

namespace Xango.Web.Controllers
{
    public class CouponController : Controller
    {
        private readonly ICouponHttpClient _couponClient;
        public CouponController(ICouponHttpClient couponClient)
        {
            _couponClient = couponClient;
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
