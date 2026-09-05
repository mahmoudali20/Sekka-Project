🚗 Sekka

Sekka is a full-featured ride-hailing web application inspired by platforms such as Uber.

The system connects passengers with drivers, enables real-time ride requests and notifications, provides ride management and rating functionality, and includes a complete complaint management system with AI-powered complaint summarization.

The application is built using ASP.NET Core MVC and focuses on real-time communication, role-based authorization, external authentication, and scalable business workflows.

📌 Project Overview

Sekka provides a complete ride-booking experience for passengers and drivers while giving administrators a dedicated system for managing drivers, complaints, and support tasks.

The application is divided into four main roles:

👤 Passenger

🚗 Driver

🛠️ Admin

👑 Super Admin

Each role has its own responsibilities and permissions.

✨ Main Features

👤 Passenger Features

Passengers can:

• Create a new account.
• Login using their credentials.
• Login using an external authentication provider.
• Book a ride.
• Receive real-time ride updates.
• Cancel a ride when allowed.
• Receive driver acceptance updates.
• Follow the ride lifecycle.
• View their previous rides through Ride History.
• Rate drivers after completing a ride.
• Submit complaints related to specific rides.
• Track complaint status and feedback.

🚗 Driver Features

Drivers can:

• Register and manage their account.
• Login and logout securely.
• Set their availability status.
• Receive new ride requests in real time.
• Accept ride requests.
• Start accepted rides.
• Complete rides.
• Receive passenger ratings.
• Manage their ride-related activities.

When a passenger creates a ride request, available drivers receive the request immediately using SignalR, without requiring a page refresh.

🛠️ Admin Features

Admins are responsible for handling operational tasks assigned by the Super Admin.

Admins can:

• View assigned complaints and tasks.
• Investigate passenger complaints.
• Handle driver-related issues.
• Resolve assigned tasks.
• Provide feedback after resolving a complaint.
• Manage driver-related operations.

👑 Super Admin Features

The Super Admin has the highest level of access.

Super Admins can:

• Create and manage Admin accounts.
• Receive passenger complaints.
• Review complaints submitted against drivers.
• Use AI to summarize long complaints.
• Assign complaints and tasks to specific Admins.
• Monitor the complaint resolution process.
• Review feedback submitted by Admins.

🔄 Ride Lifecycle

The main ride workflow is designed to simulate a real-world ride-hailing platform.

Passenger
|
| Book Ride
↓
Ride Request Created
|
| SignalR
↓
Available Drivers
|
| Accept
↓
Driver Accepts Ride
|
↓
Passenger
|
├── Cancel Ride
|
└── Continue
|
↓
Ride Started
|
↓
Ride Completed
|
↓
Passenger Rates Driver
|
↓
Ride History
|
↓
Submit Complaint

⚡ Real-Time Communication

One of the main features of Sekka is its use of SignalR for real-time communication.

Instead of requiring users to manually refresh the page, important events are pushed to connected clients immediately.

SignalR is used for:

• New ride notifications.
• Driver ride requests.
• Ride acceptance notifications.
• Ride started notifications.
• Ride completed notifications.
• Ride cancellation notifications.
• New complaint notifications for Super Admins.

Example:

Passenger
|
| Create Ride
↓
Backend
|
| SignalR
↓
Drivers Group
|
├── Driver 1 🔔
├── Driver 2 🔔
└── Driver 3 🔔

Available drivers receive the ride request instantly.

🤖 AI Integration

Sekka includes an AI-powered complaint summarization feature.

Passengers can submit detailed complaints about their rides or drivers.

When a complaint is long, the Super Admin can use the AI integration to generate a concise summary.

Example:

Original Complaint
|
↓
AI Processing
|
↓
Complaint Summary
|
↓
Super Admin Review
|
↓
Assign to Admin

This helps administrators understand lengthy complaints quickly and focus on the important information.

📩 Complaint Management System

Sekka provides a complete complaint workflow.

1. Passenger Submits a Complaint

A passenger can select a specific ride from their Ride History and submit a complaint related to that ride.

2. Super Admin Receives the Complaint

The complaint is delivered to the Super Admin interface in real time using SignalR.

3. AI Summarizes the Complaint

If the complaint contains a large amount of text, the Super Admin can use the AI integration to generate a summary.

4. Super Admin Assigns the Task

The complaint can then be assigned to one of the available Admins.

5. Admin Handles the Complaint

The assigned Admin investigates the issue and takes the required action.

6. Admin Provides Feedback

After resolving the issue, the Admin submits feedback describing the resolution.

Complete Complaint Workflow:

Passenger
|
| Submit Complaint
↓
Super Admin
|
├── Review
|
├── AI Summary
|
└── Assign
|
↓
Admin
|
| Resolve
↓
Feedback

🔐 Authentication & Authorization

Sekka uses a role-based authentication and authorization system.

The application supports:

• User registration.
• Secure login.
• Logout.
• External login.
• Role-based access control.

Roles and Responsibilities:

👤 Passenger

