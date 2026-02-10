using MediatR;
using Microsoft.EntityFrameworkCore;
using MyPartFolioAPI.Context;

namespace MyPartFolioAPI.Modules.Users.Query;
public class UserDetailExist
{
    public class UserDetailExistQuery : IRequest<(Models.Users, string)>
    {
        public int UserId { get; set; }
    }
    public class UserDetailExistQueryHandler : IRequestHandler<UserDetailExistQuery, (Models.Users, string)>
    {
        private readonly DBContext DBContext;
        public UserDetailExistQueryHandler(DBContext dBContext)
        {
            DBContext = dBContext;
        }
        public async Task<(Models.Users, string)> Handle(UserDetailExistQuery query, CancellationToken cancellationToken)
        {
            (Models.Users, string) result;
            result.Item1 = new Models.Users();
            result.Item2 = string.Empty;

            if (query.UserId != null)
            {

                result.Item1 = await DBContext.users.FirstOrDefaultAsync(x => x.UserId == query.UserId);
            }
            else
            {
                result.Item2 = "User detail does not exists.";
            }
            return result;
        }
    }
}
