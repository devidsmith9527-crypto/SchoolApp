using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Models
{
    public class Categories
    {
        [Key]
        public int CategoryId { get; set;}
        [Required]
        [StringLength(50)]
        public string CategoryName { get; set;}
        public ICollection<Product> Products { get; set;} = new List<Product>();
    }
}