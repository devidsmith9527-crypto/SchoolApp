using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Models
{
    public class Student
    {
        [Key] // Primary Key (Auto Identity ក្នុង SQL Server)
        public int Id { get; set; }

        [Required(ErrorMessage = "សូមបញ្ចូលឈ្មោះពេញរបស់និស្សិត!")]
        [StringLength(100, ErrorMessage = "ឈ្មោះមិនអាចលើសពី ១០០ អក្សរឡើយ!")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "សូមជ្រើសរើសភេទ!")]
        public string Gender { get; set; } = "";

        [Range(0.0, 4.0, ErrorMessage = "GPA ត្រូវតែស្ថិតនៅចន្លោះ ០.០ ដល់ ៤.០!")]
        public double GPA { get; set; }
    }
}