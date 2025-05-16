> ⚠️ Warning: This is .NET 7 project

# Melody Muse Backend API

This repository contains the backend API for Melody Muse, an innovative platform designed to generate and manage music tracks based on user inputs. Built with .NET 7 and MongoDB, it offers robust user authentication, track generation, and management functionalities.

## Features

- **User Authentication:** Secure login and registration with JWT token support.
- **Track Generation:** Users can generate tracks by specifying genres and other parameters.
- **Track Management:** Users can view, download, and manage their generated tracks.
- **User Profile Management:** Update user profiles including email and password.
- **Points System:** Manage points for track generation, enabling a freemium model.

## Getting Started

### Prerequisites

- .NET 7 SDK
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

### Authentication

- **POST /api/auth/login**  
  Logs in a user by validating their credentials. This endpoint expects a `UserLoginDto` object.

- **POST /api/auth/register**  
  Registers a new user with the system. This endpoint takes a `UserRegistrationDto` object.

### Track Management

- **POST /api/tracks/generate**  
  Generates a new track based on provided parameters encapsulated in a `TrackCreationDto` object.

- **GET /api/tracks/audio/{id}**  
  Retrieves the audio file for a specific track by its ID.

- **GET /api/tracks/{id}**  
  Fetches detailed information about a specific track using its ID.

- **DELETE /api/tracks/{id}**  
  Deletes a specific track identified by its ID.

- **GET /api/tracks/user/{userId}**  
  Lists all tracks created by a specific user, identified by their user ID.

- **POST /api/tracks/search**  
  Searches for tracks based on criteria defined within the request body.

### User Management

- **GET /api/u**  
  Retrieves a list of all users.

- **GET /api/u/{id}**  
  Gets detailed information about a specific user by their user ID.

- **PUT /api/u/{id}**  
  Updates the profile of a specific user. This endpoint consumes a `UserProfileUpdateDto` object.

- **DELETE /api/u/{id}**  
  Deletes a user from the system using their user ID.

### Data Transfer Objects (DTOs)

- **TrackCreationDto**  
  Contains information necessary for creating a new track, such as the title, duration, and audio file.

- **UserLoginDto**  
  Includes credentials (email and password) required for logging into the system.

- **UserRegistrationDto**  
  Holds information required for registering a new user, including their email, password, and username.

- **UserProfileUpdateDto**  
  Contains fields that can be updated in a user's profile, such as their username and email.

### Authorization

Certain endpoints require the user to be authenticated. Such endpoints will respond with a 401 Unauthorized status code if the request lacks a valid authentication token.

