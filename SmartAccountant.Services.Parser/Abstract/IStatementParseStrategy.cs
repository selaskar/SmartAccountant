using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface IStatementParseStrategy</*in*/ TTransaction> //TODO: any benefit on contravariance? Probably allows the cast in factory class.
    where TTransaction : Transaction
{
    /// <exception cref="ParserException"/>
    /// <exception cref="ServerException"/>
    /// <exception cref="InvalidCastException"/>
    /// <exception cref="ArgumentNullException"/>
    void ParseStatement(IStatement<TTransaction> statement, Worksheet worksheet, SharedStringTable stringTable);

    /// <exception cref="ParserException"/>
    /// <exception cref="InvalidCastException"/>
    void CrossCheck(IStatement<TTransaction> statement);
}
