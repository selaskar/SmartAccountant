using AutoMapper;

namespace SmartAccountant.Repositories.Core.Mappers;

internal sealed class EntityToModelMappings : Profile
{
    public EntityToModelMappings()
    {
        CreateMap<Entities.Account, Models.Account>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.Bank, opt => opt.MapFrom(e => e.Bank))
            .ForMember(x => x.FriendlyName, opt => opt.MapFrom(e => e.FriendlyName));

        CreateMap<Entities.SavingAccount, Models.SavingAccount>()
            .IncludeBase<Entities.Account, Models.Account>()
            .ForMember(x => x.Currency, opt => opt.MapFrom(e => e.Currency))
            .ForMember(x => x.AccountNumber, opt => opt.MapFrom(e => e.AccountNumber));

        CreateMap<Models.Statement, Entities.Statement>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.Account, opt => opt.Ignore())
            .ForMember(x => x.PeriodStart, opt => opt.MapFrom(e => e.PeriodStart))
            .ForMember(x => x.PeriodEnd, opt => opt.MapFrom(e => e.PeriodEnd))
            .ForMember(x => x.Documents, opt => opt.MapFrom(e => e.Documents));

        CreateMap<Models.Statement<Models.DebitTransaction>, Entities.Statement>()
            .IncludeBase<Models.Statement, Entities.Statement>()
            .ForMember(x => x.Transactions, opt => opt.MapFrom(e => e.Transactions));

        CreateMap<Models.DebitStatement, Entities.DebitStatement>()
            .IncludeBase<Models.Statement, Entities.Statement>()
            .ForMember(x => x.Currency, opt => opt.MapFrom(e => e.Currency));

        CreateMap<Models.StatementDocument, Entities.StatementDocument>()
            .ForSourceMember(x => x.Id, x => x.DoNotValidate())
            .ForMember(x => x.DocumentId, opt => opt.MapFrom(e => e.DocumentId))
            .ForMember(x => x.StatementId, opt => opt.MapFrom(e => e.StatementId))
            .ForMember(x => x.Statement, opt => opt.MapFrom(e => e.Statement))
            .ForMember(x => x.FilePath, opt => opt.MapFrom(e => e.FilePath));

        CreateMap<Models.Transaction, Entities.Transaction>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.StatementId, opt => opt.MapFrom(e => e.StatementId))
            .ForMember(x => x.Statement, opt => opt.Ignore())
            .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(e => e.ReferenceNumber))
            .ForMember(x => x.Timestamp, opt => opt.MapFrom(e => e.Timestamp))
            .ForMember(x => x.Amount, opt => opt.MapFrom(e => e.Amount.Amount))
            .ForMember(x => x.AmountCurrency, opt => opt.MapFrom(e => e.Amount.Currency))
            .ForMember(x => x.Note, opt => opt.MapFrom(e => e.Note));

        CreateMap<Models.DebitTransaction, Entities.DebitTransaction>()
            .IncludeBase<Models.Transaction, Entities.Transaction>()
            .ForMember(x => x.RemainingAmount, opt => opt.MapFrom(e => e.RemainingBalance.Amount))
            .ForMember(x => x.RemainingAmountCurrency, opt => opt.MapFrom(e => e.RemainingBalance.Currency))
            .ForMember(x => x.Order, opt => opt.MapFrom(e => e.Order));
    }
}
