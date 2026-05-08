using Models;

namespace Interfaces
{
    public interface INotificationSender
    {
        void Validate(User user, string message);
        void Send(User user, Notification notification);
    }
}

