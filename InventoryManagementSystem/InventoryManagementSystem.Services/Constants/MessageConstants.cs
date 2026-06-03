namespace InventoryManagementSystem.Services.Constants
{
    public static class MessageConstants
    {
        public const string SuccessCreate = "{0} was created successfully.";
        public const string SuccessEdit = "{0} was edited successfully.";
        public const string SuccessDelete = "{0} was deleted successfully.";
        public const string SuccessStockAdd = "Stock was added successfully.";
        public const string SuccessStockRemove = "Stock was removed successfully.";

        public const string ErrorGeneral = "An unexpected error occurred. Please try again.";
        public const string ErrorNotFound = "The requested {0} was not found.";
        public const string ErrorInsufficientStock = "Cannot remove more stock than available.";
    }
}
