# Backend Authentication Module Documentation

## Overview

Complete authentication and authorization system built with ASP.NET Core Identity, JWT tokens, and Clean Architecture principles.

## Features Implemented

### ✅ Completed

1. **ASP.NET Core Identity Setup**
   - Custom `ApplicationUser` entity with additional properties
   - Role-based identity (Admin, Vendor, Customer)
   - Password policies and lockout configuration
   - Token providers for password reset

2. **JWT Authentication**
   - Access token generation with user claims and roles
   - Refresh token mechanism with rotation
   - Token validation middleware
   - Configurable expiration times

3. **Authentication Endpoints**
   - `POST /api/auth/register` - User registration
   - `POST /api/auth/login` - User login
   - `POST /api/auth/refresh` - Refresh access token
   - `POST /api/auth/forgot-password` - Request password reset
   - `POST /api/auth/reset-password` - Reset password with token
   - `GET /api/auth/me` - Get current user (protected)
   - `POST /api/auth/logout` - Logout (protected)

4. **Authorization Policies**
   - `AdminOnly` - Admin role required
   - `VendorOnly` - Vendor role required
   - `CustomerOnly` - Customer role required
   - `VendorOrAdmin` - Vendor or Admin role required

5. **Security Features**
   - Password strength validation
   - Account lockout after failed attempts
   - Refresh token rotation
   - Token revocation
   - Secure password hashing (Identity default)

6. **Database Seeding**
   - Automatic role creation (Admin, Vendor, Customer)
   - Default admin user creation

## Architecture

### Domain Layer
- `ApplicationUser` - User entity extending IdentityUser
- `RefreshToken` - Refresh token entity
- `UserRole` enum - Role definitions

### Application Layer
- **Commands:**
  - `RegisterCommand` + Handler + Validator
  - `LoginCommand` + Handler + Validator
  - `RefreshTokenCommand` + Handler
  - `ForgotPasswordCommand` + Handler
  - `ResetPasswordCommand` + Handler + Validator
- **Queries:**
  - `GetCurrentUserQuery` + Handler
- **Interfaces:**
  - `ITokenService` - Token generation and management
  - `ICurrentUserService` - Current user context
  - `IEmailService` - Email sending (placeholder)

### Infrastructure Layer
- `TokenService` - JWT token generation and refresh token management
- `CurrentUserService` - Current user from HTTP context
- `EmailService` - Email service (placeholder for implementation)
- `ApplicationDbContext` - EF Core DbContext with Identity
- `SeedData` - Database seeding logic

### API Layer
- `AuthController` - Authentication endpoints
- JWT middleware configuration
- Authorization policies
- CORS configuration

## API Endpoints

### Register
```http
POST /api/auth/register
Content-Type: application/json

{
  "firstName": "John",
  "lastName": "Doe",
  "email": "john@example.com",
  "password": "Password123!",
  "confirmPassword": "Password123!"
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Registration successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-token",
    "expiresIn": 3600,
    "user": {
      "id": "guid",
      "email": "john@example.com",
      "firstName": "John",
      "lastName": "Doe",
      "role": "Customer",
      "avatar": null,
      "isEmailVerified": false,
      "createdAt": "2026-01-19T12:00:00Z"
    }
  }
}
```

### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "Password123!",
  "rememberMe": false
}
```

### Refresh Token
```http
POST /api/auth/refresh
Content-Type: application/json

{
  "refreshToken": "base64-encoded-refresh-token"
}
```

### Forgot Password
```http
POST /api/auth/forgot-password
Content-Type: application/json

{
  "email": "john@example.com"
}
```

### Reset Password
```http
POST /api/auth/reset-password
Content-Type: application/json

{
  "email": "john@example.com",
  "token": "reset-token-from-email",
  "password": "NewPassword123!",
  "confirmPassword": "NewPassword123!"
}
```

### Get Current User
```http
GET /api/auth/me
Authorization: Bearer {accessToken}
```

## Configuration

### appsettings.json
```json
{
  "Jwt": {
    "Key": "Your-secret-key-minimum-32-characters-long",
    "Issuer": "https://localhost:5001",
    "Audience": "https://localhost:5001",
    "ExpireMinutes": 60,
    "RefreshTokenExpireDays": 7
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=marketplace_db;Username=postgres;Password=postgres"
  }
}
```

## Database Migration

To create and apply migrations:

```bash
# Create migration
dotnet ef migrations add MigrationName --project src/Marketplace.Infrastructure --startup-project src/Marketplace.API

# Apply migration
dotnet ef database update --project src/Marketplace.Infrastructure --startup-project src/Marketplace.API
```

## Default Admin User

On first run, the system creates:
- **Email:** admin@marketplace.com
- **Password:** Admin@123!
- **Role:** Admin

**⚠️ IMPORTANT:** Change the default admin password in production!

## Security Considerations

1. **JWT Key:** Use a strong, randomly generated key in production (minimum 32 characters)
2. **HTTPS:** Enable HTTPS in production
3. **CORS:** Configure allowed origins properly
4. **Email Service:** Implement actual email sending for password reset
5. **Rate Limiting:** Add rate limiting to prevent brute force attacks
6. **Token Storage:** Consider HttpOnly cookies for refresh tokens in production

## Next Steps

1. Implement email verification flow
2. Add email service integration (SendGrid, SMTP, etc.)
3. Implement two-factor authentication (optional)
4. Add rate limiting middleware
5. Implement audit logging for authentication events
6. Add password history to prevent reuse

## Testing

Test the endpoints using:
- Swagger UI: http://localhost:5000/swagger
- Postman/Insomnia
- Frontend application (already implemented)

## Integration with Frontend

The backend endpoints match the frontend API service layer:
- Frontend expects the same response format
- JWT tokens are stored in localStorage
- Automatic token refresh on 401 errors
- Role-based UI rendering based on user role
