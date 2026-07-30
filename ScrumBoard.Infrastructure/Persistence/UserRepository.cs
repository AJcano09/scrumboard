using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;

namespace ScrumBoard.Infrastructure.Persistence
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ScrumDbContext context) : base(context)
        {  
        }

        public async Task<User?> GetByEmailAsync(string email)=> 
           await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
