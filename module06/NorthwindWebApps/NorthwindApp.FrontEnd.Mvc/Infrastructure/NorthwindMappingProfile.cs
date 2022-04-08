using System;
using AutoMapper;
using Northwind.Services.Products;
using NorthwindApp.FrontEnd.Mvc.Identity.Models;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Products;

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
        }
    }
}
