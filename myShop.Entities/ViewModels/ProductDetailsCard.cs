using myShop.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myShop.Entities.ViewModels
{
    public class ProductDetailsCard
    {
        public Product Product { get; set; }
        [Range(1,100,ErrorMessage ="You must enter number from 1 to 100")]
        public int Count { get; set; }  
    }
}
