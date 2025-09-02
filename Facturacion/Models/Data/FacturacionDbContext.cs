using Microsoft.EntityFrameworkCore;
using Facturacion.Models.Entities;

namespace Facturacion.Models.Data
{
    public class FacturacionDbContext : DbContext
    {
        public FacturacionDbContext(DbContextOptions<FacturacionDbContext> options) : base(options)
        {
        }
        
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<FacturaDetalle> FacturaDetalles { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // Configuración de Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.HasIndex(e => e.Nombre).IsUnique();
            });
            
            // Configuración de Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Apellido).IsRequired().HasMaxLength(100);
                entity.Property(e => e.NIF).HasMaxLength(9);
                entity.Property(e => e.CIF).HasMaxLength(9);
                entity.Property(e => e.Direccion).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Telefono).HasMaxLength(15);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.NIF).IsUnique();
                entity.HasIndex(e => e.CIF).IsUnique();
            });
            
            // Configuración de Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Descripcion).HasMaxLength(500);
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)").IsRequired();
                
                entity.HasOne(p => p.Categoria)
                      .WithMany(c => c.Productos)
                      .HasForeignKey(p => p.CategoriaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Configuración de Factura
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.NumeroFactura).IsRequired().HasMaxLength(20);
                entity.Property(e => e.Fecha).IsRequired();
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IRPF).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IVA).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Total).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Observaciones).HasMaxLength(1000);
                entity.Property(e => e.FormaPago).HasMaxLength(100);
                entity.HasIndex(e => e.NumeroFactura).IsUnique();
                
                entity.HasOne(f => f.Cliente)
                      .WithMany(c => c.Facturas)
                      .HasForeignKey(f => f.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Configuración de FacturaDetalle
            modelBuilder.Entity<FacturaDetalle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cantidad).IsRequired();
                entity.Property(e => e.PrecioUnitario).HasColumnType("decimal(18,2)").IsRequired();
                entity.Property(e => e.Subtotal).HasColumnType("decimal(18,2)");
                
                entity.HasOne(fd => fd.Factura)
                      .WithMany(f => f.FacturaDetalles)
                      .HasForeignKey(fd => fd.FacturaId)
                      .OnDelete(DeleteBehavior.Cascade);
                      
                entity.HasOne(fd => fd.Producto)
                      .WithMany(p => p.FacturaDetalles)
                      .HasForeignKey(fd => fd.ProductoId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
            
            // Datos semilla para Categorías
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Servicios", Descripcion = "Servicios profesionales" },
                new Categoria { Id = 2, Nombre = "Productos", Descripcion = "Productos físicos" },
                new Categoria { Id = 3, Nombre = "Consultoría", Descripcion = "Servicios de consultoría" }
            );

            // Datos semilla para Clientes
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente 
                { 
                    Id = 1, 
                    Nombre = "María", 
                    Apellido = "García López", 
                    NIF = "12345678A", 
                    Direccion = "Calle Mayor, 123, 28001 Madrid", 
                    Telefono = "+34 600 123 456", 
                    Email = "maria.garcia@email.com" 
                },
                new Cliente 
                { 
                    Id = 2, 
                    Nombre = "Innovación", 
                    Apellido = "Tecnológica S.L.", 
                    CIF = "B12345678", 
                    Direccion = "Avenida de la Innovación, 45, 08001 Barcelona", 
                    Telefono = "+34 930 123 456", 
                    Email = "contacto@innovaciontech.es" 
                },
                new Cliente 
                { 
                    Id = 3, 
                    Nombre = "Carlos", 
                    Apellido = "Martínez Ruiz", 
                    NIF = "23456789B", 
                    Direccion = "Plaza del Sol, 8, 41001 Sevilla", 
                    Telefono = "+34 650 234 567", 
                    Email = "carlos.martinez@hotmail.com" 
                },
                new Cliente 
                { 
                    Id = 4, 
                    Nombre = "Ana", 
                    Apellido = "Rodríguez Fernández", 
                    NIF = "34567890C", 
                    Direccion = "Calle de la Paz, 67, 46001 Valencia", 
                    Telefono = "+34 670 345 678", 
                    Email = "ana.rodriguez@gmail.com" 
                },
                new Cliente 
                { 
                    Id = 5, 
                    Nombre = "Construcciones", 
                    Apellido = "del Norte S.A.", 
                    CIF = "A23456789", 
                    Direccion = "Polígono Industrial Norte, Nave 12, 48001 Bilbao", 
                    Telefono = "+34 940 456 789", 
                    Email = "info@construccionesnorte.com" 
                },
                new Cliente 
                { 
                    Id = 6, 
                    Nombre = "David", 
                    Apellido = "López González", 
                    NIF = "45678901D", 
                    Direccion = "Ronda de la Universidad, 34, 37001 Salamanca", 
                    Telefono = "+34 680 567 890", 
                    Email = "david.lopez@outlook.com" 
                },
                new Cliente 
                { 
                    Id = 7, 
                    Nombre = "Elena", 
                    Apellido = "Sánchez Moreno", 
                    NIF = "56789012E", 
                    Direccion = "Calle del Carmen, 19, 18001 Granada", 
                    Telefono = "+34 690 678 901", 
                    Email = "elena.sanchez@yahoo.es" 
                },
                new Cliente 
                { 
                    Id = 8, 
                    Nombre = "Comercial", 
                    Apellido = "Mediterráneo S.L.", 
                    CIF = "B34567890", 
                    Direccion = "Paseo Marítimo, 156, 03001 Alicante", 
                    Telefono = "+34 960 789 012", 
                    Email = "ventas@comercialmediterraneo.es" 
                },
                new Cliente 
                { 
                    Id = 9, 
                    Nombre = "Javier", 
                    Apellido = "Hernández Jiménez", 
                    NIF = "67890123F", 
                    Direccion = "Gran Vía, 89, 50001 Zaragoza", 
                    Telefono = "+34 600 890 123", 
                    Email = "javier.hernandez@telefonica.net" 
                },
                new Cliente 
                { 
                    Id = 10, 
                    Nombre = "Laura", 
                    Apellido = "Díaz Romero", 
                    NIF = "78901234G", 
                    Direccion = "Calle Real, 45, 15001 A Coruña", 
                    Telefono = "+34 610 901 234", 
                    Email = "laura.diaz@icloud.com" 
                }
            );

            // Datos semilla para Productos
            modelBuilder.Entity<Producto>().HasData(
                new Producto 
                { 
                    Id = 1, 
                    Nombre = "Consultoría de Marketing Digital", 
                    Descripcion = "Análisis completo de estrategia digital y plan de mejora personalizado", 
                    PrecioUnitario = 850.00m, 
                    CategoriaId = 3 
                },
                new Producto 
                { 
                    Id = 2, 
                    Nombre = "Desarrollo Web Personalizado", 
                    Descripcion = "Creación de sitio web responsive con diseño personalizado y CMS", 
                    PrecioUnitario = 1200.00m, 
                    CategoriaId = 1 
                },
                new Producto 
                { 
                    Id = 3, 
                    Nombre = "Licencia Software Anual", 
                    Descripcion = "Licencia de software de gestión empresarial por 12 meses", 
                    PrecioUnitario = 299.99m, 
                    CategoriaId = 2 
                },
                new Producto 
                { 
                    Id = 4, 
                    Nombre = "Mantenimiento Técnico Mensual", 
                    Descripcion = "Servicio de mantenimiento y soporte técnico especializado", 
                    PrecioUnitario = 150.00m, 
                    CategoriaId = 1 
                },
                new Producto 
                { 
                    Id = 5, 
                    Nombre = "Formación Especializada", 
                    Descripcion = "Curso de formación presencial de 16 horas en nuevas tecnologías", 
                    PrecioUnitario = 450.00m, 
                    CategoriaId = 1 
                }
            );
        }
    }
}