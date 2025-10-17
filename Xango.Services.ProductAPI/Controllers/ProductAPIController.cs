using AutoMapper;
using Xango.Services.ProductAPI.Data;
using Xango.Services.ProductAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Xango.Models.Dto;
using Xango.Services.Client.Utility;
using Xango.Services.Utility;
using System.IO;

namespace Xango.Services.ProductAPI.Controllers
{
    [Route("api/product")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly AppDbContext _db;
        private new ResponseDto _response;
        private IMapper _mapper;
        private readonly IConfiguration _configuration;

        public ProductAPIController(AppDbContext db, IMapper mapper, IConfiguration configuration)
        {
            _db = db;
            _mapper = mapper;
            _response = new ResponseDto();
            _configuration = configuration;
        }

        [HttpGet]
        public ResponseDto Get()
        {
            try
            {
                IEnumerable<Product> objList = _db.Products.ToList();
                _response.Result = _mapper.Map<IEnumerable<ProductDto>>(objList);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpGet]
        [Route("{id:int}")]
        public ResponseDto Get(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                _response.Result = _mapper.Map<ProductDto>(obj);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

        [HttpPost]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Post([FromBody] ProductDto ProductDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(ProductDto);
                _db.Products.Add(product);
                _db.SaveChanges();

                if (!string.IsNullOrEmpty(ProductDto.Base64Image))
                {
                    string fileName = product.ProductId + ".jpg";
                    string filePath = Directory.GetCurrentDirectory() + "/wwwroot/ProductImages/";

                    try
                    {
                        if (System.IO.File.Exists(filePath + fileName))
                        {
                            System.IO.File.Delete(filePath + fileName);
                        }
                        System.IO.File.WriteAllBytes(filePath + fileName, Convert.FromBase64String(ProductDto.Base64Image));
                    }
                    catch
                    {

                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }
                else
                {
                    product.ImageUrl = "https://placehold.co/600x400";
                }
                _db.Products.Update(product);
                _db.SaveChanges();
                _response.Result = _mapper.Map<ProductDto>(product);
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }


        [HttpPut]
        [Authorize(Roles = "ADMIN")]
        public ResponseDto Put([FromBody] ProductDto ProductDto)
        {
            try
            {
                Product product = _mapper.Map<Product>(ProductDto);

                if (!string.IsNullOrEmpty(ProductDto.Base64Image))
                {
                    string fileName = product.ProductId + ".jpg";
                    string filePath = Directory.GetCurrentDirectory() + "/wwwroot/ProductImages/";
                    if (System.IO.File.Exists(filePath + fileName))
                    {
                        System.IO.File.Delete(filePath + fileName);
                    }

                    try
                    {
                        System.IO.File.WriteAllBytes(filePath + fileName, Convert.FromBase64String(ProductDto.Base64Image));
                    }
                    catch
                    {

                    }
                    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.Value}{HttpContext.Request.PathBase.Value}";
                    product.ImageUrl = baseUrl + "/ProductImages/" + fileName;
                    product.ImageLocalPath = filePath;
                }


                _db.Products.Update(product);
                _db.SaveChanges();

                _response.Result = _mapper.Map<ProductDto>(product);
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
        public async Task<ResponseDto> Delete(int id)
        {
            try
            {
                Product obj = _db.Products.First(u => u.ProductId == id);
                if (!string.IsNullOrEmpty(obj.ImageLocalPath))
                {
                    var oldFilePathDirectory = Path.Combine(Directory.GetCurrentDirectory(), obj.ImageLocalPath);
                    FileInfo file = new FileInfo(oldFilePathDirectory);
                    if (file.Exists)
                    {
                        file.Delete();
                    }
                }
                _db.Products.Remove(obj);
                _db.SaveChanges();
                _response.IsSuccess = true;
                _response.Message = "Product deleted successfully";
            }
            catch (Exception ex)
            {
                return ResponseProducer.ErrorResponse(ex.Message);
            }
            return _response;
        }

    }
}
