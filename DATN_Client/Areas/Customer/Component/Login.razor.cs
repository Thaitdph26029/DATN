﻿using DATN_Shared.Models;
using DATN_Shared.ViewModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DATN_Client.Areas.Customer.Component
{
	public partial class Login
	{
		[Inject] Blazored.SessionStorage.ISessionStorageService _SessionStorageService { get; set; }
		HttpClient _httpClient = new HttpClient();
		public string Message { get; set; } = null;
		HttpContext HttpContext { get; set; }
		[Inject ] NavigationManager navigationManager { get; set; }
		public List<User> Users { get; set; }
		LoginUser loginUser = new LoginUser();
		public string Idsession { get; set; } = string.Empty;
		public async Task login()
		{
			var response = await _httpClient.PostAsJsonAsync<LoginUser>("https://localhost:7141/api/user/login/", loginUser);
			if (response.IsSuccessStatusCode)
			{

				var token = await response.Content.ReadAsStringAsync();

				var handler = new JwtSecurityTokenHandler();
				var jwt = handler.ReadJwtToken(token);

				// tạo đối tượng xác thực
				var claims = new List<Claim>
				{
					new Claim("Id", jwt.Claims.FirstOrDefault(u => u.Type == "Id").Value),
					new Claim(ClaimTypes.NameIdentifier, jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.NameIdentifier).Value),
					new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role).Value)
				};
				var identity = new ClaimsIdentity(claims, JwtBearerDefaults.AuthenticationScheme);
				var principal = new ClaimsPrincipal(identity);

				var data = claims.Select(c => c.Value).ToArray();
				var id = data[0];
                
                
                try
				{
					_SessionStorageService.SetItemAsStringAsync("session", id);
				}
				catch (Exception ex)
				{

				}

				_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
				var responseAuthorize = await _httpClient.GetAsync("https://localhost:7141/api/user/get-user");
				if (principal.IsInRole("Admin"))
				{
                    navigationManager.NavigateTo("https://localhost:7075/Admin", true);
                }
				else
				{
					navigationManager.NavigateTo("https://localhost:7075/Customer", true);
				}

			}
			else
			{
				Message = "fail";
			}
		}
	}
}
