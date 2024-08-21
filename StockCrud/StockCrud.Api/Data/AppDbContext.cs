using Microsoft.EntityFrameworkCore;
using StockCrud.Api.Entities;

namespace StockCrud.Api.Data;

public class AppDbContext : DbContext
{ 
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<Category> Categories { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Supplier> Suppliers { get; set; }
    public DbSet<Order> Orders { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Category>()
            .Property(c => c.CreatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        modelBuilder.Entity<Category>()
            .Property(c => c.UpdatedDate)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        // Converte todos os nomes de tabelas e colunas para minúsculas
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            // Define o nome da tabela
            entity.SetTableName(entity.GetTableName().ToLower());

            // Define o nome das colunas
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.GetColumnName().ToLower());
            }

            // Define o nome das chaves
            foreach (var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToLower());
            }

            // Define o nome dos índices
            foreach (var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToLower());
            }

            // Define o nome das chaves estrangeiras
            foreach (var foreignKey in entity.GetForeignKeys())
            {
                foreignKey.SetConstraintName(foreignKey.GetConstraintName().ToLower());
            }
        }
    }
}