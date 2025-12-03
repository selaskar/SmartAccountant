using AutoMapper;

namespace SmartAccountant.API.Mappers;

public class ModelToDtoMappings : Profile
{
    public ModelToDtoMappings()
    {
        CreateMap<Dtos.BaseDto, Models.BaseModel>()
            .ForMember(x => x.Id, opt => opt.MapFrom(e => e.Id))
            .ReverseMap();

        CreateMap<Dtos.Account, Models.Account>()
            .IncludeBase<Dtos.BaseDto, Models.BaseModel>()
            .ForMember(x => x.Bank, opt => opt.MapFrom(e => e.Bank))
            .ForMember(x => x.FriendlyName, opt => opt.MapFrom(e => e.FriendlyName))
            .ReverseMap();

        CreateMap<Dtos.SavingAccount, Models.SavingAccount>()
            .IncludeBase<Dtos.Account, Models.Account>()
            .ForMember(x => x.Currency, opt => opt.MapFrom(e => e.Currency))
            .ForMember(x => x.AccountNumber, opt => opt.MapFrom(e => e.AccountNumber))
            .ReverseMap();

        CreateMap<Dtos.AbstractCreditCard, Models.AbstractCreditCard>()
            .IncludeBase<Dtos.Account, Models.Account>()
            .ForMember(x => x.CardNumber, opt => opt.MapFrom(e => e.CardNumber))
            .ReverseMap();

        CreateMap<Dtos.CreditCard, Models.CreditCard>()
            .IncludeBase<Dtos.AbstractCreditCard, Models.AbstractCreditCard>()
            .ForMember(x => x.Limits, opt => opt.MapFrom(e => e.Limits))
            .ReverseMap();

        CreateMap<Dtos.VirtualCard, Models.VirtualCard>()
            .IncludeBase<Dtos.AbstractCreditCard, Models.AbstractCreditCard>()
            .ForMember(x => x.ParentId, opt => opt.MapFrom(e => e.ParentId))
            .ForMember(x => x.Parent, opt => opt.MapFrom(e => e.Parent))
            .ReverseMap();

        CreateMap<Dtos.Transaction, Models.Transaction>()
            .IncludeBase<Dtos.BaseDto, Models.BaseModel>()
            .ForMember(x => x.AccountId, opt => opt.MapFrom(e => e.AccountId))
            .ForMember(x => x.Account, opt => opt.Ignore())
            .ForMember(x => x.ReferenceNumber, opt => opt.MapFrom(e => e.ReferenceNumber))
            .ForMember(x => x.Timestamp, opt => opt.MapFrom(e => e.Timestamp))
            .ForMember(x => x.Amount, opt => opt.MapFrom(e => e.Amount))
            .ForMember(x => x.Description, opt => opt.MapFrom(e => e.Description))
            .ForMember(x => x.PersonalNote, opt => opt.MapFrom(e => e.PersonalNote))
            .ForMember(x => x.Category, opt => opt.MapFrom(e => e.Category))
            .ReverseMap();

        CreateMap<Dtos.DebitTransaction, Models.DebitTransaction>()
            .IncludeBase<Dtos.Transaction, Models.Transaction>()
            .ForMember(x => x.RemainingBalance, opt => opt.MapFrom(e => e.RemainingBalance))
            .ReverseMap();

        CreateMap<Dtos.CreditCardTransaction, Models.CreditCardTransaction>()
            .IncludeBase<Dtos.Transaction, Models.Transaction>()
            .ForMember(x => x.ProvisionState, opt => opt.MapFrom(e => e.ProvisionState))
            .ReverseMap();

        CreateMap<Dtos.MonthlySummary, Models.MonthlySummary>()
            .IncludeBase<Dtos.BaseDto, Models.BaseModel>()
            .ForMember(x => x.Month, opt => opt.MapFrom(e => e.Month))
            .ForMember(x => x.Currencies, opt => opt.MapFrom(e => e.Currencies))
            .ReverseMap();

        CreateMap<Dtos.CurrencySummary, Models.CurrencySummary>()
            .ForMember(x => x.RemainingBalancesTotal, opt => opt.MapFrom(e => e.RemainingBalancesTotal))
            .ForMember(x => x.OriginalLimitsTotal, opt => opt.MapFrom(e => e.OriginalLimitsTotal))
            .ForMember(x => x.IncomeTotal, opt => opt.MapFrom(e => e.IncomeTotal))
            .ForMember(x => x.ExpensesTotal, opt => opt.MapFrom(e => e.ExpensesTotal))
            .ForMember(x => x.InterestAndFeesTotal, opt => opt.MapFrom(e => e.InterestAndFeesTotal))
            .ForMember(x => x.PlannedExpensesTotal, opt => opt.MapFrom(e => e.PlannedExpensesTotal))
            .ForMember(x => x.LoansTotal, opt => opt.MapFrom(e => e.LoansTotal))
            .ForMember(x => x.SavingsTotal, opt => opt.MapFrom(e => e.SavingsTotal))
            .ForMember(x => x.Net, opt => opt.MapFrom(e => e.Net))
            .ForMember(x => x.ExpensesBreakdown, opt => opt.MapFrom(e => e.ExpensesBreakdown))
            .ReverseMap();
    }
}
