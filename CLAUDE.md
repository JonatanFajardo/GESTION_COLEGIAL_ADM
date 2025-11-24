# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

GESTION_COLEGIAL_ADM is a comprehensive school management system built with ASP.NET MVC 5 (.NET Framework 4.7.2). The application manages academic operations (students, employees, courses, schedules), financial processes (payments, accounts receivable, discounts), and administrative catalogs (educational levels, sections, durations).

**Localization:** All UI text is in Spanish (Honduras) - `es-HN` culture setting.

## Solution Structure

```
GESTION_COLEGIAL_ADM.sln
├── GESTION_COLEGIAL.UI (Presentation Layer - ASP.NET MVC 5)
└── GESTION_COLEGIAL.Business (Business Logic Layer - Class Library)
```

The application uses a **three-layer architecture** where the UI and Business layers communicate with an external RESTful API (separate microservice) for data persistence.

**External API Repository:**
- Path: `C:\Users\movie\Documents\GitHub\Gestion.Colegial.Api`
- This is a separate ASP.NET Core Web API project that handles data persistence and business logic
- The API must be running at `https://localhost:7076/api/v1/` for the MVC application to function

## Build & Development

**Prerequisites:**
- Visual Studio 2019+ or MSBuild tools
- .NET Framework 4.7.2
- External API running at `https://localhost:7076/api/v1/`

**Build commands:**
```bash
# Restore NuGet packages
nuget restore GESTION_COLEGIAL_ADM.sln

# Build Debug
msbuild GESTION_COLEGIAL.UI\GESTION_COLEGIAL.UI.csproj /p:Configuration=Debug

# Build Release
msbuild GESTION_COLEGIAL.UI\GESTION_COLEGIAL.UI.csproj /p:Configuration=Release

# Clean solution
msbuild GESTION_COLEGIAL_ADM.sln /t:Clean
```

**Running the application:**
- Set `GESTION_COLEGIAL.UI` as startup project in Visual Studio
- IIS Express HTTPS port: 44329
- Default route: `Account/Login`
- Ensure external API is running before starting

**SCSS Compilation:**
- SCSS files in `GESTION_COLEGIAL.UI/Content/sass/` compile to `Content/css/`
- Configuration: `compilerconfig.json`
- Asset bundling: `App_Start/BundleConfig.cs`

## Architecture Patterns

### Service Layer Pattern

All services in `GESTION_COLEGIAL.Business/Services/` follow a thin wrapper pattern:

```
Controller → Service → ApiRequests → SendHttpClient → External API
```

**Standard service methods:**
- `ListAsync()` - GET all records
- `Find(int id)` - GET single record by ID
- `Detail(int id)` - GET detailed record information
- `Create(T model)` - POST new record
- `Edit(T model)` - PUT update record
- `Delete(int id)` - PUT soft delete (not HTTP DELETE)
- `Exist(string value)` - GET check duplicate

**Important:** The `DeleteAsync` method uses HTTP PUT (not DELETE) for soft deletes via `SendHttpClient.DeleteAsync()`.

### Controller Pattern

All controllers inherit from `BaseController` which provides `AjaxResult()` helper methods:

```csharp
// Standard response
AjaxResult(dynamic response)

// With success flag
AjaxResult(dynamic item, bool success)

// With custom alert messages
AjaxResult(dynamic response, AlertMessageType type, string mensaje)
AjaxResult(dynamic response, AlertMessageCustomType type)
```

**Standard controller actions:**
- `Index()` - Returns main view with DataTable
- `CreateAsync()` - Returns create/edit modal view
- `[HttpGet] ListAsync()` - AJAX endpoint returning JSON list
- `[HttpGet] FindAsync(int id)` - AJAX endpoint for single record
- `[HttpGet] DetailAsync(int id)` - AJAX endpoint for detail view
- `[HttpPost] CreateAsync(ViewModel model)` - AJAX create/update endpoint
- `[HttpPost] DeleteAsync(ViewModel model)` - AJAX delete endpoint
- `[HttpPost] ExistAsync(int? id, string field)` - AJAX duplicate check for Remote validation

### ViewModel Pattern

ViewModels use Data Annotations for validation and inherit from `BaseViewModel`:

```csharp
[Required(ErrorMessage = "El campo es requerido")]
[StringLength(100)]
[Display(Name = "Descripción")]
[Remote("ExistAsync", "ControllerName", HttpMethod = "POST")]  // Client-side duplicate check
public string PropertyName { get; set; }
```

**ViewModel types:**
- `[Resource]ViewModel` - Used for list views and create/edit operations
- `[Resource]DetailViewModel` - Used for detailed information display (new pattern being implemented)

**Naming convention:** Properties follow `[TablePrefix]_[PropertyName]` pattern (e.g., `Car_Id`, `Car_Descripcion` for Cargos table).

### API Communication

The `SendHttpClient` class handles all HTTP communication:

