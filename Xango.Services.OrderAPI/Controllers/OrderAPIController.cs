using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;
using Stripe.Checkout;
using Stripe.Climate;
using Xango.Models.Dto;
using Xango.Service.InventoryAPI.Client;
using Xango.Services.Client.Utility;
using Xango.Services.Dto;
using Xango.Services.OrderAPI.Data;
using Xango.Services.OrderAPI.Models;
using Xango.Services.Utility;
using Xango.Services.Server.Utility;

namespace Xango.Services.OrderAPI.Controllers
{
    [Route("api/order")]
    [ApiController]
    [Authorize]
    public class OrderAPIController : ControllerBase
    {
        protected ResponseDto _response;
        private IMapper _mapper;
        private readonly AppDbContext _db;

        private readonly IConfiguration _configuration;
        private readonly IInventoryttpClient _inventoryClient;

        public OrderAPIController(AppDbContext db, IInventoryttpClient inventoryClient, IMapper mapper, IConfiguration configuration)

        {
            _db = db;
            _response = new ResponseDto();
            _mapper = mapper;
            _configuration = configuration;
            _inventoryClient = inventoryClient;
        }

        [HttpGet("GetAll")]
        [Authorize]
        [ResponseCache(Duration = 60, Location = ResponseCacheLocation.Any, NoStore = true)]
        public ResponseDto? GetAll(string userId, string status = "all")
        {
            try
            {
                IEnumerable<OrderHeader> objList;

                var id = User.Claims.Where((claim) => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value;
                if (User.IsInRole(SD.RoleAdmin))
                {
                    objList = _db.OrderHeaders.AsNoTracking().Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).Where(u => status == "all" || u.Status == status);
                }
                else
                {
                    objList = _db.OrderHeaders.AsNoTracking().Include(u => u.OrderDetails).OrderByDescending(u => u.OrderHeaderId).Where(u => (status == "all" || u.Status == status) && u.UserId == id );
                }
                _response.Result = _mapper.Map<IEnumerable<OrderHeaderDto>>(objList);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpGet("GetOrder/{id:int}")]
        [Authorize]
        public ResponseDto? Get(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include(u => u.OrderDetails).First(u => u.OrderHeaderId == id);
                _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpGet("CancelOrder/{id:int}")]
        [Authorize]
        public async Task<ResponseDto> CancelOrder(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == id);
                if (orderHeader != null && orderHeader.Status == SD.Status_Pending)
                {
                    foreach (var orderDetail in orderHeader.OrderDetails)
                    {
                        await _inventoryClient.ReturnQty(orderDetail.ProductId, orderDetail.Count);
                        //await _inventoryService.ReturnQty(orderDetail.ProductId, orderDetail.Count);
                    }
                    orderHeader.Status = SD.Status_Cancelled;
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.Message = "Order cancelled successfully";
                }
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpDelete("DeleteOrder/{id:int}")]
        [Authorize]
        public async Task<ResponseDto> DeleteOrder(int id)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include((d) => d.OrderDetails).First(u => u.OrderHeaderId == id);
                if (orderHeader != null)
                {
                    foreach (var orderDetail in orderHeader.OrderDetails)
                    {
                        //await _inventoryService.ReturnQty(orderDetail.ProductId, orderDetail.Count);
                        await _inventoryClient.ReturnQty(orderDetail.ProductId, orderDetail.Count);
                        _db.OrderDetails.Remove(orderDetail);
                    }
                    _db.OrderHeaders.Remove(orderHeader);
                    _db.SaveChanges();
                    _response.IsSuccess = true;
                    _response.Message = "Order deleted successfully";
                }
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost("CreateOrder")]

