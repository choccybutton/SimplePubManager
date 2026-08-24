# SimplePubManager System Design

**Date:** 2026-08-24  
**Project:** SimplePubManager  
**Version:** 1.0  
**Status:** Design Approved

---

## Overview

SimplePubManager is a web-based pub management application designed to handle core operational tasks: shift management (planning, timekeeping, payments), staff holidays, task management (including recurring tasks), and bill payment capture.

**Scope:** Single pub with 10-15 staff initially. Designed with SaaS potential for multi-pub expansion.

**Tech Stack:**
- Frontend: React web app
- Backend: C# .NET API (monolithic, vertically sliced)
- Database: PostgreSQL
- Future: React Native apps for iOS/Android

---

## System Architecture

### High-Level Design

```
┌─────────────────────────────────────────────────────────────────┐
│                    React Web Application                        │
│         (Authentication, UI, Role-based Views)                  │
└────────────────────────┬────────────────────────────────────────┘
                         │ (REST + JWT)
┌────────────────────────▼────────────────────────────────────────┐
│                  C# .NET API (Monolithic)                       │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ Presentation Layer: Controllers, Auth, Authorization     │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Application Layer: Use Cases, Handlers per Slice         │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Domain Layer: Models, Business Logic per Feature         │  │
│  ├──────────────────────────────────────────────────────────┤  │
│  │ Infrastructure: Database, Auth, Logging, Error Handling  │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────┬────────────────────────────────────────┘
                         │ (SQL)
┌────────────────────────▼────────────────────────────────────────┐
│                      PostgreSQL Database                        │
└─────────────────────────────────────────────────────────────────┘
```

### Architecture Principles

- **Clean Architecture:** Separation of concerns across layers (Domain, Application, Infrastructure, Presentation)
- **Vertical Slicing:** Each feature (Shifts, Tasks, Holidays, Payments) is a self-contained vertical slice with its own domain, use cases, and persistence logic
- **Monolithic but Modular:** Single deployment artifact, but organized to allow service extraction as SaaS grows
- **Separation of Concerns:** Shared infrastructure (auth, logging, DB) in core layers; feature-specific logic in slices

---

## Domain Model & Data Schema

### Organizational Entities

**Organization**
- `id` (UUID, primary key)
- `name` (string)
- `created_at` (timestamp)

**User (Staff Member)**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `name` (string)
- `email` (string, unique per org)
- `role` (enum: Manager, Supervisor, Staff)
- `status` (enum: active, inactive)
- `created_at` (timestamp)

**Area (Location/Department)**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `name` (string) - e.g., "Kitchen", "Bar", "Front of House", "Admin"
- `description` (text, optional)
- `created_at` (timestamp)

### Shift Management Entities

**Shift**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `staff_id` (FK to User)
- `type` (enum: planned, adhoc)
- `start_time` (timestamp)
- `end_time` (timestamp, nullable until clocked out)
- `status` (enum: active, pending_approval, approved, paid, cancelled)
- `created_at` (timestamp)
- `created_by` (FK to User, manager/supervisor who created it)

**ShiftArea (Junction Table)**
- `shift_id` (FK to Shift)
- `area_id` (FK to Area)
- `assigned_at` (timestamp)
- Primary key: (shift_id, area_id)

**ShiftLog (Time Tracking)**
- `id` (UUID, primary key)
- `shift_id` (FK to Shift)
- `clock_in_time` (timestamp)
- `clock_out_time` (timestamp, nullable)
- `status` (enum: clocked_in, clocked_out)
- `created_at` (timestamp)

**ShiftPayment**
- `id` (UUID, primary key)
- `shift_id` (FK to Shift)
- `hourly_rate` (decimal)
- `hours_worked` (decimal, calculated from ShiftLog)
- `amount` (decimal, calculated)
- `status` (enum: pending, approved, paid)
- `created_at` (timestamp)

### Holiday Management Entities

