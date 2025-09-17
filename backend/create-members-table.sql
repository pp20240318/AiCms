-- Create Members table
CREATE TABLE IF NOT EXISTS "Members" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Members" PRIMARY KEY AUTOINCREMENT,
    "MemberCode" TEXT NOT NULL,
    "Name" TEXT NOT NULL,
    "Gender" TEXT,
    "DateOfBirth" TEXT,
    "IdNumber" TEXT,
    "Phone" TEXT,
    "Email" TEXT,
    "Address" TEXT,
    "MembershipType" TEXT NOT NULL DEFAULT 'Regular',
    "Status" TEXT NOT NULL DEFAULT 'Active',
    "JoinDate" TEXT NOT NULL,
    "ExpiryDate" TEXT,
    "Notes" TEXT,
    "Avatar" TEXT,
    "Occupation" TEXT,
    "Company" TEXT,
    "EmergencyContact" TEXT,
    "EmergencyPhone" TEXT,
    "Balance" REAL,
    "Points" INTEGER NOT NULL DEFAULT 0,
    "LastVisitDate" TEXT,
    "ReferralCode" TEXT,
    "ReferredBy" TEXT,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0
);

-- Create unique index on MemberCode
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Members_MemberCode" ON "Members" ("MemberCode");

-- Insert sample data
INSERT OR REPLACE INTO "Members" (
    "Id", "MemberCode", "Name", "Gender", "Phone", "Email",
    "MembershipType", "Status", "JoinDate", "Balance", "Points",
    "CreatedAt", "UpdatedAt", "IsDeleted"
) VALUES (
    1, 'M20250916001', '张三', '男', '13800138001', 'zhangsan@example.com',
    'VIP', 'Active', '2025-01-01', 1000.50, 2500,
    '2025-01-01 00:00:00', '2025-01-01 00:00:00', 0
);