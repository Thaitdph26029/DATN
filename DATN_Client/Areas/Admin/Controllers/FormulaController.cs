﻿using DATN_Client.Areas.Customer.Component;
using Microsoft.AspNetCore.Mvc;

namespace DATN_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class FormulaController : Controller
    {
        [Route("formula-management")]
        public IActionResult Index()
        {

            if (Login.Roleuser == "Admin")
            {
                return View();
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
