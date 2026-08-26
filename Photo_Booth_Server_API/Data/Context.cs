using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Photo_Booth_Server_API.Models;

namespace Photo_Booth_Server_API.Data
{
    public class Context : IdentityDbContext<ApplicationUser>
    {
        public Context(DbContextOptions<Context> options) : base(options) { } //constructor DataBase

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ImageFile>().HasData(
            new ImageFile { Id = 1, Subject = "Sample Image 1", FilePath = "path/to/image1.jpg" },
            new ImageFile { Id = 2, Subject = "Sample Image 2", FilePath = "path/to/image2.jpg" },
            new ImageFile { Id = 3, Subject = "Sample Image 3", FilePath = "path/to/image3.jpg" }

                    );
        }
        public DbSet <ImageFile> ImageFiles { get; set; } //tabela ImageFiles


    }
}
