using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servify.Data;
using Servify.DTOs;
using Servify.Models;
using Servify.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Servify.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize] // Requires authorization for all methods in this controller
    public class UserController : ControllerBase
    {
        private readonly ServifyDbContext _context;

        public UserController(ServifyDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("UploadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadFile(IFormFile file, CancellationToken cancellationtoken)
        {
            try
            {
                // Get current user's ID from the token
                var userId = GetCurrentUser()?.Id;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in the token." });
                }

                // Upload the file
                var filename = await FileHandler.UploadFile(file);

                // Construct a DocumentDto
                var document= new Document
                {
                    FileName = filename,
                    FilePath = Path.Combine(FileHandler.UploadPath, filename),
                    UserId = new Guid(userId),
                    FileSize = file.Length
                };

                _context.Documents.Add(document);
                await _context.SaveChangesAsync();

                return Ok(new { message = "File uploaded successfully", document});
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "Internal server error" });
            }
        }


        [HttpGet("DownloadFile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            try
            {
                // Get the current user's ID from the token
                var userId = GetCurrentUser()?.Id;

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized(new { message = "User ID not found in the token." });
                }

                // Check if the file exists
                var filePath = Path.Combine(FileHandler.UploadPath, filename);
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound(new { message = "File not found." });
                }

                // Check if the file belongs to the current user
                var document = GetDocumentByFileNameAndUserId(filename, userId);
                if (document == null)
                {
                    return Forbid("You are not authorized to access this file.");
                }

                // Download the file using the FileHandler helper function
                var fileBytes = await FileHandler.DownloadFile(filename);

                // Return the file
                return File(fileBytes, "application/octet-stream", filename);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, new { message = "Internal server error" });
            }
        }



        private User? GetCurrentUser()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;
                return new User
                {
                    Id = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                    UserName = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value,
                    Email = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                };
            }
            return null;
        }

        private Document? GetDocumentByFileNameAndUserId(string filename, string userIdString)
        {
            if (!Guid.TryParse(userIdString, out Guid userId))
            {
                // Handle invalid userIdString here, such as returning null or throwing an exception
                return null;
            }

            var document = _context.Documents
                .FirstOrDefault(d => d.FileName == filename && d.UserId == userId);

            return document;
        }
    }
}