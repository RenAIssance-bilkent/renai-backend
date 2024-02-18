namespace MelodyMuseAPI_DotNet8.Dtos
{
    public class UserTokenDto
    {
        public string Token { get; set; } 
        public DateTime ExpiryDate { get; set; }
        public string Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }

}
