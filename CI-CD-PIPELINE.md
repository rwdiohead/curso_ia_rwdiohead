# DevSecOpsDemo - Pipeline CI/CD

Este archivo documenta la configuración del pipeline de CI/CD con GitHub Actions.

## 📋 Workflow: .NET CI Pipeline

**Archivo**: `.github/workflows/ci.yml`

### Triggers
- ✅ **Push** a ramas `main` o `master`
- ✅ **Pull Request** a ramas `main` o `master`

### Runner
- **OS**: `ubuntu-latest`
- **Job**: `build-and-test`

### Pasos del Pipeline

1. **Checkout code** (`actions/checkout@v4`)
   - Descarga el código del repositorio

2. **Setup .NET 8** (`actions/setup-dotnet@v4`)
   - Instala .NET SDK 8.0.x

3. **Restore dependencies**
   ```bash
   dotnet restore
   ```
   - Restaura todos los paquetes NuGet

4. **Build (Release)**
   ```bash
   dotnet build --configuration Release --no-restore
   ```
   - Compila el proyecto en modo Release

5. **Run tests**
   ```bash
   dotnet test --configuration Release --no-build --verbosity normal
   ```
   - Ejecuta todas las pruebas (8 pruebas de integración)

6. **Publish test results** (Opcional)
   - Publica los resultados de las pruebas en el PR
   - Se ejecuta incluso si los tests fallan (`if: always()`)

## 🚀 Uso

El pipeline se ejecutará automáticamente cuando:
- Hagas push a `main` o `master`
- Crees o actualices un Pull Request hacia `main` o `master`

## ✅ Estado del Pipeline

Una vez configurado en GitHub, verás:
- ✅ Checks en cada commit
- ✅ Estado de build y tests en PRs
- ✅ Badge de estado (opcional)

## 🔧 Comandos Locales

Para replicar el pipeline localmente:

```bash
# Restaurar
dotnet restore

# Compilar (Release)
dotnet build --configuration Release --no-restore

# Ejecutar tests
dotnet test --configuration Release --no-build --verbosity normal
```
