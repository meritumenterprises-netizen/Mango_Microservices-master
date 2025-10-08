using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Interfaces;
using Xango.Services.Dto;
using Xango.Service.InventoryAPI.Client;
using Xango.Service.ShoppingCartAPI.Client;

namespace Xango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IShoppingCartHttpClient _shoppingCartHttpClient;
        private readonly IMapper _mapper;
        private readonly IInventoryttpClient _inventoryttpClient;   

        public OrderController(IOrderService orderService, IShoppingCartHttpClient shoppingCartHttpClient, IMapper mapper, IInventoryttpClient inventoryClient)
        {
            _orderService = orderService;
            _shoppingCartHttpClient = shoppingCartHttpClient;
            _mapper = mapper;
            _inventoryttpClient = inventoryClient;
        }

        [Authorize]
        public async Task<IActionResult> OrderIndex()
        {
            string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            var status = Request.Query["status"];
            if (string.IsNullOrEmpty(status))
            {
                status = "all";
            }
            ResponseDto response = _orderService.GetAll(userId, status).GetAwaiter().GetResult();
            var list = new List<OrderHeaderDto>();
            return View(response);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
            //string userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;

            var response = await _orderService.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = DtoConverter.ToDto<OrderHeaderDto>(response);
                orderHeaderDto.OrderTotalWithCurrency = orderHeaderDto.OrderTotal.ToString("C2");

            }
            if (!User.IsInRole(SD.RoleAdmin) && User.Identity.Name != orderHeaderDto.Email)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CompleteOrder")]
        public async Task<IActionResult> CompleteOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Completed);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
                return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
            }
            return View();
        }

        [HttpPost("CancelOrder")]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var response = await _orderService.UpdateOrderStatus(orderId, SD.Status_Cancelled);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Status updated successfully";
            }
            else
            {
                TempData["error"] = response.Message;
            }
            return RedirectToAction(nameof(OrderDetail), new { orderId = orderId });
        }

        [HttpPost("DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var response = await _orderService.DeleteOrder(orderId);
            return RedirectToAction(nameof(OrderIndex));
        }

        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeaderDto> list;
            string userId = "";
            //if (!User.IsInRole(SD.RoleAdmin))
            {
                userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            }
            ResponseDto response = _orderService.GetAll(userId, status).GetAwaiter().GetResult();
            if (response != null && response.IsSuccess)
            {
                list = DtoConverter.ToDto<List<OrderHeaderDto>>(response);
                foreach (var order in list)
                {
                    order.OrderTotalWithCurrency = order.OrderTotal.ToString("C2");
                }
            }
            else
            {
                list = new List<OrderHeaderDto>();
            }
            return Json(new { data = list.OrderByDescending(u => u.OrderHeaderId) });
        }

    }
}

