# Mayer-Dua E‑Commerce (MDUA)

Single-page E‑Commerce application where customers can view a product, choose variants (size, color, type), see dynamic pricing, and place an order directly from the same page. Orders are stored in the backend for further processing, and admins can manage products for multiple companies.

---

## 🛍️ Key Features

### Customer (Single Product Page)
- View product details (title, description, images, base price).
- Select product variants (e.g., size, color, package).
- Dynamic price update based on selected variant and quantity.
- Simple one-page checkout:
  - Customer info (name, phone, address, email).
  - Variant & quantity selection.
- Place order without leaving the page.
- Success / failure confirmation after order placement.

### Admin (Back Office)
- Manage companies:
  - Create, edit, activate / deactivate companies.
- Manage products per company:
  - Create products and upload images.
  - Configure base price and description.
- Manage product variants:
  - Add variants with SKU, additional price, and stock.
- Manage orders:
  - View order list (filter by date, company, status).
  - View order details (customer, items, totals).
  - Update order status (Pending, Processing, Shipped, Completed, Cancelled).

---

## 🧱 Solution Structure

The solution uses a multi-layer architecture with custom ADO.NET for data access.

```text
src/
 ├─ MDUA.DataAccess   → Data access layer (ADO.NET repositories, SQL operations)
 ├─ MDUA.Entities     → Domain entities / POCO classes
 ├─ MDUA.Facade       → Application / facade layer (services exposed to Web.UI)
 ├─ MDUA.Framework    → Core business logic, utilities, common helpers
 └─ MDUA.Web.UI       → Web layer (ASP.NET Core 9.0 MVC / Razor UI)
```

### Layer Responsibilities

#### `MDUA.Web.UI` (Presentation Layer)
- ASP.NET Core 9.0 web project.
- Contains Controllers, Razor Views/Pages, ViewModels.
- Renders:
  - Customer-facing single product page.
  - Admin pages for products, companies, and orders.
- Calls the **Facade** layer (services) instead of direct database access.

#### `MDUA.Facade` (Application / Service Layer)
- Exposes high-level operations for the UI, e.g.:
  - `PlaceOrder`, `GetSingleProduct`, `GetProductVariants`, `GetOrdersForAdmin`.
- Orchestrates business workflows:
  - Validates input using Framework logic.
  - Calls **DataAccess** repositories via **Framework** services.
- Acts as a clean boundary between UI and inner layers.

#### `MDUA.Framework` (Core Business Logic & Utilities)
- Contains shared business rules, utilities, and cross-cutting helpers, such as:
  - Price calculation logic (base price + variant + quantity).
  - Order status transitions.
  - Validation helpers, logging helpers, etc. (as needed).
- Uses **Entities** for domain models and **DataAccess** for persistence.
- Keeps domain logic centralized and reusable.

#### `MDUA.Entities` (Domain Model)
- Contains all POCO/entity classes, for example:
  - `Company`, `Product`, `ProductVariant`
  - `Customer`, `Order`, `OrderItem`
- Pure C# classes without UI or database-specific code.
- Used across all layers to represent core business data.

#### `MDUA.DataAccess` (Data Layer – Custom ADO.NET)
- Encapsulates all database operations using ADO.NET:
  - `SqlConnection`, `SqlCommand`, `SqlDataReader`, transactions, etc.
- Implements repository classes/interfaces, e.g.:
  - `ProductRepository`, `OrderRepository`, `CompanyRepository`, `VariantRepository`.
- Reads connection string from configuration (provided by Web.UI / Framework).
- Maps data rows to **Entities** and returns them to **Framework/Facade**.

---

## 🛠️ Tech Stack

- **Backend Framework**: ASP.NET Core 9.0
- **Language**: C#
- **Database**: Microsoft SQL Server
- **Data Access**: Custom ADO.NET (no Entity Framework)
- **Architecture**: Multi-layer / n-tier
- **Frontend**: Razor Views (or MVC Views) with HTML, CSS, JS (Bootstrap optional)

---

## 🗄️ Database Overview

Core suggested tables:

- `Companies`
- `Products`
- `ProductVariants`
- `Customers`
- `Orders`
- `OrderItems`

Each order links to a customer, company, product, and product variant, and stores quantity, unit price, and totals.

---

## ⚙️ Getting Started

### 1. Requirements

- .NET SDK 9.0
- SQL Server
- Visual Studio / VS Code

### 2. Database Setup

1. Create database:
   ```sql
   CREATE DATABASE MDUA;
   ```
2. Run/create the tables according to the schema (e.g., `/db/scripts/initial.sql`).

### 3. Configure Connection String

In `MDUA.Web.UI/appsettings.json` (example):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=MDUA;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
  }
}
```

The `MDUA.DataAccess` layer will use this connection string via configuration/DI.

### 4. Build & Run

From the solution root:

```bash
dotnet restore
dotnet build
dotnet run --project src/MDUA.Web.UI
```

Open the URL shown in the console (e.g. `https://localhost:5001`).

---

## 🔄 Order Flow (End-to-End)

1. **Admin** configures companies, products, and variants.
2. **Customer** opens the single product page, selects variant & quantity, enters details, and submits the order.
3. **MDUA.Web.UI** sends the request to the **MDUA.Facade** services.
4. **MDUA.Facade** validates & orchestrates logic using **MDUA.Framework**.
5. **MDUA.Framework** uses **MDUA.DataAccess** to create customer, order, and order items in the database.
6. Admin can later view and process the order from the admin interface.

---

## 🔮 Future Enhancements

- Multiple product listing page.
- Coupon / discount support.
- Payment gateway integration (bKash, SSLCommerz, etc.).
- Email/SMS notifications.
- Role-based admin (Super Admin, Company Admin, Staff).

---

## 🤝 Contribution

- Follow the existing layer boundaries (no UI ↔ DB direct calls).
- Add new features via Facade & Framework services.
- Use Entities for all domain structures and DataAccess only for persistence logic.
