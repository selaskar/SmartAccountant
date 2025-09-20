using DocumentFormat.OpenXml.Spreadsheet;
using SmartAccountant.Abstractions.Exceptions;
using SmartAccountant.Models;
using SmartAccountant.Services.Parser.Abstract;
using SmartAccountant.Services.Parser.Extensions;
using SmartAccountant.Services.Parser.Resources;

namespace SmartAccountant.Services.Parser.ParseStrategies;

internal sealed partial class GarantiCreditCardStatementParseStrategy : AbstractGarantiCreditCardStatementParseStrategy,
    IStatementParseStrategy<CreditCardTransaction>    
{
    /// Non-empty rows
    internal const int HeaderRowCount = 3;
    internal const int OpenProvisionHeaderRowCount = 3;
    internal const int FooterRowCount = 1;

    internal const string OpenProvisionLabel = "Açık Provizyon - TL";
    internal const string OpenProvisionTotalAmountLabel = "Toplam Açık Provizyon:";
    internal const string RegularTransactionsLabel = "Dönemiçi İşlemler - TL";

    /// <inheritdoc/>
    public void ParseStatement(Statement<CreditCardTransaction> statement, Worksheet worksheet, SharedStringTable stringTable)
    {
        var creditCardStatement = Cast<CreditCardStatement>(statement);

        Row[] rows = worksheet.Descendants<Row>().ToArray();

        Parse(creditCardStatement, rows, stringTable);
    }

    /// <inheritdoc/>
    public void CrossCheck(Statement<CreditCardTransaction> statement)
    {
        var creditCardStatement = Cast<CreditCardStatement>(statement);
        
        //TODO: will give wrong results when there are cancelled transactions.
        decimal totalExpenses = creditCardStatement.Transactions.Select(t => t.Amount.Amount)
            .Where(d => d > 0) //debt payments doesn't count toward total transactions.
            .DefaultIfEmpty().Sum();

        if (creditCardStatement.TotalExpenses != totalExpenses)
            throw new ParserException(Messages.TransactionAmountAndTotalExpensesDontMatch);
    }

    /// <exception cref="ParserException"/>
    private static void Parse(CreditCardStatement statement, Row[] rows, SharedStringTable stringTable)
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
        catch (Exception ex) when (ex is not ParserException)
        {
            throw new ParserException(Messages.UnexpectedErrorParsingStatement, ex);
        }
    }
}
