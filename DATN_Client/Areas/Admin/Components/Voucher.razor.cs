﻿using DATN_Shared.ViewModel;
using Microsoft.AspNetCore.Components;

namespace DATN_Client.Areas.Admin.Components
{
    public partial class Voucher
    {
        HttpClient httpClient = new HttpClient();
        public Voucher_VM voucher_VM = new Voucher_VM();
        public Voucher_VM voucher_VM1 = new Voucher_VM();
        [Inject] NavigationManager navigationManager { get; set; }

        List<Voucher_VM> vouchers = new List<Voucher_VM>();
        public string Message { get; set; } = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            vouchers = await httpClient.GetFromJsonAsync<List<Voucher_VM>>("https://localhost:7141/api/Voucher");

            await AutoChangeStatusVoucher();
        }
        public async IAsyncEnumerable<Voucher_VM> SelectOneVoucher()
        {
            foreach (var voucher in vouchers)
            {
                yield return voucher;
            }
        }
        public async Task AutoChangeStatusVoucher()
        {
           await foreach (var voucher in  SelectOneVoucher())
           {
                if (voucher.EndDate <DateTime.Now)
                {
                    voucher.Status = 0;
                    await httpClient.PutAsJsonAsync<Voucher_VM>("https://localhost:7141/api/Voucher/Put-Voucher", voucher);
                }
           }

        }

        public async Task AddVoucher()
        {
            voucher_VM.Id = Guid.NewGuid();

            await httpClient.PostAsJsonAsync<Voucher_VM>("https://localhost:7141/api/Voucher/Post-Voucher", voucher_VM);
            navigationManager.NavigateTo("https://localhost:7075/Admin/Voucher", true);


        }


        public async Task ChangeStatusVoucher(Voucher_VM voucher)
        {
            voucher.Status = 0;
            await httpClient.PutAsJsonAsync<Voucher_VM>("https://localhost:7141/api/Voucher/Put-Voucher", voucher);
        }
        public async Task UpdateVoucher(Voucher_VM voucher)
        {
            await httpClient.PutAsJsonAsync<Voucher_VM>("https://localhost:7141/api/Voucher/Put-Voucher", voucher);
            navigationManager.NavigateTo("https://localhost:7075/Admin/Voucher", true);
        }

        public async Task LoadFormVoucher(Voucher_VM GetValueFromList)
        {
            voucher_VM1.Id = GetValueFromList.Id;
            voucher_VM1.Name = GetValueFromList.Name;
            voucher_VM1.Code = GetValueFromList.Code;
            voucher_VM1.Percent = GetValueFromList.Percent;
            voucher_VM1.Quantity = GetValueFromList.Quantity;
            voucher_VM1.Discount_Conditions = GetValueFromList.Discount_Conditions;
            voucher_VM1.Maximum_Reduction = GetValueFromList.Maximum_Reduction;
            voucher_VM1.StartDate = GetValueFromList.StartDate;
            voucher_VM1.EndDate = GetValueFromList.EndDate;
            voucher_VM1.Status = GetValueFromList.Status;

        }
    }
}
