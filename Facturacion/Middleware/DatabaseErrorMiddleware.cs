using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace Facturacion.Middleware
{
    public class DatabaseErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<DatabaseErrorMiddleware> _logger;

        public DatabaseErrorMiddleware(RequestDelegate next, ILogger<DatabaseErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "Error de conexión a la base de datos SQL Server");
                
                // Redirigir a página de error personalizada
                context.Response.Redirect("/Home/DatabaseError");
            }
            catch (InvalidOperationException invEx) when (invEx.Message.Contains("database"))
            {
                _logger.LogError(invEx, "Error de operación con la base de datos");
                
                context.Response.Redirect("/Home/DatabaseError");
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Error al actualizar la base de datos");
                
                // Permitir que los controladores manejen estos errores específicamente
                throw;
            }
        }
    }
}
