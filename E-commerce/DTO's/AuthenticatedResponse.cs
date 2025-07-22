namespace E_commerce.DTO_s
{
    public class AuthenticatedResponse
    {
        public string? Token { get; set; }
        public string RefreshToken { get; set; }
        public  string? Message { get; set; }
    }
}
