﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.ProductItems
{
    public class CreatOrUpdateProductItemsDTO
    {
        public int Id { get; set; }
        public int ItemQuantity { get; set; }
        public float? Pro_Weight { get; set; }
        public string? Pro_Size { get; set; }
        public string? Color { get; set; }
        public int? Storge { get; set; }  //1GB ....
        public List<byte[]>? Images { get; set; }
        [ForeignKey("Product")]
        public int Pro_Id { get; set; }
    }
}
