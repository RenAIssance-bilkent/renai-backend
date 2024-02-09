namespace MelodyMuseAPI_DotNet8.Dtos
{
    public class UserTokenDto
    {
        public string Token { get; set; } // holds the JWT (or another form of token)
        public DateTime ExpiryDate { get; set; }
        public string Username { get; set; }
        public string UserId { get; set; }
    }

}
