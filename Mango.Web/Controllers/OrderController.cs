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
using Xango.Service.OrderAPI.Client;
using Xango.Services.Server.Utility;
using Xango.Web.Extensions;

namespace Xango.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IShoppingCartHttpClient _shoppingCartHttpClient;
        private readonly IMapper _mapper;
        private readonly IInventoryttpClient _inventoryttpClient;
        private readonly IOrderHttpClient _orderHttpClient;
        private readonly ITokenProvider _tokenProvider;

		public OrderController(IShoppingCartHttpClient shoppingCartHttpClient, IMapper mapper, IInventoryttpClient inventoryClient, IOrderHttpClient orderHttpClient, ITokenProvider tokenProvider)
        {
            _shoppingCartHttpClient = shoppingCartHttpClient;
            _mapper = mapper;
            _inventoryttpClient = inventoryClient;
            _orderHttpClient = orderHttpClient;
            _tokenProvider = tokenProvider;
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
            this.SetClientToken(_orderHttpClient, _tokenProvider);
			ResponseDto response = _orderHttpClient.GetAll(userId, status).GetAwaiter().GetResult();
            var list = new List<OrderHeaderDto>();
            return View(response);
        }

        [Authorize]
        public async Task<IActionResult> OrderDetail(int orderId)
        {
            OrderHeaderDto orderHeaderDto = new OrderHeaderDto();
			this.SetClientToken(_orderHttpClient, _tokenProvider);
			var response = await _orderHttpClient.GetOrder(orderId);
            if (response != null && response.IsSuccess)
            {
                orderHeaderDto = DtoConverter.ToDto<OrderHeaderDto>(response);

            }
            if (!User.IsInRole(SD.RoleAdmin) && User.Identity.Name != orderHeaderDto.UserEmail)
            {
                return NotFound();
            }
            return View(orderHeaderDto);
        }

        [HttpPost("OrderReadyForPickup")]
        public async Task<IActionResult> OrderReadyForPickup(int orderId)
        {
			this.SetClientToken(_orderHttpClient, _tokenProvider);
			var response = await _orderHttpClient.UpdateOrderStatus(orderId, SD.Status_ReadyForPickup);
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
			this.SetClientToken(_orderHttpClient, _tokenProvider);
			var response = await _orderHttpClient.UpdateOrderStatus(orderId, SD.Status_Completed);
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
			this.SetClientToken(_orderHttpClient, _tokenProvider);
			var response = await _orderHttpClient.UpdateOrderStatus(orderId, SD.Status_Cancelled);
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
			this.SetClientToken(_orderHttpClient, _tokenProvider);
			var response = await _orderHttpClient.DeleteOrder(orderId);
            return RedirectToAction(nameof(OrderIndex));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(string status)
        {
            IEnumerable<OrderHeaderDto> list;
            string userId = "";
			userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            
            ResponseDto response = await _orderHttpClient.GetAll(userId, status);
            if (response != null && response.IsSuccess)
            {
                list = DtoConverter.ToDto<List<OrderHeaderDto>>(response);
            }
            else
            {
                list = new List<OrderHeaderDto>();
            }
            return Json(new { data = list.OrderByDescending(u => u.OrderHeaderId) });
        }

    }
}

