/**----------------------------------------------------------------------------//
@file   : RobotServer.cs

@brief  : Server

@par	: E-Mail
            609043941@qq.com

@par	: history
            2017/03/08 Ver 0.00 Created by Liuhaixia
//-----------------------------------------------------------------------------*/
using LitJson;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;

namespace BenNetwork
{
    class RobotServer
    {
        readonly int PORT = 1307;
        const string DATA = "DATA";
        const string MODULE = "MODULE";
        const string MODULE_HEARTBEAT = "heartbeat";
        const string MODULE_SOCKET_DISCONNECT = "socketDisconnect";

        Socket _listener = null;

        Socket _client = null;

        DateTime _rcvHeartBeatTime;
        Timer _heartExcepTimer = null;

        public Queue<string> MessageQueue = new Queue<string>();

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public void Init(Action successCallback, Action errorCallback)
        {
            if (createServer())
            {
                if (successCallback != null)
                    successCallback();
            }
            else
            {
                if (errorCallback != null)
                    errorCallback();
            }
        }

        /// <summary>
        /// 获取本机ip
        /// </summary>
        /// <returns></returns>
        string getLocalAddress()
        {
            string address = "127.0.0.1";
            IPHostEntry ipHostEntry = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in ipHostEntry.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    address = ip.ToString();
                    break;
                }
            }
            return address;
        }

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        bool createServer()
        {
            _listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                _listener.Bind(new IPEndPoint(IPAddress.Any, PORT));
                _listener.Listen(5);
                _listener.BeginAccept(new AsyncCallback(listenClient), _listener);
            }
            catch (Exception ex)
            {
                if (_listener != null)
                    _listener.Close();
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "连接错误: " + ex.Message);
                return false;
            }

            Console.WriteLine("创建本地服务器成功");
            return true;
        }

        /// <summary>
        /// listen client 
        /// </summary>
        /// <param name="ar"></param>
        void listenClient(IAsyncResult ar)
        {
            NetCache cache = new NetCache();
            try
            {
                Console.WriteLine("开始监听Client连接");
                cache._socket = _listener.EndAccept(ar);
                cache._socket.SendTimeout = 3;
                cache._socket.ReceiveTimeout = 3;

                _client = cache._socket;

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "有Client连接 remote ip: "+IPAddress.Parse(((IPEndPoint)cache._socket.RemoteEndPoint).Address.ToString()));

                cache._socket.BeginReceive(cache.Data, cache.Length, NetCache.DEFAULE_CACHE_LEN-cache.Length, SocketFlags.None, new AsyncCallback(receiveData), cache);
            }
            catch (Exception ex)
            {
                if (cache._socket != null)
                {
                    cache._socket.Close();
                }
                cache.Clear();

                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "监听成功，开始获取数据错误，关闭Socket，清空Cache，错误信息: " + ex.Message);
            }

            if (_listener!=null)
                _listener.BeginAccept(new AsyncCallback(listenClient), _listener);
        }

        /// <summary>
        /// receive socket data
        /// </summary>
        /// <param name="ar"></param>
        void receiveData(IAsyncResult ar)
        {
            NetCache cache = (NetCache)ar.AsyncState;
            try
            {
                int read = cache._socket.EndReceive(ar);

                if (read < 1)
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "获取信息为空");
                    return;
                }
                cache.Length += read;
                cache.Parse(parseAllReceivePacket);
                
                cache._socket.BeginReceive(cache.Data, cache.Length, NetCache.DEFAULE_CACHE_LEN - cache.Length, SocketFlags.None, new AsyncCallback(receiveData), cache);
            }
            catch (Exception ex)
            {
                if (cache._socket != null)
                {
                    cache._socket.Close();
                }
                cache.Clear();
                Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "获取数据成功，再次接收错误，关闭Socket，清空Cache，错误信息： " + ex.Message);
            }
        }

        /// <summary>
        /// 解析所有收到的包
        /// </summary>
        /// <param name="packetList"></param>
        void parseAllReceivePacket(List<NetPacket> packetList)
        {
            foreach (NetPacket packet in packetList)
            {
                ParseReceive(packet.Body);
                //MessageQueue.Enqueue(packet.Body);
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        void sendHeartBeat()
        {
            JsonData heartJson = new JsonData();
            heartJson[MODULE] = MODULE_HEARTBEAT;
            SendData(heartJson.ToJson());
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        public void SendData(string message)
        {
            if (_client != null && message != null && message != "")
            {
                NetPacket packet = new NetPacket(message, 5);
                try
                {
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "发送数据：" + packet.Body);
                    _client.Send(packet.ToBytes());
                }
                catch (Exception ex)
                {
                    _client.Close();
                    _client = null;
                    Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + "发送信息错误，关闭所有Socket，重新启动，错误信息： " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Parse Receive Info
        /// </summary>
        /// <param name="message"></param>
        public void ParseReceive(string message)
        {
            Console.WriteLine("Message : " + message);

            JsonData totalJson = JsonMapper.ToObject<JsonData>(message);
            string module = Convert.ToString(totalJson[MODULE]);
            if (module != "")
            {
                switch (module)
                {
                    case MODULE_SOCKET_DISCONNECT:
                        {
                            // disconnect operation

                            break;
                        }
                    case MODULE_HEARTBEAT:
                        {
                            startHeartExcepTimer();

                            sendHeartBeat();
                            //startSendHeartBeatTimer();
                            break;
                        }
                    default:
                        {
                            // Handle Receive Message
                            break;
                        }
                }
            }
        }

        public void OnDestory()
        {
            if(_client != null && _client.Connected)
            {
                try
                {
                    _client.Shutdown(SocketShutdown.Both);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("关闭Client异常： " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + " | Exception: " + ex.Message);
                }
                finally {
                    _client.Close();
                    _client = null;
                }
            }

            if (_listener != null) //&& _listener.Connected)
            {
                _listener.Close();
                _listener = null;
            }

            closeHeartExcepTimer();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region 计时器

        /// <summary>
        /// 初始化心跳包异常计时器
        /// </summary>
        void startHeartExcepTimer()
        {
            if (_heartExcepTimer == null)
            {
                _heartExcepTimer = new Timer(6 * 1000);
                _heartExcepTimer.Elapsed += new ElapsedEventHandler(startHeartExcepTimerCallback);
                _heartExcepTimer.AutoReset = true;
                _heartExcepTimer.Enabled = true;
            }

            _heartExcepTimer.Start();

            _rcvHeartBeatTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            Console.WriteLine("收到心跳包：" + _rcvHeartBeatTime.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
        }

        /// <summary>
        /// 心跳包异常计时检测
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        void startHeartExcepTimerCallback(object sender, ElapsedEventArgs args)
        {
            Console.WriteLine("心跳包检测时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            TimeSpan ts = DateTime.Now - _rcvHeartBeatTime;
            if (ts.Seconds > 6)
            {
                if (_client != null)
                {
                    try
                    {
                        _client.Shutdown(SocketShutdown.Both);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("心跳包检测超时, 关闭Client异常： " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + " | Exception: " + ex.Message);
                    }
                    finally
                    {
                        _client.Close();
                        _client = null;
                    }
                }

                _heartExcepTimer.Stop();

                JsonData socketDisconnect = new JsonData();
                socketDisconnect[MODULE] = MODULE_SOCKET_DISCONNECT;
                MessageQueue.Enqueue(socketDisconnect.ToJson());

                Console.WriteLine("心跳包断开：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff") + " || 上次>>>心跳包: " + _rcvHeartBeatTime.ToString("yyyy-MM-dd HH:mm:ss:ffff"));
            }
        }

        /// <summary>
        /// 关闭心跳包异常计时器
        /// </summary>
        void closeHeartExcepTimer()
        {
            if (_heartExcepTimer != null)
            {
                _heartExcepTimer.Stop();
                _heartExcepTimer.Dispose();
                _heartExcepTimer.Close();
                _heartExcepTimer = null;
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
