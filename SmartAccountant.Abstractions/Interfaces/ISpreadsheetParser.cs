using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ISpreadsheetParser
{
    /// <remarks>
    /// Statement object should have a reference to a valid account object.
    /// Can leave statement object in dirty state, if encounters an error during parse.
    /// </remarks>
    /// <exception cref="ParserException" />
    /// <exception cref="ArgumentNullException" />
    void ReadStatement<TTransaction>(Statement<TTransaction> statement, Stream stream)
         where TTransaction : Transaction;
}
