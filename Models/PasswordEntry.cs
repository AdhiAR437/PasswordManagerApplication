using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace PasswordManagerApplication.Models
{
    public class PasswordEntry
    {

        [Key]
        [Required]
        public int PasswordEntryId { get; set; } // Primary key for the PasswordEntry table


        // Title is required, with a maximum length and an error message if exceeded
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(400, ErrorMessage = "Maximum Length Exceeded")]
        public string Title { get; set; } = string.Empty; // A brief title for the password (e.g., 'Google Account')


        // Website should be a valid URL, and optional
        [Url(ErrorMessage = "Invalid website URL format.")]
        [StringLength(500, ErrorMessage = "Website URL is too long.")]
        public string Website { get; set; } = string.Empty; // URL of the website


        // Username is required, but a maximum length could be specified.
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(150, ErrorMessage = "Username cannot be longer than 150 characters.")]
        public string Username { get; set; } = string.Empty; // Username for the site


        // Password should be required and have a minimum length.
        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty; // Password (could be encrypted)


        // UserId is required, ensuring this is a foreign key relationship to a User.
        [Required(ErrorMessage = "User ID is required.")]
        public string UserId { get; set; } = string.Empty; // Foreign key to User


        // Navigation property for User (no annotation needed for the relationship itself)
        [JsonIgnore]
        [ForeignKey("UserId")]  // This tells EF Core that UserId is the foreign key to the User model
        public virtual User? User { get; set; } // Navigation property for User
    }
}
