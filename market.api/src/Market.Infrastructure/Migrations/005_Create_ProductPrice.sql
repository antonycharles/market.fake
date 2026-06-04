CREATE EXTENSION IF NOT EXISTS btree_gist;

CREATE TABLE "ProductPrice" (
    "Id" UUID PRIMARY KEY,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITHOUT TIME ZONE NULL,
    "ProductId" UUID NOT NULL,
    "OriginalPrice" NUMERIC(18, 2) NOT NULL,
    "SalePrice" NUMERIC(18, 2) NOT NULL,
    "Currency" VARCHAR(3) NOT NULL,
    "ValidFrom" TIMESTAMP WITHOUT TIME ZONE NOT NULL,
    "ValidTo" TIMESTAMP WITHOUT TIME ZONE NULL,
    "Status" INT NOT NULL,
    CONSTRAINT "CK_ProductPrice_ValidInterval" CHECK ("ValidTo" IS NULL OR "ValidTo" >= "ValidFrom")
);

ALTER TABLE "ProductPrice"
ADD CONSTRAINT "FK_ProductPrice_Product"
FOREIGN KEY ("ProductId") REFERENCES "Product"("Id");

ALTER TABLE "ProductPrice"
ADD CONSTRAINT "EX_ProductPrice_Product_ValidInterval_Active"
EXCLUDE USING gist (
    "ProductId" WITH =,
    tsrange("ValidFrom", COALESCE("ValidTo", 'infinity'::timestamp), '[]') WITH &&
)
WHERE ("DeletedAt" IS NULL);