        public async Task<ResponseDto> CreateOrder(CartDto cartDto)
        {
            try
            {
                OrderHeaderDto orderHeaderDto = _mapper.Map<OrderHeaderDto>(cartDto.CartHeader);
                orderHeaderDto.OrderTime = DateTime.Now;
                orderHeaderDto.Status = SD.Status_Pending;
                orderHeaderDto.UserEmail = User.Claims.Where((claim) => claim.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress").First().Value;
				orderHeaderDto.OrderDetails = _mapper.Map<IEnumerable<OrderDetailsDto>>(cartDto.CartDetails);
                foreach (var orderDetail in orderHeaderDto.OrderDetails)
                {
                    await _inventoryClient.SubtractFromStock(orderDetail.ProductId, orderDetail.Count);
                }
                orderHeaderDto.OrderTotal = Math.Round(orderHeaderDto.OrderTotal, 2);
                OrderHeader orderCreated = _db.OrderHeaders.Add(_mapper.Map<OrderHeader>(orderHeaderDto)).Entity;
                await _db.SaveChangesAsync();

                orderHeaderDto.OrderHeaderId = orderCreated.OrderHeaderId;
                _response.Result = orderHeaderDto;
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost("CreateStripeSession")]
        public async Task<ResponseDto> CreateStripeSession(StripeRequestDto stripeRequestDto)
        {
            try
            {

                var options = new SessionCreateOptions
                {
                    SuccessUrl = stripeRequestDto.ApprovedUrl,
                    CancelUrl = stripeRequestDto.CancelUrl,
                    LineItems = new List<SessionLineItemOptions>(),
                    Mode = "payment",

                };

                var DiscountsObj = new List<SessionDiscountOptions>()
                {
                    new SessionDiscountOptions
                    {
                        Coupon=stripeRequestDto.OrderHeader.CouponCode
                    }
                };

                foreach (var item in stripeRequestDto.OrderHeader.OrderDetails)
                {
                    var sessionLineItem = new SessionLineItemOptions
                    {
                        PriceData = new SessionLineItemPriceDataOptions
                        {
                            UnitAmount = (long)(item.Price * 100), // $20.99 -> 2099
                            Currency = "usd",
                            ProductData = new SessionLineItemPriceDataProductDataOptions
                            {
                                Name = item.Product.Name
                            }
                        },
                        Quantity = item.Count
                    };

                    options.LineItems.Add(sessionLineItem);
                }

                if (stripeRequestDto.OrderHeader.Discount > 0)
                {
                    options.Discounts = DiscountsObj;
                }
                var service = new SessionService();
                Session session = service.Create(options);
                stripeRequestDto.StripeSessionUrl = session.Url;
                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == stripeRequestDto.OrderHeader.OrderHeaderId);
                orderHeader.StripeSessionId = session.Id;
                _db.SaveChanges();
                _response.Result = stripeRequestDto;

            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost("ValidateStripeSession")]
        [Authorize]
        public async Task<ResponseDto> ValidateStripeSession([FromBody] int orderHeaderId)
        {
            try
            {

                OrderHeader orderHeader = _db.OrderHeaders.First(u => u.OrderHeaderId == orderHeaderId);

                var service = new SessionService();
                Session session = service.Get(orderHeader.StripeSessionId);

                var paymentIntentService = new PaymentIntentService();
                PaymentIntent paymentIntent = paymentIntentService.Get(session.PaymentIntentId);

                if (paymentIntent.Status == "succeeded")
                {
                    //then payment was successful
                    orderHeader.PaymentIntentId = paymentIntent.Id;
                    orderHeader.Status = SD.Status_Approved;
                    _db.SaveChanges();
                    _response.Result = _mapper.Map<OrderHeaderDto>(orderHeader);
                }
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost("UpdateOrderStatus/{orderId:int}")]
        [Authorize]
        public async Task<ResponseDto> UpdateOrderStatus(int orderId, [FromBody] string newStatus)
        {
            try
            {
                OrderHeader orderHeader = _db.OrderHeaders.Include((od) => od.OrderDetails).First(u => u.OrderHeaderId == orderId);
                if (orderHeader != null)
                {
                    if (newStatus == SD.Status_Cancelled)
                    {
                        foreach (var orderDetail in orderHeader.OrderDetails)
                        {
                            await _inventoryClient.ReturnQty(orderDetail.ProductId, orderDetail.Count);
                        }
                    }
                    orderHeader.Status = newStatus;
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }
	}
}
