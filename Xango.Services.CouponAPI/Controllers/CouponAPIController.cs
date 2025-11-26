using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.CouponAPI.Data;
using Xango.Services.CouponAPI.Models;
using Xango.Services.Utility;

namespace Xango.Services.CouponAPI.Controllers
{
    [Route("api/coupon")]
    [ApiController]
    public class CouponAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ResponseDto _response;
        private IMapper _mapper;
        private IConfiguration _configuration;

        public CouponAPIController(AppDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _configuration = configuration;
        }

        [HttpGet]
		[Authorize(Roles = "ADMIN")]
		public async Task<ResponseDto> Get()
        {
            try
            {
                IEnumerable<Coupon> objList = _db.Coupons.ToList();
                _response.Result = _mapper.Map<IEnumerable<CouponDto>>(objList);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message, stackTrace: ex.StackTrace);
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(message: ex.Message, stackTrace: ex.StackTrace);
            }
            return _response;
        }

        [HttpGet]
        [Route("GetByCode/{code}")]
        public ResponseDto GetByCode(string code)
        {
            try
            {
                Coupon obj = _db.Coupons.FirstOrDefault(u => u.CouponCode.ToLower() == code.ToLower());
                if (obj == null)
                {
                    throw new ApplicationException($"Coupon code {code} is invalid");
				}
				_response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] CouponDto couponDto)
        {
            try
            {
                couponDto.CouponId = 0;
				if (_db.Coupons.ToList().FirstOrDefault((c) => c.CouponCode.ToLowerInvariant() == couponDto.CouponCode.ToLowerInvariant()) != null)
				{
                    throw new ApplicationException($"Coupon with code {couponDto.CouponCode} already exists");
                }
                Coupon obj = _mapper.Map<Coupon>(couponDto);
                _db.Coupons.Add(obj);
                _db.SaveChanges();

                var options = new Stripe.CouponCreateOptions
                {
                    AmountOff = (long)(couponDto.DiscountAmount * 100),
                    Name = couponDto.CouponCode,
                    Currency = "usd",
                    Id = couponDto.CouponCode,
                };
                var service = new Stripe.CouponService();
                service.Create(options);


                _response.Result = _mapper.Map<CouponDto>(obj);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put(CouponDto couponDto)
        {
            try
            {
                if (_db.Coupons.ToList().FirstOrDefault((c) => 
                    c.CouponCode.ToLowerInvariant() == couponDto.CouponCode.ToLowerInvariant() &&
                    c.CouponId != couponDto.CouponId) != null)
                {
                    throw new ApplicationException($"Coupon with code {couponDto.CouponCode} already exists");
                }

                var existing = _db.Coupons.Find(couponDto.CouponId);
				if (existing == null)
				{
                    throw new ApplicationException("Coupon not found");
				}

				// Map only the properties you want to update:
				existing.CouponCode = couponDto.CouponCode;
				existing.DiscountAmount = couponDto.DiscountAmount;
				existing.MinAmount = couponDto.MinAmount;

				_db.SaveChanges();

				//Coupon obj = _mapper.Map<Coupon>(couponDto);
    //            _db.Coupons.Update(obj);
    //            _db.SaveChanges();

                _response.Result = _mapper.Map<CouponDto>(existing);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpDelete]
        [Route("{id:int}")]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Delete(int id)
        {
            try
            {
                Coupon obj = _db.Coupons.First(u => u.CouponId == id);
                _db.Coupons.Remove(obj);
                _db.SaveChanges();

                var service = new Stripe.CouponService();
                service.Delete(obj.CouponCode);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }
    }
}
