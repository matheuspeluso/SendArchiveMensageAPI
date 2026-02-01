using Mensageria.Domain.Entities;
using Mensageria.Domain.Interfaces;
using Mensageria.InfraData.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.InfraData.Repositories
{
    public class ArchiveRepositories : IArchiveRepositories
    {
        private readonly DataContext _context;

        public ArchiveRepositories(DataContext context)
        {
            _context = context;
        }

        public Archive Create(Archive archive)
        {
            _context.Set<Archive>().Add(archive);
            _context.SaveChanges();
            return archive;
        }
    }
}
