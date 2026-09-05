CREATE TABLE "Organizations" (
    "Id" uuid NOT NULL,
    "Name" character varying(255) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Organizations" PRIMARY KEY ("Id")
);


CREATE TABLE "Areas" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Description" character varying(500),
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Areas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Areas_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Bills" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "Description" character varying(500) NOT NULL,
    "Amount" numeric(10,2) NOT NULL,
    "DueDate" timestamp with time zone NOT NULL,
    "PaidDate" timestamp with time zone,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Bills" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Bills_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Devices" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "DeviceId" character varying(255) NOT NULL,
    "DeviceKeyHash" text NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Enabled" boolean NOT NULL,
    "Location" character varying(500),
    "LastLocationUpdate" timestamp with time zone,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Devices" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Devices_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Users" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "Name" character varying(255) NOT NULL,
    "Email" character varying(255) NOT NULL,
    "PasswordHash" text NOT NULL,
    "Role" integer NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Users_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE
);


CREATE TABLE "DeviceSessions" (
    "Id" uuid NOT NULL,
    "DeviceId" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "SessionToken" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "LastActivityAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_DeviceSessions" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_DeviceSessions_Devices_DeviceId" FOREIGN KEY ("DeviceId") REFERENCES "Devices" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_DeviceSessions_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Holidays" (
    "Id" uuid NOT NULL,
    "StaffId" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "Type" character varying(50) NOT NULL,
    "Status" integer NOT NULL,
    "RequestedAt" timestamp with time zone NOT NULL,
    "ApprovedBy" uuid,
    "ApprovedAt" timestamp with time zone,
    CONSTRAINT "PK_Holidays" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Holidays_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Holidays_Users_ApprovedBy" FOREIGN KEY ("ApprovedBy") REFERENCES "Users" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Holidays_Users_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE TABLE "RecurringTaskTemplates" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "Title" character varying(255) NOT NULL,
    "Description" character varying(2000),
    "AssignedToUserId" uuid,
    "AssignedToAreaId" uuid,
    "RecurrencePattern" integer NOT NULL,
    "NextOccurrenceDate" timestamp with time zone NOT NULL,
    "Active" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_RecurringTaskTemplates" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_RecurringTaskTemplates_Areas_AssignedToAreaId" FOREIGN KEY ("AssignedToAreaId") REFERENCES "Areas" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_RecurringTaskTemplates_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_RecurringTaskTemplates_Users_AssignedToUserId" FOREIGN KEY ("AssignedToUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);


CREATE TABLE "Shifts" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "StaffId" uuid NOT NULL,
    "Type" integer NOT NULL,
    "StartTime" timestamp with time zone NOT NULL,
    "EndTime" timestamp with time zone,
    "Status" integer NOT NULL,
    "CreatedBy" uuid NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Shifts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Shifts_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Shifts_Users_CreatedBy" FOREIGN KEY ("CreatedBy") REFERENCES "Users" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Shifts_Users_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Tasks" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "Title" character varying(255) NOT NULL,
    "Description" character varying(2000),
    "AssignedToUserId" uuid,
    "AssignedToAreaId" uuid,
    "DueDate" timestamp with time zone NOT NULL,
    "Status" integer NOT NULL,
    "CompletedBy" uuid,
    "CompletedAt" timestamp with time zone,
    "CompletionNotes" character varying(2000),
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Tasks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tasks_Areas_AssignedToAreaId" FOREIGN KEY ("AssignedToAreaId") REFERENCES "Areas" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Tasks_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Tasks_Users_AssignedToUserId" FOREIGN KEY ("AssignedToUserId") REFERENCES "Users" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Tasks_Users_CompletedBy" FOREIGN KEY ("CompletedBy") REFERENCES "Users" ("Id") ON DELETE SET NULL
);


