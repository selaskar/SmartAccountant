using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.ParseStrategies;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Services.Parser.Factories;

internal class SpreadsheetParseStrategyFactory : ISpreadsheetParseStrategyFactory
{
    public ISpreadsheetParseStrategy<TTransaction> Create<TTransaction>(Bank bank)
         where TTransaction : Transaction
    {
        return typeof(TTransaction) switch
        {
            Type t when t == typeof(DebitTransaction) => bank switch
            {
                Bank.GarantiBBVA => Cast<TTransaction, DebitTransaction>(new GarantiDebitStatementParseStrategy()),
                _ => throw new NotImplementedException($"Transaction type ({typeof(TTransaction).Name}) is not implemented for the bank ({bank})."),
            },
            Type t when t == typeof(CreditCardTransaction) => bank switch
            {
                Bank.GarantiBBVA => Cast<TTransaction, CreditCardTransaction>(new GarantiCreditCardStatementParseStrategy()),
                _ => throw new NotImplementedException($"Transaction type ({typeof(TTransaction).Name}) is not implemented for the bank ({bank})."),
            },
            Type t when t == typeof(CreditCardTransactionX) => bank switch
            {
                Bank.GarantiBBVA => Cast<TTransaction, CreditCardTransactionX>(new GarantiMultipartStatementParseStrategy()),
                _ => throw new NotImplementedException($"Transaction type ({typeof(TTransaction).Name}) is not implemented for the bank ({bank})."),
            },
            _ => throw new NotImplementedException($"Transaction type ({typeof(TTransaction).Name}) is not implemented yet."),
        };
    }


    private static ISpreadsheetParseStrategy<TTransaction> Cast<TTransaction, TTransaction2>(ISpreadsheetParseStrategy<TTransaction2> parseStrategy)
        where TTransaction : Transaction
        where TTransaction2 : Transaction
    {
        return (ISpreadsheetParseStrategy<TTransaction>)parseStrategy;
    }
}
