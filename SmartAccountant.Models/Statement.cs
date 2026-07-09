using System.Text.Json.Serialization;

namespace SmartAccountant.Models;

public interface IStatement<out TTransaction> : IModel
    where TTransaction : Transaction
{
    Guid AccountId { get; init; }

    Account Account { get; set; }

    IList<StatementDocument> Documents { get; init; }

    IEnumerable<TTransaction> Transactions { get; }
}

//[JsonDerivedType(typeof(DebitStatement), typeDiscriminator: "debit")]
//[JsonDerivedType(typeof(CreditCardStatement), typeDiscriminator: "creditCard")]
//[JsonDerivedType(typeof(SharedStatement), typeDiscriminator: "shared")]
//public abstract record class Statement : BaseModel
//{
//    public Guid AccountId { get; init; }

//    public Account? Account { get; set; }

//    public IList<StatementDocument> Documents { get; init; } = [];
//}

//public abstract record class Statement<TTransaction> : Statement where TTransaction : Transaction
//{
//    public IList<TTransaction> Transactions { get; init; } = [];
//}

[JsonDerivedType(typeof(DebitStatement), typeDiscriminator: "debit")]
[JsonDerivedType(typeof(CreditCardStatement), typeDiscriminator: "creditCard")]
[JsonDerivedType(typeof(SharedStatement), typeDiscriminator: "shared")]
public abstract record class Statement<TTransaction> : BaseModel, IStatement<TTransaction>
    where TTransaction : Transaction
{
    public Guid AccountId { get; init; }

    public required Account Account { get; set; }

    public IList<StatementDocument> Documents { get; init; } = [];

    public IList<TTransaction> Transactions { get; init; } = [];

    IEnumerable<TTransaction> IStatement<TTransaction>.Transactions => Transactions;
}
