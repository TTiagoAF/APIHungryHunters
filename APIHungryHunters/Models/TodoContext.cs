using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace APIHungryHunters.Models
{
    public class TodoContext : DbContext
    {
        public TodoContext(DbContextOptions<TodoContext> opcao)
        : base(opcao)
        {
        }

        public DbSet<Contas> Contas { get; set; } = null!;
    }
}
