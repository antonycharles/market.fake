using Market.Domain.Entities;
using Market.Domain.Interfaces;
using Market.Infrastructure.Data;

namespace Market.Infrastructure.Repositories
{
    public class ProductPhotoRepository : CrudRepository<ProductPhoto>, IProductPhotoRepository
    {
        public ProductPhotoRepository(IDbConnectionFactory connectionFactory) : base(connectionFactory)
        {
        }

        protected override string TableName => "ProductPhoto";
        protected override string SelectColumns => @"""Id"", ""ProductId"", ""FileId"", ""Url"", ""Description"", ""Order"", ""Type"", ""CreatedAt"", ""UpdatedAt"", ""DeletedAt"", ""Status""";
        protected override string InsertColumns => @"""Id"", ""ProductId"", ""FileId"", ""Url"", ""Description"", ""Order"", ""Type"", ""CreatedAt"", ""UpdatedAt"", ""Status""";
        protected override string InsertValues => @"@Id, @ProductId, @FileId, @Url, @Description, @Order, @Type, @CreatedAt, @UpdatedAt, @Status";
        protected override string UpdateAssignments => @"""ProductId"" = @ProductId, ""FileId"" = @FileId, ""Url"" = @Url, ""Description"" = @Description, ""Order"" = @Order, ""Type"" = @Type, ""Status"" = @Status, ""UpdatedAt"" = @UpdatedAt";
    }
}
