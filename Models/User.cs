using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PasswordManagerApplication.Models
{
    public class User
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }= string.Empty;


        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Maximum Length Exceeded")]
        public string PasswordHash { get; set; } = string.Empty; // Store hashed password



        [Key]
        [Required]
        [EmailAddress]
        [StringLength(100, ErrorMessage = "Maximum Length Exceeded")]
        public string Email { get; set; } = string.Empty;


        [StringLength(400, ErrorMessage = "Maximum Length Exceeded")]
        public string SecurityQuestion { get; set; } = string.Empty;

        
        [StringLength(100, ErrorMessage = "Maximum Length Exceeded")]
        public string SecurityQAns { get; set; } = string.Empty;


        [JsonIgnore]
        //Reference to PasswordEntry
        public virtual ICollection<PasswordEntry>? PasswordEntries { get; set; } // User's password entries
    }

}
