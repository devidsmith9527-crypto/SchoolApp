using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

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

        // ១. GET: /Student/Create (បង្ហាញ Form ទទេ)
        [HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // ២. POST: /Student/Create (ទទួលទិន្នន័យមក Save)
        [HttpPost("Create")]
        [ValidateAntiForgeryToken] // ការពារការវាយប្រហារ CSRF
        public async Task<IActionResult> Create(Student model)
        {
            // ឆែកមើល Validation តាម Model Rules
            if (!ModelState.IsValid)
            {
                return View(model); // បើខុស បង្ហាញ Form វិញអមជាមួយ Error
            }

            await _context.Students.AddAsync(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "រក្សាទុកទិន្នន័យសិស្សជោគជ័យ!";
            return RedirectToAction(nameof(Index)); // 302 Redirect ទៅទំព័របញ្ជី
        }
    }
}