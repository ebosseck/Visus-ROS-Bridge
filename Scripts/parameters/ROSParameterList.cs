using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterList: ROSParameter
    {
        public List<ROSParameter> value { get; private set; }

        public ROSParameterList(string name, List<ROSParameter> value) : base(name, ROSParameterType.LIST)
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
            return value;
        }

        public override byte[] getValueBinary()
        {
            throw new NotImplementedException();
        }

        public override Dictionary<string, ROSParameter> getValueDictionary()
        {
            throw new NotImplementedException();
        }

        public override List<byte> getTypeArray()
        {
            List<byte> types = new List<byte>();
            types.Add(LIST_START);

            foreach (ROSParameter val in value)
            {
                types.AddRange(val.getTypeArray());
            }
            
            types.Add(LIST_END);

            return types;
        }

        public override void serializeValue(Message message)
        {
            message.AddInt(value.Count);
            foreach (ROSParameter val in value)
            {
                val.serializeValue(message);
            }
        }
    }
}