**Holiday**
- `id` (UUID, primary key)
- `staff_id` (FK to User)
- `organization_id` (FK to Organization)
- `start_date` (date)
- `end_date` (date)
- `type` (enum: paid, unpaid)
- `status` (enum: pending, approved, rejected)
- `requested_at` (timestamp)
- `approved_by` (FK to User, nullable)
- `approved_at` (timestamp, nullable)

### Task Management Entities

**Task**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `title` (string)
- `description` (text, optional)
- `assigned_to_user_id` (FK to User, nullable)
- `assigned_to_area_id` (FK to Area, nullable)
- `due_date` (date)
- `status` (enum: pending, in_progress, completed, cancelled)
- `completed_by` (FK to User, nullable)
- `completed_at` (timestamp, nullable)
- `completion_notes` (text, optional)
- `created_at` (timestamp)

**RecurringTaskTemplate**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `title` (string)
- `description` (text, optional)
- `assigned_to_user_id` (FK to User, nullable)
- `assigned_to_area_id` (FK to Area, nullable)
- `recurrence_pattern` (enum: daily, weekly, monthly)
- `next_occurrence_date` (date)
- `active` (boolean)
- `created_at` (timestamp)

### Payment & Billing Entities

**Bill**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `description` (string)
- `amount` (decimal)
- `due_date` (date)
- `paid_date` (date, nullable)
- `status` (enum: pending, paid)
- `created_at` (timestamp)

**Payment**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `staff_id` (FK to User, nullable)
- `amount` (decimal)
- `type` (enum: shift_payment, bonus, deduction)
- `related_shift_id` (FK to Shift, nullable)
- `date` (date)
- `created_at` (timestamp)

### Device & Authentication Entities

**Device (Shared Tablet/Kiosk)**
- `id` (UUID, primary key)
- `organization_id` (FK to Organization)
- `device_id` (string, unique)
- `device_key` (string, hashed)
- `name` (string) - human-readable name, e.g., "Bar Tablet 1"
- `enabled` (boolean)
- `location` (string, optional) - for geofencing
- `last_location_update` (timestamp, optional)
- `created_at` (timestamp)

**UserPin**
- `id` (UUID, primary key)
- `user_id` (FK to User)
- `pin_hash` (string, bcrypt hashed)
- `device_restriction_id` (FK to Device, nullable) - if set, PIN only works on this device
- `created_at` (timestamp)

**DeviceSession**
- `id` (UUID, primary key)
- `device_id` (FK to Device)
- `user_id` (FK to User)
- `session_token` (string, JWT)
- `created_at` (timestamp)
- `expires_at` (timestamp)
- `last_activity_at` (timestamp)

---

## Feature Slices

Each feature is organized as a vertical slice in Clean Architecture:

### Shift Management Slice

**Domain Models:**
- Shift, ShiftArea, ShiftLog, ShiftPayment

**Key Use Cases:**
- CreateShift (planned or ad-hoc)
- AutoCreateAdHocShift (triggered on staff login)
- ClockIn / ClockOut
- ApproveShift (manager verifies times)
- AdjustShiftAreas (manager updates areas during shift)
- CalculateShiftPayment (after approval)
- CancelShift

**Responsibilities:**
- Business logic: Validate clock-in/out times, prevent duplicate clock-ins, calculate hours worked
- Persistence: Store shifts, time logs, and payment records
- State management: Transition shifts through active → pending_approval → approved → paid

**API Endpoints:**
- POST /shifts (create planned/ad-hoc)
- GET /shifts (list with filters)
- GET /shifts/{id}
- PUT /shifts/{id} (update details)
- POST /shifts/{id}/clock-in
- POST /shifts/{id}/clock-out
- PUT /shifts/{id}/areas (manager adjusts areas)
- POST /shifts/{id}/approve (manager approval)
- DELETE /shifts/{id} (cancel)

### Task Management Slice

**Domain Models:**
- Task, RecurringTaskTemplate, Area

**Key Use Cases:**
- CreateTask (assigned to user or area)
- CompleteTask (with optional notes)
- GenerateRecurringTasks (daily scheduled job)
- AssignTaskToArea / AssignTaskToUser
- UpdateTaskStatus

