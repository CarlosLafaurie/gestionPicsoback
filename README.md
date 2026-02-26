# Enterprise CRM System – Backend API (.NET 8)

Backend REST API developed with ASP.NET Core for an enterprise workforce and operations management system.

The API handles authentication, user management, workday calculations, holiday validation, and role-based access control, following secure and scalable architectural practices.

---

## 🚀 Core Features

- JWT Authentication
- Password hashing using BCrypt
- Role-Based Access Control (RBAC)
- Workday calculation engine (diurnal, nocturnal, overtime)
- Integration with external holiday API (Colombia)
- In-memory caching for holiday data
- Inventory and operational management
- Structured DTO projections
- Soft delete for users
- Azure-ready configuration

---

## 🔐 Authentication & Security

### Password Security

Passwords are securely hashed using BCrypt before being stored in the database.

```csharp
BCrypt.Net.BCrypt.HashPassword(password);
BCrypt.Net.BCrypt.Verify(password, hash);
```

No plain-text passwords are stored.

---

### JWT Authentication

Upon successful login:

- Claims are generated (Id, Role, Name, ObraId)
- Token signed using HMAC SHA256
- Token expiration configured (1 hour)

Example claims:

- sub (Cedula)
- name
- role
- id
- obraId

Protected endpoints use role-based authorization.

---

## 🧠 Business Logic: Workday Calculation Engine

The `CalculadoraJornada` service processes employee entries and exits to calculate:

- Diurnal hours
- Nocturnal hours
- Overtime (diurnal & nocturnal)
- Sunday work
- Holiday work

### Key Characteristics

- Processes time in 10-minute intervals
- Automatically excludes configured break periods
- Differentiates diurnal (6:00–21:00) and nocturnal hours
- Calculates overtime after 8 accumulated hours
- Handles overnight shifts
- Integrates Colombian holidays via external API

This encapsulated service separates business rules from controllers, following clean architecture principles.

---

## 🚀 CI/CD – Automated Deployment

This project includes a GitHub Actions workflow for automatic deployment to Azure Web App.

### Workflow Details

- Triggered on push to `master` branch
- Builds project using .NET 9
- Publishes application in Release mode
- Authenticates using Azure Service Principal
- Deploys automatically to Azure Web App (Production slot)

### Technologies Used

- GitHub Actions
- Azure Web App
- OIDC Authentication
- Azure Service Principal
- dotnet publish

This ensures continuous integration and continuous delivery (CI/CD), enabling automatic production updates on each approved push.

---

## 🌐 External API Integration

The system integrates with Calendarific API to retrieve official Colombian holidays.

### Features

- HttpClient-based service
- Year-based in-memory caching
- Configuration via appsettings
- Optional forced refresh

### Example configuration

```json
"Calendarific": {
  "ApiKey": "YOUR_API_KEY",
  "BaseUrl": "https://calendarific.com/api/v2"
}
```

---

## 📎 Document Management Module

The system includes a document management module for employee permission records.

### Features

- Upload PDF documents
- Store files in database (byte array)
- View document inline (application/pdf)
- Download document with original filename
- Update document metadata
- Delete documents

### Endpoints

- POST `/api/DocumentoPermiso/SubirDocumento`
- GET `/api/DocumentoPermiso`
- GET `/api/DocumentoPermiso/{id}`
- GET `/api/DocumentoPermiso/ver/{id}`
- GET `/api/DocumentoPermiso/descargar/{id}`
- PUT `/api/DocumentoPermiso/{id}`
- DELETE `/api/DocumentoPermiso/{id}`

### Implementation Details

- Uses `IFormFile` for file upload
- Files converted to byte array before storage
- Metadata and file stored in SQL Server
- Validates date ranges before saving
- Returns optimized projections for listing

This module demonstrates handling of binary file storage and RESTful document management.


---

## 🏗 Architecture Overview

- Controllers → API endpoints
- Services → Business logic
- Data → EF Core DbContext
- DTO Projections → Optimized responses
- Dependency Injection
- Configuration via appsettings.json

The project follows separation of concerns and maintainable layered architecture.

---

## 🗄 Database

- SQL Server
- Entity Framework Core
- Soft delete pattern for users
- Relationship management (Usuario ↔ Obra)

---

## ⚙ Development Setup

### Restore dependencies

```bash
dotnet restore
```

### Run migrations

```bash
dotnet ef database update
```

### Run project

```bash
dotnet run
```

Swagger available at:

https://localhost:{port}/swagger

---

## ☁ Deployment

- Azure-ready
- Environment-based JWT configuration
- External API key secured via configuration
- Production-ready structure

---

## 📌 Author

Carlos Lafaurie  
Full Stack Developer – .NET & Angular
