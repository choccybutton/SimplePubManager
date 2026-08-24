# SDD ledger — plan: docs/superpowers/plans/2026-08-24-simplepubmanager-implementation.md

## Setup
- Workspace: .superpowers/sdd/2026-08-24-simplepubmanager-implementation
- Spec: docs/superpowers/specs/2026-08-24-simplepubmanager-design.md
- Status: Setup complete, pre-flight scan pending

## Tasks
- [ ] Task 1: Initialize .NET Backend Project Structure
- [ ] Task 2: Create Domain Enums
- [ ] Task 3: Create Core Domain Entities
- [ ] Task 4: Create Shift-Related Domain Entities
- [ ] Task 5: Create Task, Holiday, and Payment Domain Entities
- [ ] Task 6: Create Device and Authentication Domain Entities
- [ ] Task 7: Create AppDbContext and Entity Configurations
- [ ] Task 8: Create Database Repositories
[...remaining tasks...]

## Pre-Flight Scan Results
[To be filled after scan]

## Task Execution

## Pre-Flight Scan

### Dependency Chain Analysis
Task 1 (Project Setup)
  → Task 2 (Domain Enums) — creates UserRole, ShiftType, ShiftStatus, etc.
  → Task 3 (Core Entities) — creates Organization, Area, User (depends on enums)
  → Task 4 (Shift Entities) — creates Shift, ShiftArea, ShiftLog, ShiftPayment (depends on Task 3)
  → Task 5 (Task/Holiday/Payment Entities) — creates Task, Holiday, Bill, Payment (depends on Task 3)
  → Task 6 (Device Entities) — creates Device, UserPin, DeviceSession (depends on Task 3)
  → Task 7 (DbContext) — aggregates all entities, depends on Tasks 3-6
  → Task 8+ (Repositories, Services, Controllers) — depend on Task 7

### Critical Conflicts Found

**CONFLICT 1: Forward References in User Entity Navigation**
- Location: Task 3, Step 2 (User.cs)
- Issue: User entity references Shift (Task 4), Holiday (Task 5), Models.Task (Task 5), UserPin (Task 6), DeviceSession (Task 6)
- Problem: These types don't exist when Task 3 compiles, causing `CS0246: The type or namespace name 'Shift' could not be found`
- Impact: Task 3 Step 4 build will fail

**RULING: Navigation Property Deferral**
- Task 3 User entity will include ONLY Organization navigation (already defined in Task 3)
- Defer other User navigations:
  - Task 4: Add Shift navigation to User
  - Task 5: Add Holiday and Models.Task navigation to User
  - Task 6: Add UserPin and DeviceSession navigation to User
- This prevents forward-reference errors while maintaining the same final schema
- Cost if wrong: User entity needs to be partially assembled across tasks, but EF Core handles this well

---

### File Path Consistency
✓ All Task 1 paths create vs. modify: consistent
✓ Task 2 enum paths: no conflicts
✓ Task 3-6 entities: no file collisions
✓ Task 3 Models namespace for Task to avoid collision: verified
✓ Entity configuration paths (Task 7): all unique

### Test Coverage Validation
✓ Task 1: Covers solution setup, no tests needed
✓ Task 2: Enums only, compile-time validation sufficient
✓ Task 3: Compilation verify step included
✓ All entities: Compilation checks included before commit

### Scan Complete
No blockers found. RULING recorded above.
Next: Dispatch Task 1 Implementer.

### Task 1: Initialize .NET Backend Project Structure
**Status:** Dispatched (implementer: ad1d5434586c152f3)
**Dispatch time:** 2026-08-24 15:15:48
**Report file:** task-1-report.md
**Base commit:** 1b0bbdd1f2e16a673988b2c05b4175a2bd9661e5

Waiting for completion...

**Status:** CANCELLED — MediatR licensing concern raised
**Decision:** Remove MediatR dependency. Implement CQRS handlers manually with DI.
**Action:** Re-dispatching Task 1 without MediatR package.

---

### Task 1 (Revised): Initialize .NET Backend Project Structure
**Status:** Dispatched (revised, without MediatR)
**Dispatch time:** 2026-08-24 16:14:03

Waiting for completion...
