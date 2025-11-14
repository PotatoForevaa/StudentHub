using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.DTOs
{
    public class Error
    {
        public string Message { get; set; } = default!;
        public string? Field { get; set; }
    }
}
