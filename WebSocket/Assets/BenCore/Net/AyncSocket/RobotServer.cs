//*************************************************
//File:		RobotServer.cs
//
//Brief:    Robot Server
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/12/11 Created by Liuhaixia
//*************************************************

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Timers;
using Ben;
using LitJson;
using UnityEngine;

namespace Ben.Net {
    class RobotServer {
        readonly Int32 _port = 1307;

        String _address = "";

        Socket _listener = null;

        Socket _client = null;

        DateTime _rcvHeartBeatTime;

        Timer _heartExcepTimer = null;

        Action<String, String> _parseReceiveCallback = null;

        Queue<String> _messageQueue = new Queue<String> ();

        public RobotServer (Action successCallback, Action errorCallback, Action<String,String> parseReceiveCallback) {
            if (createServer ()) {
                if (successCallback != null) {
                    successCallback ();
                }

                if (parseReceiveCallback != null) {
                    this._parseReceiveCallback = parseReceiveCallback;
                }
            } else {
                if (errorCallback != null) {
                    errorCallback ();
                }
            }
        }

        public void OnDestory () {
            if (_client != null && _client.Connected) {
                _client.Shutdown (SocketShutdown.Both);
                _client.Close ();
                _client = null;
            }
            if (_listener != null) {
                _listener.Close ();
                _listener = null;
            }

            closeHeartExcepTimer ();
        }

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Boolean createServer () {
            _listener = new Socket (AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                Debug.Log ("开始创建本地服务器");
                IPEndPoint ipe = new IPEndPoint (IPAddress.Any, _port);

                _listener.Bind (ipe);
                Debug.Log ("开始创建本地服务器， Bind成功");
                _listener.Listen (5);
                Debug.Log ("开始创建本地服务器， Listen成功");
                _listener.BeginAccept (new AsyncCallback (listenClient), _listener);
                Debug.Log ("开始创建本地服务器， BeginAccept成功");
            } catch (Exception ex) {
                if (_listener != null)
                    _listener.Close ();
                Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "连接错误: " + ex.Message);
                return false;
            }

            Debug.Log ("创建本地服务器成功， Address: " + _address);
            return true;
        }

