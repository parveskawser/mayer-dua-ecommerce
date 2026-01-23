# MDUA - Enterprise E-Commerce & Inventory Management System

**MDUA** is a robust, multi-tenant, N-Layered ASP.NET Core application designed for enterprise-grade e-commerce and inventory management. It supports complex product variants, real-time inventory tracking, accounting automation, role-based security, SPA-enabled media sharing, and dynamic company-level configurations.
  
-----

## 1\. 🛍️ Key Features

  * **Advanced Product Management:**
      * Support for **Product Variants** (Size, Color, Storage, etc.) generated via dynamic Attributes
      * **Inventory Tracking:** Real-time stock management with `VariantPriceStock` logic
      * **Discount Engine:** Supports Flat and Percentage-based discounts with effective date ranges
      * **Media Management:** Multiple images per product and variant, videos from YouTube, Vimeo, and Facebook
  * **Order Processing System:**
      * **Dual Channel Support:** Handles both **Online Orders** (generated via web) and **Direct Orders** (POS/Admin entry).
      * **Order Tracking:** Public-facing order tracking via Order ID.
      * **Automated Calculations:** Dynamic calculation of Delivery Charges (Inside/Outside Dhaka) and Discounts.
  * **Financial & Accounting:**
      * **Journal Entries:** Automated Double-Entry bookkeeping for payments and purchases.
      * **Dynamic Payment Configuration:** Each tenant/company can configure its own payment methods (Cash, Bank, Mobile Banking, Gateway-based). Payment settings are fully dynamic and configurable from the Admin Panel.
      * **Audit Logging:** Comprehensive database-level tracking of every Insert, Update, and Delete operation via Triggers.
  * **Security & Access Control:**
      * **Custom RBAC:** Granular permissions (e.g., `Product.View`, `Order.Place`) assigned to User Groups or specific Users.
      * **Session Management:** Secure cookie-based authentication with device tracking.
  * **Admin Dashboard:**
      * Visual analytics for Sales Trends and real-time KPI tracking.
  * **Real-Time Communication (SignalR):**
      * **Live Support Chat:** Instant messaging channel between Customers and Admin support agents.
      * **Persistent History:** Chat history is saved to the database (for 1 hour) via Facade/DAL, ensuring conversations are never lost even if the connection drops.
      * **Live Message Notifications with sound:** Real-time push notifications when new message is received.
  * **DateTime & Timezone Handling:**
      * **Timezone-Aware System:** All DateTime values are stored in UTC at database level. Automatically converted to specific local time in UI and reports. Ensures accuracy across Orders, Payments, Audit Logs and Chat timestamps.
  * **Two-Factor Authentication (2FA):**
      * **OTP-Based Authentication:** Time-based One-Time Password (OTP) verification applied after successful username/password validation. OTPs are single-use and expire automatically after a configurable duration.
  * **Session & Device Aware:** OTP validation is bound to the login session and tracked device.
-----

## 2\. 🧱 Solution Structure

The solution follows a strict separation of concerns using a 4-tier architecture:

| Project | Type | Description |
| :--- | :--- | :--- |
| **MDUA.Web.UI** | ASP.NET Core MVC | The presentation layer containing Hubs, Controllers, Views (Razor), and Client-side scripts (jQuery/AJAX). |
| **MDUA.Facade** | Class Library | The **Business Logic Layer (BLL)**. It acts as the orchestrator, transforming entities and applying business rules before data storage. |
| **MDUA.DataAccess** | Class Library | The **Data Access Layer (DAL)**. Executes raw SQL queries and Stored Procedures via ADO.NET. |
| **MDUA.Entities** | Class Library | Contains POCO classes, DTOs, Enums, and Base Entities matching the DB schema. |
| **MDUA.Framework** | Class Library | Infrastructure layer containing `BaseDataAccess` (SQL Wrapper), Encryption helpers, and custom Exception handling. |

-----

## 3\. Layer Responsibilities

### 🖥️ Web UI Layer (`MDUA.Web.UI`)

  * **Controllers:** Thin controllers (e.g., `OrderController`, `ProductController`) that inject Facade interfaces.
  * **Views:** Razor views using Partial Views for modularity (e.g., `_ProductVariantsPartial.cshtml`, `_CustomerDetailsPartial.cshtml`).
  * **Client-Side:** Heavy use of **jQuery** and **AJAX** in `wwwroot/js` (e.g., `admin-order.js`) to handle dynamic form submissions without page reloads.

### 🧠 Facade Layer (`MDUA.Facade`)

  * **The Brain:** Contains the core business logic.
  * **Responsibilities:**
      * Orchestrating multiple DA calls (e.g., `ProductFacade.AddProduct` saves the Product, then Attributes, then Variants, then Stock).
      * Calculations: `GetBestDiscount()` logic resides here to determine the lowest price based on active promotions.
      * Data Transformation: Converts raw DB entities into ViewModels (`UserLoginResult`, `ProductResult`).

