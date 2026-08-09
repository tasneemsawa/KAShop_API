using KASHOP.BLL.Common;
using KASHOP.DAL.Dto;
using KASHOP.DAL.Models;
using Mapster;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Configuration;
namespace KASHOP.BLL.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _config;
        public AuthenticationService(UserManager<ApplicationUser> userManager, IEmailSender emailSender, IConfiguration config)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            _config = config;
        }
        public async Task<RegisterResponse> RegisterAsync(RegisterRequest request)
        {
            var user = request.Adapt<ApplicationUser>();
            var result = await _userManager.CreateAsync(user, request.Password);
            foreach (var item in result.Errors)
            {
                Console.WriteLine(item.Description);
            }

            if (!result.Succeeded)
                return new RegisterResponse()
                {
                    Message = "User registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            //this will convert each character in the token to its ASCII value and then convert it to a string representation of that value, separated by dashes.
            token = Uri.EscapeDataString(token);
            var emailUrl = $"http://localhost:5276/api/Account/ConfirmEmail?token={token}&userId={user.Id}";
            await _emailSender.SendEmailAsync(
                request.Email,
                "Welcome to KASHOP",
$@"
<div style='background-color: #f4f6f8; padding: 40px 10px; font-family: -apple-system, BlinkMacSystemFont, ""Segoe UI"", Roboto, Helvetica, Arial, sans-serif;'>
    <div style='max-width: 560px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 12px rgba(0, 0, 0, 0.05); border: 1px solid #e1e4e8;'>
        
        <!-- Header / Banner -->
        <div style='background-color: #DB4444; padding: 32px 20px; text-align: center;'>
            <h1 style='color: #ffffff; margin: 0; font-size: 26px; font-weight: 700; letter-spacing: 0.5px;'>
                Welcome to KASHOP 🎉
            </h1>
        </div>

        <!-- Body Content -->
        <div style='padding: 32px 28px; text-align: left;'>
            <p style='font-size: 16px; color: #2d3748; line-height: 1.6; margin-top: 0;'>
                Thank you for registering with <strong>KASHOP</strong>! We're excited to have you on board.
            </p>

            <p style='font-size: 15px; color: #4a5568; line-height: 1.6;'>
                Please verify your email address to complete your setup and activate your account:
            </p>

            <!-- Call to Action Button -->
            <div style='text-align: center; margin: 36px 0;'>
                <a href='{emailUrl}'
                   style='background-color: #DB4444;
                          color: #ffffff;
                          text-decoration: none;
                          padding: 14px 32px;
                          border-radius: 8px;
                          display: inline-block;
                          font-weight: 600;
                          font-size: 16px;
                          box-shadow: 0 4px 6px rgba(219, 68, 68, 0.25);'>
                    Confirm Email Address
                </a>
            </div>

            <p style='font-size: 13px; color: #718096; line-height: 1.5; margin-bottom: 0;'>
                If you didn't create an account with us, you can safely ignore this email.
            </p>
        </div>

        <!-- Footer -->
        <div style='background-color: #f8fafc; padding: 20px; border-top: 1px solid #edf2f7; text-align: center;'>
            <p style='font-size: 12px; color: #a0aec0; margin: 0;'>
                © 2026 <strong>KASHOP</strong>. All rights reserved.
            </p>
        </div>

    </div>
</div>"
            );

            return new RegisterResponse()
            {
                Message = "User registration successful, Thank you"
            };
        }

        public async Task<bool> ConfirmEmail(ConfirmEmailRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user is null) return false;
            // Decode the token from the query string(return to its original shape)
            request.Token = Uri.UnescapeDataString(request.Token);
            var result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (!result.Succeeded) return false;
            return true;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);

            if (user is null)
            {
                return new LoginResponse()
                {
                    Message = "Invalid Email"
                };
            }
            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new LoginResponse()
                {
                    Message = "Email not confirmed"
                };
            }

            var result = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!result)
            {
                return new LoginResponse()
                {
                    Message = "Invalid Password"
                };
            }

            return new LoginResponse()

            {
                Message = "Login successful",
                AccessToken = await GenerateJwt(user)
            };
        }


        private async Task<string> GenerateJwt(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var userClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, string.Join(",",roles))
            };
            var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["ApiSettings:SecretKey"]));

            var creds = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["ApiSettings:issuer"],
                audience: _config["ApiSettings:audience"],
                claims: userClaims,
                expires: DateTime.Now.AddDays(20),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);


        }
    }
}