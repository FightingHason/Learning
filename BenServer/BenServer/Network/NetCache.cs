/**----------------------------------------------------------------------------//
@file   : NetCache.cs

@brief  : 数据包缓存

@par	: E-Mail
            609043941@qq.com

@par	: history
            2017/03/09 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace BenNetwork
{
    public class NetCache
    {
        public const int DEFAULE_CACHE_LEN = 1024 * 50;
        public Socket _socket = null;

        public int Length { get; set; }
        public byte[] Data { get; set; }

        public NetCache()
        {
            Length = 0;
            Data = new byte[DEFAULE_CACHE_LEN];
        }

        public void Parse(Action<List<NetPacket>> callback)
        {
            List<NetPacket> packetList = new List<NetPacket>();

            int count = BitConverter.ToInt32(Data, 0);

            while (count <= Length)
            {
                NetPacket packet = new NetPacket();
                packet.Length = count;
                packet.MsgID = BitConverter.ToUInt16(Data, 4);
                byte[] data = new byte[count - 6];
                Array.Copy(Data, 6, data, 0, count - 6);
                packet.Body = Encoding.Default.GetString(data);

                packetList.Add(packet);
                Console.WriteLine("Count: " + count + " |Length: " + Length + " |Packet Body:" + packet.Body);

                Length -= count;
                if (Length > 0)
                {
                    byte[] ret = new byte[Length];
                    Array.Copy(Data, count, ret, 0, Length);
                    Array.Clear(Data, 0, DEFAULE_CACHE_LEN);
                    ret.CopyTo(Data, 0);
                    if (Length > 6)
                    {
                        count = BitConverter.ToInt32(Data, 0);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            Console.WriteLine("数据解析成功");

            if (callback != null)
                callback(packetList);
        }

        public void Clear()
        {
            Length = 0;
            Array.Clear(Data, 0, Data.Length);
        }
    }
}
