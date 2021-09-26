using DarkRift;
using DarkRift.Server;
using MeatInc.ActionGunnersShared;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MeatInc.ActionGunnersServer.Network.Components
{
    public class NetworkRelay 
    {
        private Dictionary<ushort, List<Action<Message>>> _messageHandlers;
        private IClient _client;

        public NetworkRelay(
            IClient client)
        {
            _client = client;

            _messageHandlers = new Dictionary<ushort, List<Action<Message>>>();
            InitializeMessageHandlers();
        }

        public void Initialize()
        {
            _client.MessageReceived += HandleMessage;
        }


        public void Dispose()
        {
            _client.MessageReceived -= HandleMessage;
        }

        private void InitializeMessageHandlers()
        {
            Type TagsType = typeof(Tags);
            var fieldsInfo = TagsType.GetFields().Where(Type => Type.FieldType == typeof(ushort));

            foreach (FieldInfo field in fieldsInfo)
            {
                ushort tag = (ushort)field.GetValue(null);
                _messageHandlers.Add(tag, new List<Action<Message>>());
            }
        }

        private void HandleMessage(object sender, MessageReceivedEventArgs e)
        {
            using (Message message = e.GetMessage())
            {
                ushort tag = message.Tag;

                List<Action<Message>> list;
                _messageHandlers.TryGetValue(tag, out list);

                foreach (Action<Message> action in list)
                {
                    action?.Invoke(message);
                }

            }
        }

        public void Subscribe(ushort tag, Action<Message> handlerMethod)
        {
            List<Action<Message>> list;
            _messageHandlers.TryGetValue(tag, out list);
            list.Add(handlerMethod);
        }
        public void Unsubscribe(ushort tag, Action<Message> handlerMethod)
        {
            List<Action<Message>> list;
            _messageHandlers.TryGetValue(tag, out list);
            list.Remove(handlerMethod);
        }
    }
}
