using SmartAccountant.Shared.Enums;

namespace SmartAccountant.Shared.Structs;

public readonly record struct TransactionCategory(MainCategory Category, byte SubCategory)
{

}
