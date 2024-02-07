# Melody Muse Backend API

This repository contains the backend API for Melody Muse, an innovative platform designed to generate and manage music tracks based on user inputs. Built with .NET 8 and MongoDB, it offers robust user authentication, track generation, and management functionalities.

## Features

- **User Authentication:** Secure login and registration with JWT token support.
- **Track Generation:** Users can generate tracks by specifying genres and other parameters.
- **Track Management:** Users can view, download, and manage their generated tracks.
- **User Profile Management:** Update user profiles including email and password.
- **Points System:** Manage points for track generation, enabling a freemium model.

## Getting Started

### Prerequisites

- .NET 8 SDK
- MongoDB installed on your local machine or access to a MongoDB instance
- Visual Studio or VS Code

### Installation

1. Clone the repository:

```bash
   git clone https://github.com/RenAIssance-bilkent/renai-backend.git
```
2. Navigate to the cloned repository:
```bash
   git cd renai-backend
```
3. Restore dependencies:
```bash
   dotnet restore
```
4. Update the appsettings.json file with your MongoDB connection string:
```json
   "MongoDB": {
    "ConnectionString": "Your Connection String Here",
    "DatabaseName": "MelodyMuseDb"
  }
```
