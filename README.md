<h1>Events API</h1>

<p>The Events API is a RESTful API designed to manage events, invitations, and participants. It allows users to create, retrieve, update, and delete events, as well as manage invitations and participants for each event.</p>

<h2>Features</h2>
<ul>
  <li>Create, retrieve, update, and delete events</li>
  <li>Manage invitations and participants for events</li>
  <li>Pagination support for retrieving events</li>
  <li>Built with .NET 7.0 and Entity Framework Core</li>
  <li>Unit tests for the API</li>
  <li>Containerized with Docker</li>
</ul>

<h2>Prerequisites</h2>
<ul>
  <li>.NET SDK 7.0</li>
  <li>dotnet ef 7.</li>
  <li>Docker (Optional)</li>
</ul>

<h2>Getting Started</h2>
<p>After cloning the repository please run the following commands:</p>

<pre>
cd Events.API
dotnet ef migrations add "Initial Create"
dotnet ef database update
dotnet build
dotnet watch run
</pre>

<p>If you want to run it using the container:</p>

<pre>
cd Events.API
dotnet ef migrations add "Initial Create"
dotnet ef database update

cd ..
docker compose build
docker compose up
</pre>

<p>On Initialization the database seeds itself with data and a copy of that data is made available filename UserDataSeed.json


<p>After your API is ready for test</p>

<p>The Swagger Documentation can be found</p>
<pre> http://localhost:5006/swagger/index.html</pre>
