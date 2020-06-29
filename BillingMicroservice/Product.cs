using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;

namespace BillingMicroservice
{
    public class Product
    {
       
            // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public int Price { get; set; }
        public int StockNumber { get; set; }
    

    }
}

