# Gestor de Eventos - API Backend

API robusta y escalable que sirve de motor para plataforma de Gestión de Eventos. Desarrollada con **.NET 8**, implementa las mejores prácticas para el manejo de identidades, persistencia de datos y seguridad.

## Tecnologías y Herramientas
* **Framework:** .NET 8 (ASP.NET Core Web API)
* **ORM:** Entity Framework Core
* **Base de Datos:** SQL Server
* **Mapeo de Objetos:** AutoMapper (para la transformación de Entidades a DTOs)
* **Seguridad:** Autenticación y Autorización basada en JWT (JSON Web Tokens)
* **Documentación:** Swagger UI

## Funcionalidades Principales
- **Sistema de Autenticación:** Registro e inicio de sesión de usuarios con contraseñas encriptadas.
- **Gestión de Perfil:** Endpoint seguro para que los usuarios actualicen sus datos personales.
- **CRUD de Eventos:** Los usuarios pueden crear, editar, listar y eliminar sus propios eventos.
- **Inscripciones:** Lógica de negocio para que los usuarios se inscriban a eventos y consulten sus "tickets" adquiridos.
- **Validación de Propiedad:** Seguridad a nivel de controlador para asegurar que solo el creador de un evento pueda modificarlo o eliminarlo.