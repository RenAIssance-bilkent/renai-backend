namespace MelodyMuseAPI_DotNet8.Dtos
{
    public class UserResetPasswordDto
    {
        //TODO: Implemet after aquiering corporate email
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

}
