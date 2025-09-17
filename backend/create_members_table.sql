-- 创建Members表的SQL脚本
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
    "Balance" REAL DEFAULT 0,
    "Points" INTEGER NOT NULL DEFAULT 0,
    "LastVisitDate" TEXT,
    "ReferralCode" TEXT,
    "ReferredBy" TEXT,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT,
    "IsDeleted" INTEGER NOT NULL DEFAULT 0
);

-- 创建唯一索引
CREATE UNIQUE INDEX IF NOT EXISTS "IX_Members_MemberCode" ON "Members" ("MemberCode");

-- 插入一些示例数据
INSERT OR IGNORE INTO "Members" (
    "Id", "MemberCode", "Name", "Gender", "Phone", "Email",
    "MembershipType", "Status", "JoinDate", "Balance", "Points", "CreatedAt"
) VALUES
(1, 'M20250916001', '张三', '男', '13800138001', 'zhangsan@example.com',
 'VIP', 'Active', '2025-01-15', 1500.00, 2800, '2025-01-15 10:00:00'),
(2, 'M20250916002', '李四', '女', '13900139002', 'lisi@example.com',
 'Regular', 'Active', '2025-02-01', 300.50, 650, '2025-02-01 14:30:00'),
(3, 'M20250916003', '王五', '男', '13700137003', 'wangwu@example.com',
 'Diamond', 'Active', '2025-03-10', 5000.00, 8500, '2025-03-10 09:15:00');