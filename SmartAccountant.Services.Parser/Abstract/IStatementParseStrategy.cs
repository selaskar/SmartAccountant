using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface IStatementParseStrategy<TTransaction> where TTransaction : Transaction
{
    /// <exception cref="ParserException"/>
    /// <exception cref="InvalidCastException"/>
    /// <exception cref="ArgumentNullException"/>
    void ParseStatement(Statement<TTransaction> statement, Worksheet worksheet, SharedStringTable stringTable);

    /// <exception cref="ParserException"/>
    /// <exception cref="InvalidCastException"/>
    void CrossCheck(Statement<TTransaction> statement);
}
