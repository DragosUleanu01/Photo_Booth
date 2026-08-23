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
      


        private readonly Context _context;
        public ImageController(Context context)
        {
            _context = context;
        }

        //asyncronous sa nu se blocheze threadul intre request-uri
        //metode pentru stocarea metadatelor imaginilor in baza de date, nu pentru stocarea imaginilor in sine

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

        // Endpoint pentru upload-ul imaginilor + metadata unui obiect ImageFile in baza de date
        [HttpPost("upload")]
        
        public async Task<ActionResult<ImageFile>> UploadImage(IFormFile file, [FromForm]string subject)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            // Salveaza upload-ul la un path specificat, in folderul "Uploads" din proiect
            var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
            
            // double-check daca exista folderul, daca nu exista il creeaza
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            //Generare nume unic pentru fisierul uploadat

            var uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            var filePath = Path.Combine(uploadsPath, uniqueFileName);

            //Salvare imagine in folderul "Uploads"

            using(var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            //Creare obiect ImageFile pentru a stoca metadatele in baza de date
            var image = new ImageFile
            { 
              Subject = subject,
              FilePath = $"/Uploads/{uniqueFileName}"

            };

            _context.ImageFiles.Add(image);
            await _context.SaveChangesAsync();
            return Ok(image);

        }

    }
}
