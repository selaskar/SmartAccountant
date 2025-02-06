using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

public interface IStatementParseStrategyFactory
{
    /// <exception cref="NotImplementedException" />
    IStatementParseStrategy Create(Bank bank);
}
