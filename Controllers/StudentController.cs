using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;

namespace SchoolApp.Controllers
{
    // ស្នងពី Controller ធម្មតា (មាន View Support)
    [Route("Student")]
    public class StudentController : Controller
    {
        private readonly SchoolDbContext _context;

        public StudentController(SchoolDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Index()
        {
            // ១. ទាញយកទិន្នន័យសិស្សពី SQL Server
            var students = await _context.Students.ToListAsync();

            // ២. បោះទិន្នន័យ Model ទៅឱ្យ Razor View (.cshtml)
            return View(students);
        }
    }
}