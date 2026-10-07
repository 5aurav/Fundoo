using BusinessLayer.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModelLayer;

namespace Fundoo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FundooController : ControllerBase
    {
        private readonly IUserBL _userBL;

        public FundooController(IUserBL userBL)
        {
            _userBL = userBL;
        }

        [HttpPost]
        public ActionResult<ResponseModel<RegistrationModel>> RegisterUser(
            [FromBody] RegistrationModel registrationModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ResponseModel<RegistrationModel>
                    {
                        success = false,
                        message = "Invalid input",
                        data = null
                    }
                );
            }

            var result = _userBL.RegisterUserBL(registrationModel);

            if (result == null)
            {
                return Conflict(
                    new ResponseModel<RegistrationModel>
                    {
                        success = false,
                        message = "Email already exists",
                        data = null
                    }
                );
            }

            var response = new ResponseModel<RegistrationModel>
            {
                success = true,
                message = "User Registered Successfully",
                data = result
            };

            return CreatedAtAction(nameof(RegisterUser), null, response);
        }

        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ResponseModel<string>
                    {
                        success = false,
                        message = "Invalid input",
                        data = string.Empty
                    }
                );
            }

            var token = _userBL.LoginUserBL(loginModel);

            if (token == null)
            {
                return Unauthorized(
                    new ResponseModel<string>
                    {
                        success = false,
                        message = "Invalid credentials",
                        data = string.Empty
                    }
                );
            }

            return Ok(
                new ResponseModel<string>
                {
                    success = true,
                    message = "Login successful",
                    data = token
                }
            );
        }
        [HttpPost("forgotpassword")]
        public async Task<IActionResult> ForgotPassword(
            [FromBody] ForgotPasswordModel forgotPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(
                    new ResponseModel<string>
                    {
                        success = false,
                        message = "Invalid input",
                        data = string.Empty
                    }
                );
            }

            await _userBL.ForgotPasswordBL(
                forgotPasswordModel
            );

            return Ok(
                new ResponseModel<string>
                {
                    success = true,
                    message = "If an account exists for this email, a password reset email has been sent.",
                    data = string.Empty
                }
            );
        }
        [Authorize]
        [HttpPost("resetpassword")]
        public IActionResult ResetPassword(
             ResetPasswordModel resetPasswordModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _userBL.ResetPasswordBL(
                resetPasswordModel
            );

            if (!result)
            {
                return BadRequest("Invalid or expired reset token");
            }

            return Ok("Password reset successfully");
        }
    }
}