using ScrumBoard.Application.Common;
using ScrumBoard.Application.Ports;

namespace ScrumBoard.Application.Users;
public class UserService(IUserRepository userRepository)
{
    public async Task<PagedResult<UserDto>> GetAllPagedAsync(int pageNumber, int pageSize)
    {
        var (items,totalCount) = await userRepository.GetAllPagedAsync(pageNumber, pageSize);
        
        var userDtos = items
            .Select(u => new UserDto { Id = u.Id, Name = u.Name, Email = u.Email })
            .ToList();
        return new PagedResult<UserDto>
        {
            Items = userDtos,
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }
}