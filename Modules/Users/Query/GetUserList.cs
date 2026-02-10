using MediatR;
using Microsoft.EntityFrameworkCore;
using MyPartFolioAPI.Context;
using MyPartFolioAPI.Modules.DataProtection.Services;
using MyPartFolioAPI.Modules.Users.Models.Response;

namespace MyPartFolioAPI.Modules.Users.Query;
public class GetUserList
{
    public class GetUserListQuery : IRequest<(List<GetUserListResponseModel>, string, int)>
    {
    }
    public class GetUserListQueryHandler : IRequestHandler<GetUserListQuery, (List<GetUserListResponseModel>, string, int)>
    {
        private readonly DBContext DBContext;
        private IDataProtectionService DataProtectionService { get; }
        public IConfiguration Configuration;
        public GetUserListQueryHandler(DBContext dbContext, IDataProtectionService dataProtectionService, IConfiguration configuration)
        {
            DBContext = dbContext;
            DataProtectionService = dataProtectionService;
            Configuration = configuration;
        }
        public async Task<(List<GetUserListResponseModel>, string, int)> Handle(GetUserListQuery query, CancellationToken cancellationToken)
        {
            (List<GetUserListResponseModel>, string, int) result;
            result.Item1 = new List<GetUserListResponseModel>();
            result.Item2 = string.Empty;
            result.Item3 = 0;
            var userList = await DBContext.users.ToListAsync();

            if (userList.Count() > 0)
            {
                result.Item1 = userList.Select(x => new GetUserListResponseModel()
                {
                    EmcyptedUserId = DataProtectionService.GetProtectedValue(x.UserId.ToString()),
                    UserId = x.UserId,
                    UserName = x.UserName,
                    UserEmail = x.Email,
                    MobileNumber = x.MobileNumber,
                    Address = x.Address,
                }).ToList();
            }
            else
            {
                result.Item2 = "List data not found!";
            }
            return result;
        }
    }
}

