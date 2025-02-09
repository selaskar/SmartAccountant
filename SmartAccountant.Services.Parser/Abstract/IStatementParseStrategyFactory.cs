using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface IStatementParseStrategyFactory
{
    /// <exception cref="NotImplementedException" />
    IStatementParseStrategy<TTransaction> Create<TTransaction>(Bank bank)
         where TTransaction : Transaction;
}
