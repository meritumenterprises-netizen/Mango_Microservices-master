using AutoMapper;
using Xango.Services.ShoppingCartAPI.Data;
using Xango.Services.ShoppingCartAPI.Models;
using Xango.Services.ShoppingCartAPI.Service.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Service.CouponAPI.Client;
using Xango.Services.Utility;
using Xango.Service.AuthenticationAPI.Client;
using Xango.Service.ProductAPI.Client;
using System.Collections.Generic;
using System.Linq;

namespace Xango.Services.ShoppingCartAPI.Controllers
{
    [Route("api/cart")]
    [ApiController]
    [Authorize]
    public class CartAPIController : ControllerBase
    {
        private ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;
        private IProductHttpClient _productHttpClient;
        private IAuthenticationHttpClient _authenticationHttpClient;
        private IConfiguration _configuration;
        private ICouponHttpClient _couponHttpClient;

        public CartAPIController(AppDbContext db,
            IMapper mapper, IProductHttpClient productHttpClient, IConfiguration configuration, IAuthenticationHttpClient authenticationHttpClient, ICouponHttpClient couponHttpClient)
        {
            _db = db;
            _productHttpClient = productHttpClient;
            this._response = new ResponseDto();
            _mapper = mapper;
            _configuration = configuration;
            _authenticationHttpClient = authenticationHttpClient;
            _couponHttpClient = couponHttpClient;

        }
        [HttpGet("GetCart/{userId}")]
        public async Task<ResponseDto> GetCart(string userId)
        {
            try
            {
                CartDto cart = new CartDto()
                {
                    CartHeader = _mapper.Map<CartHeaderDto>(_db.CartHeaders.First(u => u.UserId== userId))
                };
                var response = await _authenticationHttpClient.GetUserById(userId);
                var userDto = DtoConverter.ToDto<UserDto>(Convert.ToString(response.Result));
                cart.CartHeader.Name = userDto.Name;
                cart.CartHeader.Email = userDto.Email;
                cart.CartHeader.Phone = userDto.PhoneNumber;
                cart.CartDetails = _mapper.Map<List<CartDetailsDto>>(_db.CartDetails.Where(u => u.CartHeaderId == cart.CartHeader.CartHeaderId));

                List<ProductDto> productDtos = DtoConverter.ToDto<List<ProductDto>>(await _productHttpClient.GetAllProducts());

                var cartDetailsToDelete = new List<CartDetailsDto>();
                foreach (var item in cart.CartDetails)
                {
                    item.Product = productDtos.FirstOrDefault(u => u.ProductId == item.ProductId);
                    if (item.Product != null)
                    {
                        cart.CartHeader.CartTotal += (item.Count * item.Product.Price);
                    }
                    else
                    {
                        cartDetailsToDelete.Add(item);
                    }
                }

                foreach (var detail in cartDetailsToDelete)
                {
                    cart.CartDetails.Remove(detail);
                }

                //apply coupon if any
                if (!string.IsNullOrEmpty(cart.CartHeader.CouponCode))
                {
                    //CouponDto coupon = await _couponService.GetCoupon(cart.CartHeader.CouponCode);

                    var response2 = await _couponHttpClient.GetCoupon(cart.CartHeader.CouponCode);
                    CouponDto coupon = DtoConverter.ToDto<CouponDto>((ResponseDto)(response2.Result));
                    if (coupon != null && cart.CartHeader.CartTotal > coupon.MinAmount)
                    {
                        cart.CartHeader.CartTotal -= coupon.DiscountAmount;
                        cart.CartHeader.Discount = coupon.DiscountAmount;
                    }
                }

                _response.Result = cart;
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }


        [HttpPost("ApplyCoupon")]
        public async Task<object> ApplyCoupon([FromBody] CartDto cartDto)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == cartDto.CartHeader.UserId);
                cartFromDb.CouponCode = cartDto.CartHeader.CouponCode;
                _db.CartHeaders.Update(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost("CartUpsert")]
        public async Task<ResponseDto> CartUpsert(CartDto cartDto)
        {
            try
            {
                var cartHeaderFromDb = await _db.CartHeaders.AsNoTracking()
                    .FirstOrDefaultAsync(u => u.UserId == cartDto.CartHeader.UserId);
                if (cartHeaderFromDb == null)
                {
                    //create header and details
                    CartHeader cartHeader = _mapper.Map<CartHeader>(cartDto.CartHeader);
                    _db.CartHeaders.Add(cartHeader);
                    await _db.SaveChangesAsync();
                    cartDto.CartDetails.First().CartHeaderId = cartHeader.CartHeaderId;
                    _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                    await _db.SaveChangesAsync();
                }
                else
                {
                    //if header is not null
                    //check if details has same product
                    var cartDetailsFromDb = await _db.CartDetails.AsNoTracking().FirstOrDefaultAsync(
                        u => u.ProductId == cartDto.CartDetails.First().ProductId &&
                        u.CartHeaderId == cartHeaderFromDb.CartHeaderId);
                    if (cartDetailsFromDb == null)
                    {
                        //create cartdetails
                        cartDto.CartDetails.First().CartHeaderId = cartHeaderFromDb.CartHeaderId;
                        _db.CartDetails.Add(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                    else
                    {
                        //update count in cart details
                        cartDto.CartDetails.First().Count += cartDetailsFromDb.Count;
                        cartDto.CartDetails.First().CartHeaderId = cartDetailsFromDb.CartHeaderId;
                        cartDto.CartDetails.First().CartDetailsId = cartDetailsFromDb.CartDetailsId;
                        _db.CartDetails.Update(_mapper.Map<CartDetails>(cartDto.CartDetails.First()));
                        await _db.SaveChangesAsync();
                    }
                }
                _response.Result = cartDto;
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpDelete("DeleteCart/{userId}")]
        public async Task<ResponseDto> DeleteCart(string userId)
        {
            try
            {
                var cartFromDb = await _db.CartHeaders.FirstAsync(u => u.UserId == userId);
                foreach (var cartDetail in _db.CartDetails.Where(u => u.CartHeaderId == cartFromDb.CartHeaderId))
                {
                    _db.CartDetails.Remove(cartDetail);
                }
                _db.CartHeaders.Remove(cartFromDb);
                await _db.SaveChangesAsync();
                _response.Result = true;
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }


        [HttpPost("RemoveCart")]
        public async Task<ResponseDto> RemoveCart([FromBody] int cartDetailsId)
        {
            try
            {
                CartDetails cartDetails = _db.CartDetails
                   .First(u => u.CartDetailsId == cartDetailsId);

                int totalCountofCartItem = _db.CartDetails.Where(u => u.CartHeaderId == cartDetails.CartHeaderId).Count();
                _db.CartDetails.Remove(cartDetails);
                if (totalCountofCartItem == 1)
                {
                    var cartHeaderToRemove = await _db.CartHeaders
                       .FirstOrDefaultAsync(u => u.CartHeaderId == cartDetails.CartHeaderId);

                    _db.CartHeaders.Remove(cartHeaderToRemove);
                }
                await _db.SaveChangesAsync();

                _response.Result = true;
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }
    }
}
