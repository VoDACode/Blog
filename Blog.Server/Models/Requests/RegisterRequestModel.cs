﻿using System.ComponentModel.DataAnnotations;

namespace Blog.Server.Models.Requests
{
    public class RegisterRequestModel
    {
        [Required]
        [MaxLength(64)]
        public string Username { get; set; } = null!;
        [Required]
        public string Password { get; set; } = null!;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }
}
