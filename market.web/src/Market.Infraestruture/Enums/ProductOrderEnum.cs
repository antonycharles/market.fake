using System.ComponentModel.DataAnnotations;

namespace Market.Infraestruture.Enums
{
    public enum ProductOrderEnum
    {
        [Display(Name = "Name Ascending")]
        NameAsc = 1,
        [Display(Name = "Name Descending")]
        NameDesc = 2,
        [Display(Name = "Created At Ascending")]
        CreatedAtAsc = 3,
        [Display(Name = "Created At Descending")]
        CreatedAtDesc = 4,
        [Display(Name = "Price Ascending")]
        PriceAsc = 5,
        [Display(Name = "Price Descending")]
        PriceDesc = 6,
        [Display(Name = "Best Selling Ascending")]
        BestSellingAsc = 7,
        [Display(Name = "Best Selling Descending")]
        BestSellingDesc = 8
    }
}