using AutoMapper;
using Northwind.Services.Entities;
using NorthwindApp.FrontEnd.Mvc.ViewModels.Categories;

namespace NorthwindApp.FrontEnd.Mvc.Infrastructure
{
    public class NorthwindMappingProfile : Profile
    {
        public NorthwindMappingProfile()
        {
            this.CreateMap<Category, CategoryResponseViewModel>();
            this.CreateMap<CategoryInputViewModel, Category>();
        }
    }
}
