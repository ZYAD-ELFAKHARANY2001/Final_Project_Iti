﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.Product
{
    public class CreateOrUpdateProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? LongDescription { get; set; }
        public string? ShortDescription { get; set; }
        public decimal RealPrice { get; set; }
        public decimal? Discount { get; set; } = 0;
        public int? SubCategoryID { get; set; }
        public string? SubCategoryName { get; set; } // Include for easier presentation
        public int? BrandID { get; set; }
        public string? BrandName { get; set; } // Include for easier presentation
        //public ICollection<IFormFile>? Images { get; set; } // Simplified image representation
    }
}
