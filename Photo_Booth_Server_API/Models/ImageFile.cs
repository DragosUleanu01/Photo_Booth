namespace Photo_Booth_Server_API.Models
{
    public class ImageFile
    {
        public int Id { get; set; }
        public string Subject { get; set; } = string.Empty;
        
        public string FilePath { get; set; } = string.Empty;    

        public string UserId { get; set; } = string.Empty; // Cheie externa pentru ApplicationUser

        //Content Type generic pentru acceptarea mai multor tipuri de fisiere (ex: image/jpeg, image/png, etc.)
        public string ContentType { get; set; } = string.Empty;

        //Proprietati pentru criptare

        public string Salt { get; set; } = string.Empty;
        public string Nonce { get; set; } = string.Empty;
        public string Tag {  get; set; } = string.Empty;


    }
}
