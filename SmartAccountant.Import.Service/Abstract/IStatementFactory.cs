using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Abstract;

internal interface IStatementFactory
{
    /// <exception cref="ImportException"/>
    /// <exception cref="NotImplementedException"/>
    Statement Create(AbstractStatementImportModel model, Account account);
}
