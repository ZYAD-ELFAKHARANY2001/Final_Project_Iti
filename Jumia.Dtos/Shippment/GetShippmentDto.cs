﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jumia.Dtos.Shippment
{
    public class GetShippmentDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string AdressInformation { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public decimal Cost { get; set; }
        public int OrderId { get; set; }
    }
}