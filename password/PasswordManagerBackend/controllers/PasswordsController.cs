using Microsoft.AspNetCore.Mvc;
using PasswordManagerBackend.Models;
using PasswordManagerBackend.Services;
using System.Collections.Generic;
using System.Threading.Tasks; // For async actions

namespace PasswordManagerBackend.Controllers
{
    [ApiController] // Indicates this class is an API controller
    [Route("api/[controller]")] // Sets the base route for this controller, e.g., /api/Passwords
    public class PasswordsController : ControllerBase
    {
        private readonly PasswordService _passwordService;

        // Dependency Injection: PasswordService is injected here
        public PasswordsController(PasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        // GET /api/Passwords
        [HttpGet]
        public ActionResult<IEnumerable<PasswordEntry>> Get()
        {
            return Ok(_passwordService.GetAllEntries());
        }

        // POST /api/Passwords
        [HttpPost]
        public ActionResult<PasswordEntry> Post([FromBody] PasswordEntry newEntryRequest)
        {
            // Validate incoming data
            if (string.IsNullOrWhiteSpace(newEntryRequest.Website) ||
                string.IsNullOrWhiteSpace(newEntryRequest.Username) ||
                string.IsNullOrWhiteSpace(newEntryRequest.HashedPassword) ||
                newEntryRequest.Salt == null ||
                newEntryRequest.Salt.Length != 16) // Check for 16-byte salt as expected
            {
                return BadRequest("Website, username, hashed password, and a valid 16-byte salt are required.");
            }

            // Add the entry. The frontend already performed the PBKDF2 hashing.
            // In a production app, the raw password might be sent and hashed here.
            var addedEntry = _passwordService.AddEntry(newEntryRequest);

            // Return 201 Created status with the newly created resource's URL
            return CreatedAtAction(nameof(Get), new { id = addedEntry.Id }, addedEntry);
        }

        // DELETE /api/Passwords/{id}
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            bool deleted = _passwordService.DeleteEntry(id);
            if (deleted)
            {
                return NoContent(); // 204 No Content for successful deletion
            }
            return NotFound(); // 404 Not Found if the entry doesn't exist
        }

        // DELETE /api/Passwords/clearall
        [HttpDelete("clearall")]
        public IActionResult ClearAll()
        {
            _passwordService.ClearAllEntries();
            return NoContent(); // 204 No Content for successful operation
        }
    }
}