using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Photo_Booth_Server_API.Data;
using Photo_Booth_Server_API.Models;

namespace Photo_Booth_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        //static private List<ImageFile> image = new List<ImageFile> //lista imagini test
        // {
        // new ImageFile { Id = 1, Subject = "Sample Image 1", FilePath = "path/to/image1.jpg" },
        // new ImageFile { Id = 2, Subject = "Sample Image 2", FilePath = "path/to/image2.jpg" },
        //new ImageFile { Id = 3, Subject = "Sample Image 3", FilePath = "path/to/image3.jpg" }
        //};


        private readonly Context _context;
        public ImageController(Context context)
        {
            _context = context;
        }

        //asyncronous methods for database operations - sa nu se blocheze threadul intre request-uri

        [HttpGet]
        public async Task <ActionResult<List<ImageFile>>> GetImageFile() //Intoarce toate imaginile din lista
        {
            return Ok(await _context.ImageFiles.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImageFile>> GetImageById(int id) //Intoarce o imagine dupa id
        {
            var imageFile = await _context.ImageFiles.FindAsync(id);
            if (imageFile == null)
            {
                return NotFound();
            }
            return Ok(imageFile);
        }

        [HttpPost]
        public async Task< ActionResult<ImageFile>>  AddImage(ImageFile imageFile) //Adauga o imagine in lista
        {
            if(imageFile == null)
            {
                return BadRequest();
            }
            _context.ImageFiles.Add(imageFile);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetImageById), new { id = imageFile.Id }, imageFile);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateImage(int id, ImageFile imageFile) //modifica datelele unei imagini din lista
        {
            var existingImage = await _context.ImageFiles.FindAsync(id);
            if (existingImage == null)
            {
                return NotFound();
            }
            existingImage.Id = imageFile.Id;
            existingImage.Subject = imageFile.Subject;
            existingImage.FilePath = imageFile.FilePath;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id) //sterge o imagine din lista
        {
            var existingImage = await _context.ImageFiles.FindAsync(id);
            if (existingImage == null)
            {
                return NotFound();
            }
            _context.ImageFiles.Remove(existingImage);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
