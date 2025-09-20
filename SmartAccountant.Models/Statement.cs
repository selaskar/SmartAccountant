using System.Text.Json.Serialization;

namespace SmartAccountant.Models;

[JsonDerivedType(typeof(DebitStatement), typeDiscriminator: "debit")]
[JsonDerivedType(typeof(CreditCardStatement), typeDiscriminator: "creditCard")]
[JsonDerivedType(typeof(SharedStatement), typeDiscriminator: "shared")]
public abstract record class Statement : BaseModel
{
    public Guid AccountId { get; init; }

    public Account? Account { get; set; }

    public IList<StatementDocument> Documents { get; init; } = [];
}

public abstract record class Statement<TTransaction> : Statement where TTransaction : Transaction
{
    public IList<TTransaction> Transactions { get; init; } = [];
}
