using ScrumBoard.Domain.Entities;

namespace ScrumBoard.Application.Ports
{
    public interface IUserRepository: IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
    }
}
