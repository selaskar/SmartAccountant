using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ISpreadsheetParser
{
    /// <remarks>
    /// Can leave the statement object in dirty state, if encounters an error during parse.
    /// </remarks>
    /// <exception cref="ParserException" />
    void ReadStatement<TTransaction>(Statement<TTransaction> statement, Stream stream, Bank bank)
         where TTransaction : Transaction;
}
