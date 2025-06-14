# 📌 Task Management System

## 📖 Description  
A simple and clean **Task Management System** built using **ASP.NET Core MVC** and **Entity Framework Core**.  
It allows users to create, view, filter, and manage tasks with full role-based access control and validation.

---

## 🧰 Technologies Used
- ⚙️ ASP.NET Core MVC  
- 🗃️ MSSQL  
- 🧱 Entity Framework Core  
- 🔐 ASP.NET Identity  
- 📦 MediatR with CQRS  
- 📊 Specification Pattern (Pagination & Filtering)  
- 📜 FluentValidation  
- 📂 Repository Pattern & Unit of Work  
- 🧠 Strategy Design Pattern  
- 🧩 SOLID Principles (SRP, OCP, ISP)  
- 🔍 Reflection for MediatR, FluentValidatio  
- 📈 Non-Clustered Indexes (Search Optimization)

---

## 📅 Deadline  
**2 days** from receiving the requirement document.

---

## 🎯 Features
- View all tasks  
- Filter tasks by **Priority** (Low, Medium, High)  
- View task details  
- Create new tasks  
  - ✅ Regular Users can only assign tasks to themselves  
  - ✅ Admins can assign tasks to any user  
- Edit existing tasks (Admins only)  
- Delete tasks (Admins only)  

---

## 🧾 Task Model Includes:
- 📝 Task Name  
- 📄 Description  
- 🔼 Priority (Low, Medium, High)  
- 📅 Due Date  
- 👤 Assigned User  

---

## 🔐 Roles & Permissions

| Role    | Create Task | View Tasks | Edit Task | Delete Task | Assign to Others |
|---------|-------------|------------|-----------|-------------|------------------|
| User    | ✅ (self)    | ✅         | ❌        | ❌          | ❌               |
| Admin   | ✅           | ✅         | ✅        | ✅          | ✅               |

---

## 🧱 Patterns & Principles Applied

- **Repository Pattern**  
- **Unit of Work Pattern**  
- **CQRS (with MediatR)**  
- **Specification Pattern** for filtering and pagination  
- **FluentValidation** for request model validation  
- **Strategy Design Pattern** (e.g., filtering strategies)  
- **Reflection** for **Generic Filtering** (combined with MediatR + FluentValidation)  
- **Non-Clustered Indexes** for search optimization  
- **SOLID Principles**:
  - **SRP** - Single Responsibility Principle  
  - **OCP** - Open/Closed Principle  
  - **ISP** - Interface Segregation Principle  

---

## 🚀 Getting Started

1. Clone the repository  
2. Configure the MSSQL database  
3. Apply EF Core Migrations  
4. Seed roles and users (`Admin`, `User`)  
5. Run the application

---

## 📁 Folder Structure (Overview)