CREATE TABLE "UserPins" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "PinHash" text NOT NULL,
    "DeviceRestrictionId" uuid,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_UserPins" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_UserPins_Devices_DeviceRestrictionId" FOREIGN KEY ("DeviceRestrictionId") REFERENCES "Devices" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_UserPins_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);


CREATE TABLE "Payments" (
    "Id" uuid NOT NULL,
    "OrganizationId" uuid NOT NULL,
    "StaffId" uuid,
    "Amount" numeric(10,2) NOT NULL,
    "Type" integer NOT NULL,
    "RelatedShiftId" uuid,
    "Date" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Payments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Payments_Organizations_OrganizationId" FOREIGN KEY ("OrganizationId") REFERENCES "Organizations" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Payments_Shifts_RelatedShiftId" FOREIGN KEY ("RelatedShiftId") REFERENCES "Shifts" ("Id") ON DELETE SET NULL,
    CONSTRAINT "FK_Payments_Users_StaffId" FOREIGN KEY ("StaffId") REFERENCES "Users" ("Id") ON DELETE SET NULL
);


CREATE TABLE "ShiftAreas" (
    "ShiftId" uuid NOT NULL,
    "AreaId" uuid NOT NULL,
    "AssignedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ShiftAreas" PRIMARY KEY ("ShiftId", "AreaId"),
    CONSTRAINT "FK_ShiftAreas_Areas_AreaId" FOREIGN KEY ("AreaId") REFERENCES "Areas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ShiftAreas_Shifts_ShiftId" FOREIGN KEY ("ShiftId") REFERENCES "Shifts" ("Id") ON DELETE CASCADE
);


CREATE TABLE "ShiftLogs" (
    "Id" uuid NOT NULL,
    "ShiftId" uuid NOT NULL,
    "ClockInTime" timestamp with time zone NOT NULL,
    "ClockOutTime" timestamp with time zone,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ShiftLogs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ShiftLogs_Shifts_ShiftId" FOREIGN KEY ("ShiftId") REFERENCES "Shifts" ("Id") ON DELETE CASCADE
);


CREATE TABLE "ShiftPayments" (
    "Id" uuid NOT NULL,
    "ShiftId" uuid NOT NULL,
    "HourlyRate" numeric(10,2) NOT NULL,
    "HoursWorked" numeric(10,2) NOT NULL,
    "Amount" numeric(10,2) NOT NULL,
    "Status" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ShiftPayments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ShiftPayments_Shifts_ShiftId" FOREIGN KEY ("ShiftId") REFERENCES "Shifts" ("Id") ON DELETE CASCADE
);


CREATE INDEX "IX_Areas_OrganizationId" ON "Areas" ("OrganizationId");


CREATE UNIQUE INDEX "IX_Areas_OrganizationId_Name" ON "Areas" ("OrganizationId", "Name");


CREATE INDEX "IX_Bills_DueDate" ON "Bills" ("DueDate");


CREATE INDEX "IX_Bills_OrganizationId" ON "Bills" ("OrganizationId");


CREATE INDEX "IX_Bills_Status" ON "Bills" ("Status");


CREATE INDEX "IX_Devices_Enabled" ON "Devices" ("Enabled");


CREATE UNIQUE INDEX "IX_Devices_OrganizationId_DeviceId" ON "Devices" ("OrganizationId", "DeviceId");


CREATE INDEX "IX_DeviceSessions_DeviceId" ON "DeviceSessions" ("DeviceId");


CREATE INDEX "IX_DeviceSessions_ExpiresAt" ON "DeviceSessions" ("ExpiresAt");


CREATE INDEX "IX_DeviceSessions_LastActivityAt" ON "DeviceSessions" ("LastActivityAt");


CREATE INDEX "IX_DeviceSessions_UserId" ON "DeviceSessions" ("UserId");


CREATE INDEX "IX_Holidays_ApprovedBy" ON "Holidays" ("ApprovedBy");


CREATE INDEX "IX_Holidays_OrganizationId" ON "Holidays" ("OrganizationId");


