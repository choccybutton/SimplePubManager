-- Seed Test Data for SimplePubManager

-- Insert Organization
INSERT INTO public."Organizations" ("Id", "Name", "CreatedAt")
VALUES ('00000000-0000-0000-0000-000000000001', 'Test Pub', NOW());

-- Insert Users (passwords hashed with bcrypt, work factor 10)
-- Password: TestPass123!
-- Hash: $2a$10$sZ/xmDLpVFN5/sMQNqZuBeKpLZ0H5SQfG5Pt5dXbVBmCT5GmIZfAK

INSERT INTO public."Users" ("Id", "OrganizationId", "Name", "Email", "PasswordHash", "Role", "Status", "CreatedAt")
VALUES
  ('00000000-0000-0000-0000-000000000011', '00000000-0000-0000-0000-000000000001', 'Manager User', 'manager@test.com', '$2a$10$sZ/xmDLpVFN5/sMQNqZuBeKpLZ0H5SQfG5Pt5dXbVBmCT5GmIZfAK', 0, 0, NOW()),
  ('00000000-0000-0000-0000-000000000012', '00000000-0000-0000-0000-000000000001', 'Supervisor User', 'supervisor@test.com', '$2a$10$sZ/xmDLpVFN5/sMQNqZuBeKpLZ0H5SQfG5Pt5dXbVBmCT5GmIZfAK', 1, 0, NOW()),
  ('00000000-0000-0000-0000-000000000013', '00000000-0000-0000-0000-000000000001', 'Staff Member 1', 'staff@test.com', '$2a$10$sZ/xmDLpVFN5/sMQNqZuBeKpLZ0H5SQfG5Pt5dXbVBmCT5GmIZfAK', 2, 0, NOW()),
  ('00000000-0000-0000-0000-000000000014', '00000000-0000-0000-0000-000000000001', 'Staff Member 2', 'staff2@test.com', '$2a$10$sZ/xmDLpVFN5/sMQNqZuBeKpLZ0H5SQfG5Pt5dXbVBmCT5GmIZfAK', 2, 0, NOW());

-- Insert Areas
INSERT INTO public."Areas" ("Id", "OrganizationId", "Name", "Description", "CreatedAt")
VALUES
  ('00000000-0000-0000-0000-000000000021', '00000000-0000-0000-0000-000000000001', 'Kitchen', 'Kitchen area - food preparation', NOW()),
  ('00000000-0000-0000-0000-000000000022', '00000000-0000-0000-0000-000000000001', 'Bar', 'Bar area - drink service', NOW()),
  ('00000000-0000-0000-0000-000000000023', '00000000-0000-0000-0000-000000000001', 'Front of House', 'Front of house area - customer service', NOW());

-- Verify inserted data
SELECT 'Organizations' as Table_Name, COUNT(*) as Count FROM public."Organizations"
UNION ALL
SELECT 'Users', COUNT(*) FROM public."Users"
UNION ALL
SELECT 'Areas', COUNT(*) FROM public."Areas";
