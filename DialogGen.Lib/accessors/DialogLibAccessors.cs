using System;
using DialogGen.Lib.States;
using Microsoft.Bot.Builder;

namespace DialogGen.Lib.Accessors
{
    /// <summary>
    /// This class is created to persist UserState and ConversationState between sessions. 
    /// All values are stored in the storage defined in the Startup.
    /// </summary>
    public class DialogLibAccessors
    {
        /// <summary>
        /// Stores the short term topic and connected variables of a conversation
        /// </summary>
        public IStatePropertyAccessor<TopicState> TopicState { get; set; }

        /// <summary>
        /// Stores the long term user profile and connected variables for a user
        /// </summary>
        public IStatePropertyAccessor<UserProfile> UserProfile { get; set; }

        public static string TopicStateName { get; } = $"{nameof(DialogLibAccessors)}.TopicState";
        
        public static string UserProfileName { get; } = $"{nameof(DialogLibAccessors)}.UserProfile";

        public ConversationState ConversationState { get; }
        public UserState UserState { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogLibAccessors"/> class.
        /// </summary>
        /// <param name="conversationState">The short term state object that stores our values.</param>
        /// <param name="userState">The long term state object that stores our values.</param>
        public DialogLibAccessors(ConversationState conversationState, UserState userState)
        {
            ConversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            UserState = userState ?? throw new ArgumentNullException(nameof(userState));
        }
    }
}