CREATE INDEX "IX_Holidays_StaffId" ON "Holidays" ("StaffId");


CREATE INDEX "IX_Holidays_StaffId_StartDate" ON "Holidays" ("StaffId", "StartDate");


CREATE INDEX "IX_Holidays_Status" ON "Holidays" ("Status");


CREATE INDEX "IX_Organizations_CreatedAt" ON "Organizations" ("CreatedAt");


CREATE INDEX "IX_Payments_Date" ON "Payments" ("Date");


CREATE INDEX "IX_Payments_OrganizationId" ON "Payments" ("OrganizationId");


CREATE INDEX "IX_Payments_RelatedShiftId" ON "Payments" ("RelatedShiftId");


CREATE INDEX "IX_Payments_StaffId" ON "Payments" ("StaffId");


CREATE INDEX "IX_Payments_Type" ON "Payments" ("Type");


CREATE INDEX "IX_RecurringTaskTemplates_Active" ON "RecurringTaskTemplates" ("Active");


CREATE INDEX "IX_RecurringTaskTemplates_AssignedToAreaId" ON "RecurringTaskTemplates" ("AssignedToAreaId");


CREATE INDEX "IX_RecurringTaskTemplates_AssignedToUserId" ON "RecurringTaskTemplates" ("AssignedToUserId");


CREATE INDEX "IX_RecurringTaskTemplates_NextOccurrenceDate" ON "RecurringTaskTemplates" ("NextOccurrenceDate");


CREATE INDEX "IX_RecurringTaskTemplates_OrganizationId" ON "RecurringTaskTemplates" ("OrganizationId");


CREATE INDEX "IX_ShiftAreas_AreaId" ON "ShiftAreas" ("AreaId");


CREATE INDEX "IX_ShiftLogs_ShiftId" ON "ShiftLogs" ("ShiftId");


CREATE INDEX "IX_ShiftLogs_Status" ON "ShiftLogs" ("Status");


CREATE UNIQUE INDEX "IX_ShiftPayments_ShiftId" ON "ShiftPayments" ("ShiftId");


CREATE INDEX "IX_ShiftPayments_Status" ON "ShiftPayments" ("Status");


CREATE INDEX "IX_Shifts_CreatedBy" ON "Shifts" ("CreatedBy");


CREATE INDEX "IX_Shifts_OrganizationId_StartTime" ON "Shifts" ("OrganizationId", "StartTime");


CREATE INDEX "IX_Shifts_OrganizationId_Status" ON "Shifts" ("OrganizationId", "Status");


CREATE INDEX "IX_Shifts_StaffId" ON "Shifts" ("StaffId");


CREATE INDEX "IX_Shifts_Status" ON "Shifts" ("Status");


CREATE INDEX "IX_Tasks_AssignedToAreaId" ON "Tasks" ("AssignedToAreaId");


CREATE INDEX "IX_Tasks_AssignedToUserId" ON "Tasks" ("AssignedToUserId");


CREATE INDEX "IX_Tasks_CompletedBy" ON "Tasks" ("CompletedBy");


CREATE INDEX "IX_Tasks_DueDate" ON "Tasks" ("DueDate");


CREATE INDEX "IX_Tasks_OrganizationId" ON "Tasks" ("OrganizationId");


CREATE INDEX "IX_Tasks_Status" ON "Tasks" ("Status");


CREATE INDEX "IX_UserPins_DeviceRestrictionId" ON "UserPins" ("DeviceRestrictionId");


CREATE INDEX "IX_UserPins_UserId" ON "UserPins" ("UserId");


CREATE INDEX "IX_Users_OrganizationId" ON "Users" ("OrganizationId");


CREATE UNIQUE INDEX "IX_Users_OrganizationId_Email" ON "Users" ("OrganizationId", "Email");


CREATE INDEX "IX_Users_Status" ON "Users" ("Status");


