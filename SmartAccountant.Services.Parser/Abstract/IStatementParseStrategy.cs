using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

public interface IStatementParseStrategy
{
    /// <exception cref="ParserException"/>
    public void ParseDebitStatement(Statement statement, Worksheet worksheet, SharedStringTable stringTable);

    /// <exception cref="ParserException"/>
    public void ParseCreditStatement(Statement statement, Worksheet worksheet, SharedStringTable stringTable);
}
