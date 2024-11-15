using Riptide;
using Visus.Robotics.RosBridge.Parameter;

namespace Visus.Robotics.RosBridge
{
    public class BridgeMessageFactory
    {
        public static Message unifiedLogMessage(int secs, int nsecs, string origin, byte lvl,
            string name, string msg_content, string file, string function, int line, string[] topics)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 0);
            
            msg.AddInt(secs);
            msg.AddInt(nsecs);
            msg.AddString(origin);
            msg.AddByte(lvl);
            msg.AddString(name);
            msg.AddString(msg_content);
            msg.AddString(file);
            msg.AddString(function);
            msg.AddInt(line);
            msg.AddStrings(topics);

            return msg;
        }

        public static Message listParameters(string requestID)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 1);

            msg.AddString(requestID);
            
            return msg;
        }
        
        public static Message requestParameter(string name, string requestID)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 2);

            msg.AddString(name);
            msg.AddString(requestID);
            
            return msg;
        }
        
        public static Message setParameter(string name, ROSParameter parameter)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 3);

            msg.AddString(name);
            parameter.serializeToMessage(msg);
            
            return msg;
        }
        
        public static Message listTopics(string topic_namespace, string requestID)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 4);

            msg.AddString(topic_namespace);
            msg.AddString(requestID);
            
            return msg;
        }
        
        public static Message requestMessageStructure(string messageName, string requestID)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 5);

            msg.AddString(messageName);
            msg.AddString(requestID);
            
            return msg;
        }
        
        public static Message requestSubscription(string topic, string type)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 6);

            msg.AddString(topic);
            msg.AddString(type);
            
            return msg;
        }
        
        public static Message requestUnsubscription(string topic)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 7);

            msg.AddString(topic);
            
            return msg;
        }
        
        public static Message createPublisher(string topic, string type, int queue, bool latch)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 8);

            msg.AddString(topic);
            msg.AddString(type);
            msg.AddInt(queue);
            msg.AddBool(latch);
            
            return msg;
        }
        
        public static Message rosMessage(string topic, ROSMessage rosmsg)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 9);

            msg.AddString(topic);
            rosmsg.serializeToMessage(msg);
            return msg;
        }
        
        public static Message registerService(string topic, string type)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 10);

            msg.AddString(topic);
            msg.AddString(type);
            
            return msg;
        }
        
        public static Message callService(string name, ROSMessage payload, string requestID)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 11);

            msg.AddString(name);
            payload.serializeToMessage(msg);
            msg.AddString(requestID);
            
            return msg;
        }
        
        public static Message serviceResponse(string name, ROSMessage payload, string requestID)
        {
            Message msg = Message.Create(MessageSendMode.Unreliable, 12);

            msg.AddString(name);
            payload.serializeToMessage(msg);
            msg.AddString(requestID);
            
            return msg;
        }
    }
}