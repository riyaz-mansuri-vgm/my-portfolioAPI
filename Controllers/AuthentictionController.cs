using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Cors;

using MyPartFolioAPI.Models;
using MyPartFolioAPI.Modules.DataProtection.Services;
using MyPartFolioAPI.Modules.LoginUser.Models.Request;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using static MyPartFolioAPI.Modules.LoginUser.Query.CheckSecurityUser;

namespace MyPartFolioAPI.Controllers;
[Route("api")]
[EnableCors("AllowReact")]
public class AuthentictionController : ControllerBase
{
    private IMediator Mediator { get; set; }
    private readonly IConfiguration Configuration;
    private IDataProtectionService DataProtectionService { get; }
    public AuthentictionController(IMediator mediator, IConfiguration configuration, IDataProtectionService dataProtectionService)
    {
        Mediator = mediator;
        Configuration = configuration;
        DataProtectionService = dataProtectionService;
    }

    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestModel request)
    {
        ResponseModel response = new ResponseModel();

        try
        {
            if (string.IsNullOrEmpty(request.UserId))
            {
                response.Message = "Please provide user name, email or phone number.";
            }
            else if (string.IsNullOrEmpty(request.LoginPassword))
            {
                response.Message = "Please provide password.";
            }

            if (!string.IsNullOrEmpty(response.Message))
                return Ok(response);

            var result = await Mediator.Send(new CheckSecurityUserQuery
            {
                UserId = request.UserId,
                LoginPassword = request.LoginPassword
            });

            if (result.Item1 == null || !string.IsNullOrEmpty(result.Item2))
            {
                response.Message = result.Item2;
                return Ok(response);
            }

            if (result.Item1 != null && string.IsNullOrEmpty(result.Item1.Password))
            {
                //string encryptedUserId = DataProtectionService.GetProtectedValue(result.Item1.UserId.ToString());
                //var resetPasswordPageUrl = $"{Configuration["Settings:WebsiteUrl"]}reset-password/{encryptedUserId}";

                //var clientDetail = await Mediator.Send(new GetClientDetailQuery { ClientCode = result.Item1.Client_Code });

                //var emailPlaceholders = new EmailPlaceholderModel();
                //emailPlaceholders.URL = resetPasswordPageUrl;
                //emailPlaceholders.Username = result.Item1.Login_User_Name;
                //emailPlaceholders.ClientName = clientDetail.Client_Name;

                //string serializedString = JsonConvert.SerializeObject(emailPlaceholders);

                //BackgroungJobClient.Enqueue(() => EmailService.SendMail("Welcome to Khalihan", result.Item1.Login_User_EmailId, EmailFor.Welcome, serializedString, null, ""));
                //response.Data = new { emailSent = "true" };
                //response.Message = "Seems password is not setup yet in your account, please check your email and set your password to login!";
                //return Ok(response);
            }

            //var transactionDetails = await Mediator.Send(new GetClientRegistrationAuthorizationListQuery
            //{
            //    ClientCode = result.Item1.Client_Code
            //});

            //if (transactionDetails.Item1 != null &&
            //    transactionDetails.Item1.Count() > 0)
            //{
            //    int ongoingSubscriptionCount = transactionDetails.Item1.Count(x => x.EndDate >= DateTime.Now.Date);

            //    if (ongoingSubscriptionCount <= 0)
            //    {
            //        DateTime maxExpiryDate = transactionDetails.Item1.Max(details => details.EndDate);
            //        response.Message = string.Format("Your subscription was expired on {0:dd-MMM-yyyy}, please renew your subscription.", maxExpiryDate);
            //        return Ok(response);
            //    }
            //}
            //else
            //{
            //    response.Message = "You do not have any subscription, please contact Administrator.";
            //    return Ok(response);
            //}

            //await Mediator.Send(new UpdateSecurityUserCommand { User = result.Item1 });

            //var claims = new[] {
            //            new Claim(JwtRegisteredClaimNames.Sub, Configuration["Jwt:Subject"]),
            //            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            //            new Claim(JwtRegisteredClaimNames.Iat, DateTime.Now.ToString()),
            //            new Claim("UserId", result.Item1.UserId.ToString()),
            //            new Claim("UserName", result.Item1.UserName),
            //            new Claim("Email", result.Item1.Email),
            //            new Claim("MobileNumber", result.Item1.MobileNumber),
            //            new Claim(ClaimTypes.Role, "Client")
            //        };

            //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]));
            //var signInCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            //var token = new JwtSecurityToken(
            //    Configuration["Jwt:Issuer"],
            //    Configuration["Jwt:Audience"],
            //    claims,
            //    expires: DateTime.Now.AddYears(1),
            //    signingCredentials: signInCredentials);

            string encryptedSecurityUserId = DataProtectionService.GetProtectedValue(result.Item1.UserId.ToString());
            //string encryptedClientCode = DataProtectionService.GetProtectedValue(result.Item1.Client_Code);
            //string encryptedCompanyCode = DataProtectionService.GetProtectedValue(result.Item1.Company_Code);

            //var client = await Mediator.Send(new GetClientDetailQuery
            //{
            //    ClientCode = result.Item1.Client_Code
            //});

            response.Status = (int)ResponseCode.Success;
            response.Data = new
            {
                UserId = result.Item1.UserId,
                EncryptedSecurityUserId = encryptedSecurityUserId,
                //EncryptedClientCode = encryptedClientCode,
                Name = result.Item1.UserName,
                LoginUserName = result.Item1.UserId,
                LoginUserEmailId = result.Item1.Email,
                LoginUserMobileNumber = result.Item1.MobileNumber,
                //token = new JwtSecurityTokenHandler().WriteToken(token),
       
            };
            response.Message = "Login successful!";
        }
        catch
        {
            response.Status = (int)ResponseCode.Failure;
            response.Message = "Something went wrong while login, please contact Administrator.";
        }

        return Ok(response);
    }
}
