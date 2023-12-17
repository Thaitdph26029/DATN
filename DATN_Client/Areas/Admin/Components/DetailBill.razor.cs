﻿using Blazored.Toast.Configuration;
using Blazored.Toast;
using DATN_Client.Areas.Admin.Controllers;
using DATN_Shared.Models;
using DATN_Shared.ViewModel;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace DATN_Client.Areas.Admin.Components
{
	public partial class DetailBill
	{
		private HttpClient _client = new HttpClient();
        [Inject] Blazored.Toast.Services.IToastService _toastService { get; set; }
        private Bill_ShowModel _billModel = new Bill_ShowModel();
		private List<BillDetailShow> _lstBillDetail = new List<BillDetailShow>();
		private AddressShip addressShip = new AddressShip();
		private Bill_VM b = new Bill_VM();
		

		[Inject]
		private NavigationManager nav { get; set; }

		public Bill_ShowModel _bill = new Bill_ShowModel();
        public Guid BillId { get; set; }
        public string TotalText { get; set; }
        public int PhiShip { get; set; }
        public string Note { get; set; }
		public bool ActiveTabFee { get; set; } = false;

        protected override async Task OnInitializedAsync()
		{
			await getDataBill();
            _lstBillDetail = await _client.GetFromJsonAsync<List<BillDetailShow>>("https://localhost:7141/api/BillItem/getbilldetail/" + BillId);					
		}
		public async Task getDataBill()
		{
            BillId = BillManagementController._billId;
            List<Bill_ShowModel> _lstbill = await _client.GetFromJsonAsync<List<Bill_ShowModel>>("https://localhost:7141/api/Bill/get_alll_bill");

			_bill = _lstbill.FirstOrDefault(x => x.Id == BillId);
			TotalText = NumberToText(Convert.ToDouble(_bill.TotalAmount.ToString()));
        }

		public void ReturnBill()
		{
            JSRuntime.InvokeVoidAsync("history.back");
        }
        public void CheckFeeShip(ChangeEventArgs e)
		{
            if (int.TryParse(e.Value.ToString(), out int inputValue))
            {
				if (inputValue>0)
				{
                    ActiveTabFee = true;
                }
				else
				{
                    ActiveTabFee = false;
                }
            }
        }
        public async void UpdateFeeship()
        {

			CheckFee();
            _bill.ShippingFee = PhiShip;
			_bill.Note = _bill.Note +  "," + Note;
			_bill.ConfirmationDate = DateTime.Now;
			if (_bill.Type == 1)
			{
                _bill.Status = 3;
            }
			var reponse = await _client.PutAsJsonAsync("https://localhost:7141/api/Bill/Put-Bill", _bill);
			if (reponse.StatusCode.ToString()=="OK")
			{
				_toastService.ShowSuccess("Cập nhật phí vận chuyển thành công");
				await OnInitializedAsync();				
            }
        }
		public void CheckFee()
		{
            if (PhiShip == 0 || string.IsNullOrEmpty(PhiShip.ToString()))
            {
                _toastService.ShowError("Vui lòng điền phí vận chuyển");
                return;
            }
            else if (PhiShip < 0)
            {
                _toastService.ShowError("Phí vận chuyển phải lớn hơn 0");
                return;
            }
            ActiveTabFee = true;
        }
		public async void Bangiao()
		{
            if (_bill.Type == 1)
            {
                _bill.Status = 4;
            }
			else if(_bill.Type == 2)
			{
                _bill.Status = 2;
            }
            _bill.ConfirmationDate = DateTime.Now;
            var reponse = await _client.PutAsJsonAsync("https://localhost:7141/api/Bill/Put-Bill", _bill);
            if (reponse.StatusCode.ToString() == "OK")
            {
                _toastService.ShowSuccess("Bàn giao thành công");
                await OnInitializedAsync();
            }
        }
		public async void ThanhCong()
		{
			if (_bill.Type == 1)
			{
				_bill.Status = 5;
			}
			else if (_bill.Type == 2)
			{
				_bill.Status = 3;
			}
			_bill.CompletionDate = DateTime.Now;

			if (_bill.UserId == null || _bill.UserId == default)
			{
                
                var _lstfomula = await _client.GetFromJsonAsync<List<Formula_VM>>("https://localhost:7141/api/Formula/get_formula");
                var Fomula = _lstfomula.FirstOrDefault(x => x.Status == 1);

                var _lstPoint = await _client.GetFromJsonAsync<List<CustomerPoint_VM>>("https://localhost:7141/api/CustomerPoint/getAllCustomerPoint");

                CustomerPoint_VM PointUser = _lstPoint.FirstOrDefault(x => x.UserID == _bill.UserId);
				//Tạo bản ghi 
				HistoryConsumerPoint_VM htr = new HistoryConsumerPoint_VM();
				htr.Id = Guid.NewGuid();
				htr.FormulaId = Fomula.Id;
				htr.BillId = _bill.Id;
				htr.ConsumerPointId = PointUser.UserID;
				htr.Point = _bill.TotalAmount / Fomula.Coefficient ?? 0;
				htr.Status = 1;

				var reponse1 = await _client.PostAsJsonAsync(" https://localhost:7141/api/HistoryConsumerPoint/update-HistoryConsumerPoint", htr);
            }
            var reponse = await _client.PutAsJsonAsync("https://localhost:7141/api/Bill/Put-Bill", _bill);




        }


        private static string NumberToText(double inputNumber, bool suffix = true)
		{
			string[] unitNumbers = new string[] { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
			string[] placeValues = new string[] { "", "nghìn", "triệu", "tỷ" };
			bool isNegative = false;

			// -12345678.3445435 => "-12345678"
			string sNumber = inputNumber.ToString("#");
			double number = Convert.ToDouble(sNumber);
			if (number < 0)
			{
				number = -number;
				sNumber = number.ToString();
				isNegative = true;
			}
			int ones, tens, hundreds;

			int positionDigit = sNumber.Length;   // last -> first

			string result = " ";

			if (positionDigit == 0)
				result = unitNumbers[0] + result;
			else
			{
				// 0:       ###
				// 1: nghìn ###,###
				// 2: triệu ###,###,###
				// 3: tỷ    ###,###,###,###
				int placeValue = 0;

				while (positionDigit > 0)
				{
					// Check last 3 digits remain ### (hundreds tens ones)
					tens = hundreds = -1;
					ones = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
					positionDigit--;
					if (positionDigit > 0)
					{
						tens = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
						positionDigit--;
						if (positionDigit > 0)
						{
							hundreds = Convert.ToInt32(sNumber.Substring(positionDigit - 1, 1));
							positionDigit--;
						}
					}

					if ((ones > 0) || (tens > 0) || (hundreds > 0) || (placeValue == 3))
						result = placeValues[placeValue] + result;

					placeValue++;
					if (placeValue > 3) placeValue = 1;

					if ((ones == 1) && (tens > 1))
						result = "một " + result;
					else
					{
						if ((ones == 5) && (tens > 0))
							result = "lăm " + result;
						else if (ones > 0)
							result = unitNumbers[ones] + " " + result;
					}
					if (tens < 0)
						break;
					else
					{
						if ((tens == 0) && (ones > 0)) result = "lẻ " + result;
						if (tens == 1) result = "mười " + result;
						if (tens > 1) result = unitNumbers[tens] + " mươi " + result;
					}
					if (hundreds < 0) break;
					else
					{
						if ((hundreds > 0) || (tens > 0) || (ones > 0))
							result = unitNumbers[hundreds] + " trăm " + result;
					}
					result = " " + result;
				}
			}
			result = result.Trim();
			if (isNegative) result = "Âm " + result;
			return result + (suffix ? " đồng chẵn" : "");
		}
	}
}