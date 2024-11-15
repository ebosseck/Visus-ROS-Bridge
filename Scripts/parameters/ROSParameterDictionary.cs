using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterDictionary: ROSParameter
    {
        public Dictionary<string, ROSParameter> value { get; private set; }

        public ROSParameterDictionary(string name, Dictionary<string, ROSParameter> value) : base(name, 
            ROSParameterType.DICTIONARY)
        {
            this.value = value;
        }

        public override int getValueInt()
        {
            throw new NotImplementedException();
        }

        public override bool getValueBool()
        {
            throw new NotImplementedException();
        }

        public override string getValueString()
        {
            throw new NotImplementedException();
        }

        public override double getValueDouble()
        {
            throw new NotImplementedException();
        }

        public override DateTime getValueDate()
        {
            throw new NotImplementedException();
        }

        public override List<ROSParameter> getValueList()
        {
            throw new NotImplementedException();
        }

        public override byte[] getValueBinary()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, ROSParameter> getValueDictionary()
        {
            return value;
        }

        public override List<byte> getTypeArray()
        {
            List<byte> types = new List<byte>();
            types.Add(DICT_START);

            foreach (string key in value.Keys)
            {
                types.AddRange(value[key].getTypeArray());
            }
            
            types.Add(DICT_END);

            return types;
        }

        public override void serializeValue(Message message)
        {
            message.AddInt(value.Count);
            foreach (string key in value.Keys)
            {
                message.AddString(key);
                value[key].serializeValue(message);
            }
        }
    }
}