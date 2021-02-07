using System;
using System.Collections.Generic;
using Athena.Infrastructure.Models;
using Athena.Infrastructure.Models.Enums;
using TicketTask = Athena.Infrastructure.Models.Task;

namespace Athena.Core.Services
{
    public class NotificationBuilder
    {
        private readonly User _notifier;
        private readonly IReadOnlyCollection<User> _observers;


        public NotificationBuilder(User notifier, IReadOnlyCollection<User> observers)
        {
            _notifier = notifier;
            _observers = observers;
        }


        public IReadOnlyCollection<Notification> BuildTicketUpdateNotifications(Project project, Ticket ticket)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Задча обновлена",
                    Message = $"{project.Name}: {_notifier.FirstName} {_notifier.LastName} обновил задачу {ticket.Title}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildTicketCreateNotifications(Project project)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Задача добавлена",
                    Message = $"{project.Name}: {_notifier.FirstName} {_notifier.LastName} создал задачу.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildTaskCreateNotifications(Project project, Ticket ticket, TicketTask task)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Подзадача добавлена",
                    Message = $"{project.Name}: {_notifier.FirstName} {_notifier.LastName} создал подзадачу {task.Title} в задаче {ticket.Title}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildTaskModifyNotifications(Project project, Ticket ticket, TicketTask task)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Подзадача выполнена",
                    Message = $"{project.Name}: {_notifier.FirstName} {_notifier.LastName} выполнил подзадачу {task.Title} в задаче {ticket.Title}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildTaskDeleteNotifications(Project project ,Ticket ticket, TicketTask task)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Подзадача удалена",
                    Message = $"{project.Name}: {_notifier.FirstName} {_notifier.LastName} удалил подзадачу {task.Title} в задаче {ticket.Title}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildInvitations(Project project)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Приглашение на проект",
                    Message = $"{_notifier.FirstName} {_notifier.LastName} пригласил вас на проект {project.Name}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildChangeRoleNotifications(Project project, int newRole)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Роль обновлена",
                    Message = $"{project.Name}: {_notifier.FirstName} {_notifier.LastName} изменил вашу роль на {roles[newRole]}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        public IReadOnlyCollection<Notification> BuildRemoveMemberNotifications(Project project)
        {
            var notifications = new List<Notification>();

            foreach (User observer in _observers) {
                var notification = new Notification
                {
                    CreatedDate = DateTime.Now,
                    Title = "Исключение из проекта",
                    Message = $"{_notifier.FirstName} {_notifier.LastName} исключил вас из проекта {project.Name}.",
                    RecipientId = observer.UserId,

                };
                notifications.Add(notification);
            }

            return notifications;
        }

        private readonly static Dictionary<int, string> roles = new Dictionary<int, string>
        {
            {MemberRole.Admin, "Администратор"},
            {MemberRole.Member, "Участник проекта" },
            {MemberRole.Owner, "Владелец проекта"}
        };
    }
}