# Task Management App
A full-stack task management application with microservices, built to showcase modern development skills.

## Tech Stack
- **Backend**: ASP.NET Core microservices (Task Service, Payment Service) with Clean Architecture
- **Frontend**: Angular
- **Database**: SQL Server
- **DevOps**: Docker, Azure, GitHub Actions
- **Payments**: Stripe
- **Testing**: xUnit

## Setup
1. Clone the repo: `git clone https://github.com/your-username/TaskManagementApp.git`
2. Backend: `cd TaskService && dotnet restore`
3. Frontend: `cd Frontend && npm install && ng serve`
4. Database: Run `Database/TaskDb/SeedData.sql` in SQL Server

## Features
- User registration/login (JWT authentication)
- Task CRUD operations
- Premium subscriptions via Stripe
