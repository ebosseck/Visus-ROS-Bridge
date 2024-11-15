using System.Collections.Generic;
using Riptide;

using Visus.Robotics.RosBridge.Parameter;

namespace Visus.Robotics.RosBridge
{
    public class BridgeMessageUnpacker
    {
        public static void unpackLogMessage(Message msg, out int time_sec, out int time_ns, out string origin, out byte level,
            out string name, out string message, out string file, out string function, out int line, out string[] topics)
        {
            time_sec = msg.GetInt();
            time_ns = msg.GetInt();
            origin = msg.GetString();
            level = msg.GetByte();
            name = msg.GetString();
            message = msg.GetString();
            file = msg.GetString();
            function = msg.GetString();
            line = msg.GetInt();
            topics = msg.GetStrings();
        }

        public static void unpackParameterListResponse(Message msg, out string[] parameters, out string requestID)
        {
            parameters = msg.GetStrings();
            requestID = msg.GetString();
        }
        
        public static void unpackParameterResponse(Message msg, out string parameterName, out ROSParameter parameter, out string requestID)
        {
            parameterName = msg.GetString();

            parameter = ROSParameter.deserialize(msg);

            requestID = msg.GetString();
        }
        
        public static void unpackTopicListResponse(Message msg, out string[][] topics, out string requestID)
        {
            List<string[]> topic_list = new List<string[]>();
            int len = ReadArrayLength(msg);
            for (int i = 0; i < len; i++) {
                topic_list.Add(msg.GetStrings(2));
            }

            topics = topic_list.ToArray();

            requestID = msg.GetString();
        }
        
        public static void unpackMessageStructure(Message msg, out string name, out bool isExpanded, 
            out string[] types, out string requestID)
        {
            name = msg.GetString();
            isExpanded = msg.GetBool();
            types = msg.GetStrings();
            requestID = msg.GetString();
        }
        
        public static void unpackSubscriptionAck(Message msg, out string topic, out int status, out string message)
        {
            topic = msg.GetString();
            status = msg.GetInt();
            message = msg.GetString();
        }
        
        public static void unpackUnsubscriptionAck(Message msg, out string topic, out int status, out string message)
        {
            topic = msg.GetString();
            status = msg.GetInt();
            message = msg.GetString();
        }
        
        public static void unpackROSMessage(Message msg, out string topic, out ROSMessage message)
        {
            topic = msg.GetString();
            message = ROSMessageFactory.deserialize(msg);

        }
        
        public static void unpackCallService(Message msg, out string name, out ROSMessage message, out string requestID)
        {
            name = msg.GetString();
            message = ROSMessageFactory.deserialize(msg);
            requestID = msg.GetString();
        }
        
        public static void unpackServiceResponse(Message msg, out string name, out ROSMessage message, out string requestID)
        {
            name = msg.GetString();
            message = ROSMessageFactory.deserialize(msg);
            requestID = msg.GetString();
        }
        
        public static void unpackTextBroadcast(Message msg, out string message, out string format, out string origin)
        {
            message = msg.GetString();
            format = msg.GetString();
            origin = msg.GetString();
        }
        
        protected static ushort ReadArrayLength(Message message) {
            byte firstByte = message.GetByte();
            if ((firstByte & 0b_1000_0000) == 0) {
                return firstByte;
            }

            byte secondByte = message.GetByte();
            return (ushort)(((firstByte << 8) | secondByte) & 0b_0111_1111_1111_1111);
        }
    }
}