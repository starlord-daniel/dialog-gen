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

        // <summary>
        /// Stores the dialog state.
        /// </summary>
        public const string StoreState = "storeState";
    }
}