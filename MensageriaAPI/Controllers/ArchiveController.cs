using Mensageria.Domain.Dtos;
using Mensageria.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MensageriaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArchiveController : ControllerBase
    {
        private readonly IArchiveServices _archiveServices;

        public ArchiveController(IArchiveServices archiveServices)
        {
            _archiveServices = archiveServices;
        }

        [HttpPost]
        public IActionResult CreateArchive([FromBody] ArchiveRequestDto request)
        {

            try
            {
                return Ok(_archiveServices.CreateArchive(request));
            }catch(Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetById/{id}")]
        public IActionResult GetArchiveById(Guid id, string email)
        {
            try
            {
               return Ok(_archiveServices.GetArchiveBydId(id, email));
            }
            catch (ApplicationException e)
            {
                return BadRequest(e.Message);
            }
            catch(Exception e)
            {
                return StatusCode(500, 
                    new { message = "Erro ao consultar arquivo.", exception = e.Message });
            }
        }

    }
}
