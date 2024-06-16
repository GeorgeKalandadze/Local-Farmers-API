﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalFarmersApi.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }

        public virtual Product Product { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}