using Mensageria.Domain.Dtos;
using Mensageria.Domain.Entities;
using Mensageria.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.Domain.Services
{
    public class ArchiveServices : IArchiveServices
    {
        private readonly IArchiveRepositories _archiveRepositories;

        public ArchiveServices(IArchiveRepositories archiveRepositories)
        {
            _archiveRepositories = archiveRepositories;
        }

        public ArchiveResponseDto CreateArchive(ArchiveRequestDto archiveDto)
        {
            var archive = new Archive
            {
                Name = archiveDto.Name,
                Type = archiveDto.Type,
                Content = archiveDto.Content
            };

            var archiveCreated = _archiveRepositories.Create(archive);

            return new ArchiveResponseDto
            {
                Id = archiveCreated.Id,
                Name = archiveCreated.Name,
                Type = archiveCreated.Type,
                Content = archiveCreated.Content
            };

        }

        public ArchiveResponseDto GetArchiveBydId(Guid Id)
        {
            var archive = _archiveRepositories.GetById(Id);

            return new ArchiveResponseDto
            {
                Id = archive.Id,
                Name = archive.Name,
                Type = archive.Type,
                Content = archive.Content
            };
        }
    }
}
