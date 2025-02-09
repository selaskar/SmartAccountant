using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface IStatementParseStrategy<TTransaction> where TTransaction : Transaction
{
    /// <exception cref="ParserException"/>
    /// <exception cref="ArgumentException"/>
    public void ParseStatement(Statement<TTransaction> statement, Worksheet worksheet, SharedStringTable stringTable);
}
