# ASP.NET Web API with MySQL Database

## Dependencies

### NuGet Packages:
- Entity Framework Core
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools
- Pomelo.EntityFrameworkCore.MySql

---

## Terminal Commands

### Project Setup:
```sh
dotnet new webapi  # Create a new ASP.NET Web API project
```

### Running the Application:
```sh
dotnet watch run  # Run the application with hot reload
```

### Entity Framework Migrations:
```sh
dotnet ef migrations add init  # Initialize the migration (set up models & DBContext first)
```
- Configure `appsettings.json` connection string:
```json
"DefaultConnection": "Server=localhost;Port=3306;Database=DatabaseTest;User Id=root;Password=Pogipogi96;"
```

```sh
dotnet ef migrations remove  # Undo or remove migrations
dotnet ef database update    # Update the database with the latest migrations
```

---

## Connecting ASP.NET to MySQL and Setting Up Basic API

### Steps:
1. **Install Dependencies**
2. **Set Up Your Models**
3. **Configure ApplicationDBContext**:
   - Add your model to `ApplicationDbContext.cs`
   - Edit the connection string in `appsettings.json`
4. **Initialize DbContext in `Program.cs`**:
   - Register `DbContext` in builder services
5. **Run Migrations and Database Update**:
   - Check if the database initializes successfully
6. **Set Up Controllers and Routes**
7. **Configure Middleware in `Program.cs`**:
   ```csharp
   builder.Services.AddControllers();
   app.MapControllers();
   ```
8. **Run and Test the API**

---

## Handling API Requests

### POST Request (Create Data)
1. **Create DTO Class**:
   - Define properties for input validation (e.g., `Required`, `MaxLength` attributes)
2. **Create Mapper Static Class**:
   ```csharp
   public static Todo TodoRequestToTodo(this CreateTodoRequestDto TodoDto)
   {
       return new Todo
       {
           Title = TodoDto.Title,
           Description = TodoDto.Description,
           IsComplete = TodoDto.IsComplete,
           CreatedOn = TodoDto.CreatedOn,
           UpdatedOn = TodoDto.UpdatedOn
       };
   }
   ```
3. **Implement HTTP POST Controller Method**

---

### PUT/PATCH Request (Update Data)
1. **Create DTO Schema**:
   - Define properties for the values you want to update
2. **Create Mapper Method**:
   - Map the updated values accordingly
3. **Set Up Route Method**:
   - Use `[HttpPatch]` or `[HttpPut]` with `FromRoute` (ID) and `FromBody` (DTO)
4. **Find Data and Update**:
   - Validate if the record exists
   - Update using `DbContext`
   - Save changes

---

## Notes
- Always ensure the database connection is correctly configured.
- Run `dotnet ef database update` after making model changes.
- Ensure your DTOs handle required validation and security properly.
- Use dependency injection for `DbContext` to manage database interactions efficiently.

---

## Running & Testing
- Run `dotnet watch run` to start the API
- Use Postman or any API testing tool to test endpoints
- Verify database changes using MySQL client tools

