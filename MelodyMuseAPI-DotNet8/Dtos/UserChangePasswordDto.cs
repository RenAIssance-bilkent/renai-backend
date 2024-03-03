namespace MelodyMuseAPI.Dtos
{
    public class UserChangePasswordDto
    {
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
    }

}
