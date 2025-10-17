using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.DTOs.Commands
{
    public class CreateProjectCommand
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string? Url { get; set; }
        public Guid AuthorId { get; set; }
        public List<string> FilePaths { get; set; }
    }
}
