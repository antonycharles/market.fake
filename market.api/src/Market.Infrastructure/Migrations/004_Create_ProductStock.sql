CREATE TABLE "ProductStock" (
    "Id" UUID PRIMARY KEY,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITHOUT TIME ZONE NULL,
    "ProductId" UUID NOT NULL,
    "AvailableStock" BIGINT NOT NULL DEFAULT 0,
    "ReservedStock" BIGINT NOT NULL DEFAULT 0,
    "SoldStock" BIGINT NOT NULL DEFAULT 0,
    "Status" INT NOT NULL
);

ALTER TABLE "ProductStock"
ADD CONSTRAINT "FK_ProductStock_Product"
FOREIGN KEY ("ProductId") REFERENCES "Product"("Id");

CREATE UNIQUE INDEX "UX_ProductStock_ProductId_Active"
ON "ProductStock" ("ProductId")
WHERE "DeletedAt" IS NULL;
