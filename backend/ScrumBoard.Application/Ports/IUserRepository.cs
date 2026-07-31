using ScrumBoard.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrumBoard.Application.Ports
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
