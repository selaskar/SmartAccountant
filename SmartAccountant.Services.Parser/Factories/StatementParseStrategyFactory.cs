using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.ParseStrategies;

namespace SmartAccountant.Services.Parser.Factories;

public class StatementParseStrategyFactory : IStatementParseStrategyFactory
{
    public IStatementParseStrategy Create(Bank bank)
    {
        return bank switch
        {
            Bank.GarantiBBVA => new GarantiStatementParseStrategy(),
            _ => throw new NotImplementedException($"Bank ({bank}) is not implemented yet."),
        };
    }
}
