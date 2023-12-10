﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace DATN_Shared.ViewModel
{
    public class Size_VM
    {
        public Guid Id { get; set; }
		[Required(ErrorMessage = "Size không được để trống")]
		public string Name { get; set; }     
        public int Status { get; set; }
    }
}
