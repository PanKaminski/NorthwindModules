using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Entities;

namespace Northwind.Services.DataAccess.Infrastructure
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProductCategoryTransferObject, Category>().ReverseMap();
            this.CreateMap<ProductTransferObject, Product>().ReverseMap();
            this.CreateMap<EmployeeTransferObject, Employee>().ReverseMap();
        }
    }
}