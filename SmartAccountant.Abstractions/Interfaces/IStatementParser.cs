using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IStatementParser
{
    /// <remarks>
    /// May leave the statement object in dirty state, if encounters an error during parse.
    /// </remarks>
    /// <exception cref="ParserException" />
    void ReadStatement<TTransaction>(IStatement<TTransaction> statement, Stream stream, Bank bank)
         where TTransaction : Transaction;
}
