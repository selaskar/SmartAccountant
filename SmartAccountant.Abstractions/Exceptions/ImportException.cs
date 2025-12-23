namespace SmartAccountant.Abstractions.Exceptions;

public class ImportException(ImportErrors error, string message, Exception? innerException) 
    : EnumException<ImportErrors>(error, message, innerException)
{
    public ImportException(ImportErrors error, string message) : this(error, message, null)
    { }

    public ImportException(ImportErrors error, Exception? innerException) : this(error, error.ToString(), innerException)
    { }

    public ImportException(ImportErrors error) : this(error, error.ToString(), null)
    { }
}

public enum ImportErrors
{
    Unspecified = 0,
    UploadedStatementFileEmpty = 1,
    UploadedStatementFileTooBig = 2,
    UploadedStatementFileTypeNotSupported = 3,
    SavingAccountExpected = 4,
    CannotCheckExistingTransactions = 5,
    AccountDoesNotBelongToUser = 6,
    CannotValidateAccountHolder = 7,
    CannotSaveUploadedStatementFile = 8,
    CannotSaveImportedStatement = 9,
    StatementTypeMismatch = 10,
    CannotParseUploadedStatementFile = 11,
    AbstractCreditCardExpected = 12,
    PrimaryCardNumberNotDetermined = 13,
    SecondaryCardNumberNotDetermined = 14,
    DiscoveredCardNumbersMismatch = 15,
    CannotDetermineSecondaryAccount = 16,
}
