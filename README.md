# CMPE232 Book Club Project

## The libraries used in your project
I used the following libraries and packages:
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Swashbuckle.AspNetCore (For Swagger UI)

## Step-by-step instructions on how to run the application

1. Open the "appsettings.json" file in the project. Check the connection string ("Server=..."). If you are using a different SQL Server name, please update it.

2. Open Visual Studio. Go to "Tools" -> "NuGet Package Manager" -> "Package Manager Console".

3. Type the following command and press Enter to create the database:
   Update-Database

4. To load the sample data (10 items for each table):
   - Open SQL Server Management Studio (SSMS).
   - Open the "VerileriYukle.sql" file located in the project folder.
   - Run (Execute) the script.

5. Press the RUN button in Visual Studio to start the application.