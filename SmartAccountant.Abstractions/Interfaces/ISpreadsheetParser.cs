using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;

namespace SmartAccountant.Abstractions.Interfaces;

public interface ISpreadsheetParser
{
    /// <remarks>
    ///     <para>
    ///         Statement object should have a reference to a valid account object.
    ///     </para>
    /// 
    ///     <para>
    ///         Can leave statement object in dirty state, if encounters an error during parse.
    ///     </para>
    /// </remarks>
    /// <exception cref="ParserException" />
    /// <exception cref="ArgumentNullException">
    ///     If statement or its account is null.
    /// </exception>
    void ReadStatement<TTransaction>(Statement<TTransaction> statement, Stream stream)
         where TTransaction : Transaction;
}
