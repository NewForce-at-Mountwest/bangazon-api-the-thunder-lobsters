using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Order
    {
        [Required]
        public int Id { get; set; }
        public int PaymentTypeId { get; set; }
        [Required]
        public int CustomerId { get; set; }


    }
}
