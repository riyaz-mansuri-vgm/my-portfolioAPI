using MediatR;
using MyPartFolioAPI.Context;

namespace MyPartFolioAPI.Modules.Users.Command;

public class AddUser
{
    public class AddUserCommand : IRequest<int>
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Address { get; set; }
    }
    public class AddUserCommandHandler : IRequestHandler<AddUserCommand, int>
    {
        private readonly DBContext DBContext;
        public AddUserCommandHandler(DBContext dbContext)
        {
            DBContext = dbContext;
        }
        public async Task<int> Handle(AddUserCommand command, CancellationToken cancellationToken)
        {
            var addUserDetail = new Models.Users()
            {
                UserName = command.UserName,
                Email = command.Email,
                MobileNumber = command.MobileNumber,
                Address = command.Address,
                Created_At = DateTime.UtcNow,
                Update_Date = DateTime.UtcNow,
                Status = true
            };
            await DBContext.AddAsync(addUserDetail, cancellationToken);
            await DBContext.SaveChangesAsync(cancellationToken);
            return addUserDetail.UserId;

        }
    }
}
