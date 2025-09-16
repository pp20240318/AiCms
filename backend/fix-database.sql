-- Create Contacts table if it doesn't exist
CREATE TABLE IF NOT EXISTS "Contacts" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Contacts" PRIMARY KEY AUTOINCREMENT,
    "Name" TEXT NOT NULL CHECK(length("Name") <= 50),
    "Email" TEXT NOT NULL CHECK(length("Email") <= 100),
    "Phone" TEXT NULL CHECK(length("Phone") <= 20),
    "Company" TEXT NULL CHECK(length("Company") <= 100),
    "Subject" TEXT NOT NULL CHECK(length("Subject") <= 200),
    "Message" TEXT NOT NULL,
    "Status" INTEGER NOT NULL,
    "Reply" TEXT NULL,
    "RepliedAt" TEXT NULL,
    "RepliedById" INTEGER NULL,
    "IpAddress" TEXT NULL CHECK(length("IpAddress") <= 50),
    "UserAgent" TEXT NULL CHECK(length("UserAgent") <= 500),
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL
);

-- Create Pages table if it doesn't exist
CREATE TABLE IF NOT EXISTS "Pages" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Pages" PRIMARY KEY AUTOINCREMENT,
    "Title" TEXT NOT NULL CHECK(length("Title") <= 100),
    "Slug" TEXT NOT NULL CHECK(length("Slug") <= 200),
    "Content" TEXT NOT NULL,
    "Excerpt" TEXT NULL CHECK(length("Excerpt") <= 500),
    "MetaTitle" TEXT NULL CHECK(length("MetaTitle") <= 100),
    "MetaDescription" TEXT NULL CHECK(length("MetaDescription") <= 300),
    "MetaKeywords" TEXT NULL CHECK(length("MetaKeywords") <= 200),
    "FeaturedImage" TEXT NULL CHECK(length("FeaturedImage") <= 500),
    "Status" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "Template" TEXT NULL CHECK(length("Template") <= 100),
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "CreatedById" INTEGER NOT NULL,
    "UpdatedById" INTEGER NULL,
    "IsDeleted" INTEGER NOT NULL
);

-- Create WebsiteConfigs table if it doesn't exist
CREATE TABLE IF NOT EXISTS "WebsiteConfigs" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_WebsiteConfigs" PRIMARY KEY AUTOINCREMENT,
    "Key" TEXT NOT NULL CHECK(length("Key") <= 100),
    "Value" TEXT NULL,
    "Description" TEXT NULL CHECK(length("Description") <= 200),
    "Group" TEXT NOT NULL CHECK(length("Group") <= 50),
    "DataType" TEXT NOT NULL CHECK(length("DataType") <= 20),
    "IsPublic" INTEGER NOT NULL,
    "SortOrder" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    "IsDeleted" INTEGER NOT NULL
);

-- Mark migrations as applied
INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES ('20250910025449_InitialCreate', '8.0.0');
INSERT OR IGNORE INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion") VALUES ('20250915070314_AddWebsiteFeaturesOnly', '8.0.0');