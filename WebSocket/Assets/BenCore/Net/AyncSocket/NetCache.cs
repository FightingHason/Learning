/**----------------------------------------------------------------------------//
@file   : NetCache.cs

@brief  : 数据包缓存

@par	: E-Mail
            liu_haixia@gowild.cn

@par	: Copyright(C) Gowild Robotics Inc.

@par	: history
            2017/03/09 Ver 0.00 Created by Duanbangchao
//-----------------------------------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

namespace Ben.Net {
    public class NetCache {
        public const Int32 DEFAULE_CACHE_LEN = 1024 * 50;
        public Int32 length = 0;
        public Byte[] cache = null;
        // 使用数据流的Socket
        public Socket _socket = null;

        public NetCache () {
            cache = new Byte[DEFAULE_CACHE_LEN];
        }

        public Int32 getCacheCapacity () {
            return DEFAULE_CACHE_LEN;
        }

        public Int32 getLength () {
            return length;
        }

        public void parse (Action<List<LinuxPacket>> callback) {
            List<LinuxPacket> packetList = new List<LinuxPacket> ();

            Int32 count = BitConverter.ToInt32 (cache, 0);

            while (count <= length) {
                LinuxPacket packet = new LinuxPacket ();
                packet.length = count;
                packet.msgid = BitConverter.ToInt16 (cache, 4);
                Byte[] data = new Byte[count - 6];
                Array.Copy (cache, 6, data, 0, count - 6);
                packet.body = Encoding.Default.GetString (data);

                packetList.Add (packet);
                Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "Count: " + count + " |Length: " + length + " |Packet Body:" + packet.body);

                length -= count;
                if (length > 0) {
                    Byte[] ret = new Byte[length];
                    Array.Copy (cache, count, ret, 0, length);
                    Array.Clear (cache, 0, DEFAULE_CACHE_LEN);
                    ret.CopyTo (cache, 0);
                    if (length > 6) {
                        count = BitConverter.ToInt32 (cache, 0);
                    } else {
                        break;
                    }
                } else {
                    break;
                }
            }

            Debug.Log ("数据解析成功");

            if (callback != null)
                callback (packetList);
        }

        public void Clear () {
            length = 0;
            Array.Clear (cache, 0, cache.Length);
        }
    }
}