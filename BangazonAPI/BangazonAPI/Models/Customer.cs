using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonAPI.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(25)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(25)]
        public string LastName { get; set; }

        [Required]
        [StringLength(25)]
        public string CreationDate { get; set; }

        [Required]
        [StringLength(25)]
        public string LastActiveDate { get; set; }

        public List<Product> ProductList
        {
            get; set;
        } = new List<Product>();

        public List<PaymentType> PaymentTypeList
        {
            get; set;
        } = new List<PaymentType>();
    }
}
