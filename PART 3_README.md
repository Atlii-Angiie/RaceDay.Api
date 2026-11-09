# RaceDay — Part 3 POE

## Project Overview

RaceDay is a race and event management application developed using ASP.NET Core and .NET 8.

The application consists of an API backend and an MVC frontend, allowing organisers and participants to interact with race events.

## Technologies Used

- C#
- ASP.NET Core Web API
- ASP.NET Core MVC
- .NET 8
- Visual Studio 2022
- Git and GitHub
- GitHub Actions
- HTML and CSS

## Main Features

### Organiser

- Log in to the organiser dashboard
- Create and manage race events
- Create race categories
- Link categories to events
- View enrolment information

### Participant

- Log in to the participant dashboard
- Browse available race events
- Enrol in races
- View enrolment information
- Access results-related pages

## User Interface

RaceDay features an Ocean Blue and Cyan sports-themed interface, including:

- Organiser and Participant dashboards
- Running-themed images
- Responsive page layouts
- Dashboard statistic cards
- Styled tables and navigation

## Project Structure

The solution contains the following projects:

- **RaceDay.Api** — Backend API
- **RaceDay.Mvc** — MVC frontend
- **RaceDay.Api.Tests** — Test project

## Running the Application

1. Clone the repository.
2. Open `RaceDay.Api.sln` in Visual Studio 2022.
3. Restore NuGet packages.
4. Configure the required database connection and application settings for your environment.
5. Set both `RaceDay.Api` and `RaceDay.Mvc` as startup projects.
6. Build the solution.
7. Run the application and open the MVC website.

The API must be accessible to the MVC application for API-dependent features to work.

## Testing and Continuous Integration

The project includes a test project and a GitHub Actions workflow.

The latest GitHub Actions workflow was reported as successful during Part 3 development.

## GitHub Branch

Part 3 development is maintained on the `part3-mvc` branch.

## Academic Purpose

This project was developed as part of a Portfolio of Evidence (POE) assessment.

## Youtube link
https://youtu.be/tlzjk3nKwUE?si=0SXVtsKnNgx7_e_I
