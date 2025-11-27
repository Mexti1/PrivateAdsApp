namespace AdsClientWpf.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public int AdId { get; set; }
        public Ad? Ad { get; set; }
        public byte[]? ImageData { get; set; }
        public string? FileName { get; set; }
        public bool IsMain { get; set; }
    }
}
