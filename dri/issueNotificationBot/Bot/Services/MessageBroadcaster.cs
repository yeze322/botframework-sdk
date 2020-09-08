using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.Threading.Tasks;

namespace IssueNotificationBot.Services
{
    public class MessageBroadcaster
    {
        private readonly NotificationHelper NotificationHelper;
        private readonly UserStorage UserStorage;
        public IMessageActivity BroadcastMessage = MessageFactory.Text("**No Broadcast Message Set**");

        public MessageBroadcaster(NotificationHelper notificationHelper, UserStorage userStorage)
        {
            NotificationHelper = notificationHelper;
            UserStorage = userStorage;
        }

        public async Task SendBroadcastMessage(IMessageActivity activity = null)
        {
            var users = await UserStorage.GetGitHubUsers();

            foreach(var user in users.Values)
            {
                await NotificationHelper.SendProactiveNotificationToUserAsync(user, activity ?? BroadcastMessage);
            }
        }
    }
}
