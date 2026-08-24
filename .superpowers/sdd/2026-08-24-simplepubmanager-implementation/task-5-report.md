# Task 5 Implementation Report: Domain Entities

## Status: DONE

## Summary
Successfully created all 5 domain entities (Task, Holiday, Bill, Payment, RecurringTaskTemplate) and updated the User entity with navigation properties for Holiday and AssignedTasks.

## Entities Created

### 1. Task.cs (in Models subdirectory)
- **Path**: `src/SimplePubManager.Domain/Entities/Models/Task.cs`
- **Namespace**: `SimplePubManager.Domain.Entities.Models`
- **Properties**:
  - `Id` (Guid): Unique identifier
  - `OrganizationId` (Guid): Foreign key to Organization
  - `Title` (string): Task title
  - `Description` (string?): Optional task description
  - `AssignedToUserId` (Guid?): Optional FK to assigned User
  - `AssignedToAreaId` (Guid?): Optional FK to assigned Area
  - `DueDate` (DateTime): Task due date
  - `Status` (TaskStatus enum): Current status (Pending, InProgress, Completed, Cancelled)
  - `CompletedBy` (Guid?): Optional FK to completing User
  - `CompletedAt` (DateTime?): Optional completion timestamp
  - `CompletionNotes` (string?): Optional completion notes
  - `CreatedAt` (DateTime): Creation timestamp
- **Navigations**: Organization, AssignedUser, AssignedArea, CompletedByUser
- **Note**: Uses `Enums.TaskStatus` to avoid ambiguity with System.Threading.Tasks.TaskStatus

### 2. RecurringTaskTemplate.cs
- **Path**: `src/SimplePubManager.Domain/Entities/RecurringTaskTemplate.cs`
- **Properties**:
  - `Id`, `OrganizationId`, `Title`, `Description`
  - `AssignedToUserId`, `AssignedToAreaId`
  - `RecurrencePattern` (RecurrencePattern enum)
  - `NextOccurrenceDate` (DateTime)
  - `Active` (bool)
  - `CreatedAt` (DateTime)
- **Navigations**: Organization, AssignedUser, AssignedArea

### 3. Holiday.cs
- **Path**: `src/SimplePubManager.Domain/Entities/Holiday.cs`
- **Properties**:
  - `Id`, `OrganizationId`
  - `StaffId` (Guid): FK to staff member
  - `StartDate`, `EndDate` (DateTime)
  - `Type` (string): "paid" or "unpaid"
  - `Status` (HolidayStatus enum)
  - `RequestedAt` (DateTime)
  - `ApprovedBy` (Guid?): Optional FK to approver
  - `ApprovedAt` (DateTime?)
- **Navigations**: Staff (User), Organization, ApprovedByUser (User?)

### 4. Bill.cs
- **Path**: `src/SimplePubManager.Domain/Entities/Bill.cs`
- **Properties**:
  - `Id`, `OrganizationId`
  - `Description` (string)
  - `Amount` (decimal)
  - `DueDate` (DateTime)
  - `PaidDate` (DateTime?)
  - `Status` (BillStatus enum)
  - `CreatedAt` (DateTime)
- **Navigations**: Organization

### 5. Payment.cs
- **Path**: `src/SimplePubManager.Domain/Entities/Payment.cs`
- **Properties**:
  - `Id`, `OrganizationId`
  - `StaffId` (Guid?): Optional FK to staff
  - `Amount` (decimal)
  - `Type` (PaymentType enum)
  - `RelatedShiftId` (Guid?): Optional FK to shift
  - `Date` (DateTime)
  - `CreatedAt` (DateTime)
- **Navigations**: Organization, Staff (User?), RelatedShift (Shift?)

## User Entity Updates

Added two navigation collections to `User.cs`:
- `public ICollection<Holiday> Holidays` - for holidays requested by the user
- `public ICollection<Models.Task> AssignedTasks` - for tasks assigned to the user

Both initialized with `new List<T>()`.

## Models Namespace

Task.cs successfully moved to `src/SimplePubManager.Domain/Entities/Models/` with namespace `SimplePubManager.Domain.Entities.Models` to avoid conflicts with `System.Task`.

## Build Result

**Status**: SUCCESS (0 errors, 0 warnings)

Build output:
```
SimplePubManager.Domain -> C:\Dev\repos\SimplePubManager\src\SimplePubManager.Domain\bin\Debug\net8.0\SimplePubManager.Domain.dll

Build succeeded.
    0 Warning(s)
    0 Error(s)
```

## Commit Information

- **Commit Hash**: 193b4cd
- **Message**: `feat: create task, holiday, and payment domain entities`
- **Files Changed**: 7
  - Created: `src/SimplePubManager.Domain/Entities/Models/Task.cs`
  - Deleted: `src/SimplePubManager.Domain/Entities/Task.cs` (moved to Models)
  - Modified: Bill.cs, Holiday.cs, Payment.cs, RecurringTaskTemplate.cs, User.cs

## Concerns

None. All entities have been created with the required properties and navigations. All enums (TaskStatus, HolidayStatus, RecurrencePattern, PaymentType, BillStatus) were already present in the project and are properly referenced. The TaskStatus ambiguity with System.Threading.Tasks was resolved using the fully qualified `Enums.TaskStatus` type reference.
