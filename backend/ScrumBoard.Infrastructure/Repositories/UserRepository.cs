using Microsoft.EntityFrameworkCore;
using ScrumBoard.Application.Ports;
using ScrumBoard.Domain.Entities;
using ScrumBoard.Infrastructure.Persistence;

namespace ScrumBoard.Infrastructure.Repositories
{
    public class UserRepository(ScrumBoardDbContext context) : Repository<User>(context), IUserRepository
    {
        private readonly ScrumBoardDbContext _context =context;
        public async Task<User?> GetByEmailAsync(string email)=> 
           await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }
}
