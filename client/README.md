# SimplePubManager Frontend

React 18 + TypeScript web application for SimplePubManager, a pub/restaurant management system.

## Features

- Shift management and time tracking
- Task assignment and completion
- Holiday request and approval workflow
- Payment and billing management
- Staff management (managers only)
- Area management (managers only)
- Shared device support with PIN-based authentication
- Role-based access control (Manager, Supervisor, Staff)

## Setup

### Prerequisites

- Node.js (v16 or higher)
- npm or yarn

### Installation

```bash
npm install
```

### Configuration

Copy `.env.example` to `.env` and update with your API URL:

```
REACT_APP_API_URL=http://localhost:5000/api/v1
REACT_APP_ORG_ID=your-organization-id
```

### Development

```bash
npm start
```

Runs the app in development mode. Open [http://localhost:3000](http://localhost:3000) to view it in the browser.

### Build

```bash
npm run build
```

Builds the app for production to the `build` folder.

## Project Structure

```
src/
├── components/       # React components
│   ├── Auth/        # Authentication components
│   ├── Common/      # Shared/common components
│   ├── Layout/      # Layout components
│   ├── Shifts/      # Shift management components
│   ├── Tasks/       # Task management components
│   ├── Holidays/    # Holiday management components
│   └── Payments/    # Payment/billing components
├── pages/           # Page components for routing
├── services/        # API and auth services
│   ├── api/        # API client methods
│   ├── auth/       # Authentication services
│   └── localStorage/ # Local storage utilities
├── hooks/           # Custom React hooks
├── context/         # React context providers
├── types/           # TypeScript types and interfaces
├── styles/          # CSS styles
└── App.tsx          # Main app component

```

## API Integration

The frontend communicates with the SimplePubManager API at the configured `REACT_APP_API_URL`.

### Authentication

- Email/password login for regular users
- PIN-based quick-swap for shared devices
- JWT token-based authentication
- Automatic token refresh on 401 responses

### Key API Services

- `authApi` - Authentication endpoints
- `shiftsApi` - Shift management endpoints
- `tasksApi` - Task management endpoints
- `holidaysApi` - Holiday request endpoints
- `paymentsApi` - Payment/billing endpoints
- `staffApi` - Staff management endpoints
- `areasApi` - Area management endpoints
- `devicesApi` - Shared device endpoints

## Context Providers

### AuthContext

Manages user authentication state and login/logout operations.

```typescript
const { user, token, login, logout, isAuthenticated } = useAuth();
```

### DeviceContext

Manages shared device session state and PIN-based quick-swap.

```typescript
const { deviceId, sessionToken, quickSwap, logout } = useDeviceSession();
```

## Custom Hooks

- `useAuth()` - Access authentication context
- `useDeviceSession()` - Access device session context
- `useApi<T>()` - Generic API hook for data fetching
- `useShifts()` - Shift data fetching and actions
- `useTasks()` - Task data fetching and actions
- `useHolidays()` - Holiday data fetching and actions

## Styling

The application uses inline styles for components with a focus on responsive design. CSS is minimal and located in `src/styles/index.css`.

Color scheme:
- Primary: #3498db (blue)
- Success: #27ae60 (green)
- Error: #e74c3c (red)
- Background: #f5f5f5 (light gray)

## Browser Support

Modern browsers supporting ES2020 and above.

## Environment Variables

- `REACT_APP_API_URL` - Backend API URL (default: http://localhost:5000/api/v1)
- `REACT_APP_ORG_ID` - Organization ID
- `REACT_APP_APP_NAME` - Application name (default: SimplePubManager)

## License

Proprietary - SimplePubManager