**Responsibilities:**
- Business logic: Validate task assignment (if assigned to area, verify user has shift covering that area)
- Persistence: Store tasks and recurring templates
- Auto-generation: Recurring task templates generate Task instances

**API Endpoints:**
- POST /tasks (create)
- GET /tasks (list, filterable by area/assignee/status)
- GET /tasks/{id}
- PUT /tasks/{id} (update)
- POST /tasks/{id}/complete (mark complete with notes)

### Holiday Management Slice

**Domain Models:**
- Holiday, User

**Key Use Cases:**
- RequestHoliday (staff member requests)
- ApproveHoliday (manager/supervisor approves)
- RejectHoliday (manager/supervisor rejects)
- GetHolidaysForStaff (view staff holidays)

**Responsibilities:**
- Business logic: Validate date ranges, check for conflicts
- Persistence: Store holiday requests and approvals
- Audit: Track who approved and when

**API Endpoints:**
- POST /holidays (staff requests)
- GET /holidays (list, filterable by status)
- GET /holidays/{id}
- PUT /holidays/{id}/approve (manager approves)
- PUT /holidays/{id}/reject (manager rejects)

### Payment Capture Slice

**Domain Models:**
- Bill, Payment

**Key Use Cases:**
- RecordBill (manager records expense)
- RecordPayment (record payment received or made)
- CalculateShiftPayments (batch process after shift approvals)

**Responsibilities:**
- Business logic: Calculate total payments owed to staff
- Persistence: Store bills and payment records
- Reporting: Aggregate payment data

**API Endpoints:**
- POST /bills (create)
- GET /bills (list, filterable by status)
- GET /bills/{id}
- PUT /bills/{id} (update)
- POST /payments (record payment)
- GET /payments (list)

### Cross-Cutting Infrastructure

**Shared Concerns:**
- Authentication (JWT token generation, validation)
- Authorization (role-based access control)
- Logging (request/response logging)
- Error Handling (consistent error responses)
- Database Context (entity framework, transaction management)
- Middleware (auth headers, error catching)

---

## API Design

### Base URL
```
https://api.simplepubmanager.app/api/v1/organizations/{orgId}
```

### Authentication
- User login: Bearer token (JWT) in Authorization header
- Device login: Device credentials (device_id + device_key) with device session token

### Endpoints

#### Shifts
```
GET    /shifts                      - List shifts (filterable)
POST   /shifts                      - Create shift (planned or adhoc)
GET    /shifts/{id}                 - Get shift details
PUT    /shifts/{id}                 - Update shift
DELETE /shifts/{id}                 - Cancel shift
POST   /shifts/{id}/clock-in        - Clock in
POST   /shifts/{id}/clock-out       - Clock out
PUT    /shifts/{id}/areas           - Update areas during shift
POST   /shifts/{id}/approve         - Approve shift (manager)
```

#### Tasks
```
GET    /tasks                       - List tasks (filterable)
POST   /tasks                       - Create task
GET    /tasks/{id}                  - Get task details
PUT    /tasks/{id}                  - Update task
POST   /tasks/{id}/complete         - Mark task complete
```

#### Recurring Tasks
```
GET    /recurring-tasks             - List templates
POST   /recurring-tasks             - Create template
PUT    /recurring-tasks/{id}        - Update template
```

#### Holidays
```
GET    /holidays                    - List holidays
POST   /holidays                    - Request holiday
GET    /holidays/{id}               - Get holiday details
PUT    /holidays/{id}/approve       - Approve
PUT    /holidays/{id}/reject        - Reject
```

#### Bills & Payments
```
GET    /bills                       - List bills
POST   /bills                       - Create bill
GET    /bills/{id}                  - Get bill details
PUT    /bills/{id}                  - Update bill
GET    /payments                    - List payments
POST   /payments                    - Record payment
```

#### Staff & Areas
```
GET    /staff                       - List staff
POST   /staff                       - Add staff member
PUT    /staff/{id}                  - Update staff
GET    /areas                       - List areas
POST   /areas                       - Create area
PUT    /areas/{id}                  - Update area
```

