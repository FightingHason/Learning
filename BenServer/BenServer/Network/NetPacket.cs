/**----------------------------------------------------------------------------//
@file   : NetPacket.cs

@brief  : Net数据包

@par	: E-Mail
            609043941@qq.com

@par	: history
            2017/03/09 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System;
using System.Text;

namespace BenNetwork
{
    public class NetPacket
    {
        public int Length { get; set; }
        public int MsgID { get; set; }
        public string Body { get; set; }

        public NetPacket()
        {
            Length = 0;
            MsgID = 5;
            Body = null;
        }

        public NetPacket(byte[] dataArray) : this()
        {
            int length = BitConverter.ToInt32(dataArray, 0);
            int msgid = BitConverter.ToUInt16(dataArray, 4);
            byte[] tmp = new byte[length - 6];
            Array.Copy(dataArray, 6, tmp, 0, length - 6);
            Body = BitConverter.ToString(tmp);
        }

        public NetPacket(string data, ushort msgId) : this()
        {
            Body = data;
            MsgID = msgId;
        }

        /// <summary>
        /// Convert to Byte(Length + MsgID + Body)
        /// </summary>
        public byte[] ToBytes()
        {
            int total = Body.Length + 2 + 4;
            byte[] result = new byte[total];
            byte[] temp = BitConverter.GetBytes(total);
            temp.CopyTo(result, 0);
            temp = BitConverter.GetBytes(MsgID);
            temp.CopyTo(result, 4);
            temp = Encoding.Default.GetBytes(Body);
            temp.CopyTo(result, 6);
            return result;
        }

    }
}
