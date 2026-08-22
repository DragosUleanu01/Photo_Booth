using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Photo_Booth_Server_API.Models;

namespace Photo_Booth_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        static private List<ImageFile> image = new List<ImageFile>
        {
            new ImageFile { Id = 1, Subject = "Sample Image 1", FilePath = "path/to/image1.jpg" },
            new ImageFile { Id = 2, Subject = "Sample Image 2", FilePath = "path/to/image2.jpg" },
            new ImageFile { Id = 3, Subject = "Sample Image 3", FilePath = "path/to/image3.jpg" }
        };

        [HttpGet]
        public ActionResult<List<ImageFile>> GetImageFile()
        {
            return Ok(image);
        }

        [HttpGet("{id}")]
        public ActionResult<ImageFile> GetImageById(int id)
        {
            var imageFile = image.FirstOrDefault(i => i.Id == id);
            if (imageFile == null)
            {
                return NotFound();
            }
            return Ok(imageFile);
        }

    }
}
