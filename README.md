# AttendEase


## What is AttendEase?

This project focuses on the development of AttendEase, a smart and efficient attendance management system designed for small organizations and educational institutions. The system aims to streamline attendance tracking, improve data accuracy and provide secure access control while maintaining a simple and user-friendly interface. Unlike traditional attendance systems that rely on manual tracking, AttendEase automates the process, reducing errors and saving time for both administrators and users.

The system was developed to function on both web and mobile platforms, ensuring accessibility for users across different devices. To achieve this, the web-based system was integrated into a mobile application using WebView, allowing mobile users to access the same platform conveniently. A key focus was on authentication and security, ensuring that only authorized from registered institutions or companies could access the system. Additionally, integration capabilities were included to support business users, enabling connections with third-party systems such as payroll and HR platforms to automate processes and enhance efficiency for future enhancements.

To ensure reliability, testing and debugging were prioritized throughout the development process. Various test cases were conducted to evaluate system performance, user experience and data accuracy. These efforts ensured that AttendEase remains stable, efficient and easy to use for organizations looking for a dependable tracking solution. While the initial development focused on core functionality, future enhancements have been identified for further improvement. These features will provide deeper insights into attendance trends and allow for automated decision making based on user patterns.

The project was carefully managed using a Gantt chart. All development phases including planning, design, implementation and testing were completed within the expected timeline. This structured approach helped in organizing tasks effectively and maintaining project goals. As this project addresses the growing demand for cost-effective and scalable attendance management systems for organizations with moderate budgets, it  will help organizations make better data driven decisions to improve their operations.


## Prerequisites (Development Environment: Windows):

* [Visual Studio 2022](https://visualstudio.microsoft.com/)
  * Select the following workloads during installation"
    * ASP.NET and web development
    * .NET Multi-platform App UI development
    * .NET desktop development

* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

* [Windows Subsystem for Linux (WSL)](https://learn.microsoft.com/en-us/windows/wsl/install)


## Project Structure

* AttendEase
  * .NET Multi-platform App UI (MAUI) mobile development project.
  * Handles services registration in `MauiProgram.cs` file.
  * Currently tested for Android (API 35) & Windows (WinUI3) platforms.
  * For iOS and MacOS, Apple hardware is required for development.

* AttendEase.DB
  * Entity Framework (EF) Core database project.
  * ORM to interact with a PostgreSQL database.

* AttendEase.Shared
  * Razor Class Library (RCL) project.
  * Contains shared components, pages, services, and utilities.

* AttendEase.Web
  * ASP.NET Core minimal API & Blazor Interactive Server web projects.
  * Handles services registration in `Program.cs` file.
  * Defines API endpoints structure in `Endpoints` folder.
  * Defines the index page in `Components\App.razor` file.


## Getting Started

* Clone the repository:
  ```bash
  git clone https://github.com/indraPSB/AttendEase.git
  ```

* Running the database in Docker:
  ```bash
  docker pull postgres:17.2
  docker run --name postgres-server -p 5432:5432 -e POSTGRES_DB=postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres -d postgres:17.2
  ```
  > Change the `POSTGRES_DB`, `POSTGRES_USER`, and `POSTGRES_PASSWORD` environment variables as required.

* Modify the following files to match your environment:
   * `AttendEase.Web/appsettings.json` or ideally copy settings to `AttendEase.Web/appsettings.Development.json`
   * `AttendEase.Web/Properties/launchSettings.json`
   * `AttendEase/appsettings.json`

* If database scaffolding is required:
  * Comment out the codes from the following files:
    * `AttendEase.DB/AttendEase.DB.csproj`
    * `AttendEase.DB/Program.cs`
  * Install the EF Core CLI tool:
    ```bash
    dotnet tool install --global dotnet-ef --version 9.0.2
    ```
  * Run the scaffolding command from the `AttendEase.DB` project directory:
    ```bash
    dotnet ef dbcontext scaffold "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=postgres" Npgsql.EntityFrameworkCore.PostgreSQL --verbose --force --framework net9.0 --context-dir Contexts --context AttendEaseDbContext --output-dir Models
    ```
    > Change the connection string as required.
    * It is advisable to change the access modifier of `AttendEaseDbContext` to `internal`. In the `AttendEase.DB/Properties/AssemblyInfo.cs` file, internal assembly visibility is extended to the `AttendEase.Web` project.
    * Change all `DateTime` properties to `DateTimeOffset` in the generated models.
    * Remove the `OnConfiguring` overriding method from the `AttendEaseDbContext` class. DB context configuration and injection is handled in the `AttendEase.Web/Program.cs` file.

* The web project can be run using the following command:
  ```bash
  dotnet run --project AttendEase.Web
  ```
  > The web project is configured to run on `https://localhost:7200`. Change as required from `AttendEase.Web/Properties/launchSettings.json` (debug) or `AttendEase.Web/appsettings.json` (release).
  * The web project must be running before launching the mobile project.
