using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Books.OtherClasses
{
    public class ChatClient: INotifyPropertyChanged
    {
        private HubConnection Connection;

        private IHubProxy ChatHubProxy;



        public delegate void MessageReceived(string message, Guid fromId, ChatHubParams hubParams);
        public delegate void BookExchangeRequest(int requestId, BookExchangeParams hubParams, Guid bookId);
        public delegate void RejectBookExchange(int requestId, Guid toUserId);
        public delegate void TimeRequested(int requestId, Guid toUserId, DateTime proposedDate);
        public delegate void TimeRequestAnswered(int requestId, Guid toUserId, bool answer);
        public delegate void BookOffered(int requestId, Guid bookOfferId, DateTime date);
        public delegate void ReturnOffered(int requestId);

        public event MessageReceived OnMessageReceived;
        public event BookExchangeRequest OnBookExchangeRequest;
        public event RejectBookExchange OnExchangeRejected;
        public event TimeRequested OnExtraTimeRequested;
        public event TimeRequestAnswered OnTimeRequestAnswered;
        public event BookOffered OnBookOffered;
        public event ReturnOffered OnReturnOffered;



        public ChatClient(string url)

        {

            Connection = new HubConnection(url);



            Connection.StateChanged += (StateChange obj) => {

                OnPropertyChanged("ConnectionState");

            };



            ChatHubProxy = Connection.CreateHubProxy("ChatHub");

            ChatHubProxy.On<string, Guid, ChatHubParams>("sendPrivateMessage", (message, fromId, hubParams) => {

                OnMessageReceived?.Invoke(message, fromId, hubParams);

            });

            ChatHubProxy.On<int, BookExchangeParams, Guid>("offerBookExchange", (requestId, hubParams, bookId) => {

                OnBookExchangeRequest?.Invoke(requestId, hubParams, bookId);

            });

            ChatHubProxy.On<int, Guid>("rejectBookExchange", (requestId, toUserId) => {

                OnExchangeRejected?.Invoke(requestId, toUserId);

            });

            ChatHubProxy.On<int, Guid, DateTime>("requestTime", (requestId, toUserId, proposedDate) => {

                OnExtraTimeRequested?.Invoke(requestId, toUserId, proposedDate);

            });

            ChatHubProxy.On<int, Guid, DateTime>("offerBook", (requestId, bookOfferId, date) => {

                OnBookOffered?.Invoke(requestId, bookOfferId, date);

            });

            ChatHubProxy.On<int>("returnBook", (requestId) => {

                OnReturnOffered?.Invoke(requestId);

            });

            ChatHubProxy.On<int, Guid, bool>("answerTimeRequest", (requestId, toUserId, answer) => {

                OnTimeRequestAnswered?.Invoke(requestId, toUserId, answer);

            });
        }



        public void SendMessage(string name, string message)

        {

            ChatHubProxy.Invoke("Send", name, message);

        }

        public void Disconnect()
        {
            ChatHubProxy.Invoke("Disconnect");
        }

        public void Connect(string name, Guid appId)

        {

            ChatHubProxy.Invoke("Connect", name, appId);

        }

        public void SendPrivateMessage(string message, Guid toUserId, int requestId)

        {

            ChatHubProxy.Invoke("SendPrivateMessage", toUserId, message, requestId, DateTime.UtcNow);

        }

        public void OfferExchangeBook(string title, string authors, Guid toUserId, int requestId, Guid bookId)
        {
            ChatHubProxy.Invoke("OfferExchangeBook", title, authors, toUserId, requestId, bookId);
        }

        public void RejectExchangeBook(int requestId, Guid toUserId)
        {
            ChatHubProxy.Invoke("RejectExchangeBook", requestId, toUserId);
        }

        public void RequestTime(int requestId, Guid toUserId, DateTime proposedDate)
        {
            ChatHubProxy.Invoke("RequestTime", requestId, toUserId, proposedDate);
        }

        public void AnswerTimeRequest(int requestId, Guid toUserId, bool answer)
        {
            ChatHubProxy.Invoke("AnswerTimeRequest", requestId, toUserId, answer);
        }

        public void OfferBook(int requestId, Guid toUserId, Guid bookOfferId, DateTime date)
        {
            ChatHubProxy.Invoke("OfferBook", requestId, toUserId, bookOfferId, date);
        }

        public void ReturnBook(int requestId, Guid toUserId)
        {
            ChatHubProxy.Invoke("ReturnBook", requestId, toUserId);
        }

        public Task Start()

        {

            return Connection.Start();

        }



        public bool IsConnectedOrConnecting
        {

            get
            {

                return Connection.State != ConnectionState.Disconnected;

            }

        }



        public ConnectionState ConnectionState { get { return Connection.State; } }



        public static async Task<ChatClient> CreateAndStart(string url)

        {

            var client = new ChatClient(url);

            await client.Start();

            return client;

        }



        public event PropertyChangedEventHandler PropertyChanged;



        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)

        {

            var handler = PropertyChanged;

            if (handler != null)

                handler(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}
