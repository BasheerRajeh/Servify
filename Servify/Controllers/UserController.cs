using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servify.Data;

namespace Servify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ServifyDbContext _context;

        public UserController(ServifyDbContext context)
        {
            _context = context;
        }


    }
}
