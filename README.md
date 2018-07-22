# aspnet-core-web-api

Default UI is Swagger.
Contains Role Based Authentication with Jwt.

Before Navigate Web Api Folfer. Optinally create Migration
dotnet ef migrations add YourMigrationName --context LibraryDbContext -p ../Library.Infrastructure/Library.Infrastructure.csproj -s Library.WebApi.csproj -o Migrations

for db creation
dotnet ef database update -c LibraryDbContext -p ../Library.Infrastructure/Library.Infrastructure.csproj -s Library.WebApi.csproj
