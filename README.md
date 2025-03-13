Task Management API 
A .NET-based Task Management API that allows users to create, assign, and track tasks efficiently. The project includes Docker support, Redis caching, and a fully automated CI/CD pipeline using GitHub Actions.

 Tech Stack
Backend: .NET Framework
Database: SQL Server / Azure SQL / AWS RDS
Caching: Redis
Containerization: Docker, Docker Compose
CI/CD: GitHub Actions
Cloud Services: AWS / Azure
Event Handling: AWS SQS / Kafka

 Features
 Create, update, and delete tasks
 Assign tasks to users
 Task status tracking (Pending, In Progress, Completed)
 User authentication and authorization (JWT)
 API caching using Redis
 Dockerized deployment
 Automated CI/CD pipeline

 Getting Started
 Prerequisites
Ensure you have the following installed:

.NET Framework
SQL Server or Azure SQL
Docker & Docker Compose
Git
Redis
 Clone the Repository
sh
Copy
Edit
git clone https://github.com/vivekamarathi/TaskManagementAPI.git
cd TaskManagementAPI
 Setup Environment Variables
Create a .env file in the root folder and add the following:

env
Copy
Edit
DB_CONNECTION_STRING="your-database-connection-string"
REDIS_HOST="localhost"
JWT_SECRET="your-secret-key"
 Running the Application
 Run Locally
sh
Copy
Edit
dotnet restore
dotnet build
dotnet run
API will be available at: http://localhost:5000

 Run with Docker
sh
Copy
Edit
docker-compose up --build
The API, Redis, and database will start inside Docker containers.

 API Endpoints
Method	Endpoint	Description
POST	/api/auth/login	User Login
POST	/api/tasks	Create a new task
GET	/api/tasks	Get all tasks
GET	/api/tasks/{id}	Get task by ID
PUT	/api/tasks/{id}	Update a task
DELETE	/api/tasks/{id}	Delete a task
🛠 CI/CD Pipeline (GitHub Actions)
The GitHub Actions workflow automates:

Building the project
Running unit tests
Pushing Docker images to Docker Hub
Deploying to AWS/Azure
GitHub Actions Workflow File
Located in .github/workflows/deploy.yml

yml
Copy
Edit
name: CI/CD Pipeline

on:
  push:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-latest
    steps:
      - name: Checkout Code
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '6.0'

      - name: Build Project
        run: dotnet build --configuration Release

      - name: Run Tests
        run: dotnet test --no-build --verbosity normal

      - name: Login to Docker Hub
        run: echo "${{ secrets.DOCKER_PASSWORD }}" | docker login -u "${{ secrets.DOCKER_USERNAME }}" --password-stdin

      - name: Build & Push Docker Image
        run: |
          docker build -t vivekamarathi/task-api .
          docker push vivekamarathi/task-api

      - name: Deploy to Server
        run: ssh user@server 'docker pull vivekamarathi/task-api && docker-compose up -d'
 Deployment
 Deploy to AWS / Azure
Create an RDS/Azure SQL instance
Set up Redis on AWS ElastiCache/Azure Cache
Run Docker containers on AWS ECS / Azure App Service
 Deploy using Docker Compose
sh
Copy
Edit
docker-compose up -d
 Database Schema
sql
Copy
Edit
CREATE TABLE Tasks (
  Id INT PRIMARY KEY IDENTITY,
  Title NVARCHAR(255),
  Description NVARCHAR(MAX),
  Status NVARCHAR(50),
  AssignedTo NVARCHAR(100),
  CreatedAt DATETIME DEFAULT GETDATE()
);
 Authentication (JWT)
Secure API using JWT-based authentication
Add Authorization: Bearer <token> in headers
 Contributing
Fork the repository
Create a new branch (feature/new-feature)
Commit changes (git commit -m "Added new feature")
Push the branch (git push origin feature/new-feature)
Create a Pull Request
 License
This project is licensed under the MIT License.

