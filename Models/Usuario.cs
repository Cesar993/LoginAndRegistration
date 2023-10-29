#pragma warning disable CS8618
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace InicioSesion.Models;
public class Usuario
{
    [Key]
    [Required]
    public int UserId { get; set; }

    [MinLength(3, ErrorMessage = "Oops el minimo es de 3 caracteres")]
    [Required]
    public string Nombre { get; set; }
    [MinLength(3, ErrorMessage = "Oops el minimo es de 3 caracteres")]
    [Required]
    public string Apellido { get; set; }

    [EmailAddress(ErrorMessage = "por favor proporciona un correo valido")]
    [Required]
    [UniqueEmailAttribute(ErrorMessage ="El correo debe ser distinto")]
    public string Email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public DateTime Fecha_Creacion {get;set;} = DateTime.Now;
    public DateTime Fecha_Actualizacion {get;set;} = DateTime.Now;

    [NotMapped]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Pasword confirmado")]
    public string PasswordConfirm { get; set; }

}

public class UniqueEmailAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
    	// Though we have Required as a validation, sometimes we make it here anyways
    	// In which case we must first verify the value is not null before we proceed
        if(value == null)
        {
    	    // If it was, return the required error
            return new ValidationResult("Email is required!");
        }
    
    	// This will connect us to our database since we are not in our Controller
        MyContext _context = (MyContext)validationContext.GetService(typeof(MyContext));
        // Check to see if there are any records of this email in our database
    	if(_context.Usuarios.Any(e => e.Email == value.ToString()))
        {
    	    // If yes, throw an error
            return new ValidationResult("Email must be unique!");
        } else {
    	    // If no, proceed
            return ValidationResult.Success;
        }
    }
}

