using System;
using Riptide;

namespace Visus.Robotics.RosBridge
{
    public abstract class ROSMessage
    {
        public abstract void serializeToMessage(Message message, bool includeHeader = true);

        public abstract void deserializeFromMessage(Message message);
        
        protected void AddArrayLength(Message message, int length) {
            if (length <= 0b_0111_1111)
                message.AddByte((byte)length);
            else
            {
                if (length > 0b_0111_1111_1111_1111)
                    throw new ArgumentOutOfRangeException(nameof(length), "Messages do not support auto-inclusion of array lengths for arrays with more than 32767 elements! Either send a smaller array or set the 'includeLength' paremeter to false in the Add method and manually pass the array length to the Get method.");

                length |= 0b_1000_0000_0000_0000;
                message.AddByte((byte)(length >> 8));
                message.AddByte((byte)length);
            }
        }

        protected ushort ReadArrayLength(Message message) {
            byte firstByte = message.GetByte();
            if ((firstByte & 0b_1000_0000) == 0) {
                return firstByte;
            }

            byte secondByte = message.GetByte();
            return (ushort)(((firstByte << 8) | secondByte) & 0b_0111_1111_1111_1111);
        }
    }
}