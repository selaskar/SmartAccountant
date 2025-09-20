using System.Text.RegularExpressions;
using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Services.Parser.Abstract;

internal interface IMultipartStatementParseStrategy
{
    /// <exception cref="ParserException"/>
    /// <exception cref="RegexMatchTimeoutException"/>
    /// <exception cref="ArgumentOutOfRangeException"/>
    /// <exception cref="ArgumentNullException"/>
    void ParseMultipartStatement(SharedStatement statement, Worksheet worksheet, SharedStringTable stringTable);

    /// <exception cref="ParserException"/>
    void CrossCheck(SharedStatement statement);
}
