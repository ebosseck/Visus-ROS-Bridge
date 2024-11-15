using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

namespace Visus.Robotics.RosBridge
{
    public class ROSMessageFactory
    {
        private static Dictionary<string, Func<Message, ROSMessage>> messageConstructors 
            = new Dictionary<string, Func<Message, ROSMessage>>();

        private static Dictionary<Type, string> typeMap = new Dictionary<Type, string>();

        public static void registerMessage<T>(string type, Func<Message, T> constructor) where T: ROSMessage
        {
            if (messageConstructors.ContainsKey(type))
            {
                Debug.LogWarning("Message type is already registered: " + type);
                return;
            }

            typeMap[typeof(T)] = type;
            messageConstructors[type] = constructor;
        }

        public static string mapROSType(Type type)
        {
            return typeMap[type];
        }

        public static ROSMessage deserialize(Message message)
        {
            return deserialize(message.GetString(), message);
        }
        
        public static ROSMessage deserialize(string type, Message message)
        {
            return messageConstructors[type](message);
        }

    }
}