﻿using System.ComponentModel.DataAnnotations;

namespace CourseManagment.DTOs
{
	public class RegisterUserDto
	{
		[Required]
		public string UserName { get; set; }
		[Required, EmailAddress]
		public string Email { get; set; }
		[Required, MinLength(6)]
		public string Password { get; set; }
	}
}
