# Forum Application - Frontend

This is the React frontend application for the Forum system, built with React, TypeScript, Tailwind CSS, and shadcn/ui.

## Features

- 🔐 **Authentication** - Login and Register with JWT tokens
- 📝 **Create Questions** - Ask questions to the community
- 🛡️ **Protected Routes** - Secure access to authenticated areas
- 🎨 **Beautiful UI** - Modern design with Tailwind CSS and shadcn/ui
- 🌙 **Dark Mode** - Built-in dark mode support
- 🔄 **Auto Token Refresh** - Automatic JWT token refresh on expiration

## Tech Stack

- **React 19** - UI library
- **TypeScript** - Type safety
- **Vite** - Build tool
- **React Router** - Routing
- **Tailwind CSS v4** - Styling
- **shadcn/ui** - UI components
- **Axios** - HTTP client
- **Lucide React** - Icons

## Prerequisites

- Node.js 18+ and npm
- Running User API (http://localhost:5249)
- Running Forum API (http://localhost:5126)

## Setup Instructions

### 1. Install Dependencies

```bash
cd ui/forum-app
npm install
```

### 2. Configure Environment (Optional)

The app works with default configuration, but you can customize API URLs by creating a `.env.local` file:

```bash
VITE_USER_API_URL=http://localhost:5249/api
VITE_FORUM_API_URL=http://localhost:5126
```

### 3. Start Backend Services

Make sure both backend services are running:

**User API (Terminal 1):**
```bash
cd src/User
dotnet run
```

**Forum API (Terminal 2):**
```bash
cd src/Forum
dotnet run
```

### 4. Start the Frontend

```bash
npm run dev
```

The app will be available at: http://localhost:5173

## Project Structure

```
src/
├── components/
│   ├── ui/              # shadcn/ui components
│   ├── AuthScreen.tsx   # Login/Register form
│   └── ProtectedRoute.tsx # Route protection wrapper
├── contexts/
│   └── AuthContext.tsx  # Authentication state management
├── lib/
│   ├── api.ts           # Axios instances & interceptors
│   ├── auth-service.ts  # Authentication API calls
│   ├── question-service.ts # Question API calls
│   └── utils.ts         # Utility functions
├── pages/
│   ├── QuestionsPage.tsx # Questions list page
│   └── CreateQuestionPage.tsx # Create question page
├── App.tsx              # Main app with routing
└── main.tsx             # App entry point
```

## Available Routes

- `/login` - Login and registration page
- `/questions` - Questions list (protected)
- `/questions/create` - Create new question (protected)
- `/` - Redirects to `/questions`

## Authentication Flow

1. User logs in or registers via `/login`
2. Backend returns JWT access token (15 min) and refresh token (30 days)
3. Tokens are stored in localStorage
4. Access token is automatically attached to API requests
5. On 401 response, app attempts to refresh the token
6. If refresh fails, user is redirected to login

## API Integration

### User API Endpoints
- `POST /api/auth/login` - Login
- `POST /api/auth/register` - Register
- `POST /api/auth/refresh` - Refresh token
- `POST /api/auth/revoke` - Logout
- `GET /api/auth/me` - Get current user

### Forum API Endpoints
- `POST /question` - Create question

## Development Scripts

```bash
# Start development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run linter
npm run lint
```

## Testing the Application

### 1. Register a New User
1. Go to http://localhost:5173/login
2. Click "Sign up"
3. Enter email and password
4. Click "Create Account"

### 2. Create a Question
1. After login, you'll be redirected to `/questions`
2. Click "Ask Question"
3. Enter title and content
4. Click "Publish Question"

### 3. Logout
1. Click "Logout" in the header
2. You'll be redirected to login page

## Troubleshooting

### CORS Errors
Make sure CORS is enabled in both backend services. The following origins should be allowed:
- http://localhost:5173
- http://localhost:3000

### Connection Refused
- Verify User API is running on port 5002
- Verify Forum API is running on port 5000
- Check the console for specific error messages

### Authentication Issues
- Clear localStorage and try again
- Check browser console for error messages
- Verify JWT settings in backend configuration

## Backend CORS Configuration

CORS has been configured in both backend services to accept requests from the frontend:

**User/Program.cs & Forum/Program.cs:**
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});
```

## Next Steps

- [ ] Implement questions list display
- [ ] Add pagination for questions
- [ ] Implement answers functionality
- [ ] Add search and filtering
- [ ] Implement user profile page
- [ ] Add markdown support for questions/answers
- [ ] Add file attachments support
- [ ] Implement voting system
- [ ] Add notifications integration

## License

This project is part of the Forum application system.

