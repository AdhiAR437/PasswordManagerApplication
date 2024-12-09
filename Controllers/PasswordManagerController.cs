using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PasswordManagerApplication.Data;
using PasswordManagerApplication.Helpers;
using PasswordManagerApplication.Models;
using System.Security.Claims;

namespace PasswordManagerApplication.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    
    public class PasswordManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PasswordManagerController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: PasswordManager
        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Index()
        {
            var userId = GetCurrentUserId();
            var passwords = _context.PasswordEntries_tb.Where(pe => pe.UserId == userId).ToList();
            return View(passwords);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // GET: PasswordManager/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PasswordManager/Create
        [HttpPost]
        public async Task<IActionResult> Create(PasswordEntry model)
        {
            if (ModelState.IsValid)
            {
                model.Password= SimpleEncryptionHelper.Encrypt(model.Password);
                model.UserId = GetCurrentUserId();  // Assuming GetCurrentUserId() returns the current user's ID
                _context.PasswordEntries_tb.Add(model);  // Add the new password entry to the context
                await _context.SaveChangesAsync();  // Save the changes to the database

                // After saving, the PasswordEntryId will be automatically populated by the database
                return Ok(new
                {
                    PasswordEntryId = model.PasswordEntryId,  // The database-generated ID
                    UserId = model.UserId,
                    Title = model.Title,
                    Website = model.Website,
                    Username = model.Username,
                });
            }

            // If the model is invalid, return a BadRequest with validation errors
            return BadRequest(ModelState);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // GET: PasswordManager/Edit/5
        public IActionResult Edit(int id)
        {
            var passwordEntry = _context.PasswordEntries_tb.FirstOrDefault(pe => pe.PasswordEntryId == id && pe.UserId == GetCurrentUserId());
            if (passwordEntry == null)
            {
                return NotFound();
            }
            return View(passwordEntry);
        }


        // POST: PasswordManager/Edit/5
        [HttpPatch]
        public async Task<IActionResult> Edit(PasswordEntry model)
        {
            if (model.UserId != GetCurrentUserId())
            {
                return NotFound();
            }

            var existingEntry = await _context.PasswordEntries_tb.FirstOrDefaultAsync(p => p.PasswordEntryId == model.PasswordEntryId);

            // If the entry is not found, return a NotFound result
            if (existingEntry == null)
            {
                return NotFound("Password entry not found.");
            }

            // Update the fields of the existing entry
            existingEntry.Website = model.Website;
            existingEntry.Username = model.Username;
            existingEntry.Password = SimpleEncryptionHelper.Encrypt(model.Password);
            existingEntry.Title = model.Title;  // Allow the title to be updated as well

            // Save changes to the database
            _context.Update(existingEntry);  // Mark the existing entity as modified
            await _context.SaveChangesAsync();  // Commit the changes to the database

            return Ok("Entry Updated Successfully");
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        // GET: PasswordManager/Delete/5
        public IActionResult Delete(int id)
        {
            var passwordEntry = _context.PasswordEntries_tb.FirstOrDefault(pe => pe.PasswordEntryId == id && pe.UserId == GetCurrentUserId());
            if (passwordEntry == null)
            {
                return NotFound();
            }
            return View(passwordEntry);
        }

        // POST: PasswordManager/Delete/5
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(DeletePasswordEntryDTO request)
        {
            var userId=GetCurrentUserId();
            var title = request.Title;
            var passwordEntry = await _context.PasswordEntries_tb.FirstOrDefaultAsync(pe => pe.UserId == userId && pe.Title == title);

            // If the password entry doesn't exist, return a NotFound or handle the error
            if (passwordEntry == null)
            {
                return NotFound("Password entry not found or you don't have permission to delete it.");
            }

            // Remove the password entry from the database
            _context.PasswordEntries_tb.Remove(passwordEntry);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return Ok("Deleted Password Entry Successfully");
        }


        [HttpGet]   
        public ActionResult<IEnumerable<PasswordEntry>> LoadEntries()
        {
            var userEmail = GetCurrentUserId(); // Assuming the user is logged in and their email is in User.Identity.Name

            // Fetch the password entries for the logged-in user
            var entries = _context.PasswordEntries_tb
                                  .Where(p => p.UserId == userEmail)
                                  .Select(p => new
                                  {
                                      p.PasswordEntryId,
                                      p.Title,
                                      p.Website,
                                      p.Username,
                                      DecryptedPassword = SimpleEncryptionHelper.Decrypt(p.Password)
                                  })
                                  .ToList();

            if (entries == null || !entries.Any())
            {
                return NotFound("No password entries found.");
            }

            return Ok(entries); // Return the password entries as JSON
        }


        private string GetCurrentUserId()
        {
            return HttpContext.Session.GetString("UserEmail");
        }
    }
}
