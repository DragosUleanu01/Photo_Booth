using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photo_Booth_Server_API.Models;

namespace Photo_Booth_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        static private List<ImageFile> image = new List<ImageFile> //lista imagini test
        {
            new ImageFile { Id = 1, Subject = "Sample Image 1", FilePath = "path/to/image1.jpg" },
            new ImageFile { Id = 2, Subject = "Sample Image 2", FilePath = "path/to/image2.jpg" },
            new ImageFile { Id = 3, Subject = "Sample Image 3", FilePath = "path/to/image3.jpg" }
        };

        [HttpGet]
        public ActionResult<List<ImageFile>> GetImageFile() //Intoarce toate imaginile din lista
        {
            return Ok(image);
        }

        [HttpGet("{id}")]
        public ActionResult<ImageFile> GetImageById(int id) //Intoarce o imagine dupa id
        {
            var imageFile = image.FirstOrDefault(i => i.Id == id);
            if (imageFile == null)
            {
                return NotFound();
            }
            return Ok(imageFile);
        }

        [HttpPost]
        public ActionResult<ImageFile> AddImage(ImageFile imageFile) //Adauga o imagine in lista
        {
            if(imageFile == null)
            {
                return BadRequest();
            }
            image.Add(imageFile);
            return CreatedAtAction(nameof(GetImageById), new { id = imageFile.Id }, imageFile);
        }
        [HttpPut("{id}")]
        public IActionResult UpdateImage(int id, ImageFile imageFile) //modifica datelele unei imagini din lista
        {
            var existingImage = image.FirstOrDefault(i => i.Id == id);
            if (existingImage == null)
            {
                return NotFound();
            }
            existingImage.Id = imageFile.Id;
            existingImage.Subject = imageFile.Subject;
            existingImage.FilePath = imageFile.FilePath;
            return NoContent();
        }
        [HttpDelete("{id}")]
        public IActionResult DeleteImage(int id) //sterge o imagine din lista
        {
            var existingImage = image.FirstOrDefault(i => i.Id == id);
            if (existingImage == null)
            {
                return NotFound();
            }
            image.Remove(existingImage);
            return NoContent();
        }
    }
}
