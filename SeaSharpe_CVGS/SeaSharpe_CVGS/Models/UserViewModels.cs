using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SeaSharpe_CVGS.Models
{
    public class ProfileViewModel
    {
        public Member Member { get; set; }
        public Address BillingAddress { get; set; }
        public Address ShippingAddress { get; set; }
    }
}
