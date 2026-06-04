CREATE TABLE "ProductInformation" (
    "Id" UUID PRIMARY KEY,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITHOUT TIME ZONE NULL,
    "ProductId" UUID NOT NULL,
    "Type" INT NOT NULL,
    "Label" VARCHAR(255) NOT NULL,
    "Value" TEXT NOT NULL,
    "Order" INT NOT NULL DEFAULT 0,
    "Status" INT NOT NULL
);

ALTER TABLE "ProductInformation"
ADD CONSTRAINT "FK_ProductInformation_Product"
FOREIGN KEY ("ProductId") REFERENCES "Product"("Id");
