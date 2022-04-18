using System;
using AutoMapper;
using Northwind.Services.Blogging;
using Northwind.Services.Products;
using NorthwindApp.FrontEnd.Mvc.Identity.Models;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Account;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Blogging;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Customers;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;
using Customer = Northwind.Services.Customers.Customer;

namespace NorthwindApp.FrontEnd.Mvc.Infrastructure
{
    public class NorthwindMappingProfile : Profile
    {
        public NorthwindMappingProfile()
        {
            this.CreateMap<ProductCategory, CategoryResponseViewModel>()
                .ForMember(dest => dest.HasImage, opt =>
                    opt.MapFrom(src => src.Picture != null && src.Picture.Length != 0));
            this.CreateMap<CategoryInputViewModel, ProductCategory>();
            this.CreateMap<Product, ProductResponseViewModel>();
            this.CreateMap<ProductInputViewModel, Product>()
                .ForMember(dest => dest.Discontinued, opt =>
                opt.MapFrom(src => !string.IsNullOrWhiteSpace(src.ProductStatus)
                && src.ProductStatus.Equals("ProductStatus", StringComparison.InvariantCultureIgnoreCase)));
            this.CreateMap<BlogArticle, BlogArticleResponseViewModel>();
            this.CreateMap<BlogArticle, BlogArticleInfoViewModel>();
            this.CreateMap<BlogComment, BlogCommentResponseViewModel>();
            this.CreateMap<BlogArticleInputViewModel, BlogArticle>();
            this.CreateMap<BlogCommentInputViewModel, BlogComment>();
            this.CreateMap<Customer, CustomerResponseViewModel>();
            this.CreateMap<User, UserResponseViewModel>()
                .ForMember(dest => dest.Role, opt =>
                    opt.MapFrom(src => src.Role.Name));
        }
    }
}
