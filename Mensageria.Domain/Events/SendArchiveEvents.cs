using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.Domain.Events
{
    public class SendArchiveEvents
    {
        public Guid Id { get; set; }
        public string? ArchiveName { get; set; }
        public string? ArchiveType { get; set; }
        public byte[]? ArchiveContent { get; set; }
        public string? Email { get; set; }
    }
}