        /// <summary>
        /// listen client 
        /// </summary>
        /// <param name="ar"></param>
        void listenClient (IAsyncResult ar) {
            NetCache cache = new NetCache ();
            try {
                Debug.Log ("创建本地服务器，开始监听Client连接");
                cache._socket = _listener.EndAccept (ar);
                cache._socket.SendTimeout = 3;
                cache._socket.ReceiveTimeout = 3;

                _client = cache._socket;

                Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "有Client连接 remote ip: " + IPAddress.Parse (((IPEndPoint) cache._socket.RemoteEndPoint).Address.ToString ()));

                cache._socket.BeginReceive (cache.cache, cache.length, NetCache.DEFAULE_CACHE_LEN - cache.length, SocketFlags.None, new AsyncCallback (receiveData), cache);
            } catch (Exception ex) {
                if (cache._socket != null) {
                    cache._socket.Close ();
                }
                cache.Clear ();

                Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "监听成功，开始获取数据错误，关闭Socket，清空Cache，错误信息: " + ex.Message);
            }

            _listener.BeginAccept (new AsyncCallback (listenClient), _listener);
        }

        /// <summary>
        /// receive socket data
        /// </summary>
        void receiveData (IAsyncResult ar) {
            NetCache cache = (NetCache) ar.AsyncState;
            try {
                Int32 read = cache._socket.EndReceive (ar);

                if (read < 1) {
                    Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "获取信息为空");
                    return;
                }
                cache.length += read;
                cache.parse (parseAllReceivePacket);

                cache._socket.BeginReceive (cache.cache, cache.length, NetCache.DEFAULE_CACHE_LEN - cache.length, SocketFlags.None, new AsyncCallback (receiveData), cache);
            } catch (Exception ex) {
                if (cache._socket != null) {
                    cache._socket.Close ();
                }
                cache.Clear ();
                Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "获取数据成功，再次接收错误，关闭Socket，清空Cache，错误信息： " + ex.Message);
            }
        }

        /// <summary>
        /// 解析所有收到的包
        /// </summary>
        /// <param name="packetList"></param>
        void parseAllReceivePacket (List<LinuxPacket> packetList) {
            foreach (LinuxPacket packet in packetList) {
                _messageQueue.Enqueue (packet.body);
            }
        }

        /// <summary>
        /// 发送心跳包
        /// </summary>
        void sendHeartBeat () {
            JsonData heartJson = new JsonData ();
            heartJson[UpperNameConst.MODULE] = "heartbeat";
            SendData (heartJson.ToJson ());
        }

        /// <summary>
        /// 发送信息
        /// </summary>
        public void SendData (String message) {
            Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "数据：" + message);

            if (_client != null && message != null && message != "") {
                LinuxPacket packet = new LinuxPacket (message, 5);
                try {
                    _client.Send (packet.toBytes ());
                    Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "开始发送数据：" + packet.body);
                } catch (Exception ex) {
                    _client.Close ();
                    _client = null;
                    Debug.Log (System.DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + "发送信息错误，关闭所有Socket，重新启动，错误信息： " + ex.Message);
                }
            }
        }

        /// <summary>
        /// Parse Receive Info
        /// </summary>
        /// <param name="message"></param>
        public void ParseReceive () {

            while (_messageQueue.Count > 0) {

                JsonData totalJson = new JsonData ();

                try {
                    totalJson = JsonMapper.ToObject (_messageQueue.Dequeue ());
                } catch (Exception ex) {
                    Debug.LogError ("ParseReceive >> Convert To JsonData Exception: " + ex.Message);
                }

                try {
                    String module = totalJson.GetString (UpperNameConst.MODULE);
                    if (module != "") {
                        switch (module) {
                            case CamelNameConst.SOCKET_DISCONNECT:
                                {
                                    // if (GameDataManager.Inst.isEnterAnimation) {
                                    //     EventMgr.Inst.Fire (EventID.Control_PersonAnima, new EventArg (CamelNameConst.SOCKET_DISCONNECT));
                                    // }
                                    break;
                                }
                            case CamelNameConst.HEARTBEAT:
                                {
                                    startHeartExcepTimer ();
                                    sendHeartBeat ();
                                    break;
                                }
                            default:
                                {
                                    String dataInfo = "";
                                    if (totalJson.ContainKey (UpperNameConst.DATA)) {
                                        dataInfo = totalJson[UpperNameConst.DATA].ToJson ();
                                    }

                                    if (false == dataInfo.IsNullOrEmpty ()) {
                                        if (_parseReceiveCallback != null) {
                                            _parseReceiveCallback (module, dataInfo);
                                        } else {
                                            Debug.LogError ("Parse receive callback is null!");
                                        }
                                    } else {
                                        Debug.LogError ("Data info is null or empty!");
                                    }

                                    break;
                                }
                        }
                    }
                } catch (Exception ex) {
                    Debug.LogError ("ParseReceive >> Parse Data Exception: " + ex.Message);
                }

            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        #region 计时器

        /// <summary>
        /// 初始化心跳包异常计时器
        /// </summary>
        void startHeartExcepTimer () {
            if (_heartExcepTimer == null) {
                _heartExcepTimer = new Timer (6 * 1000);
                _heartExcepTimer.Elapsed += new ElapsedEventHandler (startHeartExcepTimerCallback);
                _heartExcepTimer.AutoReset = true;
                _heartExcepTimer.Enabled = true;
            }

            _heartExcepTimer.Start ();

            _rcvHeartBeatTime = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Millisecond);
            Debug.Log ("收到心跳包：" + _rcvHeartBeatTime.ToString ("yyyy-MM-dd HH:mm:ss:ffff"));
        }

        /// <summary>
        /// 心跳包异常计时检测
        /// </summary>
        void startHeartExcepTimerCallback (System.Object sender, ElapsedEventArgs args) {
            Debug.Log ("心跳包检测时间：" + DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff"));
            TimeSpan ts = DateTime.Now - _rcvHeartBeatTime;
            if (ts.Seconds > 6) {
                if (_client != null) {
                    _client.Shutdown (SocketShutdown.Both);
                    _client.Close ();
                    _client = null;
                }

                _heartExcepTimer.Stop ();

                JsonData socketDisconnect = new JsonData ();
                socketDisconnect[UpperNameConst.MODULE] = CamelNameConst.SOCKET_DISCONNECT;
                _messageQueue.Enqueue (socketDisconnect.ToJson ());

                Debug.Log ("心跳包断开：" + DateTime.Now.ToString ("yyyy-MM-dd HH:mm:ss:ffff") + " || 上次>>>心跳包: " + _rcvHeartBeatTime.ToString ("yyyy-MM-dd HH:mm:ss:ffff"));
            }
        }

        /// <summary>
        /// 关闭心跳包异常计时器
        /// </summary>
        void closeHeartExcepTimer () {
            if (_heartExcepTimer != null) {
                _heartExcepTimer.Stop ();
                _heartExcepTimer.Dispose ();
                _heartExcepTimer.Close ();
                _heartExcepTimer = null;
            }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    }
}