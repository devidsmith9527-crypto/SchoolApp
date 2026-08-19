namespace SchoolApp.Models
{
    public class Student
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public string Gender { get; set; } = "";
        public double GPA { get; set; }
    }
}