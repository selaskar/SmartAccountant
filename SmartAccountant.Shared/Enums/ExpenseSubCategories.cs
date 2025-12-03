using System.ComponentModel.DataAnnotations;
using SmartAccountant.Shared.Resources;

namespace SmartAccountant.Shared.Enums;

public enum ExpenseSubCategories : byte
{
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_General), ResourceType = typeof(ModelStrings))]
    General = 0,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Rent), ResourceType = typeof(ModelStrings))]
    Rent = 1,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_RentRelated), ResourceType = typeof(ModelStrings))]
    RentRelated = 2,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_MaintenanceFee), ResourceType = typeof(ModelStrings))]
    MaintenanceFee = 3,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_HeatingOrCooling), ResourceType = typeof(ModelStrings))]
    HeatingOrCooling = 4,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Water), ResourceType = typeof(ModelStrings))]
    Water = 5,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Electricity), ResourceType = typeof(ModelStrings))]
    Electricity = 6,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Telephone), ResourceType = typeof(ModelStrings))]
    Telephone = 7,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Internet), ResourceType = typeof(ModelStrings))]
    Internet = 8,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Transportation), ResourceType = typeof(ModelStrings))]
    Transportation = 9,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Hairdressing), ResourceType = typeof(ModelStrings))]
    Hairdressing = 10,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_HouseCleaning), ResourceType = typeof(ModelStrings))]
    HouseCleaning = 11,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Subscription), ResourceType = typeof(ModelStrings))]
    Subscription = 12,
    [Display(Name = nameof(ModelStrings.ExpenseSubCategories_Custom), ResourceType = typeof(ModelStrings))]
    Custom = 255
}
