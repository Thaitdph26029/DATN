﻿using DATN_Shared.Models;
using DATN_Shared.ViewModel;
using Microsoft.AspNetCore.Components;

namespace DATN_Client.Areas.Customer.Component
{
    public partial class BilllItemByUser
    {
        [Inject] NavigationManager _navigationManager { get; set; }
        HttpClient _httpClient = new HttpClient();
        [Inject] Blazored.Toast.Services.IToastService _toastService { get; set; }

        public Bill_VM _bill = new Bill_VM();
        Bill_ShowModel _bill_ShowModel = new Bill_ShowModel();

        BillItem_VM _billItem = new BillItem_VM();  
        List<BillDetailShow> _lstBillItems = new List<BillDetailShow>();

        protected async override Task OnInitializedAsync()
        {
            _bill = BillByUser._bill_VM;
            var a = await _httpClient.GetFromJsonAsync<List<Bill_ShowModel>>("https://localhost:7141/api/Bill/get_alll_bill");
            _bill_ShowModel = a.FirstOrDefault(x => x.Id == _bill.Id);
            _lstBillItems = await _httpClient.GetFromJsonAsync<List<BillDetailShow>>($"https://localhost:7141/api/BillItem/getbilldetail/{_bill.Id}");
        }
    }
}