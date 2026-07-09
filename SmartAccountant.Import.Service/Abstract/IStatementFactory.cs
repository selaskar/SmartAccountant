using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Models.Request;

namespace SmartAccountant.Import.Service.Abstract;

internal interface IStatementFactory
{
    /// <exception cref="ImportException"/>
    /// <exception cref="NotImplementedException"/>
    IStatement<TTransaction> Create<TTransaction>(AbstractStatementImportModel model, Account account)
        where TTransaction : Transaction;
}
