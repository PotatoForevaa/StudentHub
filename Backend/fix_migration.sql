-- Remove the migration history entry if it exists
DELETE FROM "__EFMigrationsHistory" WHERE "MigrationId" = '20251208062330_AddProjectComments';

-- Create the ProjectComments table
CREATE TABLE IF NOT EXISTS "Business"."ProjectComments" (
    "Id" uuid NOT NULL,
    "AuthorId" uuid NOT NULL,
    "ProjectId" uuid NOT NULL,
    "Content" text NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_ProjectComments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProjectComments_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Business"."Projects" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProjectComments_Users_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Business"."Users" ("Id") ON DELETE CASCADE
);

-- Create indexes
CREATE INDEX IF NOT EXISTS "IX_ProjectComments_ProjectId" ON "Business"."ProjectComments" ("ProjectId");
CREATE INDEX IF NOT EXISTS "IX_ProjectComments_AuthorId" ON "Business"."ProjectComments" ("AuthorId");

-- Re-insert the migration history entry
INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251208062330_AddProjectComments', '9.0.10')
ON CONFLICT ("MigrationId") DO NOTHING;

