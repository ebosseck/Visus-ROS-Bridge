using System;
using Riptide;
using Riptide.Transports;

namespace Visus.Robotics.RosBridge
{
    public class ROSNetClient : Client
    {
        new public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public ROSNetClient(IClient transport, string logName = "ROSBRIDGE-CLIENT") : base(transport, logName)
        {
        }

        public ROSNetClient(string logName = "ROSBRIDGE-CLIENT") : base(logName)
        {
        }

        protected override void OnMessageReceived(Message message)
        {
            ushort num = message.GetUShort();
            EventHandler<MessageReceivedEventArgs> messageReceived = this.MessageReceived;
            if (messageReceived != null)
                messageReceived((object) this, new MessageReceivedEventArgs(this.Connection, num, message));
        }
    }
}