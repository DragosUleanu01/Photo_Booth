namespace Photo_Booth_Server_API.Models
{
    public class ImageFile
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        
        public string FilePath { get; set; } = string.Empty;    

        public string UserId { get; set; } = string.Empty; // Cheie externa pentru ApplicationUser


    }
}
