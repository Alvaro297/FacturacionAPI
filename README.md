# 🧾 Sistema de Facturación

Un sistema de gestión de facturas moderno y completo desarrollado en **ASP.NET Core 9** con **Entity Framework Core** y **SQL Server**.

![.NET Version](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core%209.0-512BD4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square&logo=microsoftsqlserver)
![License](https://img.shields.io/badge/License-MIT-green?style=flat-square)

## ✨ Características Principales

- 📋 **Gestión de Clientes**: CRUD completo con validaciones
- 📦 **Gestión de Productos**: Control de inventario y categorías
- 🧾 **Facturación**: Generación y gestión de facturas
- 📊 **Estadísticas**: Dashboard con métricas del negocio
- 💾 **Persistencia**: Base de datos SQL Server con Entity Framework Core
- 🔄 **Migraciones**: Sistema automático de versionado de BD
- ⚡ **Rendimiento**: Conexiones resilientes y retry automático
- 🛡️ **Validaciones**: Validación robusta del lado servidor

## 🏗️ Arquitectura

```
Sistema.Facturacion/
├── Controllers/           # Controladores MVC
│   ├── ClientesController.cs
│   ├── ProductosController.cs
│   ├── FacturasController.cs
│   ├── CategoriasController.cs
│   └── EstadisticasController.cs
├── Models/
│   ├── Entities/         # Entidades del dominio
│   │   ├── Cliente.cs
│   │   ├── Producto.cs
│   │   ├── Factura.cs
│   │   ├── FacturaDetalle.cs
│   │   └── Categoria.cs
│   └── Data/
│       └── FacturacionDbContext.cs
├── Views/                # Vistas Razor
├── Middleware/           # Middleware personalizado
└── Migrations/           # Migraciones EF Core
```

## 🚀 Inicio Rápido

### Prerequisitos

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB, Express, o Standard)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [VS Code](https://code.visualstudio.com/)

### Instalación

1. **Clona el repositorio**
   ```bash
   git clone https://github.com/Alvaro297/FacturacionAPI
   cd sistema-facturacion
   ```

2. **Configura la cadena de conexión**
   
   Edita `appsettings.json` y actualiza la cadena de conexión:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=FacturacionDB;Trusted_Connection=true;MultipleActiveResultSets=true"
     }
   }
   ```

3. **Ejecuta las migraciones**
   ```bash
   dotnet ef database update
   ```

4. **Inicia la aplicación**
   ```bash
   dotnet run
   ```

5. **Accede a la aplicación**
   
   Abre tu navegador en `https://localhost:5001` o `http://localhost:5000`

## 📦 Dependencias

| Paquete | Versión | Propósito |
|---------|---------|-----------|
| Microsoft.EntityFrameworkCore.SqlServer | 9.0.0 | Proveedor EF Core para SQL Server |
| Microsoft.EntityFrameworkCore.Tools | 9.0.0 | Herramientas CLI para migraciones |
| Microsoft.EntityFrameworkCore.Design | 9.0.0 | Herramientas de diseño para EF Core |

## 🗄️ Modelo de Datos

### Entidades Principales

- **Cliente**: Información de clientes (NIF/CIF, contacto, etc.)
- **Producto**: Catálogo de productos con categorías
- **Factura**: Documentos de facturación con cálculos automáticos
- **FacturaDetalle**: Líneas de detalle de cada factura
- **Categoria**: Clasificación de productos

### Características del Modelo

- ✅ Validaciones automáticas
- ✅ Cálculo automático de totales (IRPF, IVA)
- ✅ Relaciones entre entidades
- ✅ Soporte para NIF y CIF españoles

## 🛠️ Comandos Útiles

### Entity Framework

```bash
# Crear nueva migración
dotnet ef migrations add "NombreDeLaMigracion"

# Aplicar migraciones
dotnet ef database update

# Revertir a migración específica
dotnet ef database update NombreDeLaMigracion

# Ver migraciones pendientes
dotnet ef migrations list
```

### Desarrollo

```bash
# Ejecutar en modo desarrollo
dotnet run

# Ejecutar con hot reload
dotnet watch run

# Compilar proyecto
dotnet build

# Ejecutar tests
dotnet test
```

## 🔧 Configuración

### Variables de Entorno

Para diferentes entornos, puedes configurar:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "tu-cadena-de-conexion"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Configuración de Producción

Para despliegue en producción, asegúrate de:

1. Configurar una cadena de conexión segura
2. Deshabilitar `EnableSensitiveDataLogging`
3. Configurar HTTPS correctamente
4. Establecer variables de entorno apropiadas

## 📚 Funcionalidades

### Gestión de Clientes
- ➕ Crear nuevos clientes
- 📝 Editar información existente
- 🗑️ Eliminar clientes
- 📋 Listar y buscar clientes
- ✅ Validación de NIF/CIF

### Gestión de Productos
- 🏷️ Organización por categorías
- 💰 Control de precios
- 📊 Gestión de stock
- 🔍 Búsqueda y filtrado

### Facturación
- 📄 Creación de facturas
- 🧮 Cálculo automático de impuestos
- 📋 Gestión de líneas de detalle
- 💾 Historial de facturas
- 📊 Reportes y estadísticas

## 🤝 Contribuir

1. Fork el proyecto
2. Crea tu rama de feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📋 TODO / Roadmap

- [ ] Autenticación y autorización
- [ ] Exportar facturas a PDF
- [ ] API REST
- [ ] Dashboard con gráficos
- [ ] Integración con sistemas de pago
- [ ] Notificaciones por email
- [ ] Backup automático

## 🐛 Problemas Conocidos

Si encuentras algún problema, por favor:
1. Revisa los [issues existentes](../../issues)
2. Crea un nuevo issue con detalles específicos

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

## 👤 Autor

**Álvaro** 
- Proyecto de estudio para Azure Certification
- 📧 Email: alvarofalagan29@gmail.com
- 💼 LinkedIn: https://www.linkedin.com/in/alvaro-martin-falagan/

⭐ **¡Si este proyecto te ha sido útil, no olvides darle una estrella!** ⭐
