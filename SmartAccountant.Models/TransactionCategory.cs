namespace SmartAccountant.Models;

public readonly record struct TransactionCategory(MainCategory Category, byte SubCategory)
{

}

public enum MainCategory : byte
{
    None = 0,
    Expense = 1,
    Income = 2,
    Loan = 3,
    Saving = 4,
    InterestOrFee = 5,
}

public enum ExpenseSubCategories : byte
{
    General = 0,
    Rent = 1,
    RentRelated = 2,
    MaintenanceFee = 3,
    HeatingOrCooling = 4,
    Water = 5,
    Electricity = 6,
    Telephone = 7,
    Internet = 8,
    Transportation = 9,
    Hairdressing = 10,
    Cleaning = 11,
    Subscription = 12,
    Custom = 255
}
