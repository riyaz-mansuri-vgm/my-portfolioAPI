using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyPartFolioAPI.Models;
using MyPartFolioAPI.Modules.DataProtection.Services;
using MyPartFolioAPI.Modules.Users.Models.Request;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static MyPartFolioAPI.Modules.Users.Command.AddUser;
using static MyPartFolioAPI.Modules.Users.Command.DeleteUserDetail;
using static MyPartFolioAPI.Modules.Users.Command.UpdateUser;
using static MyPartFolioAPI.Modules.Users.Query.GetUserDetails;
using static MyPartFolioAPI.Modules.Users.Query.GetUserList;
using static MyPartFolioAPI.Modules.Users.Query.UserDetailExist;

namespace MyPartFolioAPI.Controllers;

[Route("api")]
[EnableCors("AllowReact")]
//[Authorize(Roles = "Client")]
public class UsersController : ControllerBase
{
    private IMediator Mediator { get; set; }
    private IWebHostEnvironment WebHostEnvironment { get; }
    private IDataProtectionService DataProtectionService { get; }
    public UsersController(IMediator mediator, IDataProtectionService dataProtectionService, IWebHostEnvironment webHostEnvironment)
    {
        Mediator = mediator;
        DataProtectionService = dataProtectionService;
        WebHostEnvironment = webHostEnvironment;

    }

    [HttpPost]
    [Route("add-user-details")]
    public async Task<IActionResult> AddUserDetails([FromBody] AddUserCommand command)
    {
        ResponseModel response = new ResponseModel();
        try
        {
            if (string.IsNullOrWhiteSpace(command.UserName))
            {
                response.Message = "UserName is required.";
            }
            else if (string.IsNullOrWhiteSpace(command.Email))
            {
                response.Message = "Email is required.";
            }
            else if (string.IsNullOrWhiteSpace(command.MobileNumber))
            {
                response.Message = "Mobile number is requeired.";
            }
            else if (!string.IsNullOrWhiteSpace(command.MobileNumber) && command.MobileNumber.Length != 10)
            {
                response.Message = "Mobile number field should be 10 digit";
            }
            if (!string.IsNullOrWhiteSpace(response.Message))
            {
                return Ok(response);
            }
            var userId = await Mediator.Send(command);
            if (userId != null)
            {
                response.Status = (int)ResponseCode.Success;
                response.Data = new { userId = userId, encryptedUserId = DataProtectionService.GetProtectedValue(userId.ToString()) };
                response.Message = "User detail added successfully!";
            }
            else
            {
                response.Message = "Something went wrong while adding user details, please contact Administrator.";
                response.Status = (int)ResponseCode.Failure;
                return Ok(response);
            }

        }
        catch
        {
            response.Status = (int)ResponseCode.Failure;
            response.Message = "Something went wrong, please contact Administrator.";
        }

        return Ok(response);
    }

    [HttpPost]
    [Route("update-user-detail")]
    public async Task<IActionResult> UpdateUserDetail([FromBody] UpdateUseDetailRequestModel request)
    {
        ResponseModel response = new ResponseModel();
        try
        {
            if (request.EncryptedUserId == null || string.IsNullOrWhiteSpace(request.EncryptedUserId))
            {
                response.Message = "Invalid request.";
            }
            else if (string.IsNullOrWhiteSpace(request.UserName))
            {
                response.Message = "UserName is required.";
            }
            else if (string.IsNullOrWhiteSpace(request.Email))
            {
                response.Message = "Email is required.";
            }
            else if (string.IsNullOrWhiteSpace(request.MobileNumber))
            {
                response.Message = "Mobile number is requeired.";
            }
            else if (!string.IsNullOrWhiteSpace(request.MobileNumber) && request.MobileNumber.Length != 10)
            {
                response.Message = "Mobile number field should be 10 digit";
            }
            if (!string.IsNullOrWhiteSpace(response.Message))
            {
                return Ok(response);
            }

            var userExist = await Mediator.Send(new UserDetailExistQuery
            {
                UserId = DataProtectionService.GetPlainValue<int>(request.EncryptedUserId)
            });

            if (!string.IsNullOrWhiteSpace(userExist.Item2))
            {
                response.Message = userExist.Item2;
                return Ok(response);
            }
            userExist.Item1.UserName = request.UserName;
            userExist.Item1.MobileNumber = request.MobileNumber;
            userExist.Item1.Email = request.Email;
            userExist.Item1.Address = request.Address;

            var updateDetails = await Mediator.Send(new UpdateUserCommand { users = userExist.Item1 });
            if (updateDetails)
            {
                response.Message = "Update user details successfully!";
                response.Status = (int)ResponseCode.Success;
            }
            else
            {
                response.Message = "Something went wrong while update user details!";
                response.Status = (int)ResponseCode.Failure;
            }
        }
        catch
        {
            response.Status = (int)ResponseCode.Failure;
            response.Message = "Something went wrong, please contact Administrator.";
        }
        return Ok(response);
    }

