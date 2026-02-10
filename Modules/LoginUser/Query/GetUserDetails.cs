using MediatR;
using Microsoft.EntityFrameworkCore;
using MyPartFolioAPI.Context;

namespace MyPartFolioAPI.Modules.Users.Query;

public class GetUserDetails
{
    public class GetUserDetailsQuery : IRequest<(Models.Users, string)>
    {
        public int UserId { get; set; }
    }

    public class GetUserDetailsQueryHandler : IRequestHandler<GetUserDetailsQuery, (Models.Users, string)>
    {
        private readonly DBContext DBContext;
        public GetUserDetailsQueryHandler(DBContext dbContext )
        {
            DBContext = dbContext;
        }
        public async Task<(Models.Users, string)> Handle(GetUserDetailsQuery query, CancellationToken cancellationToken)
        {
            try
            {
                (Models.Users, string) result;
                result.Item1 = new Models.Users();
                result.Item2 = string.Empty;

                result.Item1 = await DBContext.users.FirstOrDefaultAsync(c => ( c.UserId == query.UserId));

                if (result.Item1 == null ||
                    result.Item1.UserId <= 0)
                    result.Item2 = "User does not exist.";

                return result;
            }
            catch (Exception ex)
            {
                return (null, $"An error occurred: {ex.Message}");
            }
        }
    }
}
