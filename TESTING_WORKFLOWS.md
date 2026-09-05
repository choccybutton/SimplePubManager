# SimplePubManager - Testing Workflows Guide

Quick walkthroughs for testing major features after local setup.

## Pre-Flight Check

Before testing, verify:
1. PostgreSQL running: `psql --version`
2. API running: http://localhost:5000/api/v1/health (or similar)
3. Frontend running: http://localhost:3000 (should show login)
4. Database migrations applied

---

## Workflow 1: Complete Shift Management (5 minutes)

**Objective:** Test the full shift workflow: create → work → clock out → approve → pay

### As Staff Member

1. **Login**
   - URL: http://localhost:3000
   - Email: `staff@test.com`
   - Password: `TestPass123!`
   - Expected: Dashboard shows, shift auto-created in background

2. **Clock In**
   - Navigate to Shifts page
   - Find today's shift (status: Active)
   - Click "Clock In" button
   - Expected: Shows "Clocked In at HH:MM", button changes to "Clock Out"

3. **Simulate Work**
   - Wait 2-3 minutes (or manually edit if possible)
   - Keep app open

4. **Clock Out**
   - Click "Clock Out" button
   - Expected: Shows "Clocked Out at HH:MM", status changes to "Pending Approval"

### As Manager

5. **Review Pending Shifts**
   - Login as manager@test.com
   - Navigate to Shifts → Pending Approvals
   - Should see staff member's shift

6. **Approve Shift**
   - Click on the shift
   - Review clock-in/clock-out times
   - (Optional) Adjust times if needed
   - Click "Approve Shift"
   - Expected: Status changes to "Approved", payment calculated and displayed

7. **Verify Payment**
   - Navigate to Payments
   - Should see new payment record for staff
   - Amount = Hours Worked × Hourly Rate

### Expected Results
- ✅ Shift created
- ✅ Time tracked
- ✅ Approved by manager
- ✅ Payment calculated

---

## Workflow 2: Task Management (5 minutes)

**Objective:** Test task assignment and completion

### As Manager

1. **Create Task (Area-Based)**
   - Navigate to Tasks → Create New Task
   - Title: "Clean Kitchen"
   - Description: "Deep clean kitchen at end of shift"
   - Assign To: "Kitchen" (area)
   - Due Date: Today
   - Click "Create"
   - Expected: Task appears in task list with status "Pending"

2. **Create Task (Individual)**
   - Create another task
   - Title: "Prepare Report"
   - Assign To: "staff@test.com" (specific person)
   - Due Date: Today
   - Click "Create"

### As Staff Member

3. **View Assigned Tasks**
   - Login as staff@test.com
   - Navigate to Tasks
   - Should see both tasks (area-based + individual assignment)

4. **Complete Area-Based Task**
   - Click on "Clean Kitchen" task
   - Click "Complete Task"
   - Add completion notes: "Kitchen cleaned, ready for tomorrow"
   - Click "Submit"
   - Expected: Status changes to "Completed", timestamp recorded

5. **Complete Individual Task**
   - Click on "Prepare Report"
   - Click "Complete"
   - Status changes to "Completed"

### As Manager (Verification)

6. **Review Completed Tasks**
   - Navigate to Tasks → Completed
   - Should see both completed tasks
   - Verify completion notes are recorded

### Expected Results
- ✅ Tasks created (area-based and individual)
- ✅ Staff can see assigned tasks
- ✅ Tasks marked complete with notes
- ✅ Manager can view completion history

---

## Workflow 3: Holiday Management (5 minutes)

**Objective:** Test holiday request and approval workflow

### As Staff Member

1. **Request Holiday**
   - Login as staff@test.com
   - Navigate to Holidays → Request Holiday
   - Start Date: Tomorrow (or future date)
   - End Date: 3 days later
   - Type: "Paid"
   - Click "Submit"
   - Expected: Holiday appears with status "Pending"

2. **View Request**
   - Stay on Holidays page
   - Should see "Pending Approval" in yellow/warning color

### As Manager

3. **Review Pending Holidays**
   - Login as manager@test.com
   - Navigate to Holidays
   - Should see staff member's pending holiday request

4. **Approve Holiday**
   - Click on the pending holiday
   - Review dates and type
   - Click "Approve"
   - Expected: Status changes to "Approved" (green)

5. **Test Rejection** (Optional)
   - Create another holiday request as staff
   - As manager, click the pending holiday
   - Click "Reject"
   - Expected: Status changes to "Rejected" (red)

### Expected Results
- ✅ Staff can request holidays
- ✅ Manager can view pending requests
- ✅ Manager can approve/reject
- ✅ Status changes reflected in real-time

---

## Workflow 4: Shared Device Authentication (5 minutes)

**Objective:** Test PIN-based quick-swap on shared device

### Setup Shared Device Mode

1. **Register Device** (As Manager)
   - Navigate to Settings → Devices
   - Click "Register Device"
   - Device Name: "Bar-Tablet-1"
   - Click "Register"
   - Expected: Device ID and key generated

