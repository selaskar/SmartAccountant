using AutoMapper;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Repositories.Core.Mappers;

internal sealed class EntityToModelMappings : Profile
{
    public EntityToModelMappings()
    {
        //TODO: base entity and mapping

        CreateMap<Entities.Account, Models.Account>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.Bank, opt => opt.MapFrom(e => e.Bank))
            .ForMember(x => x.FriendlyName, opt => opt.MapFrom(e => e.FriendlyName));

        CreateMap<Entities.SavingAccount, Models.SavingAccount>()
            .IncludeBase<Entities.Account, Models.Account>()
            .ForMember(x => x.Currency, opt => opt.MapFrom(e => e.Currency))
            .ForMember(x => x.AccountNumber, opt => opt.MapFrom(e => e.AccountNumber));

        //TODO: abstract credit card entity and mapping

        CreateMap<Entities.CreditCard, Models.CreditCard>()
            .IncludeBase<Entities.Account, Models.Account>()
            .ForMember(x => x.CardNumber, opt => opt.MapFrom(e => e.CardNumber))
            .ForMember(x => x.Limits, opt => opt.MapFrom(e => e.Limits));

        CreateMap<Entities.VirtualCard, Models.VirtualCard>()
            .IncludeBase<Entities.Account, Models.Account>()
            .ForMember(x => x.CardNumber, opt => opt.MapFrom(e => e.CardNumber))
            .ForMember(x => x.ParentId, opt => opt.MapFrom(e => e.Parent))
            .ForMember(x => x.Parent, opt => opt.Ignore());

        CreateMap<Models.Statement, Entities.Statement>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.Account, opt => opt.Ignore())
            .ForMember(x => x.Documents, opt => opt.MapFrom(e => e.Documents));

        CreateMap<Models.Statement<Models.DebitTransaction>, Entities.Statement>()
            .IncludeBase<Models.Statement, Entities.Statement>()
            .ForSourceMember(x => x.Transactions, opt => opt.DoNotValidate());

        CreateMap<Models.DebitStatement, Entities.DebitStatement>()
            .IncludeBase<Models.Statement, Entities.Statement>()
            .ForMember(x => x.Currency, opt => opt.MapFrom(e => e.Currency));

        CreateMap<Models.CreditCardStatement, Entities.CreditCardStatement>()
            .IncludeBase<Models.Statement, Entities.Statement>()
            .ForMember(x => x.RolloverAmount, opt => opt.MapFrom(e => e.RolloverAmount))
            .ForMember(x => x.TotalDueAmount, opt => opt.MapFrom(e => e.TotalDueAmount))
            .ForMember(x => x.MinimumDueAmount, opt => opt.MapFrom(e => e.MinimumDueAmount))
            .ForMember(x => x.TotalFees, opt => opt.MapFrom(e => e.TotalFees))
            .ForMember(x => x.DueDate, opt => opt.MapFrom(e => e.DueDate));

        CreateMap<Models.StatementDocument, Entities.StatementDocument>()
            .ForSourceMember(x => x.Id, x => x.DoNotValidate())
            .ForMember(x => x.DocumentId, opt => opt.MapFrom(e => e.DocumentId))
            .ForMember(x => x.StatementId, opt => opt.MapFrom(e => e.StatementId))
            .ForMember(x => x.Statement, opt => opt.MapFrom(e => e.Statement))
            .ForMember(x => x.FilePath, opt => opt.MapFrom(e => e.FilePath));

        CreateMap<Models.Transaction, Entities.Transaction>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId!.Value))
            .ForMember(x => x.Account, opt => opt.Ignore())
            .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(e => e.ReferenceNumber))
            .ForMember(x => x.Timestamp, opt => opt.MapFrom(e => e.Timestamp))
            .ForMember(x => x.Amount, opt => opt.MapFrom(e => e.Amount.Amount))
            .ForMember(x => x.AmountCurrency, opt => opt.MapFrom(e => e.Amount.Currency))
            .ForMember(x => x.Description, opt => opt.MapFrom(e => e.Description))
            .ForMember(x => x.PersonalNote, opt => opt.MapFrom(e => e.PersonalNote))
            .ForMember(x => x.Category, opt => opt.MapFrom(e => e.Category.Category))
            .ForMember(x => x.SubCategory, opt => opt.MapFrom(e => e.Category.SubCategory));

        CreateMap<Entities.Transaction, Models.Transaction>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.Account, opt => opt.MapFrom(e => e.Account))
            .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(e => e.ReferenceNumber))
            .ForMember(x => x.Timestamp, opt => opt.MapFrom(e => e.Timestamp))
            .ForMember(x => x.Amount, opt => opt.MapFrom(e => new MonetaryValue(e.Amount, e.AmountCurrency)))
            .ForMember(x => x.Description, opt => opt.MapFrom(e => e.Description))
            .ForMember(x => x.PersonalNote, opt => opt.MapFrom(e => e.PersonalNote))
            .ForMember(x => x.Category, opt => opt.MapFrom(e => new TransactionCategory(e.Category, e.SubCategory)));

        CreateMap<Models.DebitTransaction, Entities.DebitTransaction>()
            .IncludeBase<Models.Transaction, Entities.Transaction>()
            .ForMember(x => x.RemainingAmount, opt => opt.MapFrom(e => e.RemainingBalance.Amount))
            .ForMember(x => x.RemainingAmountCurrency, opt => opt.MapFrom(e => e.RemainingBalance.Currency));

        CreateMap<Entities.DebitTransaction, Models.DebitTransaction>()
            .IncludeBase<Entities.Transaction, Models.Transaction>()
            .ForMember(x => x.RemainingBalance, opt => opt.MapFrom(e => new MonetaryValue(e.RemainingAmount, e.RemainingAmountCurrency)));

        CreateMap<Models.CreditCardTransaction, Entities.CreditCardTransaction>()
            .IncludeBase<Models.Transaction, Entities.Transaction>()
            .ForMember(x => x.ProvisionState, opt => opt.MapFrom(e => e.ProvisionState))
            .ReverseMap();

        CreateMap<Entities.CreditCardLimit, Models.CreditCardLimit>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ForMember(x => x.CreditCardId, opt => opt.MapFrom(e => e.CardId))
            .ForMember(x => x.Amount, opt => opt.MapFrom(e => new MonetaryValue(e.Amount, e.AmountCurrency)))
            .ForMember(x => x.Period, opt => opt.MapFrom(e => new Period()
            {
                ValidFrom = e.ValidSince,
                ValidTo = e.ValidUntil
            }));
    }
}
