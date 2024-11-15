using System;
using System.Collections.Generic;
using Riptide;
using UnityEngine;

namespace Visus.Robotics.RosBridge.Parameter
{
    public abstract class ROSParameter
    {
        protected const byte NULL = 0;
        protected const byte INT = 1;
        protected const byte BOOL = 2;
        protected const byte STRING = 3;
        protected const byte DOUBLE = 4;
        protected const byte DATE = 5;
        protected const byte BINARY = 6;
        protected const byte LIST_START = 252;
        protected const byte LIST_END = 253;
        protected const byte DICT_START = 254;
        protected const byte DICT_END = 255;
        
        public string name { get; private set; }
        public ROSParameterType type { get; private set; }

        public ROSParameter(string name, ROSParameterType type)
        {
            this.name = name;
            this.type = type;
        }

        public abstract int getValueInt();
        public abstract bool getValueBool();
        public abstract string getValueString();
        public abstract double getValueDouble();
        public abstract DateTime getValueDate();
        public abstract List<ROSParameter> getValueList();
        public abstract byte[] getValueBinary();
        public abstract Dictionary<string, ROSParameter> getValueDictionary();

        public abstract List<byte> getTypeArray();
        public abstract void serializeValue(Message message);
        
        public void serializeToMessage(Message messsage)
        {
            byte[] types = getTypeArray().ToArray();
            messsage.AddBytes(types);
            
            serializeValue(messsage);
        }

        public static ROSParameter deserialize(Message msg)
        {
            string name = msg.GetString();
            byte[] types = msg.GetBytes();
            int pos = 0;
            return readRosParameter(msg, name, types, ref pos);
        }

        private static ROSParameter readRosParameter(Message msg, string name, byte[] types, ref int position)
        {
            switch (types[position++])
            {
                case INT:
                    return new ROSParameterInt32(name,msg.GetInt());
                case BOOL:
                    return new ROSParameterBool(name, msg.GetBool());
                case STRING:
                    return new ROSParameterString(name, msg.GetString());
                case DOUBLE:
                    return new ROSParameterDouble(name, msg.GetDouble());
                case DATE:
                    return new ROSParameterDate(name, msg.GetString());
                case BINARY:
                    return new ROSParameterBinary(name, msg.GetBytes());
                case LIST_START:
                    List<ROSParameter> parameter_list = new List<ROSParameter>();
                    int idx = 0;
                    while (types[position] != LIST_END)
                    {
                        parameter_list.Add(readRosParameter(msg, String.Format("{0}[{1}]", name, idx), types, ref position));
                    }

                    position += 1;

                    return new ROSParameterList(name, parameter_list);
                case DICT_START:
                    Dictionary<string, ROSParameter> parameter_dict = new Dictionary<string, ROSParameter>();
                    while (types[position] != DICT_END)
                    {
                        string childName = String.Format("{0}/{1}", name, msg.GetString());
                        parameter_dict.Add(childName, readRosParameter(msg, childName, types, ref position));
                    }

                    position += 1;

                    return new ROSParameterDictionary(name, parameter_dict);
                default:
                    Debug.LogWarning("Unknown ROS Parameter Type: " + types[position-1]);
                    return null;
            }
        }

    }
}