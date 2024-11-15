using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterDouble: ROSParameter
    {
        public double value { get; private set; }
        
        public ROSParameterDouble(string name, double value) : base(name, ROSParameterType.DOUBLE)
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
            return value;
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
            return new List<byte>() {DOUBLE};
        }

        public override void serializeValue(Message message)
        {
            message.AddDouble(value);
        }
    }
}