using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterString: ROSParameter
    {
        public string value { get; private set; }

        public ROSParameterString(string name, string value) : base(name, ROSParameterType.STRING)
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
            return value;
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
            return new List<byte>() {STRING};
        }

        public override void serializeValue(Message message)
        {
            message.AddString(value);
        }
    }
}