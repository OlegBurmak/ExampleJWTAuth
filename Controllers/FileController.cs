using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Converter.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        
        [EnableCors]
        [HttpPost]
        public async Task<IActionResult> Post([FromForm]IFormFile uploadFile)
        {

            if(uploadFile == null)
            {
                return NotFound();
            }
            else{
                System.Console.WriteLine(uploadFile.FileName);

                var memory = new MemoryStream();
                await uploadFile.OpenReadStream().CopyToAsync(memory);
                memory.Position = 0;
                
                return File(memory, uploadFile.ContentType, uploadFile.FileName);
                
            }
        }

        private async Task<IActionResult> DownloadFile(IFormFile file)
        {
            var memory = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(memory);
            memory.Position = 0;
            return File(memory, file.ContentType, file.FileName);
        }
    }
}