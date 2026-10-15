# RaceDay API

**A RESTful Race Event Management API built with ASP.NET Core 8, Entity Framework Core, and SQL Server.**

RaceDay is a backend application that allows organisers to manage sporting events and participants to register for events and view their race results.

This project was developed as part of the **RaceDay Part 2 REST API assignment**, building on the database design created in Part 1.

## Table of Contents

- [About the Project](#about-the-project)
- [Features](#features)
- [Technologies Used](#technologies-used)
- [Getting Started](#getting-started)
- [Database Design](#database-design)
- [Authentication and Authorization](#authentication-and-authorization)
- [API Endpoints](#api-endpoints)
- [Testing](#testing)
- [Continuous Integration](#continuous-integration)
- [Project Structure](#project-structure)
- [Security and Validation](#security-and-validation)
- [Known Limitations](#known-limitations)
- [Future Improvements](#future-improvements)
- [Acknowledgements](#acknowledgements)

---

## About the Project

The RaceDay API provides a backend system for managing sporting events such as running, walking, and cycling races.

It supports two user roles:

**Organiser**

Organisers can create and manage events, configure race categories, view participant enrolments, and record race results.

**Participant**

Participants can browse events, enrol in available categories, cancel their enrolments, and view their own results.

The API uses JSON for requests and responses and can be tested through Swagger UI.

## Features

### User Authentication

- User registration.
- User login.
- Secure password hashing.
- JWT token generation.
- Role-based authorization.
- Access to the logged-in user's profile.

### Event Management

- View available events.
- View an event by ID.
- Create new events.
- Update existing events.
- Delete eligible events.
- Restrict event changes to the organiser who owns the event.

### Category Management

- Create, view, update, and delete categories.
- Link categories to events.
- View categories available for a specific event.
- Remove category links when allowed.

### Participant Enrolment

- Enrol in an event category.
- View personal enrolments.
- View enrolment details.
- Cancel an enrolment.
- Prevent duplicate enrolments.
- Check event registration deadlines and capacity.

### Race Results

- Allow organisers to record or update results.
- Allow participants to view their own results.
- Allow organisers to view results for their events.

## Technologies Used

| Technology | Purpose |
|---|---|
| C# | Backend programming language |
| ASP.NET Core Web API (.NET 8) | REST API framework |
| Entity Framework Core | Object-relational mapping |
| SQL Server LocalDB | Development database |
| SQL Server Management Studio | Database inspection and management |
| JWT Bearer Authentication | User authentication |
| ASP.NET Core Identity PasswordHasher | Password hashing |
| Swagger / OpenAPI | API documentation and manual testing |
| xUnit | Automated testing |
| Git | Version control |
| GitHub | Source code repository |
| GitHub Actions | Continuous integration |
| Visual Studio 2022 | Development environment |

## Getting Started

### Prerequisites

Install the following software:

- Visual Studio 2022 with the ASP.NET and web development workload.
- .NET 8 SDK.
- SQL Server LocalDB.
- SQL Server Management Studio (optional but useful).
- Git.

### Clone the Repository

```bash
git clone https://github.com/Atlii-Angie/RaceDay.Api.git
cd RaceDay.Api
```

### Restore Dependencies

```bash
dotnet restore RaceDay.Api.sln
```

### Configure the Database

The application uses SQL Server LocalDB during development.

The development connection string should point to a SQL Server instance such as:

```text
(localdb)\MSSQLLocalDB
```

The database name used during development is:

```text
RaceDayDB
```

Update the connection string in your local configuration if your SQL Server instance is different.

### Configure JWT Authentication

The API requires a JWT signing key.

For local development, configure the key using .NET User Secrets rather than committing a secret to GitHub.

From the API project directory, run:

```bash
dotnet user-secrets set "Jwt:Key" "REPLACE_WITH_A_LONG_RANDOM_SECRET_KEY"
```

The application also uses JWT issuer and audience settings. Ensure that `Jwt:Issuer` and `Jwt:Audience` are configured to match your local `appsettings.json` or other development configuration.

**Do not commit actual JWT secrets, passwords, or database credentials to the repository.**

### Apply Database Migrations

Run the following command from the API project directory:

```bash
dotnet ef database update
```

Alternatively, use the Package Manager Console in Visual Studio:

```powershell
Update-Database
```

The database is created and updated using Entity Framework Core Code First migrations.

**Note:** The development database used manually added EventType records for running, walking, and cycling. If these values have not been included in migrations or seed code, they must be added before creating events that reference them.

### Run the Application

Start the API using Visual Studio 2022 or run:

```bash
dotnet run
```

When running through the Visual Studio development profile, Swagger UI is available at:

```text
https://localhost:7199/swagger
```

The exact URL and port may differ depending on the selected launch profile.

## Database Design

The database was developed using **Entity Framework Core Code First**, based on the approved Part 1 entity relationship diagram.

### Main Entities

| Entity | Description |
|---|---|
| User | Stores registered users and their roles |
| EventType | Stores event types such as Run, Walk, and Cycle |
| Event | Stores sporting event information |
| Category | Stores race category information |
| EventCategory | Links available categories to events |
| Enrollment | Stores participant event enrolments |
| Result | Stores participant race results |

### Changes from the Part 1 ERD

The implementation includes several extensions to support the API requirements:

1. **EventCategory junction entity:** Introduced to allow events to have multiple categories.
2. **Event.CategoryId removed:** Replaced by the EventCategory relationship.
3. **Enrollment.CategoryId added:** Identifies the category selected by a participant.
4. **Event.EntryFee added:** Stores the entry fee for an event.
5. **User.Phone and User.Location added:** Support additional participant profile information.
6. **Result.FinishTime:** Represented using a nullable C# `TimeSpan`, mapped to the SQL Server `time` type.

These changes support category enrolment, event pricing, participant profiles, and race result recording.

## Authentication and Authorization

RaceDay uses **JWT bearer authentication**.

### Registration

Users register with their account details and a supported role.

Supported roles:

- `Organiser`
- `Participant`

Passwords are hashed before being stored in the database.

### Login

Users log in using their credentials.

When authentication succeeds, the API generates a JWT containing information such as:

- User ID.
- Email.
- User role.
- Token expiration.

### Using JWT in Swagger

1. Register or log in using the authentication endpoints.
2. Copy the JWT returned by the API.
3. Click **Authorize** in Swagger.
4. Enter the token using the bearer authorization option.
5. Test endpoints permitted for the user's role.

### Role-Based Access

| Action | Organiser | Participant |
|---|---|---|
| View events | Yes | Yes |
| Create events | Yes | No |
| Update owned events | Yes | No |
| Delete owned events | Yes | No |
| Manage categories | Yes | No |
| Enrol in events | No | Yes |
| View own enrolments | No | Yes |
| Record results for owned events | Yes | No |
| View own results | No | Yes |

**Assignment note:** The implementation uses JWT bearer authentication rather than server-side session authentication. This difference from the assignment terminology is documented here.

## API Endpoints

The following endpoints are implemented in the API.

### Authentication

| Method | Endpoint | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a user |
| POST | `/api/auth/login` | Authenticate and obtain a JWT |

### Profiles

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Profiles/me` | View the authenticated user's profile |

### Events

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/Events` | Get all events |
| GET | `/api/Events/{id}` | Get an event by ID |
| POST | `/api/Events` | Create an event |
| PUT | `/api/Events/{id}` | Update an owned event |
| DELETE | `/api/Events/{id}` | Delete an owned
