using System;
using System.Collections.Generic;

#nullable disable

namespace AngularAPIJWT.Models
{
    public partial class TblCustomer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public int? CreditLimit { get; set; }
    }
}
