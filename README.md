<h1>Events API<h1>

The Events API is a RESTful API designed to manage events, invitations, and participants. It allows users to create, retrieve, update, and delete events, as well as manage invitations and participants for each event.

Features
Create, retrieve, update, and delete events
Manage invitations and participants for events
Pagination support for retrieving events
Built with .NET 7.0 and Entity Framework Core
Unit tests for the API
Containerized with Docker

Prerequisites
.NET SDK 7.0
dotnet ef 7.
Docker (Optional)

Getting Started
After cloning the repository please run the following commands.

cd Events.API
dotnet ef migrations add "Initial Create"
dotnet ef database update.
dotnet build
dotnet watch run

if you want to run it using the container

cd Events.API
dotnet ef migrations add "Initial Create"
dotnet ef database update

cd ..
docker compose build
docker compose up

On Initialization the database seeds itself with data and a copy of that data is made available at
cd Events.API/Data/
FileName UserDataSeed.json

and after your api is ready for test
