CREATE TABLE "ProductCategory" (
    "Id" UUID PRIMARY KEY,
    "CreatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "UpdatedAt" TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT NOW(),
    "DeletedAt" TIMESTAMP WITHOUT TIME ZONE NULL,
    "ProductId" UUID NOT NULL,
    "CategoryId" UUID NOT NULL,
    "Order" INT NOT NULL DEFAULT 0,
    "Status" INT NOT NULL
);

ALTER TABLE "ProductCategory"
ADD CONSTRAINT "FK_ProductCategory_Product"
FOREIGN KEY ("ProductId") REFERENCES "Product"("Id");

ALTER TABLE "ProductCategory"
ADD CONSTRAINT "FK_ProductCategory_Category"
FOREIGN KEY ("CategoryId") REFERENCES "Category"("Id");
