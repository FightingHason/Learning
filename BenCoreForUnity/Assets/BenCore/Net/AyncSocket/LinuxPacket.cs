/**----------------------------------------------------------------------------//
@file   : LinuxPacket.cs

@brief  : Linux数据包

@par	: E-Mail
            liu_haixia@gowild.cn

@par	: Copyright(C) Gowild Robotics Inc.

@par	: history
            2017/03/09 Ver 0.00 Created by Duanbangchao
//-----------------------------------------------------------------------------*/
using System;
using System.Text;

namespace Ben.Net {
    public class LinuxPacket {
        public Int32 length = 0;
        public Int16 msgid = 0;
        public String body = null;

        public LinuxPacket () {

        }

        public LinuxPacket (String data, Int16 msgId) {
            body = data;
            msgid = msgId;
        }

        public LinuxPacket (Byte[] data, Int32 len) {
            Int32 len1 = BitConverter.ToInt32 (data, 0);
            //Int32 msgid = BitConverter.ToUInt16(data, 4);
            Byte[] tmp = new Byte[len1 - 6];
            Array.Copy (data, 6, tmp, 0, len1 - 6);
            body = BitConverter.ToString (data);
        }
        public String toString () {
            return body;
        }
        public Int16 getMSGID () {
            return msgid;
        }

        public void setMSGID (Int16 id) {
            msgid = id;
        }

        public String getBody () {
            return body;
        }

        public void setBody (String body) {
            this.body = body;
        }

        public Byte[] toBytes () {
            Int32 total = body.Length + 2 + 4;
            Byte[] result = new Byte[total];
            Byte[] temp = BitConverter.GetBytes (total);
            temp.CopyTo (result, 0);
            temp = BitConverter.GetBytes (msgid);
            temp.CopyTo (result, 4);
            temp = Encoding.Default.GetBytes (body);
            temp.CopyTo (result, 6);
            return result;
        }
    }
}