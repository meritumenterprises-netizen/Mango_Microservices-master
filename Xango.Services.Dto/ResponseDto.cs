namespace Xango.Models.Dto
{
    public class ResponseDto
    {
        public object? Result { get; set; } = null!;
        public bool IsSuccess { get; set; } = true;
        public string? Message { get; set; } = "";
        public string? StackTrace { get; set; } = "";
    }
}
