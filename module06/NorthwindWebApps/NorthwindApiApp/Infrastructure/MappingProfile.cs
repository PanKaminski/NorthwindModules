using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Blogging;
using Northwind.Services.EntityFrameworkCore.Entities;
using Northwind.Services.Products;
using BlogArticle = Northwind.Services.Blogging.BlogArticle;
using Product = Northwind.Services.EntityFrameworkCore.Entities.Product;

namespace NorthwindApiApp.Infrastructure
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProductCategoryTransferObject, ProductCategory>().ReverseMap();
            this.CreateMap<ProductTransferObject, Northwind.Services.Products.Product>().ReverseMap();
            this.CreateMap<EmployeeTransferObject, Northwind.Services.Employees.Employee>().ReverseMap();

            this.CreateMap<Employee, Northwind.Services.Employees.Employee>()
                .ReverseMap();
            this.CreateMap<Category, ProductCategory>()
                .ReverseMap();
            this.CreateMap<Product, Northwind.Services.Products.Product>()
                .ReverseMap();

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