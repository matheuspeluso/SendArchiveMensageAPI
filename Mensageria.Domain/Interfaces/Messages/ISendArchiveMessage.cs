using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mensageria.Domain.Events;

namespace Mensageria.Domain.Interfaces.Messages
{
    public interface ISendArchiveMessage
    {
        public void Publisher(SendArchiveEvents ArchivePayload);
    }
}
