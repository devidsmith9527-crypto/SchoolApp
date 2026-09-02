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
        // ១. [HttpGet] Edit - ទទួល Id, រកក្នុង DB, រួចបញ្ជូនទៅ View
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound(); // បើមិនហុច Id មក គឺ Error 404

            // ស្វែងរកទិន្នន័យក្នុង DB
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            // បញ្ជូនទិន្នន័យចាស់ទៅឱ្យ View ដើម្បីចាក់ចូលក្នុង Form
            return View(student); 
        }

        // ២. [HttpPost] Edit - ទទួល Model ពី Form មក Update
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken] // ការពារ CSRF Attack
        public async Task<IActionResult> Edit(int id, Student model)
        {
            if (id != model.Id) return NotFound(); // ការពារកុំឱ្យគេ Hack ប្តូរ Id

            if (ModelState.IsValid)
            {
                // បញ្ជាឱ្យ EF Core ធ្វើការ Update រួច Save
                _context.Update(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            // បើកែខុស Validation, បញ្ជូន Form មកឱ្យកែប្រែឡើងវិញ
            return View(model); 
        }

        // ១. [HttpGet] Delete - បង្ហាញទំព័រសួរបញ្ជាក់
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();

            return View(student); // បង្ហាញឯកសារ Delete.cshtml
        }

        // ២. [HttpPost] DeleteConfirmed - ទទួលការបញ្ជាក់ ទើបលុបពិតប្រាកដ
        // ចំណាំ៖ យើងប្រើ ActionName("Delete") ដើម្បីឱ្យវាត្រូវគ្នានឹង URL ខាងលើ
        [HttpPost, ActionName("Delete"), Route("DeleteConfirmed/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // រកទិន្នន័យនោះឱ្យឃើញសិន
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                // បញ្ជាឱ្យ EF Core លុបចេញពី Database
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }
    }
}