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
   cd renai-backend
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

### Architecture
## UML Class diagram
![TLDDQzn03BtxLmnxSjiaqAif8KcNqa8F2qb-WEfHlABpOIGv8PJ-zyxEs1kFIVUmW_IUf3TFunjHO9syQte78kPHaCsVngJVftjb2eEPKYg0nog97iXLcJ6LlqIstq6E1VX9ebyU6FhVrqPFXvGy1JLZJF_I_6tckPcs6Kos2fNKrI8633ndikwU4HJjtMl9xLFmG1wN_ArMrJ1PYezStyAZ](https://github.com/RenAIssance-bilkent/renai-backend/assets/33938205/7783e849-92e7-40e4-b731-d0c79fc31bb7)

!!! Track class contains string metadata, those are input features for the model that will be determined later so for now it is just string.
## API Endpoints
* POST /api/u/register: Register a new user.
* POST /api/auth/: Authenticate a user.
* POST /api/tracks/generate: Generate a new track.
* GET /api/u/[id]: Get user by id.
* PUT /api/u/[id]: Update user profile.
* DELETE /api/u/[id]: Delete user by id.
