# ASP.NET Web API with MySQL Database

## Table of Contents
- [Project Setup](#project-setup)
- [Running the Application](#running-the-application)
- [Entity Framework Migrations](#entity-framework-migrations)
- [Connecting ASP.NET to MySQL and Setting Up Basic API](#connecting-aspnet-to-mysql-and-setting-up-basic-api)
- [Handling API Requests](#handling-api-requests)
  - [POST Request (Create Data)](#post-request-create-data)
  - [PUT/PATCH Request (Update Data)](#putpatch-request-update-data)
- [Password Hashing Implementation](#password-hashing-implementation)
- [JWT with HttpOnly Cookie-Based Authentication](#jwt-with-httponly-cookie-based-authentication)
- [How to Setup CORS Configuration for Cross-Origin Requests](#how-to-setup-cors-configuration-for-cross-origin-requests)
- [Debugging Tips](#debugging-tips)
- [Conclusion](#conclusion)
- [Notes](#notes)
- [Running & Testing](#running--testing)


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

- Entity Framework Core
  - Microsoft.EntityFrameworkCore.SqlServer
  - Microsoft.EntityFrameworkCore.Design
  - Microsoft.EntityFrameworkCore.Tools
- Pomelo.EntityFrameworkCore.MySql

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

## Password Hashing Implementation

### Install Dependencies
```xml
<PackageReference Include="Microsoft.AspNetCore.Cryptography.KeyDerivation" Version="9.0.2" />
```

### Implementing Password Hashing When Creating a User
```csharp
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
    password: user.password,
    salt: System.Text.Encoding.UTF8.GetBytes("salt"),
    prf: KeyDerivationPrf.HMACSHA1,
    iterationCount: 10000,
    numBytesRequested: 256 / 8
));
```

### Example: Using Password Hashing in User Creation
```csharp
[HttpPost]
public IActionResult CreateUser([FromBody] CreateUserDto userModel) {
    if (!ModelState.IsValid)
        return BadRequest(ModelState);

    var user = userModel.CreateUserRequest();
    user.password = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: user.password,
        salt: System.Text.Encoding.UTF8.GetBytes("salt"),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8
    ));

    _context.Users.Add(user);
    _context.SaveChanges();

    return Ok(user);
}
```

### Decrypting the Hashed Password for Authentication
```csharp
string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
    password: userModel.password,
    salt: Encoding.UTF8.GetBytes("salt"),
    prf: KeyDerivationPrf.HMACSHA1,
    iterationCount: 10000,
    numBytesRequested: 256 / 8
));
```

### Example: Login Route Authentication
```csharp
[HttpPost("login")]
public async Task<IActionResult> Login([FromBody] LoginUserDto userModel)
{
    var user = await _context.Users.FirstOrDefaultAsync(u => u.username == userModel.username);
    if (user == null) 
        return Unauthorized(new { message = "User not found" });

    if (string.IsNullOrWhiteSpace(userModel.username) || string.IsNullOrWhiteSpace(userModel.password))
        return Unauthorized(new { message = "Invalid username or password" });

    string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
        password: userModel.password,
        salt: Encoding.UTF8.GetBytes("salt"),
        prf: KeyDerivationPrf.HMACSHA1,
        iterationCount: 10000,
        numBytesRequested: 256 / 8
    ));

    if (hashedPassword != user.password) 
        return Unauthorized(new { message = "Invalid username or password" });

    var token = _jwtService.GenerateJwtToken(user.username);
    var cookieOptions = new CookieOptions
    {
        HttpOnly = true,
        Secure = false,
        SameSite = SameSiteMode.Strict,
        Expires = DateTime.UtcNow.AddMinutes(30)
    };
    
    Response.Cookies.Append("jwt", token, cookieOptions);
    return Ok(new { token, message = "Logged in successfully" });
}
```

---

# JWT with HttpOnly Cookie-Based Authentication

## Introduction
This guide explains how to implement JWT-based authentication using HttpOnly cookies in an ASP.NET Core Web API application. This ensures secure authentication while preventing XSS attacks.



## Steps to Implement

### 1. Setup User Model and Routes
- Create a user model with hashed passwords.
- Define routes for user creation, authentication, and deletion.

### 2. Install JWT and Cookie Dependencies
Ensure the following dependencies are installed:
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.Cookies" Version="2.3.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```

### 3. Create JWT Token Generator Service
Create a service for generating JWT tokens:
```csharp
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Web_API.Services
{
    public class JWTService
    {
        private readonly string _secret;

        public JWTService(string secret)
        {
            _secret = secret;
        }

        public string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secret);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
```

### 4. Implement Middleware for Authentication
Modify the login controller to generate and store the JWT in an HttpOnly cookie (make sure the login login authentication is correct before generating token):
```csharp
var token = _jwtService.GenerateJwtToken(user.username);
var cookieOptions = new CookieOptions
{
    HttpOnly = true, // Prevents JavaScript access (XSS protection)
    Secure = false,  // ❌ Set to true in production
    SameSite = SameSiteMode.Strict, // Prevents CSRF attacks
    Expires = DateTime.UtcNow.AddMinutes(30)
};
Response.Cookies.Append("jwt", token, cookieOptions); // store the jwt in cookie https only storage
```

### 5. Register Authentication Services
Configure authentication services in `Program.cs`:
```csharp
var key = Encoding.UTF8.GetBytes("this is my custom Secret key for authentication");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});
```
Enable CORS:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // client http
                .AllowAnyHeader() 
                .AllowAnyMethod()
                .AllowCredentials(); // enable credentials for authentications
        });
});
```

```csharp
// Define the endpoints to allow in CORS.
app.MapGet("/profile", async context => await context.Response.WriteAsync("profile"))
    .RequireCors(MyAllowSpecificOrigins);

app.MapGet("/home", async context => await context.Response.WriteAsync("home"))
    .RequireCors(MyAllowSpecificOrigins);
```


Enable authentication and authorization middleware:
```csharp
app.UseAuthentication();
app.UseAuthorization();
```

### 6. Create Authentication Controllers
#### Validate JWT Stored in Cookies
```csharp
[HttpGet("validate")]
[EnableCors("_myAllowSpecificOrigins")]
public IActionResult ValidateJWT()
{
    var token = Request.Cookies["jwt"];
    if (string.IsNullOrEmpty(token))
    {
        return Unauthorized();
    }

    var handler = new JwtSecurityTokenHandler();
    var jwtToken = handler.ReadJwtToken(token);
    var username = jwtToken.Claims.First(claim => claim.Type == "unique_name").Value;

    return Ok(new { username });
}
```

#### Logout and Delete JWT Cookie
```csharp
[HttpPost("logout")]
public IActionResult Logout()
{
    Response.Cookies.Delete("jwt");
    return Ok(new { message = "Logged out successfully" });
}
```

---

## Debugging Tips
- Verify if the login route correctly generates a JWT token.
- Check if the JWT token is stored correctly in the client's cookies.
- Ensure authentication logic correctly validates user credentials.
- Debug CORS issues by ensuring the frontend correctly sends requests with credentials.

---

## Conclusion
Following this guide, you will successfully implement JWT-based authentication using HttpOnly cookies in an ASP.NET Core Web API, ensuring secure and seamless user authentication.


# How to Setup CORS Configuration for Cross-Origin Requests

Register Cors Builder with basic Configuration:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyAllowSpecificOrigins",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // client http
                .AllowAnyHeader() 
                .AllowAnyMethod()
                .AllowCredentials(); // enable credentials for authentications
        });
});
```

Enabling Endpoints of clients routes:

```csharp
// Define the endpoints to allow in CORS.
app.MapGet("/profile", async context => await context.Response.WriteAsync("profile"))
    .RequireCors(MyAllowSpecificOrigins);

app.MapGet("/home", async context => await context.Response.WriteAsync("home"))
    .RequireCors(MyAllowSpecificOrigins);
```

---

# Notes
- Always ensure the database connection is correctly configured.
- Run `dotnet ef database update` after making model changes.
- Ensure your DTOs handle required validation and security properly.
- Use dependency injection for `DbContext` to manage database interactions efficiently.

---

# Running & Testing
- Run `dotnet watch run` to start the API
- Use Postman or any API testing tool to test endpoints
- Verify database changes using MySQL client tools





