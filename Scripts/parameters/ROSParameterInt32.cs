using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterInt32: ROSParameter
    {
        public int value { get; private set; }

        public ROSParameterInt32(string name, int value) : base(name, ROSParameterType.INT32)
        {
            this.value = value;
        }
        
        public override int getValueInt()
        {
            return value;
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
            throw new NotImplementedException();
        }

        public override List<byte> getTypeArray()
        {
            return new List<byte>() {INT};
        }

        public override void serializeValue(Message message)
        {
            message.AddInt(value);
        }
    }
}