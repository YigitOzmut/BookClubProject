# BOOK CLUB PROJECT - CMPE232 FINAL PROJECT

## 1. LIBRARIES USED (Kullanılan Kütüphaneler)
This project uses the following NuGet packages and libraries:
- **Microsoft.EntityFrameworkCore (v8.0.x):** ORM for database operations.
- **Microsoft.EntityFrameworkCore.SqlServer (v8.0.x):** SQL Server provider for EF Core.
- **Microsoft.EntityFrameworkCore.Tools (v8.0.x):** For EF Core migrations and scaffolding.
- **Microsoft.AspNetCore.OpenApi:** For OpenAPI/Swagger support.
- **Swashbuckle.AspNetCore:** Generates API documentation and Swagger UI.

## 2. HOW TO RUN (Adım Adım Çalıştırma)
Follow these steps to run the application successfully:

**Step 1: Configure Database Connection**
- Open the `appsettings.json` file.
- Update the `"Server=..."` part in the ConnectionString to match your local SQL Server name.
  - *Example:* `Server=.;Database=BookClubDB;Trusted_Connection=True;TrustServerCertificate=True;`

**Step 2: Initialize Database**
- Open **Visual Studio**.
- Go to **Tools > NuGet Package Manager > Package Manager Console**.
- Run the following command to create the database schema:
  `Update-Database`

**Step 3: Load Sample Data (IMPORTANT)**
- To populate the database with the required sample data (10 items for each category):
  1. Open SQL Server Management Studio (SSMS).
  2. Open the file named `VerileriYukle.sql` (located in the project folder).
  3. Execute the script. This will clear old data and insert fresh sample data (Books, Authors, Members, etc.).

**Step 4: Run the Application**
- Press the **Play (Run)** button in Visual Studio.
- The browser will open the application automatically.

## 3. PROJECT DETAILS
- **Database Design:** The project uses Code-First approach with Entity Framework Core.
- **Data Integrity:** `ExecuteSqlRawAsync` is used for deletion operations to handle Foreign Key constraints safely.