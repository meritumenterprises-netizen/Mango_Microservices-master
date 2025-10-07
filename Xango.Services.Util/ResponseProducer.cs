using Xango.Models.Dto;


namespace Xango.Services.Utility
{
    public static class ResponseProducer
    {
        public static ResponseDto CreateResponse(bool isSuccess, string message, object? result = null)
        {
            return new ResponseDto
            {
                IsSuccess = isSuccess,
                Message = message,
                Result = result
            };
        }

        public static ResponseDto? OkResponse(object? result = null, string message = "Operation successful")
        {
            return CreateResponse(true, message, result);
        }   

        public static ResponseDto? OkResponse()
        {             
            return CreateResponse(true, "Operation successful", null);
        }   

        public static ResponseDto? ErrorResponse(string message, string stackTrace = "", object? result = null)
        {
            return new ResponseDto
            {
                IsSuccess = false,
                Message = message,
                StackTrace = stackTrace,
                Result = result
            };
        }
    }
}
