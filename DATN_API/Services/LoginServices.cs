﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DATN.IServices;
using DATN_Shared.Models;
using DATN_Shared.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DATN.Services
{
    public class LoginServices : ILoginServices
    {
        private readonly UserManager<User> _userManager;

        private readonly IConfiguration _configuration;
        
        public LoginServices(UserManager<User> userManager,IConfiguration configuration)
        {
            _userManager = userManager;
           
            _configuration = configuration;
        }
    
        public async Task<Response> LoginAsync(LoginUser userLogin)
        {
            var user = await _userManager.FindByNameAsync(userLogin.UserName);
            if (user == null)
            {
                return new Response
                {
                    IsSuccess= false,
                    Message= "username không tồn tại",
                    StatusCode = 400
                };
            }
            else if (!await _userManager.CheckPasswordAsync(user,userLogin.Password))
            {
                return new Response
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Message = "mat khau sai"
                };
            }
            else
            {
                //tạo danh sách claim của jwt
                var roles = await _userManager.GetRolesAsync(user);
                var claims = new List<Claim>()
                {
                    new Claim("Id",user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier,user.UserName.ToString()),
                    new Claim(ClaimTypes.Email,user.Email.ToString()),
                    new Claim(ClaimTypes.Role,roles.FirstOrDefault())

                };
                // tạo jwt token
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secretkey"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
                var token = new JwtSecurityToken(

                    claims:claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: creds
                                        
                    );
                return new Response
                {
                    IsSuccess = true,
                    StatusCode = StatusCodes.Status200OK,
                    Message = "dang nhap thanh cong",
                    Token = new JwtSecurityTokenHandler().WriteToken(token)
                };
            }



        }
    }
}
