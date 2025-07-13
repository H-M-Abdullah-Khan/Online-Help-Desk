# Online Help Desk (OHD) System

> A complete ASP.NET MVC-based Online Help Desk Ticketing System for streamlining complaint submission, task assignment, and issue resolution across facilities in an organization.

---

## 📄 Table of Contents

- [📌 Project Overview](#project-overview)
- [🛠️ Installation Guide](#installation-guide)
  - [✅ Prerequisites](#prerequisites)
  - [📦 Setup Instructions](#setup-instructions)
- [🚀 How to Use](#how-to-use)
- [📁 Project Structure](#project-structure)
- [📊 Features](#features)
- [🔐 Security Measures](#security-measures)
- [🧪 Testing Summary](#testing-summary)
- [🛡 Limitations & Future Enhancements](#limitations--future-enhancements)
- [📌 License](#license)

---

## 📌 Project Overview

The **Online Help Desk (OHD)** System is a role-based web application built in **ASP.NET MVC** with **SQL Server** as its database backend. It provides a streamlined approach to managing facility-related complaints, from submission to resolution.

This application supports 4 primary roles:

- **Admin**: Manages users, roles, and facilities.
- **End User**: Submits complaints (requests).
- **Facility Head**: Assigns complaints to appropriate Assignees.
- **Assignee**: Resolves the tasks assigned to them.

Built with usability and simplicity in mind, OHD ensures effective communication and resolution tracking.

---

## 🛠️ Installation Guide

### ✅ Prerequisites

Make sure you have the following tools installed:

- Visual Studio 2022 or later with **.NET Web Development** workload
- [.NET SDK 6.0 or later](https://dotnet.microsoft.com/en-us/download/dotnet)
- [SQL Server (Express/Developer/Standard)](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [SQL Server Management Studio (SSMS)](https://learn.microsoft.com/en-us/sql/ssms/download-sql-server-management-studio-ssms)
- [Git](https://git-scm.com/) *(optional but recommended)*

---

### 📦 Setup Instructions

#### 1. Clone the Repository

```bash
git clone https://github.com/H-M-Abdullah-Khan/Online-Help-Desk.git
cd Online-Help-Desk
```

Or download the ZIP file and extract it locally.

---

#### 2. Open the Solution in Visual Studio

- Launch **Visual Studio**
- Go to `File > Open > Project/Solution`
- Select `OnlineHelpDesk.sln`

---

#### 3. Configure the Database Connection

Open the `appsettings.json` file. Modify the **connection string**:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=OnlineHelpDeskDB;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

For SQL Server Authentication:

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=OnlineHelpDeskDB;User Id=your_user;Password=your_password;"
```

Replace:

- `YOUR_SERVER_NAME` with your SQL Server instance (like `localhost\\SQLEXPRESS`)
- Ensure SQL Server is running

---

#### 4. Build the Project

In Visual Studio:

- Press **Ctrl + Shift + B**
- Or go to **Build > Build Solution**

This restores dependencies and builds the project.

---

#### 5. Apply Migrations (Entity Framework)

If using Entity Framework Code First:

- Open **Package Manager Console**: `Tools > NuGet Package Manager > PMC`

```powershell
Add-Migration InitialCreate
Update-Database
```

Alternatively, if using a .sql file manually, open **SSMS** and run your script.

---

#### 6. Run the Application

- Press **F5** or click **Start Debugging**
- The site will launch at `https://localhost:PORT`

---

## 🚀 How to Use

- Navigate to the login/register page
- Roles include: Admin, End User, Facility Head, and Assignee

### For Each Role:

#### 🔹 End User

- Submit new complaints
- Track status of each ticket

#### 🔹 Facility Head

- View and assign complaints
- Monitor progress & stats

#### 🔹 Assignee

- View assigned tasks
- Change status: `Assigned → In Progress → Closed`

#### 🔹 Admin

- Manage users, roles, and facilities
- View global statistics and perform oversight

---

## 📁 Project Structure

```
Online-Help-Desk/
├── Controllers/              # MVC Controllers
├── Models/                   # Entity Models
├── Views/                    # Razor Views
│   ├── Shared/               # Layouts & Partials
│   ├── Account/              # Login/Register
│   ├── Requests/             # Ticket pages
│   └── Dashboard/            # Role dashboards
├── Data/                     # DB context & seeding
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
├── wwwroot/                  # Static assets (CSS/JS/img)
├── appsettings.json          # Configuration
├── Program.cs
├── Startup.cs
└── OnlineHelpDesk.sln        # Solution file
```

---

## 📊 Features

- Role-Based Login System
- Complaint Submission & Status Tracking
- Facility-wise Filtering & Assignment
- Task Dashboard with Graphs (Chart.js)
- Light/Dark Mode Support (Optional)
- Export to PDF/Excel functionality
- Fully Responsive UI with Tailwind CSS

---

## 🔐 Security Measures

- Session-based authentication
- Role validation on every controller
- Encrypted password storage (Hashing)
- Limited access per role
- Input validation
- Anti-forgery tokens in forms

---

## 🧪 Testing Summary

- Unit testing on core functionalities (Model Validations, Status Updates)
- Manual UI Testing for each role
- Admin tested with 10+ mock users
- Mobile and Desktop responsiveness verified

Sample Tests:

- ✅ User Registration & Login
- ✅ Complaint Submission Workflow
- ✅ Role Switching and Authorization
- ✅ SQL Injection Protection

---

## 🛡 Limitations & Future Enhancements

**Limitations:**

- No email or SMS notifications
- No password recovery yet
- Limited analytics for admins

**Planned Enhancements:**

- Add JWT-based API for mobile integration
- Add complaint category and severity
- Add feedback form after issue closure
- Enable chat/ticket history

---

## 📌 License

MIT License

Feel free to fork, modify, and contribute.

---

## 📬 Contact

Created by [Hafiz Muhammad Abdullah Khan](https://github.com/H-M-Abdullah-Khan)

📧 Email: [info@ohdsystem.com](mailto\:info@ohdsystem.com)

For support or bug reports, open an issue on GitHub.

---

> ⭐ Star this repo if it helped you!

