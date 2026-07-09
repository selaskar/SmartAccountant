using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Core.Helpers;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Resources;
using SmartAccountant.Shared.Enums;
using SmartAccountant.Shared.Enums.Errors;
using SmartAccountant.Shared.Structs;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed partial class GarantiCreditCardStatementParseStrategy : AbstractGarantiCreditCardStatementParseStrategy<CreditCardTransaction>
{
    /// Non-empty rows
    internal const int HeaderRowCount = 3;
    internal const int OpenProvisionHeaderRowCount = 3;
    internal const int FooterRowCount = 1;

    internal const string OpenProvisionLabel = "Açık Provizyon - TL";
    internal const string OpenProvisionTotalAmountLabel = "Toplam Açık Provizyon:";
    internal const string RegularTransactionsLabel = "Dönemiçi İşlemler - TL";

    /// <inheritdoc/>
    public override void ParseStatement(IStatement<CreditCardTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        CreditCardStatement creditCardStatement = Cast<CreditCardTransaction, CreditCardStatement>(statement);

        Row[] rows = worksheet.Descendants<Row>().ToArray();

        Parse(creditCardStatement, rows, stringTable);
    }

    /// <inheritdoc/>
    public override void CrossCheck(IStatement<CreditCardTransaction> statement)
    {
        //TODO: will give wrong results when there are cancelled transactions.
        decimal totalExpenses = statement.Transactions.Select(t => t.Amount.Amount)
            .Where(d => d > 0) //debt payments doesn't count toward total transactions.
            .DefaultIfEmpty().Sum();

        CreditCardStatement creditCardStatement = Cast<CreditCardTransaction, CreditCardStatement>(statement);

        if (creditCardStatement.TotalExpenses != totalExpenses)
            throw new ParserException(ParserErrors.TransactionAmountAndTotalExpensesMismatch);
    }


    /// <exception cref="ParserException"/>
    /// <exception cref="ServerException"/>
    private void Parse(CreditCardStatement statement, Row[] rows, SharedStringTable stringTable)
    {
        try
        {
            if (stringTable.InnerText.Contains(OpenProvisionLabel, StringComparison.InvariantCultureIgnoreCase)
                && stringTable.InnerText.Contains(RegularTransactionsLabel, StringComparison.InvariantCultureIgnoreCase))
            {
                int openProvisionRowCount = rows.Skip(HeaderRowCount)
                    .TakeWhile(r => !string.Equals(OpenProvisionTotalAmountLabel, r.GetCell(0).GetCellValue(stringTable), StringComparison.OrdinalIgnoreCase))
                    .Count();

                // Reading open provision transactions
                ParseSpan(rows.AsSpan().Slice(OpenProvisionHeaderRowCount, openProvisionRowCount),
                    statement.AccountId,
                    statement.Transactions,
                    ProvisionState.Open,
                    stringTable);

                // Reading regular transactions
                ParseSpan(rows.AsSpan()[(OpenProvisionHeaderRowCount + openProvisionRowCount + HeaderRowCount)..^FooterRowCount],
                    statement.AccountId,
                    statement.Transactions,
                    ProvisionState.Finalized,
                    stringTable);
            }
            else
            {
                if (rows.Length <= HeaderRowCount + FooterRowCount)
                    return;

                ParseSpan(rows.AsSpan()[HeaderRowCount..^FooterRowCount], statement.AccountId, statement.Transactions, ProvisionState.Finalized, stringTable);
            }
        }
        catch (Exception ex) when (ex is not ParserException and not ServerException)
        {
            throw new ServerException(Messages.UnexpectedErrorParsingStatement, ex);
        }
    }


    /// <inheritdoc/>
    private protected override CreditCardTransaction CreateTransaction(
        Guid? accountId,
        DateTimeOffset date,
        MonetaryValue amount,
        Row row,
        SharedStringTable stringTable,
        ProvisionState provisionState)
    {
        return new CreditCardTransaction()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            Timestamp = date,
            Amount = amount * -1, //Since the normal balance is credit
            ReferenceNumber = null,
            Description = row.GetCell(1).GetCellValue(stringTable),
            ProvisionState = provisionState
        };
    }
}