#### Authentication & Devices
```
POST   /auth/login                  - User login (creates JWT)
POST   /auth/device-login           - Device authentication
POST   /auth/quick-swap             - PIN-based user swap (shared device)
POST   /devices/register            - Register shared device (manager)
GET    /devices                     - List devices
PUT    /devices/{id}                - Update device settings
```

### Request/Response Format

**Successful Response (200/201):**
```json
{
  "data": {
    "id": "uuid",
    "name": "Morning Shift",
    ...
  }
}
```

**List Response (200):**
```json
{
  "data": [
    { "id": "uuid", ... },
    { "id": "uuid", ... }
  ],
  "pagination": {
    "page": 1,
    "limit": 20,
    "total": 42,
    "total_pages": 3
  }
}
```

**Error Response (4xx/5xx):**
```json
{
  "error": {
    "code": "INVALID_CLOCK_IN_TIME",
    "message": "Clock-in time cannot be in the future",
    "details": {
      "submitted_time": "2026-08-24T15:00:00Z"
    }
  }
}
```

---

## Authentication & Authorization

### Authentication Strategy

**User Authentication:**
1. Staff logs in with email + password
2. API validates credentials, issues JWT token (24-hour expiry)
3. Frontend stores token securely (httpOnly cookie recommended)
4. All requests include token in Authorization header: `Bearer <token>`
5. Refresh tokens allow renewal without full re-login

**Device Authentication:**
1. Shared device authenticates with device_id + device_key
2. API validates credentials, issues device session token
3. Device stores token and uses for subsequent requests
4. Device session has separate expiry (e.g., 8 hours for all-day tablet)

**Ad-Hoc Shift Auto-Creation:**
- On staff login, system checks if active shift exists for user
- If none exists, creates new ad-hoc shift with current timestamp
- Staff can immediately clock in or the system auto-clocks them

### Authorization (Role-Based Access Control)

**Three Roles:**

**Manager**
- Full access to all features
- Create/edit/delete shifts (planned and ad-hoc)
- Approve shifts and holidays
- Manage staff, areas, devices
- View all payments and billing
- Cannot be restricted on shared devices

**Supervisor**
- Manage shifts (view, create, cancel)
- Approve shifts and holidays
- Manage tasks
- View staff information
- Restricted: Cannot edit payments or billing

**Staff**
- View own shifts and time logs
- Clock in/out
- View assigned tasks
- Request holidays
- Restricted: Cannot create/approve shifts, cannot view other staff data

### Shared Device Security

**Device Registration:**
- Devices must be registered by a Manager
- Each device gets unique device_id and device_key
- Devices can be enabled/disabled

**Quick User Swap (PIN-Based):**
- Staff set a 4-6 digit PIN during onboarding
- On shared device, staff enters PIN (not password) to switch users
- Device validates PIN against stored hash
- Each PIN swap creates a DeviceSession with limited permissions
- Sessions expire after 15 minutes of inactivity

**Restricted Features on Shared Devices:**
- Financial data (payments, billing) hidden
- Password changes blocked
- Personal settings unavailable
- Restricted to: view shifts, clock in/out, view tasks, request holidays
- Enforced at API layer (403 Forbidden) and frontend (hidden features)

**Future Enhancement: Geofencing**
- Device sends location on each PIN swap
- API validates device is within org's geo-boundary
- Prevents PIN swap outside of premises

---

## Data Flow & Error Handling

### Shift Approval Workflow

```
1. Staff logs in
   → System creates active ad-hoc shift (if none exists)
   → Returns shift_id to frontend

2. Staff works and performs shift operations
   → Clock in: ShiftLog entry created (clock_in_time set)
   → Clock out: ShiftLog entry updated (clock_out_time set)
   → Manager updates areas: ShiftArea records updated

3. Staff clocks out
   → Shift status: active → pending_approval
   → Shift awaits manager review

4. Manager reviews pending shifts
   → Validates times are reasonable
   → Can adjust start/end times if needed
   → Can update area assignments
   → Approves shift
   → Shift status: pending_approval → approved

5. Payment calculation (nightly batch job)
   → For all approved shifts in past 24 hours
   → Calculate hours_worked from ShiftLog
   → Calculate amount = hours_worked × hourly_rate
   → Create ShiftPayment record
   → Shift status: approved → paid

6. Payroll system (future)
   → Aggregates all payments for payroll run
```

