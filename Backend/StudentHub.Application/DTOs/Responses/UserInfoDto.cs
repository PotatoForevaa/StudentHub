using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentHub.Application.DTOs.Responses
{
    public record UserInfoDto(
        string FullName,
        string Username
        );
}
