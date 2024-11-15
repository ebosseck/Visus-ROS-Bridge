using System;
using System.Collections.Generic;
using Riptide;

namespace Visus.Robotics.RosBridge.Parameter
{
    public class ROSParameterDate: ROSParameter
    {
        public DateTime value { get; private set; }

        public ROSParameterDate(string name, DateTime value) : base(name, ROSParameterType.DATE)
        {
            this.value = value;
        }
        
        public ROSParameterDate(string name, string value) : base(name, ROSParameterType.DATE)
        {
            this.value = DateTime.Parse(value);
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
            return value;
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
            return new List<byte>() {DATE};
        }

        public override void serializeValue(Message message)
        {
            message.AddString(value.ToString());
        }
    }
}