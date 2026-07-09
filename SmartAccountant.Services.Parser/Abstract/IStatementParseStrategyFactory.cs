using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface IStatementParseStrategyFactory
{
    /// <exception cref="NotImplementedException" />
    IStatementParseStrategy<TTransaction> Create<TTransaction>(Bank bank)
         where TTransaction : Transaction;
}

internal abstract class ISpreadsheetParseStrategyFactory : IStatementParseStrategyFactory
{
    /// <exception cref="NotImplementedException" />
    public abstract ISpreadsheetParseStrategy<TTransaction> Create<TTransaction>(Bank bank)
         where TTransaction : Transaction;


    IStatementParseStrategy<TTransaction> IStatementParseStrategyFactory.Create<TTransaction>(Bank bank)
    //where TTransaction : Transaction
    {
        return Create<TTransaction>(bank);
    }
}
