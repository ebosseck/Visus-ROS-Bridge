using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterBool: ROSParameter
    {
        public bool value { get; private set; }

        public ROSParameterBool(string name, bool value) : base(name, ROSParameterType.BOOLEAN)
        {
            this.value = value;
        }
        
        public override int getValueInt()
        {
            throw new NotImplementedException();
        }

        public override bool getValueBool()
        {
            return value;
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
            return new List<byte>() {BOOL};
        }

        public override void serializeValue(Message message)
        {
            message.AddBool(value);
        }
    }
}