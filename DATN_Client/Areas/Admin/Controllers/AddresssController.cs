﻿using Microsoft.AspNetCore.Mvc;

namespace DATN_Client.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AddresssController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
