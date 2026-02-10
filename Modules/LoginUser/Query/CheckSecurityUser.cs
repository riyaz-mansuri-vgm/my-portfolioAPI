using MediatR;
using Microsoft.EntityFrameworkCore;
using MyPartFolioAPI.Context;
using MyPartFolioAPI.Modules.LoginUser.Models;

namespace MyPartFolioAPI.Modules.LoginUser.Query;
public class CheckSecurityUser
{
    public class CheckSecurityUserQuery : IRequest<(LoginUsers, string)>
    {
        public string UserId { get; set; }
        public string LoginPassword { get; set; }
    }

    public class CheckSecurityUserQueryHandler : IRequestHandler<CheckSecurityUserQuery, (LoginUsers, string)>
    {
        private readonly DBContext DBContext;
        //private IEncryptionService EncryptionService { get; }

        public CheckSecurityUserQueryHandler(DBContext dbContext
            //IEncryptionService encryptionService
            )
        {
            DBContext = dbContext;
            //EncryptionService = encryptionService;
        }

        public async Task<(LoginUsers, string)> Handle(CheckSecurityUserQuery query, CancellationToken cancellationToken)
        {
            try
            {
                (LoginUsers, string) result;
                result.Item1 = new LoginUsers();
                result.Item2 = string.Empty;

                result.Item1 = await DBContext.LoginUsers.FirstOrDefaultAsync(c => (!string.IsNullOrEmpty(c.UserName) ? c.UserName == query.UserId : false) ||
                                                                                      (!string.IsNullOrEmpty(c.Email) ? c.Email == query.UserId : false) ||
                                                                                      (!string.IsNullOrEmpty(c.MobileNumber) ? c.MobileNumber == query.UserId : false));

                if (result.Item1 == null ||
                    result.Item1.UserId <= 0)
                    result.Item2 = "User does not exist.";
                //else if (result.Item1.Login_Pwd_Hash != EncryptionService.CreateHash(query.LoginPassword, result.Item1.Login_Pwd_Salt))
                //    result.Item2 = "Invalid password.";

                return result;
            }
            catch (Exception ex)
            {
                return (null, $"An error occurred: {ex.Message}");
            }
        }
    }
}
