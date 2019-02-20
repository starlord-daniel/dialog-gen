namespace DialogGen.Lib.Model
{

    /// <summary>
    /// Defines values for ActionTypes.
    /// </summary>
    public static class ActionTypes
    {
        /// <summary>
        /// Send a simple message consisting of text (and optional additional options for buttons)
        /// </summary>
        public const string SendMessage = "sendMessage";

        /// <summary>
        /// Send the intial message consisting of text (and optional additional options for buttons)
        /// </summary>
        public const string SendInitMessage = "sendInitMessage";

        /// <summary>
        /// Send the QnaMaker response to the users input.
        /// </summary>
        public const string SendQnaMessage = "sendQnaMessage";

        /// <summary>
        /// Call LUIS to generate intents and entities based on the user input.
        /// </summary>
        public const string SendLuisMessage = "sendLuisMessage";

        /// <summary>
        /// Stores the dialog state.
        /// </summary>
        public const string StoreState = "storeState";

        /// <summary>
        /// Stores a custom value.
        /// </summary>
        public const string StoreCustomValue = "storeCustomValue";

        /// <summary>
        /// Sends a message result of a search in Azure Search
        /// </summary>
        public const string SendAzureSearchMessage = "sendAzureSearchMessage";

        /// <summary>
        /// Sends a message result of a filter query in Azure Search
        /// </summary>
        public const string SendAzureSearchFilterMessage = "sendAzureSearchFilterMessage";
    }
}