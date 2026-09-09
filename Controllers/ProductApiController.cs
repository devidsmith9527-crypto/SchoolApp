using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models; // ចាំបាច់ត្រូវបន្ថែម ដើម្បីឲ្យស្គាល់ Class Student (ឬ Product)

namespace SchoolApp.Controllers
{
    [Route("api/[controller]")] // Endpoint: api/ProductApi
    public class ProductApiController : ControllerBase
    {
        private readonly SchoolDbContext _context;

        public ProductApiController(SchoolDbContext context)
        {
            _context = context;
        }

        // ១. ទាញយកទិន្នន័យទាំងអស់ (Read All)
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            var products = await _context.Products.ToListAsync();

            if (products == null || !products.Any())
            {
                // តាមស្តង់ដារ API គេអាច Return Ok(products) ជា Array ទទេក៏បាន 
                // ប៉ុន្តែទីនេះរក្សាការចង់បានរបស់អ្នកគឺ 404
                return NotFound("រកមិនឃើញទិន្នន័យផលិតផលឡើយ!"); 
            }

            return Ok(products); 
        }

        // ២. ទាញយកទិន្នន័យតែមួយតាម ID (Read One) - ថ្មី
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound($"រកមិនឃើញផលិតផលដែលមាន ID: {id} ឡើយ!");
            }

            return Ok(product);
        }

        // ៣. បង្កើតផលិតផលថ្មី (Create/POST) - ថ្មី
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] Product model) // កែពី Products មក Product
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Products.Add(model);
            await _context.SaveChangesAsync();

            // Return 201 Created រួចភ្ជាប់ Action ទៅ GetProduct ដើម្បីឲ្យដឹងថាបង្កើតបាននៅឯណា
            return CreatedAtAction(nameof(GetProduct), new { id = model.ProductId }, model);
        }

        // ៤. កែប្រែទិន្នន័យ (Update/PUT)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product model) // កែពី Products មក Product
        {
            // ផ្ទៀងផ្ទាត់ថា Id លើ URL និង Id ក្នុង JSON តួខ្លួនគឺដូចគ្នា
            if (id != model.ProductId)
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
                if (!_context.Products.Any(e => e.ProductId == id))
                    return NotFound($"រកមិនឃើញទិន្នន័យផលិតផល ID: {id} សម្រាប់កែប្រែឡើយ!");
                else
                    throw;
            }

            // ជោគជ័យ! បញ្ចេញ Status 204 No Content
            return NoContent();
        }

        // ៥. លុបទិន្នន័យ (Delete)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            // ស្វែងរកទិន្នន័យដែលត្រូវលុបនៅក្នុង Database
            var product = await _context.Products.FindAsync(id);

            // បើរកមិនឃើញ បាញ់កូដ 404 ប្រាប់ទៅ Frontend
            if (product == null)
            {
                return NotFound("រកមិនឃើញទិន្នន័យដែលអ្នកចង់លុបឡើយ!");
            }

            // បើរកឃើញ បញ្ជាឱ្យ EF Core លុបចោលរួច Save
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            // ឆ្លើយតបកូដ 204 No Content (ជោគជ័យ)
            return NoContent();
        }
    }
}