Book rides, cancel rides, rate drivers, and submit complaints.

🚗 Driver

Receive, accept, start, and complete rides.

🛠️ Admin

Handle assigned complaints and operational tasks.

👑 Super Admin

Manage admins, complaints, assignments, and feedback.

Each role has access only to the functionality required for its responsibilities.

🔑 External Authentication

Sekka supports external authentication to provide users with an easier and more convenient login experience.

External authentication is integrated into the authentication system alongside the normal registration and login flow.

🏗️ Project Architecture

Sekka follows a layered architecture to separate responsibilities between different parts of the application.

```
                ┌─────────────────────┐
                │      Sekka.PL       │
                │    Presentation     │
                │      MVC / UI       │
                └──────────┬──────────┘
                           │
                           ↓
                ┌─────────────────────┐
                │      Sekka.BLL      │
                │   Business Logic    │
                │   Services / Rules  │
                └──────────┬──────────┘
                           │
                           ↓
                ┌─────────────────────┐
                │      Sekka.DAL      │
                │   Data Access Layer │
                │    EF Core / Repos  │
                └──────────┬──────────┘
                           │
                           ↓
                ┌─────────────────────┐
                │      Database       │
                │     SQL Server      │
                └─────────────────────┘
```

Main Layers

Sekka.PL

Responsible for:

• MVC Controllers.
• Razor Views.
• Frontend JavaScript.
• User Interface.
• SignalR Hubs.
• Authentication flow.
• User dashboards.
• Driver dashboards.
• Admin dashboards.
• Super Admin dashboards.

Sekka.BLL

Responsible for:

• Business logic.
• Application services.
• Validation.
• Ride management.
• Complaint management.
• Notifications.
• AI-related business operations.

Sekka.DAL

Responsible for:

• Entity Framework Core.
• Database context.
• Entities.
• Repositories.
• Unit of Work.
• Database migrations.
• Data access operations.

🧰 Technology Stack

Backend

• C#
• ASP.NET Core MVC
• Entity Framework Core
• ASP.NET Core Identity
• SignalR

Frontend

• HTML5
• CSS3
• JavaScript
• Bootstrap
• Razor Views

Database

• SQL Server
• Entity Framework Core

Authentication

• ASP.NET Core Identity
• External Login / OAuth

Real-Time Communication

• ASP.NET Core SignalR

AI

• Hugging Face AI Integration
• AI-powered complaint summarization

Development Tools

• Visual Studio
• Git
• GitHub

🔔 Notification Architecture

Sekka separates business logic from the real-time notification mechanism.

For example:

TripService
|
↓
ITripNotificationService
|
↓
TripNotificationService
|
↓
SignalR Hub
|
↓
Connected Clients

This approach keeps the business layer independent from the SignalR implementation and makes the application easier to maintain and extend.

📊 Core Modules

The application contains several major modules:

Sekka
|
├── Authentication
|
├── Passenger Management
|
├── Driver Management
|
├── Car Management
|
├── Ride Management
|
├── Real-Time Notifications
|
├── Rating System
|
├── Complaint Management
|
├── AI Integration
|
├── Admin Management
|
└── Super Admin Dashboard

🔒 Security

Security is an important part of the Sekka application.

The application uses:

• ASP.NET Core Identity.
• Role-based authorization.
• Authentication and authorization middleware.
• Anti-forgery protection.
• Secure configuration practices.
• External authentication.

Sensitive configuration values such as API keys, access tokens, passwords, and authentication secrets should never be committed to the repository.

They should be stored using secure configuration mechanisms such as:

• User Secrets.
• Environment Variables.
• Secure deployment configuration.

🎯 Project Goals

The main goal of Sekka is to build a complete real-world ride-hailing platform that combines modern web technologies with real-time communication and AI capabilities.

• Real-Time Communication — Instant ride requests, updates, and notifications using SignalR.
• Role-Based Authorization — Separate permissions and functionality for Passengers, Drivers, Admins, and Super Admins.
• Ride Management — Complete ride lifecycle from booking and acceptance to starting, completing, rating, and cancellation.
• Driver and Passenger Interaction — Seamless interaction between passengers and drivers throughout the ride.
• Complaint Management — A complete complaint workflow from passenger submission to admin assignment, resolution, and feedback.
• External Authentication — Authentication with support for external login providers.
• AI Integration — AI-powered complaint summarization to help administrators quickly understand lengthy complaints.
• Layered Architecture — Separation of presentation, business logic, and data access responsibilities.

🔮 Future Improvements

Possible future improvements include:

• Real-time GPS tracking.
• Online payment integration.
• Advanced driver matching.
• Mobile application.
• Advanced analytics dashboard.
• Advanced driver rating analytics.
• AI-powered complaint classification.
• Push notifications.
• Automated complaint prioritization.

👨‍💻 Team Project

Sekka was developed as a team project as part of the ITI Final Project.

The project combines:

• Backend development.
• Frontend development.
• Database design.
• Real-time communication.
• Authentication and authorization.
• AI integration.
• Software architecture.
• Business workflow design.


