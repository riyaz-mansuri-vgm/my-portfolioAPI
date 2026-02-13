using MediatR;
using Microsoft.EntityFrameworkCore;
using MyPartFolioAPI.Context;

namespace MyPartFolioAPI.Modules.Users.Command;
public class DeleteUserDetail
{
    public class DeleteUserDetailCommand : IRequest<bool>
    {
        public int UserId { get; set; }
    }
    public class DeleteUserDetailCommandHandler : IRequestHandler<DeleteUserDetailCommand, bool>
    {
        private readonly DBContext DBContext;
        public DeleteUserDetailCommandHandler(DBContext dBContext)
        {
            DBContext = dBContext;
        }
        public async Task<bool> Handle(DeleteUserDetailCommand command, CancellationToken cancellationToken)
        {
            var deleteUser = await DBContext.users.FirstOrDefaultAsync(x => x.UserId == command.UserId);
            if (deleteUser == null)
            {
                return false;
            }
            else
            {
                DBContext.users.Remove(deleteUser);
                await DBContext.SaveChangesAsync(cancellationToken);
                return true;

            }
        }

    }
}
