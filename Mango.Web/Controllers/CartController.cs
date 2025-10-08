
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using Xango.Models.Dto;
using Xango.Service.OrderAPI.Client;
using Xango.Service.ShoppingCartAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.Interfaces;
using Xango.Services.Utility;
using Xango.Web.Service;


namespace Xango.Web.Controllers
{
    
    public class CartController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IShoppingCartHttpClient _shoppingCartClient;
        private readonly IOrderHttpClient _orderHttpClient;
        public CartController(IOrderHttpClient orderHttpClient, IAuthService authService, IShoppingCartHttpClient shoppingCartHttpClient)
        {
            _authService = authService;
            _shoppingCartClient = shoppingCartHttpClient;
            _orderHttpClient = orderHttpClient;
        }

        [HttpGet]
        public async Task<IActionResult> CartIndex()
        {
            return View(await LoadCartDtoBasedOnLoggedInUser());
        }

        [Authorize]
        public async Task<IActionResult> Checkout()
        {
            var responseDto = await _authService.GetUser(User.Identity.Name);
            UserDto userDto = DtoConverter.ToDto<UserDto>(responseDto);
            var cartDto = DtoConverter.ToDto<CartDto>(await _shoppingCartClient.GetCartByUserId(userDto.Id));
            //var cartDto = DtoConverter.ToDto<CartDto>(await _cartService.GetCartByUserId(userDto.Id));
            return View(cartDto);
        }

        [Authorize]
        [ActionName("DeleteCart")]
        public async Task<IActionResult> DeleteCart()
        {
            var responseDto = await _authService.GetUser(User.Identity.Name);
            UserDto userDto = DtoConverter.ToDto<UserDto>(responseDto);
            var response = await _shoppingCartClient.DeleteCart(userDto.Id);
            return Redirect("/");
        }

        [HttpPost]
        [ActionName("Checkout")]
        [Authorize]
        public async Task<IActionResult> Checkout(CartDto cartDto)
        {

            CartDto cart = await LoadCartDtoBasedOnLoggedInUser();
            cart.CartHeader.Phone = cartDto.CartHeader.Phone;
            cart.CartHeader.Email = cartDto.CartHeader.Email;
            cart.CartHeader.Name = cartDto.CartHeader.Name;

            //var response = await _orderService.CreateOrder(cart);
            var response = await _orderHttpClient.CreateOrder(cart);
            OrderHeaderDto orderHeaderDto = DtoConverter.ToDto<OrderHeaderDto>(response);
            orderHeaderDto.OrderTotalWithCurrency = orderHeaderDto.OrderTotal.ToString("C2");

            if (response != null && response.IsSuccess)
            {
                //get stripe session and redirect to stripe to place order
                var domain = Request.Scheme + "://" + Request.Host.Value + "/";

                StripeRequestDto stripeRequestDto = new()
                {
                    ApprovedUrl = domain + "cart/Confirmation?orderId=" + orderHeaderDto.OrderHeaderId,
                    CancelUrl = domain + "cart/checkout",
                    OrderHeader = orderHeaderDto
                };

                //var stripeResponse = await _orderService.CreateStripeSession(stripeRequestDto);
                var stripeResponse = await _orderHttpClient.CreateStripeSession(stripeRequestDto);
                StripeRequestDto stripeResponseResult = DtoConverter.ToDto<StripeRequestDto>(stripeResponse);
                Response.Headers.Add("Location", stripeResponseResult.StripeSessionUrl);

                return new StatusCodeResult(303);
            }
            return View();
        }

        public async Task<IActionResult> Confirmation(int orderId)
        {
            ResponseDto? response = await _orderHttpClient.ValidateStripeSession(orderId);
            if (response != null & response.IsSuccess)
            {
                OrderHeaderDto orderHeader = DtoConverter.ToDto<OrderHeaderDto>(response);
                var cartDto = _shoppingCartClient.GetCartByUserId(orderHeader.UserId);
                _shoppingCartClient.DeleteCart(orderHeader.UserId);
                if (orderHeader.Status == SD.Status_Approved)
                {
                    return View(orderId);
                }
            }
            //redirect to some error page based on status
            return View(orderId);
        }

        public async Task<IActionResult> Remove(int cartDetailsId)
        {
            var userId = User.Claims.Where(u => u.Type == JwtRegisteredClaimNames.Sub)?.FirstOrDefault()?.Value;
            ResponseDto? response = await _shoppingCartClient.RemoveFromCart(cartDetailsId);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ApplyCoupon(CartDto cartDto)
        {
            ResponseDto? response = await _shoppingCartClient.ApplyCoupon(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveCoupon(CartDto cartDto)
        {
            cartDto.CartHeader.CouponCode = "";
            ResponseDto? response = await _shoppingCartClient.ApplyCoupon(cartDto);
            if (response != null & response.IsSuccess)
            {
                TempData["success"] = "Cart updated successfully";
                return RedirectToAction(nameof(CartIndex));
            }
            return View();
        }

        private async Task<CartDto> LoadCartDtoBasedOnLoggedInUser()
        {
            var userEmail = User.Claims.Where((claim) => claim.Type == "email").First().Value;
            var responseDto = await _authService.GetUser(userEmail);
            var userDto = DtoConverter.ToDto<UserDto>(responseDto);
            ResponseDto? response = await _shoppingCartClient.GetCartByUserId(userDto.Id);
            if (response != null & response.IsSuccess)
            {
                CartDto cartDto = DtoConverter.ToDto<CartDto>(response);
                return cartDto;
            }
            return new CartDto();
        }
    }
}
