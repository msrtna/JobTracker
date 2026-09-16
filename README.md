\# JobTracker



JobTracker is a personal job application tracking RESTful API built with ASP.NET Core.



It allows users to track their job applications, manage companies and job categories, record interviews, and monitor their application progress through a dashboard.



The project is built using Clean Architecture principles and includes JWT-based authentication, Entity Framework Core, SQL Server, automated tests, and Docker Compose.



\---------------------------------------------------------------------------------------------------------------------------------



\## Features



\### Authentication \& Security



\- User registration and login

\- JWT-based authentication

\- Access token and refresh token

\- Secure refresh token storage using SHA-256 hashing

\- Change password

\- Protected API endpoints

\- User ownership and data isolation



\### Job Application Management



\- Create, read, update, and delete job applications

\- Track application status

\- Track workplace type

\- Store company and job category information

\- Store application date, salary, location, job URL, and description

\- Search and filter job applications

\- Sorting and pagination



\### Companies \& Job Categories



\- Company CRUD

\- Job category CRUD

\- Unique company and job category records

\- Reusable companies and categories across job applications



\### Interview Management



\- Create and manage interviews

\- Associate interviews with job applications

\- Track interview date and type

\- Store interview notes



\### Dashboard



\- Total applications

\- Applications by status

\- Applications by workplace type

\- User-specific statistics



\### Testing



\- Unit tests

\- Integration tests

\- Authentication and authorization tests

\- Ownership and user-isolation tests

\- Search, filtering, sorting, and pagination tests



\### Docker



\- Multi-stage Docker build

\- ASP.NET Core API container

\- SQL Server container

\- Docker Compose

\- Persistent SQL Server volume

\- SQL Server healthcheck

\- API startup dependency on database health



\---------------------------------------------------------------------------------------------------------------------------------



\## Technologies



\- \*\*.NET 9 / ASP.NET Core Web API\*\*

\- \*\*C#\*\*

\- \*\*Entity Framework Core\*\*

\- \*\*SQL Server\*\*

\- \*\*JWT Authentication\*\*

\- \*\*Refresh Tokens\*\*

\- \*\*FluentValidation\*\*

\- \*\*Swagger / OpenAPI\*\*

\- \*\*Moq\*\*

\- \*\*xUnit\*\*

\- \*\*Docker\*\*

\- \*\*Docker Compose\*\*

\- \*\*Git / GitHub\*\*



\---------------------------------------------------------------------------------------------------------------------------------



\## Authentication



JobTracker uses JWT-based authentication with short-lived access tokens and refresh tokens.



\### Authentication Flow



Register

&#x20;  │

&#x20;  ▼

Login

&#x20;  │

&#x20;  ├── Access Token

&#x20;  │

&#x20;  └── Refresh Token

&#x20;         │

&#x20;         ▼

&#x20;  Authenticated Requests

&#x20;         │

&#x20;         ▼

&#x20;  Access Token Expires

&#x20;         │

&#x20;         ▼

&#x20;  Refresh Token

&#x20;         │

&#x20;         ▼

&#x20;  New Access Token



\---------------------------------------------------------------------------------------------------------------------------------



\## Docker



The project can be run using Docker Compose with two services:



Docker Compose

│

├── JobTracker API

│   └── ASP.NET Core Web API

│

└── SQL Server

&#x20;   └── JobTrackerDb



\---------------------------------------------------------------------------------------------------------------------------------



\## Testing



The project includes both unit tests and integration tests to verify business logic and API behavior.



\### Test Coverage



\- Authentication and registration

\- Login and JWT authentication

\- Change password

\- Job application CRUD operations

\- User ownership and data isolation

\- Company and job category operations

\- Interview management

\- Search, filtering, sorting, and pagination

\- Dashboard statistics

\- Unauthorized access scenarios



\### Testing Technologies



\- \*\*xUnit\*\*

\- \*\*Moq\*\*

\- \*\*ASP.NET Core WebApplicationFactory\*\*

\- \*\*Entity Framework Core\*\*

\- \*\*SQL Server\*\*



Integration tests use a dedicated SQL Server database to verify the application behavior across the API, Application, Infrastructure, and database layers.



\---------------------------------------------------------------------------------------------------------------------------------



\## Getting Started



\### Prerequisites



Make sure the following tools are installed:



\- .NET 9 SDK

\- Docker Desktop

\- Git



\### Clone the Repository



git clone https://github.com/msrtna/JobTracker.git

cd JobTracker



\---------------------------------------------------------------------------------------------------------------------------------



\## Future Improvements



Possible future improvements include:



\- Email notifications for interview reminders

\- Advanced dashboard charts and analytics

\- Job application reminders

\- Resume and cover letter management

\- External job board integrations

\- Role-based administration for managing shared data

\- CI/CD pipeline

\- Automated deployment

\- Cloud deployment

