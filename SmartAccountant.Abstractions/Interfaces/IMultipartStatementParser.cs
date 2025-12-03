using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Abstractions.Interfaces;

public interface IMultipartStatementParser
{
    /// <remarks>
    /// May leave the statement object in dirty state, if encounters an error during parse.
    /// </remarks>
    /// <exception cref="ParserException" />
    void ReadMultipartStatement(SharedStatement statement, Stream stream, Bank bank);
}
