namespace Mensageria.Domain.Entities
{
    public class Archive
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public byte[] Content { get; set; } = Array.Empty<byte>();

    }
}
