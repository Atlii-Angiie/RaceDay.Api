# RaceDay REST API

## About the Project

RaceDay is an ASP.NET Core REST API developed to manage sporting events, participant registrations, race categories, and results.

The system supports two user roles:

- **Organiser:** Creates and manages events, categories, and race results.
- **Participant:** Views events, enrols in available categories, and views personal results.

This project was developed as part of the RaceDay Part 2 assignment.

## Built With

- C# and ASP.NET Core Web API
- .NET 8
- Entity Framework Core
- SQL Server LocalDB
- SQL Server Management Studio (SSMS)
- Swagger / OpenAPI
- JWT authentication
- xUnit
- GitHub Actions
- Visual Studio 2022

## Getting Started

### Prerequisites

- Visual Studio 2022 with ASP.NET and web development support
- .NET 8 SDK
- SQL Server LocalDB
- SQL Server Management Studio (optional)

### Installation

1. Clone the repository.
2. Open `RaceDay.Api.sln` in Visual Studio 2022.
3. Restore the NuGet packages.
4. Configure the database connection string for SQL Server LocalDB.
5. Configure a JWT signing key using .NET User Secrets.
6. Apply Entity Framework Core migrations using `Update-Database`.
7. Run the API and open Swagger.

**Note:** The database must contain the required event types before creating events. Check the database setup instructions for the values used during testing.

## Authentication and Authorization

The API uses JWT bearer tokens for authentication.

Users can register and log in using:

- `POST /api/auth/register`
- `POST /api/auth/login`

After logging in, users can enter their JWT token in Swagger's **Authorize** dialog.

Access to protected endpoints depends on the user's role. Passwords are stored as hashes rather than plain text.

**Design note:** JWT bearer authentication is token-based, rather than server-side session authentication.

## API Endpoints

| Feature | Endpoint | Access |
|---|---|---|
| Register | `POST /api/auth/register` | Public |
| Login | `POST /api/auth/login` | Public |
| View profile | `GET /api/profiles/me` | Authenticated user |
| View events | `GET /api/events` | Authenticated user |
| Create event | `POST /api/events` | Organiser |
| Update event | `PUT /api/events/{id}` | Organiser and event owner |
| Delete event | `DELETE /api/events/{id}` | Organiser and event owner |
| View categories | `GET /api/categories` | Authenticated user |
| Create category | `POST /api/categories` | Organiser |
| Enrol in event | `POST /api/enrollments` | Participant |
| View own enrolments | `GET /api/enrollments/me` | Participant |
| Record result | `POST /api/results` | Organiser and event owner |
| View own results | `GET /api/results/me` | Participant |

Additional category, enrolment, and result endpoints are documented in Swagger.

## Database Design

The API uses Entity Framework Core Code First with SQL Server.

Main entities:

- Users
- EventType
- Event
- Category
- EventCategory
- Enrollment
- Results

### Changes From Part 1

The Part 2 design extends the original database structure:

- `EventCategory` supports multiple categories per event.
- `Enrollment.CategoryId` records the category selected by a participant.
- `Event.EntryFee` stores the event entry fee.
- Additional user profile fields include phone number and location.

These changes support the functionality implemented in Part 2.

## Testing

Testing was performed using Swagger and xUnit.

### Automated Tests

Three xUnit tests verify password handling:

1. Password hashing does not return the original password.
2. The correct password passes verification.
3. An incorrect password fails verification.

### Manual API Testing

Swagger was used to test registration, login, role-based authorization, event management, enrolments, and results.

Tests included successful requests and expected error responses such as `401 Unauthorized`, `403 Forbidden`, and `404 Not Found`.

## CI/CD

GitHub Actions automatically restores dependencies, builds the solution, and runs automated tests when changes are pushed.

The workflow is defined in:

`.github/workflows/dotnet.yml`

Successful workflow runs and test results are documented in the assignment evidence screenshots.

## Known Limitations

- Authentication uses JWT tokens rather than server-side sessions.
- Automated unit tests currently focus on password handling; other API workflows were tested manually.
- The API is configured for local development using SQL Server LocalDB.

## Template Credit

README structure adapted from [Best-README-Template](https://github.com/othneildrew/Best-README-Template).

## Author

Individual academic project.
