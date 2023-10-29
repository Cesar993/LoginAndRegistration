#pragma warning disable CS8618
using Microsoft.EntityFrameworkCore;
namespace InicioSesion.Models;
public class MyContext : DbContext 
{   
    //un dbset por cada tabla
    public DbSet<Usuario> Usuarios { get; set; } 
    // This line will always be here. It is what constructs our context upon initialization  
    public MyContext(DbContextOptions options) : base(options) { }    

}
