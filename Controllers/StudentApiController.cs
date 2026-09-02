using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models; // ចាំបាច់ត្រូវបន្ថែម ដើម្បីឲ្យស្គាល់ Class Student

namespace SchoolApp.Controllers
{
    [Route("api/[controller]")] // Endpoint: api/StudentApi
    [ApiController]               // បញ្ជាក់ថាជា Web API Controller
    public class StudentApiController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public StudentApiController(SchoolDbContext context)
        {
            _context = context;
        }

        // ១. ទាញយកទិន្នន័យទាំងអស់ (Read All)
        [HttpGet]
        public async Task<IActionResult> GetStudents()
        {
            var students = await _context.Students.ToListAsync();

            if (students == null || !students.Any())
            {
                // តាមស្តង់ដារ API គេអាច Return Ok(students) ជា Array ទទេក៏បាន 
                // ប៉ុន្តែទីនេះរក្សាការចង់បានរបស់អ្នកគឺ 404
                return NotFound("រកមិនឃើញទិន្នន័យសិស្សឡើយ!"); 
            }

            return Ok(students); 
        }

        // ២. ទាញយកទិន្នន័យតែមួយតាម ID (Read One) - ថ្មី
        [HttpGet("{id}")]
        public async Task<IActionResult> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound($"រកមិនឃើញនិស្សិតដែលមាន ID: {id} ឡើយ!");
            }

            return Ok(student);
        }

        // ៣. បង្កើតនិស្សិតថ្មី (Create/POST) - ថ្មី
        [HttpPost]
        public async Task<IActionResult> CreateStudent([FromBody] Student model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Students.Add(model);
            await _context.SaveChangesAsync();

            // Return 201 Created រួចភ្ជាប់ Action ទៅ GetStudent ដើម្បីឲ្យដឹងថាបង្កើតបាននៅឯណា
            return CreatedAtAction(nameof(GetStudent), new { id = model.Id }, model);
        }

        // ៤. កែប្រែទិន្នន័យ (Update/PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] Student model)
        {
            // ផ្ទៀងផ្ទាត់ថា Id លើ URL និង Id ក្នុង JSON តួខ្លួនគឺដូចគ្នា
            if (id != model.Id)
            {
                return BadRequest("ID មិនស៊ីគ្នា (Mismatch)!");
            }

            // បញ្ជាឱ្យ EF Core Update ដោយផ្លាស់ប្តូរ State
            _context.Entry(model).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // ករណីរក Id នោះមិនឃើញក្នុង Database ពេលកំពុង Save
                if (!_context.Students.Any(e => e.Id == id))
                    return NotFound($"រកមិនឃើញទិន្នន័យនិស្សិត ID: {id} សម្រាប់កែប្រែឡើយ!");
                else
                    throw;
            }

            // ជោគជ័យ! បញ្ចេញ Status 204 No Content
            return NoContent();
        }

        // ៥. លុបទិន្នន័យ (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            // ស្វែងរកទិន្នន័យដែលត្រូវលុបនៅក្នុង Database
            var student = await _context.Students.FindAsync(id);
            
            // បើរកមិនឃើញ បាញ់កូដ 404 ប្រាប់ទៅ Frontend
            if (student == null)
            {
                return NotFound("រកមិនឃើញទិន្នន័យដែលអ្នកចង់លុបឡើយ!");
            }

            // បើរកឃើញ បញ្ជាឱ្យ EF Core លុបចោលរួច Save
            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            // ឆ្លើយតបកូដ 204 No Content (ជោគជ័យ)
            return NoContent();
        }
    }
}