using ImageMagick;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Photo_Booth_Server_API.Data;
using Photo_Booth_Server_API.DTO;
using Photo_Booth_Server_API.Models;
using Photo_Booth_Server_API.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;



namespace Photo_Booth_Server_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)] // Această linie protejeaza toate endpoint-urile din acest controller
    public class ImageController : ControllerBase
    {
      


        private readonly Context _context;
        private readonly EncryptionService _encryptionService;

        public ImageController(Context context, EncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        //asyncronous sa nu se blocheze threadul intre request-uri
        //metode pentru stocarea metadatelor imaginilor in baza de date, nu pentru stocarea imaginilor in sine

        [HttpGet]
        public async Task <ActionResult<List<ImageFile>>> GetImageFile() //Intoarce toate imaginile din lista
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var images = await _context.ImageFiles
                .Where(x => x.UserId == userId)
                .ToListAsync();

            return Ok(images);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ImageFile>> GetImageById(int id) //Intoarce o imagine dupa id
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var image = await _context.ImageFiles
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

            if (image == null)
            {
                return NotFound();
            }

            return Ok(image);
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
        public async Task<IActionResult> UpdateImage(int id, [FromBody] UpdateImageRequest request) //modifica datelele unei imagini din lista
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized();
            }

            var existingImage = await _context.ImageFiles
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);
            if (existingImage == null)
            {
                return NotFound();
            }

            existingImage.Subject = request.Subject;
            await _context.SaveChangesAsync();
            return NoContent();


        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage(int id, ImageFile imageFile) //sterge o imagine din lista
        {
           var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var existingImage = await _context.ImageFiles
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);
            if (existingImage == null)
            {
                return NotFound();
            }

            //update: stergere atat fisier fizic cat si inregistrarea din DB;

            var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), existingImage.FilePath.TrimStart('/'));

            if(System.IO.File.Exists(physicalPath))
            {
                System.IO.File.Delete(physicalPath);
            }

            _context.ImageFiles.Remove(existingImage);
            await _context.SaveChangesAsync();
            return NoContent();


        }

        // Endpoint pentru upload-ul imaginilor + metadata unui obiect ImageFile in baza de date
        [HttpPost("upload")]
        
        public async Task<ActionResult<ImageFile>> UploadImage([FromForm]IFormFile file, [FromForm]string subject, [FromForm] string encryptionPassword)
        {
            if(file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if(userId == null)
            {
                return Unauthorized();
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

            
            
            using var memoryStream = new MemoryStream();

            await file.CopyToAsync(memoryStream);

            var imageBytes = memoryStream.ToArray();

            var encryptedBytes = _encryptionService.Encrypt(imageBytes, encryptionPassword, out var salt, out var nonce, out var tag);

            await System.IO.File.WriteAllBytesAsync(filePath, encryptedBytes);

            
            

            //Creare obiect ImageFile pentru a stoca metadatele in baza de date
            var image = new ImageFile
            {
                Subject = subject,
                FilePath = $"/Uploads/{uniqueFileName}",
                UserId = userId,
                ContentType = file.ContentType,
                Salt = Convert.ToBase64String(salt),
                Nonce = Convert.ToBase64String(nonce),
                Tag = Convert.ToBase64String(tag)

            };

            _context.ImageFiles.Add(image);
            await _context.SaveChangesAsync();
            return Ok(image);

        }

        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<ImageFile>> DuplicateImage(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();
            }

            var image = await _context.ImageFiles
                .FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

            if (image == null)
            { 
                return NotFound();
            }

            var sourcePath = Path.Combine(
                Directory.GetCurrentDirectory(),
                image.FilePath.TrimStart('/')
            );

            if (!System.IO.File.Exists(sourcePath))
            {
                return NotFound();
            }
            var uploadsPath = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Uploads"
            );

            var extension = Path.GetExtension(sourcePath);
            var newFileName = Guid.NewGuid() + extension;
            var destinationPath = Path.Combine(uploadsPath, newFileName);

            System.IO.File.Copy(sourcePath, destinationPath);

            var duplicatedImage = new ImageFile
            {
                Subject = image.Subject,
                FilePath = $"/Uploads/{newFileName}",
                UserId = userId,

                ContentType = image.ContentType,
                Salt = image.Salt,
                Nonce = image.Nonce,
                Tag = image.Tag
            };

            _context.ImageFiles.Add(duplicatedImage);
            await _context.SaveChangesAsync();

            return Ok(duplicatedImage);
        }

        [HttpPost("{id}/filter")]

        public async Task<ActionResult<ImageFile>> ApplyFilter(int id, [FromBody] FilterImageRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized();

            }

            var imageFile = await _context.ImageFiles.FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

            if(imageFile == null)
            {
                return NotFound();
            }

            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), imageFile.FilePath.TrimStart('/'));
            
            if(!System.IO.File.Exists(sourcePath))
            {
                return NotFound("Fisierul nu exista");
            }

            // Folosirea unui obiect de tipul MagickImage pentru a aplica filtrele pe imaginea selectata
            // Imaginea este gasita la path-ul specificat in baza de date, iar MagickImage este folosit pentru a manipula imaginea

            try
            {
                //Citire fisier criptat
                var encryptedBytes = await System.IO.File.ReadAllBytesAsync(sourcePath);

                //Decriptare imaginea originala folosind parola si metadatele salvate in baza de date
                var salt = Convert.FromBase64String(imageFile.Salt);
                var nonce = Convert.FromBase64String(imageFile.Nonce);
                var tag = Convert.FromBase64String(imageFile.Tag);

                //Decriptare Imagine Originala
                var decryptedBytes = _encryptionService.Decrypt(encryptedBytes,request.EncryptionPassword, salt, nonce, tag);

                //Incarcare imagine direct din memorie
                using var image = new MagickImage(decryptedBytes);

                //Selectare filtre din MagickImage
                switch (request.Filter.ToLower())
                {
                    case "grayscale":
                        image.Grayscale();
                        break;
                    case "sepia":
                        image.SepiaTone();
                        break;
                    case "blur":
                        image.Blur(0, 5);
                        break;
                    case "negate":
                        image.Negate();
                        break;
                    default:
                        return BadRequest("Please specify a valid filter: grayscale, sepia, blur, negate.");


                }

                //Transformarea imaginii inapoi in bytes
                var filteredBytes = image.ToByteArray();

                //Criptare rezultatul cu metadata noua

                var encryptedFilteredBytes = _encryptionService.Encrypt(filteredBytes, request.EncryptionPassword, out var newSalt, out var newNonce, out var newTag);

                //Salvare fisier criptat nou
                var newFileName = Guid.NewGuid().ToString() + Path.GetExtension(sourcePath);
                var destinationPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", newFileName);
                await System.IO.File.WriteAllBytesAsync(destinationPath,encryptedFilteredBytes);

                //Creare obiect ImageFile pentru a stoca metadatele in baza de date
                var filteredImage = new ImageFile
                {
                    Subject = imageFile.Subject + " - " + request.Filter,
                    FilePath = $"/Uploads/{newFileName}",
                    UserId = userId,
                    ContentType = imageFile.ContentType,
                    Salt = Convert.ToBase64String(newSalt),
                    Nonce = Convert.ToBase64String(newNonce),
                    Tag = Convert.ToBase64String(newTag)
                };

                _context.ImageFiles.Add(filteredImage);
                await _context.SaveChangesAsync();
                return Ok(filteredImage);
            }

            catch (CryptographicException)
            {
                return BadRequest("Parola gresita");
            }






        }

        [HttpPost("{id}/decrypt")]
        public async Task<IActionResult> DecryptImage(int id, [FromBody] DecryptImageRequest request)
        {
            // Aflarea user-ului care face request-ul pentru a verifica daca are dreptul de a decripta imaginea
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null)
            {
                return Unauthorized(); 
            }

            // Obtinerea imaginii din baza de date pe baza id-ului si a userId-ului

            var imageFile = await _context.ImageFiles.FirstOrDefaultAsync(x =>
                    x.Id == id &&
                    x.UserId == userId);

            if (imageFile == null)
            { 
                return NotFound();
            }

            //Path-ul catre fotografie encripted
            var sourcePath = Path.Combine(Directory.GetCurrentDirectory(), imageFile.FilePath.TrimStart('/'));

            if (!System.IO.File.Exists(sourcePath))
                { 
                return NotFound(); 
                }

            //Citire fisier encripted

            var encryptedBytes = await System.IO.File.ReadAllBytesAsync(sourcePath);

            //Schimbare metadate din Base64 in byte[] pentru a putea decripta imaginea

            var salt = Convert.FromBase64String(imageFile.Salt);
            var nonce = Convert.FromBase64String(imageFile.Nonce);
            var tag = Convert.FromBase64String(imageFile.Tag);

            try
            {
                //se incearca decriptarea imaginii folosind parola si metadatele salvate in baza de date
                var decryptedBytes = _encryptionService.Decrypt(encryptedBytes, request.EncryptionPassword, salt, nonce, tag);
                return File(decryptedBytes, imageFile.ContentType);
            }


            catch (CryptographicException)
            {
                return BadRequest("Parola gresita");
            }

            
        }


        //Endpoint pentru cautarea imaginilor dupa subiect
        [HttpGet("subject/{subject}")]
        public async Task<ActionResult<List<ImageFile>>> GetImageBySubject(string subject)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }
            var images = await _context.ImageFiles
                .Where(x => x.UserId == userId && x.Subject.Contains(subject))
                .ToListAsync();
            return Ok(images);
        }

        //Endpoint pentru cautarea subiectelor disponibile pentru un anumit user
        [HttpGet("subject")]
        public async Task<ActionResult<List<string>>> GetSubjects()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if(userId==null)
            {
                return Unauthorized();
            }

            var subject = await _context.ImageFiles
                .Where(x => x.UserId == userId)
                .Select(x => x.Subject)
                .Distinct()
                .ToListAsync();

            return Ok();
        }

    }
}
