using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.DTOs.Responses
{
    public class PostDto
    {
        public int Id { get; set; }
        public int Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public float RatingAvg { get; set; }

    }
}