### 💾 Data Access Layer (`MDUA.DataAccess`)

  * **The Muscle:** Direct interaction with Microsoft SQL Server.
  * **Pattern:** Extends `BaseDataAccess` (from Framework) to utilize helper methods like `SelectRecords`, `InsertRecord`, `GetSQLCommand`.
  * **Implementation:** Uses **ADO.NET** (SqlDataReader, SqlCommand) instead of Entity Framework for performance and fine-tuned control.
  * **Partial Classes:** Classes like `ProductDataAccess` are often split to organize huge query sets.

### ⚙️ Framework Layer (`MDUA.Framework`)

  * **BaseDataAccess:** A custom abstract base class that wraps `SqlConnection` and `SqlTransaction`. It handles parameter mapping (`AddParameter`, `pInt32`), connection opening/closing, and transaction rollbacks.
  * **Utilities:** Contains `Encryptor` for password hashing and `OTPGenerator`.

-----

## 4\. 🛠️ Tech Stack

  * **Framework:** .NET 9 (ASP.NET Core)
  * **Language:** C\#
  * **Database:** Microsoft SQL Server (2016+)
  * **ORM:** Native ADO.NET (Custom Wrapper)
  * **Frontend:** Razor Views, jQuery, Bootstrap 5, Chart.js
  * **Authentication:** ASP.NET Core Cookie Authentication
  * **Reporting:** SQL Server Reporting Services (SSRS) logic implied by `Order.Export` permissions
  * **Real-Time:** ASP.NET Core SignalR
-----

## 5\. 🗄️ Database Overview

The database (`AA4`) is highly normalized and logic-heavy.

### Key Tables

  * `Product` / `ProductVariant` / `VariantPriceStock`: Handles the complex E-commerce inventory model.
  * `SalesOrderHeader` / `SalesOrderDetail`: Stores transactional data.
  * `UserPermission` / `PermissionGroup`: Handles the custom security model.
  * `AuditLog`: Stores JSON snapshots of `OldValues` and `NewValues` for every change.
  * `ChatMessage`: Stores the chat history (SenderId, ReceiverId, MessageContent, SentAt, IsRead).

### Advanced SQL Features

  * **Stored Procedures:** Heavy usage for CRUD operations (e.g., `InsertSalesOrderHeader`, `GetProductDetails`).
  * **Triggers:**
      * `TR_Product_Audit`, `TR_SOH_Audit`: Automatically logs changes to the `AuditLog` table.
      * `trg_ReduceStockOnOrder`: Automatically decrements `VariantPriceStock` when a `SalesOrderDetail` is inserted.
      * `trg_UpdateProfitAmount`: Automatically calculates profit based on UnitPrice vs AverageCost.
  * **Views:** Used for reporting (e.g., `vUnbalancedJournalEntries`).

-----

## 6\. ⚙️ Getting Started

### Prerequisites

  * Visual Studio 2022
  * SQL Server Management Studio (SSMS)
  * .NET SDK (6.0 or later)

### Installation Steps

1.  **Clone the Repository**


2.  **Database Setup**

      * Open `resource/script 2.sql` in SSMS.
      * Execute the script to create the `AA4` database, tables, stored procedures, and populate initial seed data (Admin user, Permissions, Locations).

3.  **Configure Connection**

      * Open `src/MDUA.Web.UI/appsettings.json`.
      * Update the `DefaultConnection` string to point to your local SQL Server instance.

4.  **Build & Run**

      * Open `src/MDUA.sln` in Visual Studio.
      * Set `MDUA.Web.UI` as the Startup Project.
      * Press `F5` to run.

5.  **Default Login**

      * **Username:** `admin`
      * **Password:** `Admin@123`

-----

## 7\. 🔄 Feature Flow (End-to-End)

### Scenario: Placing a Direct Order (Admin Panel)

1.  **UI:** Admin navigates to `Order/Add`. The page loads products via `OrderFacade.GetProductVariantsForAdmin()`.
2.  **Action:** Admin selects a customer (AJAX autofill via `OrderController/CheckCustomer`), adds items, and clicks "Place Order".
3.  **JavaScript:** `admin-order.js` collects the cart data and POSTs JSON to `OrderController.PlaceDirectOrder`.
4.  **Facade:** `OrderFacade.PlaceAdminOrder` is called.
      * It creates a `SalesOrderHeader` object.
      * It iterates through items to create `SalesOrderDetail` objects.
5.  **DataAccess:**
      * `BaseDataAccess.BeginTransaction()` starts a SQL Transaction.
      * `SalesOrderHeaderDataAccess` inserts the header and returns the new `Id`.
      * `SalesOrderDetailDataAccess` inserts items linked to that `Id`.
6.  **Database Trigger:** The `trg_ReduceStockOnOrder` trigger fires in SQL, immediately reducing the `StockQty` in the `VariantPriceStock` table.
7.  **Completion:** Transaction commits. Success response sent to UI. Invoice is ready for print.

-----

## 8\. 🔮 Future Enhancements

  * **API Layer:** Exposing `MDUA.Facade` via a Web API project for mobile app integration.
  * **Payment Gateway:** Integration with SSLCommerz or Stripe for automated online payments (currently handles manual payments/ledgers).
  * **Dockerization:** Adding `Dockerfile` for containerized deployment.
  * **Inventory Forecasting:** Utilizing the `ProductInventory` history for sales prediction.