**Base URL:** `https://localhost:7076/api/v1/` (configurable in [SendHttpClient.cs:15](GESTION_COLEGIAL.Business/Helpers/SendHttpClient.cs#L15))

**Key implementation details:**
- SSL certificate validation is **disabled** in development ([SendHttpClient.cs:20-23](GESTION_COLEGIAL.Business/Helpers/SendHttpClient.cs#L20-L23))
- Returns `null` or empty collections on HTTP errors
- Returns inverted boolean logic (returns `true` on error, `false` on success) for POST/PUT/DELETE operations

**Available methods:**
- `GetAsync<T>(url)` - GET list
- `FindAsync<T>(url, id)` - GET single item
- `PostAsync(url, model)` - POST create
- `PutAsync(url, model)` - PUT update
- `DeleteAsync(url, id)` - PUT soft delete
- `ExistAsync<T>(url, value)` - GET duplicate check
- `DropdownAsync<T>(url, id)` - GET dropdown options
- `PostAsyncWithResponse<T>(url, model)` - POST with typed response

## Module Organization

**Catalog Modules** - Simple administrative catalogs following the standard Index view pattern with modal create/edit:
- **Cargos** (Jobs/Positions)
- **ConceptosPago** (Payment Concepts)
- **CursosNiveles** (Course Levels)
- **Descuentos** (Discounts)
- **Duraciones** (Durations)
- **Estados** (States)
- **EstadosPago** (Payment States)
- **FormasPago** (Payment Methods)
- **Materias** (Subjects)
- **Modalidades** (Modalities)
- **NivelesEducativos** (Educational Levels)
- **Parciales** (Partial Exams)
- **Parentescos** (Family Relations)
- **Secciones** (Sections)
- **Semestres** (Semesters)
- **Titulos** (Titles/Degrees)

These catalog modules share:
- Standardized CRUD operations via modal dialogs
- DataTable-based list views
- Similar controller/service/view structure
- Validation with duplicate checking

**Other Modules:**
- **Alumnos** (Students) - Complex module with detail views
- **Empleados** (Employees) - Complex module with detail views
- **Encargados** (Guardians) - Complex module with detail views
- **Pagos** (Payments) - Financial transaction management
- **CuentasCobrar** (Accounts Receivable) - Financial tracking
- **Reportes** (Reports) - Reporting functionality

## View Structure

**Layout hierarchy:**
- `Views/Shared/_Layout.cshtml` - Master layout
- `Views/Shared/_header.cshtml` - Top navigation/branding
- `Views/Shared/_sidebar.cshtml` - Side navigation menu
- `Views/Shared/_footer.cshtml` - Footer content

**Page patterns:**

1. **Index views** - DataTable-based CRUD lists (used by all catalog modules):
   - Modal dialogs for create/edit (single form for both operations)
   - AJAX-driven data loading via DataTables
   - Inline delete actions with confirmation

2. **Detail views** - Comprehensive detail displays (new pattern for complex modules):
   - Separate detail pages (e.g., `Alumnos/Detail.cshtml`, `Empleados/Detail.cshtml`)
   - JavaScript files: `[module].detail.js`
   - CSS: `Content/css/modal-detail.css`

**JavaScript components:**
- `datatable.init.js` - Core DataTable initialization utilities
- `datatable.catalogs.init.js` - Catalog CRUD DataTable patterns
- `datatable.partials.init.js` - Section-specific DataTable patterns (new)
- Page-specific files: `[module].js`, `[module].detail.js`

## Development Conventions

**Naming conventions:**
- Controllers: `[Resource]Controller.cs` (e.g., `CargosController`)
- Services: `[Resource]Service.cs`
- ViewModels: `[Resource]ViewModel.cs`, `[Resource]DetailViewModel.cs`
- Views: `Views/[Controller]/[Action].cshtml`
- Partials: Prefix with underscore `_Partial.cshtml`

**Response handling:**
- Controllers return `AjaxResult()` for AJAX endpoints
- Services return `bool` (inverted - `true` = error, `false` = success) or typed objects
- Null responses are handled gracefully throughout the stack

**Validation:**
- Server-side: Data Annotations with Spanish error messages
- Client-side: jQuery.Validation + Unobtrusive validation
- Duplicate checking: `[Remote]` attribute pointing to `ExistAsync` action

**Error messages:** Use Spanish language for all user-facing messages (already established pattern in codebase).

## Key Technologies

**Backend:**
- ASP.NET MVC 5.2.7
- .NET Framework 4.7.2
- Newtonsoft.Json 12.0.2
- System.Net.Http for API communication

**Frontend:**
- jQuery 3.4.1
- jQuery.Validation 1.17.0
- DataTables plugin
- Bootstrap (via bundled assets)

**Build tools:**
- MSBuild
- NuGet for package management
- Web.config transformations (Debug/Release)

## Current Development

**Active branch:** `details` - Implementing detailed view functionality for various modules

**Recent patterns added:**
- Detail ViewModels for catalog entities
- Detail views with dedicated JavaScript files
- Partial DataTable initialization patterns
- `modal-detail.css` for detail view styling

**Git workflow:**
- Main branch: `master`
- Feature branches: Create from `master`, merge via pull request
- Recent focus: Financial module improvements and detail view implementation
