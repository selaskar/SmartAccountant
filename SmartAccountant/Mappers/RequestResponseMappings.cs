using AutoMapper;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;
using SmartAccountant.Models.Response;

namespace SmartAccountant.Mappers;

internal sealed class RequestResponseMappings : Profile
{
    public RequestResponseMappings()
    {
        CreateMap<IFormFile, ImportFile>()
            .ForMember(x => x.FileName, opt => opt.MapFrom(e => e.FileName))
            .ForMember(x => x.ContentType, opt => opt.MapFrom(e => e.ContentType))
            .ForMember(x => x.OpenReadStream, opt => opt.MapFrom(e => (Func<Stream>)e.OpenReadStream));


        CreateMap<AbstractUploadStatementRequest, AbstractStatementImportModel>()
            .ForMember(x => x.RequestId, opt => opt.MapFrom(e => e.RequestId))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.PeriodStart, opt => opt.MapFrom(e => e.PeriodStart))
            .ForMember(x => x.PeriodEnd, opt => opt.MapFrom(e => e.PeriodEnd))
            .ForMember(x => x.File, opt => opt.MapFrom(e => e.File));

        CreateMap<UploadDebitStatementRequest, DebitStatementImportModel>()
            .IncludeBase<AbstractUploadStatementRequest, AbstractStatementImportModel>();

        CreateMap<UploadCreditCardStatementRequest, CreditCardStatementImportModel>()
            .IncludeBase<AbstractUploadStatementRequest, AbstractStatementImportModel>()
            .ForMember(x => x.RolloverAmount, opt => opt.MapFrom(e => e.RolloverAmount))
            .ForMember(x => x.TotalDueAmount, opt => opt.MapFrom(e => e.TotalDueAmount))
            .ForMember(x => x.MinimumDueAmount, opt => opt.MapFrom(e => e.MinimumDueAmount))
            .ForMember(x => x.TotalFees, opt => opt.MapFrom(e => e.TotalFees))
            .ForMember(x => x.DueDate, opt => opt.MapFrom(e => e.DueDate));


        CreateMap<Statement, UploadStatementResponse>()
            .ForMember(x => x.StatementId, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.RequestId, opt => opt.Ignore());
    }
}
