namespace Photo_Booth_Server_API.DTO
{
    public class FilterImageRequest
    {
        public string Filter { get; set; } = string.Empty;
        public string EncryptionPassword { get; set; } = string.Empty;



    }
}
