using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;

namespace SchoolApp.Controllers
{
    [Route("api/[controller]")] // Endpoint: api/StudentApi
    [ApiController]               // បញ្ជាក់ថាជា Web API Controller
    public class StudentApiController : ControllerBase // មិនត្រូវការ View Support ឡើយ
    {
        private readonly SchoolDbContext _context;

        public StudentApiController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet] // HTTP GET Request
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();

            if (students == null || students.Count == 0)
            {
                return NotFound("រកមិនឃើញទិន្នន័យសិស្សឡើយ!"); // Status Code 404
            }

            return Ok(students); // Status Code 200 + JSON Data
        }
    }
}