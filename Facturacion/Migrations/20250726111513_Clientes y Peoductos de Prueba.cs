using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Facturacion.Migrations
{
    /// <inheritdoc />
    public partial class ClientesyPeoductosdePrueba : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clientes",
                columns: new[] { "Id", "Apellido", "CIF", "Direccion", "Email", "NIF", "Nombre", "Telefono" },
                values: new object[,]
                {
                    { 1, "García López", null, "Calle Mayor, 123, 28001 Madrid", "maria.garcia@email.com", "12345678A", "María", "+34 600 123 456" },
                    { 2, "Tecnológica S.L.", "B12345678", "Avenida de la Innovación, 45, 08001 Barcelona", "contacto@innovaciontech.es", null, "Innovación", "+34 930 123 456" },
                    { 3, "Martínez Ruiz", null, "Plaza del Sol, 8, 41001 Sevilla", "carlos.martinez@hotmail.com", "23456789B", "Carlos", "+34 650 234 567" },
                    { 4, "Rodríguez Fernández", null, "Calle de la Paz, 67, 46001 Valencia", "ana.rodriguez@gmail.com", "34567890C", "Ana", "+34 670 345 678" },
                    { 5, "del Norte S.A.", "A23456789", "Polígono Industrial Norte, Nave 12, 48001 Bilbao", "info@construccionesnorte.com", null, "Construcciones", "+34 940 456 789" },
                    { 6, "López González", null, "Ronda de la Universidad, 34, 37001 Salamanca", "david.lopez@outlook.com", "45678901D", "David", "+34 680 567 890" },
                    { 7, "Sánchez Moreno", null, "Calle del Carmen, 19, 18001 Granada", "elena.sanchez@yahoo.es", "56789012E", "Elena", "+34 690 678 901" },
                    { 8, "Mediterráneo S.L.", "B34567890", "Paseo Marítimo, 156, 03001 Alicante", "ventas@comercialmediterraneo.es", null, "Comercial", "+34 960 789 012" },
                    { 9, "Hernández Jiménez", null, "Gran Vía, 89, 50001 Zaragoza", "javier.hernandez@telefonica.net", "67890123F", "Javier", "+34 600 890 123" },
                    { 10, "Díaz Romero", null, "Calle Real, 45, 15001 A Coruña", "laura.diaz@icloud.com", "78901234G", "Laura", "+34 610 901 234" }
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "CategoriaId", "Descripcion", "Nombre", "PrecioUnitario" },
                values: new object[,]
                {
                    { 1, 3, "Análisis completo de estrategia digital y plan de mejora personalizado", "Consultoría de Marketing Digital", 850.00m },
                    { 2, 1, "Creación de sitio web responsive con diseño personalizado y CMS", "Desarrollo Web Personalizado", 1200.00m },
                    { 3, 2, "Licencia de software de gestión empresarial por 12 meses", "Licencia Software Anual", 299.99m },
                    { 4, 1, "Servicio de mantenimiento y soporte técnico especializado", "Mantenimiento Técnico Mensual", 150.00m },
                    { 5, 1, "Curso de formación presencial de 16 horas en nuevas tecnologías", "Formación Especializada", 450.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Clientes",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Productos",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
