﻿using Microsoft.AspNetCore.Http;

namespace SEP490_FTCDHMM_API.Application.Dtos.UserDtos
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public IFormFile? Avatar { get; set; }

    }
}
