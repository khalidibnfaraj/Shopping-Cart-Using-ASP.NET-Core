﻿using myShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> ShoppingCarts {  get; set; } 
        public decimal TotalCardsPrice { get; set; }
		public OrderHeader OrderHeader { get; set; }

	}
}