    [HttpGet]
    [Route("get-users-list")]
    public async Task<IActionResult> GetUserList()
    {
        ResponseModel response = new ResponseModel();
        try
        {

            var userList = await Mediator.Send(new GetUserListQuery { });
            if (userList.Item1.Count > 0)
            {
                response.Status = (int)ResponseCode.Success;
                response.Data = new { UserList = userList.Item1 };
                response.Message = "User list fetch succesfully!.";
            }
            else
            {
                response.Message = "No user list found!.";
            }
        }
        catch
        {
            response.Status = (int)ResponseCode.Failure;
            response.Message = "Something went wrong, please contact Administrator.";
        }

        return Ok(response);
    }

    [HttpPost]
    //[Authorize(Roles = "ADMIN")]
    [Route("view-user-detail")]
    public async Task<IActionResult> GetUserDetails([FromBody] GetUserDetailRequestModel request)
    {
        ResponseModel response = new ResponseModel();
        try
        {
            if (string.IsNullOrEmpty(request.EncryptedUserId))
            {
                response.Message = "Invalid request.";
                return Ok(response);
            }
            var userId = DataProtectionService.GetPlainValue<int>(request.EncryptedUserId);

            var userDetails = await Mediator.Send(new GetUserDetailsQuery
            {
                UserId = userId,
            });

            if (userDetails.Item1 != null)
            {
                response.Status = (int)ResponseCode.Success;
                response.Data = userDetails.Item1;
            }
            else
            {
                response.Status = (int)ResponseCode.NoContent;
                response.Message = userDetails.Item2;
                return Ok(response);
            }
        }
        catch
        {
            response.Status = (int)ResponseCode.Failure;
            response.Message = "Something went wrong, please contact Administrator.";
        }
        return Ok(response);
    }

    [HttpDelete]
    [Route("delete-user-detail")]
    public async Task<IActionResult> DeleteUserDetail([FromBody] DeleteUserRequestModel request)
    {
        ResponseModel response = new ResponseModel();
        try
        {
            if (string.IsNullOrEmpty(request.EncryptedUserId))
            {
                response.Message = "Invalid request.";
                return Ok(response);
            }
            var userId = DataProtectionService.GetPlainValue<int>(request.EncryptedUserId);

            var userDetails = await Mediator.Send(new GetUserDetailsQuery
            {
                UserId = userId,
            });

            if (userDetails.Item1 == null)
            {
                response.Status = (int)ResponseCode.NoContent;
                response.Data = userDetails.Item1;
                return Ok(response);
            }
            var isDeleted = await Mediator.Send(new DeleteUserDetailCommand
            {
                UserId = userId,
            });
            if (!isDeleted)
            {
                response.Status = (int)ResponseCode.Failure;
                response.Message = "Something went wrong while deleting user details, please contact Administrator.";
                return Ok(response);
            }
            response.Status = (int)ResponseCode.Success;
            response.Message = "User details deleted successfully!";
        }
        catch
        {
            response.Status = (int)ResponseCode.Failure;
            response.Message = "Something went wrong, please contact Administrator.";
        }
        return Ok(response);
    }
}
