ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "BanReason" character varying(500);
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "IsVerified" boolean NOT NULL DEFAULT false;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "LastLoginAt" timestamp with time zone;
ALTER TABLE "Users" ADD COLUMN IF NOT EXISTS "SuspensionReason" character varying(500);
UPDATE "Users" SET "Status" = 'Active' WHERE "Status" = '' OR "Status" IS NULL;

CREATE TABLE IF NOT EXISTS "KycDocuments" (
    "Id" uuid NOT NULL,
    "UserId" uuid NOT NULL,
    "Status" text NOT NULL,
    "DocumentType" character varying(50),
    "RejectionReason" character varying(500),
    "SubmittedAt" timestamp with time zone NOT NULL,
    "ReviewedAt" timestamp with time zone,
    CONSTRAINT "PK_KycDocuments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_KycDocuments_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "KycDocumentImages" (
    "Id" uuid NOT NULL,
    "KycDocumentId" uuid NOT NULL,
    "ImageUrl" character varying(500) NOT NULL,
    "Label" character varying(100),
    CONSTRAINT "PK_KycDocumentImages" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_KycDocumentImages_KycDocuments_KycDocumentId" FOREIGN KEY ("KycDocumentId") REFERENCES "KycDocuments"("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_KycDocuments_UserId" ON "KycDocuments" ("UserId");
CREATE INDEX IF NOT EXISTS "IX_KycDocumentImages_KycDocumentId" ON "KycDocumentImages" ("KycDocumentId");
