﻿using System.ComponentModel.DataAnnotations;

namespace StudentHub.Api.DTOs.Requests
{
    public class UpdatePostRequest
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
    }
}
