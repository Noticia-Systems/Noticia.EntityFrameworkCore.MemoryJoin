![Nuget](https://img.shields.io/nuget/v/Noticia.EntityFrameworkCore.MemoryJoin) [![.NET CI MSSQL](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/dotnet-ci-mssql.yml/badge.svg)](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/dotnet-ci-mssql.yml) [![.NET CI Postgres](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/dotnet-ci-postgres.yml/badge.svg)](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/dotnet-ci-postgres.yml) [![.NET CI SQLite](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/dotnet-ci-sqlite.yml/badge.svg)](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/dotnet-ci-sqlite.yml) [![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](https://opensource.org/licenses/MIT) [![CodeQL](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/Noticia-Systems/Noticia.EntityFrameworkCore.MemoryJoin/actions/workflows/codeql-analysis.yml)

This package allows passing memory data to the EFcore provider which are able to be LINQed by the server without client evaluation.

## Installation

```
dotnet add package Noticia.EntityFrameworkCore.MemoryJoin
```

## Usage

You need to register models that you want to pass to the server in your database context:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    modelBuilder.MemoryEntity<TestModel>();
}
```

Note that `TestModel` must not have a property named `Id`. This package auto-generates the underlying entity type via reflection and requires this property internally.
The memory entities **do not require** a real table. They only need to be registered in the `DbContext`. For migrations you can safely use methods to disable this command (i.e. preprocessor directives checking whether migrations are running).

For generating the server-known entities you call:

```csharp
dbContext.AsMemoryEntities(models);
```

The given reference can be passed to the LINQ join method:
```csharp
.Join(memoryEntities, ...)
```

This package was developed for using with OData and AutoMapper. Since all evaluation occurs on the server-side AutoMappers `.ProjectTo` is fully supported, even when passing your client side data.

Huge thanks to [EntityFramework.MemoryJoin](https://github.com/neisbut/EntityFramework.MemoryJoin). This package has largely been based on their work.
