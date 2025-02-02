# Stored Procedure API

A .NET Core Web API application that exposes SQL Server stored procedures as REST API endpoints, enabling seamless integration with web applications, mobile apps, and other API consumers.

## Overview

This API service acts as a bridge between your database stored procedures and client applications, allowing you to:
- Access database stored procedures through HTTP endpoints
- Convert stored procedures into RESTful APIs automatically
- Integrate with any client application that can make HTTP requests
- Maintain security and control over database access

## Features

- Automatic API endpoint generation for stored procedures
- Flexible parameter handling
- JSON response formatting
- Swagger/OpenAPI documentation
- Built-in security and authentication
- Cross-platform compatibility

## Prerequisites

- .NET 6.0 SDK or later
- SQL Server (2016 or later)
- Visual Studio 2022 or VS Code

## Configuration

1. Set up your database connection in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your_Connection_String_Here"
  }
}
```

## Usage
1. Start the server:
   ```sh
   npm start  # For Node.js
   ```
   or
   ```sh
   dotnet run  # For .NET
   ```
2. Access the API:
   ```sh
   GET /api/procedure-name?param1=value1&param2=value2
   ```
3. Call the API from your mobile or web application.

## Configuration
Ensure you configure database credentials in the `.env` file (for Node.js) or `appsettings.json` (for .NET).

## Contributing
1. Fork the repository.
2. Create a new branch.
3. Make your changes.
4. Submit a pull request.

## License
This project is licensed under the MIT License.
