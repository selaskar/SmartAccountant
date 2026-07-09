using AutoMapper;
using SmartAccountant.Dtos.Request;
using SmartAccountant.Dtos.Response;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;

namespace SmartAccountant.API.Mappers;

internal sealed class RequestResponseMappings : Profile
{
    public RequestResponseMappings()
    {
        CreateMap<IFormFile, ImportFile>()
            .ForMember(x => x.FileName, opt => opt.MapFrom(e => e.FileName))
            .ForMember(x => x.ContentType, opt => opt.MapFrom(e => e.ContentType))
            .ForMember(x => x.OpenReadStream, opt => opt.MapFrom(e => (Func<Stream>)e.OpenReadStream));


        // request mappings
        CreateMap<AbstractUploadStatementRequest, AbstractStatementImportModel>()
            .ForMember(x => x.RequestId, opt => opt.MapFrom(e => e.RequestId))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
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

        CreateMap<UploadMultipartStatementRequest, MultipartStatementImportModel>()
            .IncludeBase<UploadCreditCardStatementRequest, CreditCardStatementImportModel>()
            .ForMember(x => x.DependentAccountId, opt => opt.MapFrom(e => e.DependentAccountId));


        // response mappings
        CreateMap<IStatement<Transaction>, UploadStatementResponse>()
            .ForMember(x => x.StatementId, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.RequestId, opt => opt.Ignore());


        CreateMap<IStatement<DebitTransaction>, UploadStatementResponse>()
            .IncludeBase<IStatement<Transaction>, UploadStatementResponse>();

        CreateMap<DebitStatement, UploadStatementResponse>()
            .IncludeBase<IStatement<DebitTransaction>, UploadStatementResponse>()
            .ForSourceMember(x => x.Currency, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.RemainingBalance, opt => opt.DoNotValidate());


        CreateMap<IStatement<CreditCardTransaction>, UploadStatementResponse>()
            .IncludeBase<IStatement<Transaction>, UploadStatementResponse>();

        CreateMap<AbstractCreditCardStatement<CreditCardTransaction>, UploadStatementResponse>()
            .IncludeBase<IStatement<CreditCardTransaction>, UploadStatementResponse>()
            .ForSourceMember(x => x.RolloverAmount, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.TotalPayments, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.TotalExpenses, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.TotalFees, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.TotalDueAmount, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.MinimumDueAmount, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.DueDate, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.RemainingLimit, opt => opt.DoNotValidate());

        CreateMap<CreditCardStatement, UploadStatementResponse>()
            .IncludeBase<AbstractCreditCardStatement<CreditCardTransaction>, UploadStatementResponse>();

        //CreateMap<IStatement<CreditCardTransactionX>, UploadStatementResponse>()
        //    .IncludeBase<IStatement<Transaction>, UploadStatementResponse>();

        //CreateMap<SharedStatement, UploadStatementResponse>()
        //    .IncludeBase<AbstractCreditCardStatement<CreditCardTransactionX>, UploadStatementResponse>()
        //    .ForSourceMember(x => x.CardNumber1, opt => opt.DoNotValidate())
        //    .ForSourceMember(x => x.CardNumber2, opt => opt.DoNotValidate())
        //    .ForSourceMember(x => x.DependentAccountId, opt => opt.DoNotValidate())
        //    .ForSourceMember(x => x.SecondaryTransactions, opt => opt.DoNotValidate());
    }
}
