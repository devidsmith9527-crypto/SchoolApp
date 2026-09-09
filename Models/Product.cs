using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace SchoolApp.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set;}
        [Required]
        [StringLength(100)]
        public string ProductName { get; set;}
        [Required]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set;}
        public Categories Category { get; set;}
    }
}