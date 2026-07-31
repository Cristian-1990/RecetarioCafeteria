# Recetario Digital Cafetería

Aplicación web local para consultar recetas de una cafetería (café, infusiones y otras elaboraciones) desde tablets en la barra, sustituyendo el cuaderno físico de recetas.

## Contexto

El personal de la cafetería necesita consultar recetas de forma rápida durante el servicio, sin depender de un cuaderno en papel que se puede perder, mojar o quedar desactualizado. La aplicación está pensada para ejecutarse en tablets colocadas en la barra: el personal solo **consulta** recetas (listado, filtro por categoría, detalle paso a paso). La gestión de recetas (alta y edición) existe pero no está enlazada en la interfaz de las tablets — ver la nota de seguridad más abajo.

## Stack técnico

- **.NET 10**
- **Blazor Server** (interactividad global vía `InteractiveServer`)
- **Entity Framework Core + SQLite**
- **MudBlazor** (componentes UI, tema café)
- **CSharpFunctionalExtensions** (`Result<T, TError>` para manejo de errores sin excepciones)
- **NUnit + Moq + FluentAssertions** (tests)

## Arquitectura

### Estructura de carpetas

```
RecetarioCafeteria.Back/       Dominio, persistencia y lógica de negocio
├── Models/                    Modelos de dominio (Receta, Ingrediente, Paso, enums)
├── Entity/                    Entidades EF Core + AppDbContext
├── Mappers/                   Conversión Modelo ↔ Entidad
├── Repositories/               Acceso a datos (interfaz + implementación EF Core)
├── Services/                  Lógica de negocio y orquestación
├── Validators/                Validación de reglas de negocio
├── Errors/                    Errores de dominio tipados (DomainError)
└── Seed/                      Siembra de datos de ejemplo

RecetarioCafeteria.Blazor/     Frontend (UI)
├── Components/Pages/          Páginas (listado, detalle, formulario)
├── Components/Layout/         Layout, menú de navegación
└── Infrastructure/            Configuración de inyección de dependencias

RecetarioCafeteria.Test/       Pruebas unitarias (NUnit)
├── Repositories/
├── Services/
└── Validators/
```

### Patrón de capas

El flujo de una operación sigue capas bien separadas:

```
Página Blazor → Servicio → Repositorio → EF Core → SQLite
                    ↓
                Validador
```

- **Repositorio** (`IRecetaRepository` / `RecetaEfRepository`): abstrae el acceso a datos vía EF Core.
- **Validador** (`RecetaValidador`): valida las reglas de negocio antes de persistir (título obligatorio, al menos un ingrediente, pasos con fase válida, etc.).
- **Servicio** (`RecetaService`): orquesta validación + repositorio, y expone la API que consume la UI.

### Result pattern

En vez de lanzar excepciones para errores esperables (receta no encontrada, validación fallida), el dominio usa `Result<T, DomainError>` de **CSharpFunctionalExtensions**. `DomainError` es una jerarquía de registros (`record`) que modela errores de dominio tipados (por ejemplo `RecetaError`), de forma que el llamador puede inspeccionar el error sin `try/catch` y la UI puede mostrar mensajes específicos según el tipo de fallo.

## Modelo de dominio

- **Receta**: título, categoría (`Cafeteria`, `Infusiones`, `Otras`), tiempo estimado en minutos, foto y listas de ingredientes y pasos.
- **Ingrediente**: nombre, cantidad y unidad de medida (`Gramos`, `Mililitros`).
- **Paso**: descripción, orden (relativo a su fase, no global) y fase a la que pertenece.

### Las dos fases: Preparación / Elaboración

El concepto central del dominio es que los pasos de una receta se dividen en dos fases:

- **Preparación**: tareas previas al servicio (moler café, cortar ingredientes, precalentar agua...).
- **Elaboración**: pasos que se ejecutan en el momento de servir al cliente.

Esta separación existe porque, durante el servicio, el personal normalmente ya tiene la preparación hecha y necesita ir **directo a la Elaboración** sin tener que pasar por los pasos de preparación cada vez. La UI de detalle refleja esto con un stepper por fases y un salto directo a Elaboración.

## Funcionalidades

- **Listado de recetas** con filtro por categoría.
- **Detalle de receta** con stepper de pasos organizado por fase (Preparación / Elaboración) y salto directo a Elaboración.
- **Alta y edición de recetas** (`/recetas/nueva`, `/recetas/{id}/editar`) con listas dinámicas de ingredientes y pasos.
- **Siembra automática** de recetas de ejemplo en el primer arranque si la base de datos está vacía (`RecetaSeeder`).

## Cómo ejecutar en local

### Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)

### Configuración

La cadena de conexión a la base de datos vive en `RecetarioCafeteria.Blazor/appsettings.json`, bajo `ConnectionStrings:RecetarioDb`, en vez de estar hardcodeada en el código. Esto permite cambiar la ruta de la base de datos en el despliegue de producción (por ejemplo, en la Raspberry Pi) sin necesidad de recompilar la aplicación.

### Arrancar la aplicación

```bash
dotnet run --project RecetarioCafeteria.Blazor
```

La app se sirve por defecto en `http://localhost:5063` (perfil `http` de `launchSettings.json`). La base de datos SQLite se crea automáticamente en el primer arranque (`%LOCALAPPDATA%\recetario.db`) y se siembra con recetas de ejemplo si está vacía.

## Cómo ejecutar los tests

```bash
dotnet test
```

Cobertura actual: **22 tests** distribuidos entre repositorio, validador y servicio de recetas.

## Nota de seguridad

Las rutas de gestión de recetas (`/recetas/nueva` y `/recetas/{id}/editar`) **no están enlazadas intencionalmente** en la navegación de la aplicación. Esto es una decisión de diseño, no una funcionalidad pendiente: las tablets de la cafetería solo deben permitir consulta (listado y detalle) al personal. La gestión de recetas es accesible únicamente mediante acceso directo por URL, para quien tenga permiso de editar el recetario.

## Roadmap futuro / v2

- Despliegue en Raspberry Pi para uso en producción en la cafetería.
- Imágenes por paso de la receta (no solo una foto general).
- Mejoras visuales de interfaz.
- Posible división de `TiempoMinutos` en tiempo de Preparación y tiempo de Elaboración por separado.