### Error Handling

**Validation Errors (400 Bad Request):**
- Missing required fields
- Invalid data types or formats
- Example: `{ "error": { "code": "INVALID_EMAIL", "message": "Email format is invalid" } }`

**Authentication Errors (401 Unauthorized):**
- Invalid credentials
- Expired token
- Example: `{ "error": { "code": "INVALID_CREDENTIALS", "message": "Email or password is incorrect" } }`

**Authorization Errors (403 Forbidden):**
- User lacks permission for operation
- Shared device attempting restricted operation
- Example: `{ "error": { "code": "INSUFFICIENT_PERMISSIONS", "message": "Staff members cannot approve shifts" } }`

**Business Logic Errors (409 Conflict):**
- Operation violates business rules
- Example: Clock-out before clock-in
- Example: Duplicate shift assignment
- Example: Holiday dates conflict with existing holiday

**Not Found (404):**
- Resource doesn't exist
- Example: `{ "error": { "code": "SHIFT_NOT_FOUND", "message": "Shift with ID {id} not found" } }`

**Server Errors (500 Internal Server Error):**
- Unexpected failures, logged for investigation
- Client receives generic error message for security

### Concurrent Operation Handling

**Shift Updates:**
- Optimistic locking on Shift status changes to prevent race conditions
- Example: Two managers cannot simultaneously approve the same shift

**Recurring Task Generation:**
- Runs daily via scheduled job (not on-demand)
- Batch operation: prevents race conditions
- Next occurrence date advances atomically

**Payment Calculation:**
- Runs nightly as batch job
- Processes only approved shifts from past 24 hours
- Prevents duplicate calculations

---

## Testing Strategy

### Unit Tests

**Domain Logic:**
- Shift validation: Cannot clock out before clock-in, cannot clock-in future times
- Payment calculation: Hours × rate = amount
- Holiday conflicts: Overlapping dates rejected
- Task assignment: If assigned to area, validate staff has shift covering area

**Use Case Handlers:**
- ApproveShift: Updates status, creates audit log, validates shift is pending
- CompleteTask: Validates user has permission, updates status
- RequestHoliday: Validates date range, checks for conflicts

**Target:** 80%+ code coverage for domain and use cases

### Integration Tests

**Full Feature Flows:**
- Create shift → clock in → clock out → manager approves → payment calculated
- Create recurring task template → nightly job generates task → staff completes → task closed
- Request holiday → manager approves → user can view approved holiday
- Ad-hoc shift: staff logs in → shift auto-created → clocks in/out → manager approves

**Database Interactions:**
- Data persists correctly
- Foreign key constraints enforced
- Transactions rollback on error

**Role-Based Access:**
- Managers can approve, supervisors can view, staff cannot access
- Shared device sessions cannot access financial endpoints

### API Tests

**Endpoint Validation:**
- Correct HTTP status codes (200, 201, 400, 401, 403, 404, 409, 500)
- Response format matches spec
- Required fields present in response

**Error Scenarios:**
- Invalid input (malformed JSON, missing fields)
- Missing authentication header
- Expired tokens
- Insufficient permissions
- Resource not found
- Business logic violations

**Edge Cases:**
- Duplicate clock-ins (should fail with 409)
- Overlapping shift assignments
- Timezone handling in timestamps
- Concurrent approvals of same shift

### Frontend Tests

**Component Rendering:**
- Shift creation form renders for Manager/Supervisor
- Shift creation form hidden for Staff
- Financial data hidden on shared device sessions
- Task list filters by area for area-based assignments

**Authentication & Authorization:**
- Login form validation
- Token stored/retrieved correctly
- PIN entry on shared devices
- Logout clears session

**User Workflows:**
- Staff can clock in/out
- Manager can approve shifts with adjusted times
- Task completion form appears only for assigned user/area coverage

