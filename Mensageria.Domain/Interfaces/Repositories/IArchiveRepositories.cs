using Mensageria.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mensageria.Domain.Interfaces.Repositories
{
    public interface IArchiveRepositories
    {
        Archive Create(Archive archive);
        Archive GetById(Guid Id);
    }
}
