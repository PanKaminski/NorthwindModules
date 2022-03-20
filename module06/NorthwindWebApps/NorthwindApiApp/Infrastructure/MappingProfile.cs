using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Blogging;
using Northwind.Services.Entities;
using BlogArticle = Northwind.Services.Blogging.BlogArticle;

namespace NorthwindApiApp.Infrastructure
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProductCategoryTransferObject, Category>().ReverseMap();
            this.CreateMap<ProductTransferObject, Product>().ReverseMap();
            this.CreateMap<EmployeeTransferObject, Employee>().ReverseMap();

            this.CreateMap<Northwind.Services.EntityFrameworkCore.Blogging.Entities.BlogArticle, BlogArticle>()
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.EmployeeId))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.Posted, opt => opt.MapFrom(src => src.PublicationDate.ToString("O")))
                .ReverseMap();

            this.CreateMap<Northwind.Services.EntityFrameworkCore.Blogging.Entities.BlogComment, BlogComment>()
                .ReverseMap();
        }
    }
}