using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface ISpreadsheetParseStrategyFactory
{
    /// <exception cref="NotImplementedException" />
    ISpreadsheetParseStrategy<TTransaction> Create<TTransaction>(Bank bank)
         where TTransaction : Transaction;
}
