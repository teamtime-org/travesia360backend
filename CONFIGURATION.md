# TeamTime.NET - Configuración de Desarrollo

## 🔐 Configuración Segura de Cadenas de Conexión

Este proyecto utiliza múltiples métodos para manejar configuraciones sensibles de forma segura.

## 📋 Métodos de Configuración

### 1. User Secrets (Recomendado para desarrollo local)

Los User Secrets son la forma más segura de manejar configuraciones sensibles en desarrollo:

```bash
# Inicializar User Secrets (ya configurado)
dotnet user-secrets init

# Agregar cadena de conexión
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=your-host;Database=your-db;Username=your-user;Password=your-password;Port=5432"

# Ver secretos configurados
dotnet user-secrets list

# Remover un secreto
dotnet user-secrets remove "ConnectionStrings:DefaultConnection"
```

### 2. Archivo appsettings.Local.json

Para desarrollo rápido, puedes usar el archivo `appsettings.Local.json`:

1. Copia `appsettings.example.json` como `appsettings.Local.json`
2. Actualiza las configuraciones con tus valores reales
3. El archivo está en `.gitignore` y no se subirá al repositorio

```bash
cp appsettings.example.json appsettings.Local.json
# Edita appsettings.Local.json con tus configuraciones
```

### 3. Variables de Entorno

Para producción, usa variables de entorno:

```bash
# Linux/macOS
export ConnectionStrings__DefaultConnection="Host=prod-host;Database=prod-db;Username=prod-user;Password=prod-password;Port=5432"

# Windows
set ConnectionStrings__DefaultConnection="Host=prod-host;Database=prod-db;Username=prod-user;Password=prod-password;Port=5432"
```

### 4. Azure Key Vault (Para producción)

Para entornos de producción en Azure, configura Key Vault:

```csharp
// En Program.cs para producción
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri("https://your-keyvault.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

## 🚨 Archivos a NO Subir al Repositorio

Los siguientes archivos están en `.gitignore` y contienen información sensible:

- `appsettings.Local.json`
- `appsettings.Production.json`
- `secrets.json`
- Cualquier archivo con `.local.` en el nombre

## 🔄 Orden de Prioridad de Configuración

.NET carga la configuración en este orden (último gana):

1. `appsettings.json`
2. `appsettings.{Environment}.json`
3. `appsettings.Local.json` (agregado por nosotros)
4. User Secrets (solo en Development)
5. Variables de entorno
6. Argumentos de línea de comandos

## 🛠️ Configuración para Nuevos Desarrolladores

1. Clona el repositorio
2. Navega al proyecto API: `cd src/TeamTime.API`
3. Configura User Secrets:
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "tu-cadena-de-conexion"
   ```
   O copia y configura el archivo local:
   ```bash
   cp appsettings.example.json appsettings.Local.json
   # Edita appsettings.Local.json
   ```
4. Ejecuta el proyecto: `dotnet run`

## 🔍 Verificar Configuración

Para verificar qué configuración está cargando la aplicación:

```bash
# Ver todas las configuraciones (sin valores sensibles)
dotnet run --urls="http://localhost:5000" --environment=Development
```

## 🚀 Despliegue

### Desarrollo
- Usa User Secrets o `appsettings.Local.json`

### Staging/Testing
- Usa variables de entorno o `appsettings.Staging.json` (sin subir al repo)

### Producción
- Usa Azure Key Vault, variables de entorno del sistema, o secretos de Kubernetes

## 📚 Recursos Adicionales

- [User Secrets en .NET](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets)
- [Configuration en ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration)
- [Azure Key Vault Configuration Provider](https://docs.microsoft.com/en-us/aspnet/core/security/key-vault-configuration)