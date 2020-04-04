using BooksMiddletier.OtherClasses;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BooksApi
{
    public class ChatHub : Hub
    {
        public class ConnectedUsers
        {
            public string Name { get; set; }
            public Guid AppId { get; set; }
            public string ConnectionId { get; set; }
        }

        public static List<ConnectedUsers> Users = new List<ConnectedUsers>();

        public void Send(string name, string message)
        {
            // Call the broadcastMessage method to update clients.
            Clients.All.MessageReceived(name, message);
        }

        public void Connect(string name, Guid appId)
        {
            try
            {
                string id = Context.ConnectionId;

                var exists = Users.FirstOrDefault(x => x.AppId == appId);

                if (exists == null || (exists != null && exists.ConnectionId != id))
                {
                    if (exists != null)
                    {
                        Users.Remove(exists);
                    }
                    Users.Add(new ConnectedUsers { AppId = appId, ConnectionId = id, Name = name });
                }
            }
            catch(Exception e)
            {
                GlobalVars.Logger.Error(e, $"[ChathubConnect] There was an error registering user to ChatHub. appId: {appId}, name: {name}");
            }
        }

        public void Disconnect()
        {
            string id = Context.ConnectionId;
            Users.RemoveAll(e => e.ConnectionId == id);
        }

        public void SendPrivateMessage(Guid toUserId, string message, int requestId, DateTime sentDate)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            ChatHubParams hubParams = new ChatHubParams
            {
                RequestId = requestId,
                SentDate = sentDate
            };

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).sendPrivateMessage(message, fromUser.AppId, hubParams);
            }

            // send to caller user
            Clients.Caller.sendPrivateMessage(message, fromUser.AppId, hubParams);

        }

        public void OfferExchangeBook(string title, string authors, Guid toUserId, int requestId, Guid bookId)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            BookExchangeParams hubParams = new BookExchangeParams
            {
                Title = title,
                Authors = authors,
                ToUserId = toUserId
            };

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).offerBookExchange(requestId, hubParams, bookId);
            }

        }

        public void RejectExchangeBook(int requestId, Guid toUserId)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).rejectBookExchange(requestId, toUserId);
            }

        }

        public void RequestTime(int requestId, Guid toUserId, DateTime proposedDate)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).requestTime(requestId, toUserId, proposedDate);
            }

        }

        public void AnswerTimeRequest(int requestId, Guid toUserId, bool answer)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).answerTimeRequest(requestId, toUserId, answer);
            }

        }

        public void OfferBook(int requestId, Guid toUserId, Guid bookOfferId, DateTime date)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).offerBook(requestId, bookOfferId, date);
            }

        }

        public void ReturnBook(int requestId, Guid toUserId)
        {

            string fromUserId = Context.ConnectionId;

            var toUser = Users.FirstOrDefault(x => x.AppId == toUserId);
            var fromUser = Users.FirstOrDefault(x => x.ConnectionId == fromUserId);

            if (toUser != null)
            {
                // send to
                Clients.Client(toUser.ConnectionId).returnBook(requestId);
            }

        }
    }
}