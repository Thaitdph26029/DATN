﻿using DATN_Shared.Models;
using DATN_Shared.ViewModel;
using Microsoft.AspNetCore.Components;

namespace DATN_Client.Areas.Customer.Component
{
	public partial class ChangePassword
	{
		HttpClient _client = new HttpClient();
		public string TypeInput_Pass { get; set; } = "password";
		public string Icon_Pass { get; set; } = "fa-regular fa-eye-slash";
		public string TypeInput_New { get; set; } = "password";
		public string Icon_New { get; set; } = "fa-regular fa-eye-slash";
		public string TypeInput_CheckNew { get; set; } = "password";
		public string Icon_CheckNew { get; set; } = "fa-regular fa-eye-slash";

		[Inject] public IHttpContextAccessor _ihttpcontextaccessor { get; set; }
		[Inject] NavigationManager _navigationManager { get; set; }
		[Inject] Blazored.Toast.Services.IToastService _toastService { get; set; } // Khai báo khi cần gọi ở code-behind


		public ChangePassword_VM changePassword = new ChangePassword_VM();
		public string Message { get; set; } = string.Empty;
		User User_VM = new User();
		List<User> _lstUser_VM = new List<User>();
		private string _checkPassCu { get; set; } = "password";
		private string _checkPassMoi { get; set; } = "password";
		private string _checkPassCofirm { get; set; } = "password";
		protected override async Task OnInitializedAsync()
		{
			//var token = _ihttpcontextaccessor.HttpContext.Session.GetString("Token"); // Gọi token
			//_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); // Xác thực

			try
			{
				var a = Guid.Parse(_ihttpcontextaccessor.HttpContext.Session.GetString("UserId"));
				//var a = Guid.Parse("a4c10abe-eec2-40e6-9b6c-cf1221e9da78");
				User_VM = await _client.GetFromJsonAsync<User>($"https://localhost:7141/api/user/get_user_by_id/{a}");
			}
			catch (Exception)
			{

				_navigationManager.NavigateTo("/home", true);
			}

		}

		public async Task ChangePasswordMethor()
		{
			//changePassword.UserId = Guid.Parse(_ihttpcontextaccessor.HttpContext.Session.GetString("UserId"));
			//if(changePassword.OldPassword!=null || changePassword.NewPassword!=null || changePassword.ConfirmNewPassword != null)
			//{
			var a = _ihttpcontextaccessor.HttpContext.Session.GetString("UserId");

			if (string.IsNullOrEmpty(a))
			{
				_toastService.ShowError("Bạn cần đăng nhập để thực hiện chức năng này");
				return;
			}
			changePassword.UserId = User_VM.Id;
			var response = await _client.PutAsJsonAsync<ChangePassword_VM>("https://localhost:7141/api/user/change-password/", changePassword);
			if (response.IsSuccessStatusCode)
			{
				_toastService.ShowSuccess("Đổi mật khẩu thành công");
				changePassword.OldPassword=String.Empty;
				changePassword.NewPassword=String.Empty;
				changePassword.ConfirmNewPassword=String.Empty;
			}
			else
			{
				_toastService.ShowError("Đổi mật khẩu thất bại");
			}
		}
		public void ShowPass()
		{
			if (TypeInput_Pass == "text")
			{
				TypeInput_Pass = "password";
				Icon_Pass = "fa-regular fa-eye-slash";
			}
			else
			{
				TypeInput_Pass = "text";
				Icon_Pass = "fa-regular fa-eye";
			}
		}
		private void ShowNewPass()
		{
			if (TypeInput_New == "text")
			{
				TypeInput_New = "password";
				Icon_New = "fa-regular fa-eye-slash";
			}
			else
			{
				TypeInput_New = "text";
				Icon_New = "fa-regular fa-eye";
			}
		}
		private void ShowCheckNewPass()
		{
			if (TypeInput_CheckNew == "text")
			{
				TypeInput_CheckNew = "password";
				Icon_CheckNew = "fa-regular fa-eye-slash";
			}
			else
			{
				TypeInput_CheckNew = "text";
				Icon_CheckNew = "fa-regular fa-eye";

			}
		}

		private void CheckPassCu (ChangeEventArgs e)
		{
			if ((bool)e.Value == false) _checkPassCu = "text";
			else _checkPassCu = "password";
		}
		private void CheckPassMoi(ChangeEventArgs e)
		{
			if ((bool)e.Value == false) _checkPassMoi = "text";
			else _checkPassMoi = "password";
		}
		private void CheckPassConfirm(ChangeEventArgs e)
		{
			if ((bool)e.Value == false) _checkPassCofirm = "text";
			else _checkPassCofirm = "password";
		}
	}
}
