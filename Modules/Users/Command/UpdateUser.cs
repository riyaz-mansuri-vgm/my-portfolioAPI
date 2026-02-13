using MediatR;
using MyPartFolioAPI.Context;

namespace MyPartFolioAPI.Modules.Users.Command;

public class UpdateUser
{
    public class UpdateUserCommand : IRequest<bool>
    {
        public Models.Users users { get; set; }
    }
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, bool>
    {
        private readonly DBContext DBContext;
        public UpdateUserCommandHandler(DBContext dbContext)
        {
            DBContext = dbContext;
        }
        public async Task<bool> Handle(UpdateUserCommand command, CancellationToken cancellationToken)
        {
            command.users.Created_At = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);
            command.users.Update_Date = DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc);


            DBContext.users.Update(command.users);
            await DBContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
