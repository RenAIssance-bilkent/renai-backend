﻿namespace MelodyMuseAPI_DotNet8.Dtos
{
    public class UserRegistrationDto
    {
        public string Name { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
    }

}