### Manual Testing Checklist

- [ ] Manager creates planned shift, assigns staff and areas
- [ ] Staff logs in, ad-hoc shift auto-created
- [ ] Staff clocks in/out with correct timestamps
- [ ] Manager reviews pending shifts, approves with optional time adjustments
- [ ] Manager updates shift areas mid-shift
- [ ] Payment calculated correctly after approval
- [ ] Recurring task template generates task daily
- [ ] Staff completes area-based task (task assigned to area, staff has shift covering area)
- [ ] Staff requests holiday, manager approves
- [ ] Shared device: device registers, staff uses PIN to swap users, financial data hidden
- [ ] Concurrent operations: Two managers attempt simultaneous approvals (one succeeds, one rejected)

### CI/CD

- Unit + integration tests run on every commit
- Code coverage reports generated
- API contract tests verify no breaking changes
- Automated deployment to staging on main branch

---

## Deployment & Infrastructure

### Initial Deployment

**Development:**
- Local PostgreSQL instance
- Local .NET development server
- React development server with hot reload

**Staging:**
- Cloud PostgreSQL (AWS RDS or Azure Database for PostgreSQL)
- .NET API deployed to cloud platform (AWS App Runner, Azure App Service, or similar)
- React frontend deployed to CDN (AWS CloudFront, Azure CDN, or similar)

**Production (Future):**
- Cloud PostgreSQL with automated backups
- .NET API with horizontal scaling
- Frontend CDN distribution
- API rate limiting and DDoS protection
- Monitoring and alerting

### Database

- **Engine:** PostgreSQL 14+
- **Backups:** Daily automated backups, 30-day retention
- **Indexes:** On frequently queried columns (date, status, user_id, organization_id)
- **Connection Pooling:** Via .NET Entity Framework

### Security

- HTTPS/TLS for all communication
- JWT tokens with HS256 algorithm
- Password hashing: bcrypt (rounds: 10+)
- PIN hashing: bcrypt
- SQL injection prevention via parameterized queries
- CORS configured for frontend domain
- Rate limiting on auth endpoints

---

## Future Considerations

### SaaS Multi-Pub Expansion

**Current Design Support:**
- Organization table enables multi-org data isolation
- All entities have organization_id foreign key
- Database per organization or shared database with org_id filtering both supported

**Transition to Services:**
- Shift Management → can extract to dedicated service
- Task Management → can extract to dedicated service
- Payments → can extract to dedicated service
- Shared infrastructure (auth, logging) remains central

### Mobile Apps

- React Native apps consume same API
- Shared device features may differ (no PIN quick-swap on personal phones)
- Offline-first capability can be added (sync on reconnect)

### Integrations

- **Payroll Systems:** Payment records exported for payroll processing
- **Calendars:** Shift calendar exported to Google Calendar / Outlook
- **Payment Gateways:** Bill payment via Stripe/PayPal (future)

### Advanced Features (Future)

- Staff skills/certifications (track who can work which areas)
- Shift swapping between staff
- Scheduling optimization
- Analytics and reporting dashboard
- SMS/email notifications for shift reminders
- Real-time geofencing enforcement for shared devices

---

## Success Criteria

✅ **MVP Ready When:**
- All five core features working (shifts, tasks, holidays, payments, devices)
- Role-based access enforced
- Shift approval workflow functional
- Shared device PIN-swap tested
- API fully tested (unit, integration, endpoint tests)
- Deployment to staging successful

---

## Glossary

- **Vertical Slice:** A feature that cuts through all architectural layers (domain, application, infrastructure)
- **Ad-Hoc Shift:** Unplanned shift created when staff arrives (auto-created on login)
- **Shift Approval:** Manager verification and authentication of shift times before payment
- **Quick Swap:** PIN-based user switching on shared devices without full login
- **Area:** Location/department within a pub (Kitchen, Bar, etc.)
- **DeviceSession:** Authentication state for a shared device

---

**Document maintained by:** Martin Button  
**Last updated:** 2026-08-24
