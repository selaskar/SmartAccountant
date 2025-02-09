using SmartAccountant.Abstractions.Models.Request;
using SmartAccountant.Models;

namespace SmartAccountant.Import.Service.Abstract;

internal interface IStatementFactory
{
    /// <exception cref="NotImplementedException"/>
    Statement Create(ImportStatementModel model, Account account);
}
