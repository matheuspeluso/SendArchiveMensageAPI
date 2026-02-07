using Mensageria.Domain.Dtos;
using Mensageria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.Domain.Interfaces.Services
{
    public interface IArchiveServices
    {
        ArchiveResponseDto CreateArchive(ArchiveRequestDto archiveDto);
        ArchiveResponseDto GetArchiveBydId(Guid Id, string Email);
    }
}
