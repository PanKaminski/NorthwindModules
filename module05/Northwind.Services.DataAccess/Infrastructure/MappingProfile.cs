using AutoMapper;
using Northwind.DataAccess.Employees;
using Northwind.DataAccess.Products;
using Northwind.Services.Employees;
using Northwind.Services.Products;

namespace Northwind.Services.DataAccess.Infrastructure
{
    public sealed class MappingProfile : Profile
    {
        public MappingProfile()
        {
            this.CreateMap<ProductCategoryTransferObject, ProductCategory>().ReverseMap();
            this.CreateMap<ProductTransferObject, Product>().ReverseMap();
            this.CreateMap<EmployeeTransferObject, Employee>().ReverseMap();
        }
    }
}