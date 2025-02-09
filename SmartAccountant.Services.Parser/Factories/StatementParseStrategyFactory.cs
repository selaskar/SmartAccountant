using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Factories;

internal class StatementParseStrategyFactory : IStatementParseStrategyFactory
{
    public IStatementParseStrategy<TTransaction> Create<TTransaction>(Bank bank)
         where TTransaction : Transaction
    {
        return typeof(TTransaction) switch
        {
            Type t when t == typeof(DebitTransaction) => bank switch
            {
                Bank.GarantiBBVA => Cast<TTransaction, DebitTransaction>(new GarantiDebitStatementParseStrategy()),
                _ => throw new NotImplementedException($"Bank ({bank}) is not implemented yet."),
            },
            _ => throw new NotImplementedException($"Transaction type ({typeof(TTransaction).Name}) is not implemented yet."),
        };
    }

    private static IStatementParseStrategy<TTransaction> Cast<TTransaction, TTransaction2>(IStatementParseStrategy<TTransaction2> parseStrategy)
        where TTransaction2 : Transaction where TTransaction : Transaction
    {
        return (IStatementParseStrategy<TTransaction>)parseStrategy;
    }
}
