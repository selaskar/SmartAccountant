using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

/// <summary>
/// This interface is designated as common interface for all spreadsheet parse strategies.
/// </summary>
internal interface ISpreadsheetParseStrategy<TTransaction> : IStatementParseStrategy<TTransaction>
    where TTransaction : Transaction
{

}
