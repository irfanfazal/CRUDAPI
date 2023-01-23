using CRUD1.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CRUD1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class uploadImgController : ControllerBase
    {
        private readonly usersContext _context;

        public uploadImgController(usersContext context)
        {
            _context = context;
        }

        [HttpPost,DisableRequestSizeLimit]
        
        public async Task<IActionResult> Upload([FromForm]users user)
        {
            try
            {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPth = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream= new FileStream(fullPth, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    user.ImgPath = dbPath;
                    _context.Add(user);
                    _context.SaveChanges();
                    return Ok(new { dbPath });
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception exp)
            {
                return StatusCode(500, $"Internal Server Error:{exp}");
            }
        }
    }
}
/*
 The logic inside this action is pretty straightforward. We extract the file from the request and provide the path to store the file. 
Moreover, if the file has a length greater than zero, we just take its name and provide a full path on the server to store our file and a path to the database. 
This database path is going to be returned as a result of this action after we place our stream into the defined folder. 
We could also check if a file with the same name already exists, but didn’t want to make the code more complicated at this moment.
 */
