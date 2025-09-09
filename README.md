# ⚙️ TicDrive Backend

This backend is powering **TicDrive**, a platform designed to simplify car maintenance by connecting users with local workshops.  
It serves both the **TicDrive mobile application (iOS & Android, built in React Native)** and the **TicDrive web portal for workshops**.

---

## 🚗 Overview

TicDrive enables users to:
- Browse nearby workshops  
- View available car services (oil change, battery replacement, tire rotation, AC recharge, etc.)  
- Book appointments online  
- Compare ratings, reviews, and prices  

Workshops use the **web platform** to:
- Manage their profile and services  
- Handle bookings and customer requests  
- Upload images (stored in Azure Blob Storage)  
- Track reservations and schedules  

---

## 🛠️ Tech Stack

- [.NET Core 8](https://learn.microsoft.com/en-us/aspnet/core/?view=aspnetcore-8.0) – RESTful API backend  
- **Entity Framework Core** – ORM for database management  
- **SQL Server** – relational database  
- [Azure App Service](https://azure.microsoft.com/en-us/services/app-service/) – hosting and deployment  
- [Azure Blob Storage](https://azure.microsoft.com/en-us/services/storage/blobs/) – image storage and management  
- **JWT Authentication** – secure user & workshop authentication  

---

## 📱 Clients

- **Mobile App** used by end-users to search, compare, and book car services. <br>
  Download [IOS](https://apps.apple.com/it/app/ticdrive/id6740627366?l=en-GB) and [Android](https://play.google.com/store/apps/details?id=com.ticdrive.app&pcampaignid=web_share) versions.<br>
  Github [source code](https://github.com/AlbyCosmy99/ticdrive-app-react-native).

- **Web Platform for Workshops**:  
  Provides workshops with tools to manage services, bookings, and their visibility on the platform.<br>

  Online site [here]().<br>
  Github [source code]()

---

## 🔑 Features

- 👤 User registration, login & profile management  
- 🏪 Workshop onboarding and service management  
- 📅 Appointment booking system  
- 💾 Image storage via Azure Blob  
- 🔔 Email notifications & confirmations  
- 🔒 Role-based authentication (user vs workshop)  

---

## 👨‍💻 Development

The backend was fully developed by me (**Andrei Albu**) as part of the full **TicDrive ecosystem**, integrating mobile and web applications into a single connected platform.  

---

## 🚀 Deployment

- Continuous deployment with **Azure App Service**  
- GitHub Actions for CI/CD  
- Production-ready setup with database migrations and monitoring  
