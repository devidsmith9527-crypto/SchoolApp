using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models; // ចាំបាច់ត្រូវបន្ថែម ដើម្បីឲ្យស្គាល់ Class Student

namespace SchoolApp.Controllers
{
    [Route("api/[controller]")] // Endpoint: api/CategoryApi
    public class CategoryApiController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public CategoryApiController(SchoolDbContext context)
        {
            _context = context;
        }

        // ១. ទាញយកទិន្នន័យទាំងអស់ (Read All)
        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            if (categories == null || !categories.Any())
            {
                // តាមស្តង់ដារ API គេអាច Return Ok(categories) ជា Array ទទេក៏បាន 
                // ប៉ុន្តែទីនេះរក្សាការចង់បានរបស់អ្នកគឺ 404
                return NotFound("រកមិនឃើញទិន្នន័យប្រភេទឡើយ!"); 
            }

            return Ok(categories); 
        }

        // ២. ទាញយកទិន្នន័យតែមួយតាម ID (Read One) - ថ្មី
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound($"រកមិនឃើញប្រភេទដែលមាន ID: {id} ឡើយ!");
            }

            return Ok(category);
        }

        // ៣. បង្កើតប្រភេទថ្មី (Create/POST) - ថ្មី
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] Categories model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Categories.Add(model);
            await _context.SaveChangesAsync();

            // Return 201 Created រួចភ្ជាប់ Action ទៅ GetCategory ដើម្បីឲ្យដឹងថាបង្កើតបាននៅឯណា
            return CreatedAtAction(nameof(GetCategory), new { id = model.CategoryId }, model);
        }

        // ៤. កែប្រែទិន្នន័យ (Update/PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Categories model)
        {
            // ផ្ទៀងផ្ទាត់ថា Id លើ URL និង Id ក្នុង JSON តួខ្លួនគឺដូចគ្នា
            if (id != model.CategoryId)
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
                if (!_context.Categories.Any(e => e.CategoryId == id))
                    return NotFound($"រកមិនឃើញទិន្នន័យប្រភេទ ID: {id} សម្រាប់កែប្រែឡើយ!");
                else
                    throw;
            }

            // ជោគជ័យ! បញ្ចេញ Status 204 No Content
            return NoContent();
        }

        // ៥. លុបទិន្នន័យ (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            // ស្វែងរកទិន្នន័យដែលត្រូវលុបនៅក្នុង Database
            var category = await _context.Categories.FindAsync(id);
            
            // បើរកមិនឃើញ បាញ់កូដ 404 ប្រាប់ទៅ Frontend
            if (category == null)
            {
                return NotFound("រកមិនឃើញទិន្នន័យដែលអ្នកចង់លុបឡើយ!");
            }

            // បើរកឃើញ បញ្ជាឱ្យ EF Core លុបចោលរួច Save
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            // ឆ្លើយតបកូដ 204 No Content (ជោគជ័យ)
            return NoContent();
        }
    }
}