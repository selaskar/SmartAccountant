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
        CreateMap<UploadStatementRequest, ImportStatementModel>()
            .ForMember(x => x.RequestId, opt => opt.MapFrom(e => e.RequestId))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.PeriodStart, opt => opt.MapFrom(e => e.PeriodStart))
            .ForMember(x => x.PeriodEnd, opt => opt.MapFrom(e => e.PeriodEnd))
            .ForMember(x => x.File, opt => opt.MapFrom(e => e.File));

        CreateMap<IFormFile, ImportFile>()
            .ForMember(x => x.FileName, opt => opt.MapFrom(e => e.FileName))
            .ForMember(x => x.ContentType, opt => opt.MapFrom(e => e.ContentType))
            .ForMember(x => x.OpenReadStream, opt => opt.MapFrom(e => (Func<Stream>)e.OpenReadStream));

        CreateMap<Statement, UploadStatementResponse>()
            .ForMember(x => x.StatementId, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId));
    }
}