2. **Switch to Shared Device Mode**
   - (This requires device context switching in frontend)
   - Simulate: In frontend app, could add test mode to enable device session

### Test PIN Quick-Swap

3. **Staff Member 1 Logs In**
   - On "shared device", login as staff@test.com
   - Shift auto-created
   - Click "Clock In"

4. **Quick Swap to Different Staff**
   - (In shared device mode)
   - Click "Change User" (or similar)
   - Enter PIN: (configured for supervisor@test.com)
   - Expected: Session switches to supervisor without full login

5. **Verify Different User**
   - Dashboard should show supervisor's data
   - Can only access supervisor's shifts/tasks

### Expected Results
- ✅ Device registered successfully
- ✅ PIN quick-swap works
- ✅ Session isolation maintained
- ✅ Feature restrictions applied (no financial data)

---

## Workflow 5: Role-Based Access Control (5 minutes)

**Objective:** Verify permission restrictions work correctly

### Test Manager Permissions

1. **Login as Manager**
   - Email: manager@test.com
   - Should see all menu items
   - Can access: Dashboard, Shifts, Tasks, Holidays, Payments, Staff, Areas, Devices

### Test Supervisor Permissions

2. **Logout and Login as Supervisor**
   - Email: supervisor@test.com
   - Should see limited menu
   - Can access: Shifts, Tasks, Holidays
   - Cannot access: Staff management, Areas, Device management, Payments

3. **Verify Permission Denied**
   - Try to navigate directly to `/admin/payments` (if possible)
   - Should be redirected or show 403 error

### Test Staff Permissions

4. **Logout and Login as Staff**
   - Email: staff@test.com
   - Minimal menu: Dashboard, Shifts, Tasks, Holidays
   - Cannot see: Staff management, Payments, Device management

5. **Verify Data Isolation**
   - Cannot view other staff's data
   - Cannot approve shifts/holidays
   - Can only complete assigned tasks

### Expected Results
- ✅ Managers have full access
- ✅ Supervisors have limited access
- ✅ Staff have minimal access
- ✅ Unauthorized access returns errors

---

## Workflow 6: Error Handling & Validation (5 minutes)

**Objective:** Test error handling and user-friendly messages

### Test Input Validation

1. **Create Shift with Invalid Data**
   - Try to create shift with past date
   - Expected: Error message "Date cannot be in the past"

2. **Create Task Without Required Fields**
   - Try to save task without title
   - Expected: Error message "Title is required"

### Test Authentication Errors

3. **Invalid Login**
   - Enter wrong password
   - Expected: Error message "Invalid email or password"

4. **Expired Token** (Advanced)
   - Simulate token expiry (manual in dev tools)
   - Try to make API call
   - Expected: Redirected to login, message "Session expired"

### Test Conflict Errors

5. **Duplicate Holiday Dates**
   - Request holiday for dates already approved
   - Expected: Error message "Holiday conflict with existing request"

### Expected Results
- ✅ Validation errors are clear
- ✅ Auth errors direct to login
- ✅ Business logic errors prevent invalid operations
- ✅ All errors have user-friendly messages

---

## Integration Tests Verification

### Run All 60 Integration Tests

```bash
cd C:\Dev\repos\SimplePubManager
dotnet test tests/SimplePubManager.Api.Tests
```

Expected output:
```
Test Run Successful.
Total tests: 60
  Passed: 60
  Failed: 0
  Duration: ~30 seconds
```

### Test Categories

- **14 Authentication Tests** — Login, device auth, tokens
- **11 Shift Tests** — CRUD, clock in/out, approvals
- **12 Permission Tests** — Role-based access control
- **12 Device Tests** — Device registration, PIN auth
- **11 Data Tests** — Complex workflows, persistence

---

## Performance Testing (Optional)

### API Response Times

Test API response times:

```bash
# From terminal, time a request
curl -i -X GET http://localhost:5000/api/v1/organizations/{orgId}/shifts \
  -H "Authorization: Bearer {token}"
```

Expected: <200ms for simple queries, <500ms for complex queries

---

## Success Criteria

All workflows pass if:

- ✅ No errors in API console
- ✅ No errors in browser console
- ✅ All data persists to database
- ✅ Role-based access working
- ✅ 60/60 integration tests pass
- ✅ All major workflows complete end-to-end

---

## Troubleshooting During Testing

| Issue | Solution |
|-------|----------|
| 401 Unauthorized | Token expired or invalid. Re-login. |
| 403 Forbidden | Insufficient permissions for this action. Use different role. |
| 409 Conflict | Data conflict (duplicate, overlap). Change dates/data. |
| 500 Server Error | Check API logs in terminal for detailed error. |
| CORS Error | Verify API started with CORS middleware. Check appsettings. |
| Database Error | Verify PostgreSQL running, migrations applied. |

---

**Ready to test? Start with Workflow 1 (Shift Management) for the full experience!**
