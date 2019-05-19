//*************************************************
//File:		WebSocket.cs
//
//Brief:    Global Basic Const
//
//Author:   Liuhaixia
//
//E-Mail:   609043941@qq.com
//
//History:  2017/11/06 Created by Liuhaixia
//*************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Ben.WebSocket.Frames;
using Ben.WebSocket.Extensions;

namespace Ben.WebSocket
{
    public delegate void OnWebSocketOpenDelegate(WebSocket webSocket);
    public delegate void OnWebSocketMessageDelegate(WebSocket webSocket, string message);
    public delegate void OnWebSocketBinaryDelegate(WebSocket webSocket, byte[] data);
    public delegate void OnWebSocketClosedDelegate(WebSocket webSocket, UInt16 code, string message);
    public delegate void OnWebSocketErrorDelegate(WebSocket webSocket, Exception ex);
    public delegate void OnWebSocketErrorDescriptionDelegate(WebSocket webSocket, string reason);
    public delegate void OnWebSocketIncompleteFrameDelegate(WebSocket webSocket, WebSocketFrameReader frame);

    public sealed class WebSocket
    {
#region Properties

        /// <summary>
        /// The connection to the WebSocket server is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
#if (!UNITY_WEBGL || UNITY_EDITOR)
                return webSocket != null && !webSocket.IsClosed;
#endif
            }
        }

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        /// Set to true to start a new thread to send Pings to the WebSocket server
        /// </summary>
        public bool StartPingThread { get; set; }

        /// <summary>
        /// The delay between two Pings in millisecs. Minimum value is 100, default is 1000.
        /// </summary>
        public int PingFrequency { get; set; }

        /// <summary>
        /// The internal HTTPRequest object.
        /// </summary>
        public HTTPRequest InternalRequest { get; private set; }

        /// <summary>
        /// IExtension implementations the plugin will negotiate with the server to use.
        /// </summary>
        public IExtension[] Extensions { get; private set; }
#endif

        /// <summary>
        /// Called when the connection to the WebSocket server is estabilished.
        /// </summary>
        public OnWebSocketOpenDelegate OnOpen;

        /// <summary>
        /// Called when a new textual message is received from the server.
        /// </summary>
        public OnWebSocketMessageDelegate OnMessage;

        /// <summary>
        /// Called when a new binary message is received from the server.
        /// </summary>
        public OnWebSocketBinaryDelegate OnBinary;

        /// <summary>
        /// Called when the WebSocket connection is closed.
        /// </summary>
        public OnWebSocketClosedDelegate OnClosed;

        /// <summary>
        /// Called when an error is encountered. The Exception parameter may be null.
        /// </summary>
        public OnWebSocketErrorDelegate OnError;

        /// <summary>
        /// Called when an error is encountered. The parameter will be the description of the error.
        /// </summary>
        public OnWebSocketErrorDescriptionDelegate OnErrorDesc;

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        /// Called when an incomplete frame received. No attemp will be made to reassemble these fragments internally, and no reference are stored after this event to this frame.
        /// </summary>
        public OnWebSocketIncompleteFrameDelegate OnIncompleteFrame;
#endif

        #endregion

        #region Private Fields

#if (!UNITY_WEBGL || UNITY_EDITOR)
        /// <summary>
        /// Indicates wheter we sent out the connection request to the server.
        /// </summary>
        private bool requestSent;

        /// <summary>
        /// The internal WebSocketResponse object
        /// </summary>
        private WebSocketResponse webSocket;
#endif

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a WebSocket instance from the given uri.
        /// </summary>
        /// <param name="uri">The uri of the WebSocket server</param>
        public WebSocket(Uri uri)
            :this(uri, string.Empty, string.Empty)
        {
#if (!UNITY_WEBGL || UNITY_EDITOR)
            this.Extensions = new IExtension[] { new PerMessageCompression(/*compression level: */           Decompression.Zlib.CompressionLevel.Default,
                                                                           /*clientNoContextTakeover: */     false,
                                                                           /*serverNoContextTakeover: */     false,
                                                                           /*clientMaxWindowBits: */         Decompression.Zlib.ZlibConstants.WindowBitsMax,
                                                                           /*desiredServerMaxWindowBits: */  Decompression.Zlib.ZlibConstants.WindowBitsMax,
                                                                           /*minDatalengthToCompress: */     5) };
#endif
        }

        /// <summary>
        /// Creates a WebSocket instance from the given uri, protocol and origin.
        /// </summary>
        /// <param name="uri">The uri of the WebSocket server</param>
        /// <param name="origin">Servers that are not intended to process input from any web page but only for certain sites SHOULD verify the |Origin| field is an origin they expect.
        /// If the origin indicated is unacceptable to the server, then it SHOULD respond to the WebSocket handshake with a reply containing HTTP 403 Forbidden status code.</param>
        /// <param name="protocol">The application-level protocol that the client want to use(eg. "chat", "leaderboard", etc.). Can be null or empty string if not used.</param>
        public WebSocket(Uri uri, string origin, string protocol
#if (!UNITY_WEBGL || UNITY_EDITOR)
            , params IExtension[] extensions
#endif
            )

        {
#if (!UNITY_WEBGL || UNITY_EDITOR)
            // Set up some default values.
            this.PingFrequency = 1000;

            // If there no port set in the uri, we must set it now.
            if (uri.Port == -1)
                // Somehow if i use the UriBuilder it's not the same as if the uri is constructed from a string...
                //uri = new UriBuilder(uri.Scheme, uri.Host, uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? 443 : 80, uri.PathAndQuery).Uri;
                uri = new Uri(uri.Scheme + "://" + uri.Host + ":" + (uri.Scheme.Equals("wss", StringComparison.OrdinalIgnoreCase) ? "443" : "80") + uri.PathAndQuery);

            InternalRequest = new HTTPRequest(uri, OnInternalRequestCallback);

            // Called when the regular GET request is successfully upgraded to WebSocket
            InternalRequest.OnUpgraded = OnInternalRequestUpgraded;

            //http://tools.ietf.org/html/rfc6455#section-4

            //The request MUST contain a |Host| header field whose value contains /host/ plus optionally ":" followed by /port/ (when not using the default port).
            InternalRequest.SetHeader("Host", uri.Host + ":" + uri.Port);

            // The request MUST contain an |Upgrade| header field whose value MUST include the "websocket" keyword.
            InternalRequest.SetHeader("Upgrade", "websocket");

            // The request MUST contain a |Connection| header field whose value MUST include the "Upgrade" token.
            InternalRequest.SetHeader("Connection", "keep-alive, Upgrade");

            // The request MUST include a header field with the name |Sec-WebSocket-Key|.  The value of this header field MUST be a nonce consisting of a
            // randomly selected 16-byte value that has been base64-encoded (see Section 4 of [RFC4648]).  The nonce MUST be selected randomly for each connection.
            InternalRequest.SetHeader("Sec-WebSocket-Key", GetSecKey(new object[] { this, InternalRequest, uri, new object() }));

            // The request MUST include a header field with the name |Origin| [RFC6454] if the request is coming from a browser client.
            // If the connection is from a non-browser client, the request MAY include this header field if the semantics of that client match the use-case described here for browser clients.
            // More on Origin Considerations: http://tools.ietf.org/html/rfc6455#section-10.2
            if (!string.IsNullOrEmpty(origin))
                InternalRequest.SetHeader("Origin", origin);

            // The request MUST include a header field with the name |Sec-WebSocket-Version|.  The value of this header field MUST be 13.
            InternalRequest.SetHeader("Sec-WebSocket-Version", "13");

            if (!string.IsNullOrEmpty(protocol))
                InternalRequest.SetHeader("Sec-WebSocket-Protocol", protocol);

            // Disable caching
            InternalRequest.SetHeader("Cache-Control", "no-cache");
            InternalRequest.SetHeader("Pragma", "no-cache");

            this.Extensions = extensions;

#if !BESTHTTP_DISABLE_CACHING && (!UNITY_WEBGL || UNITY_EDITOR)
            InternalRequest.DisableCache = true;
#endif

#if !BESTHTTP_DISABLE_PROXY
            // WebSocket is not a request-response based protocol, so we need a 'tunnel' through the proxy
            if (HTTPManager.Proxy != null)
                InternalRequest.Proxy = new HTTPProxy(HTTPManager.Proxy.Address,
                                                      HTTPManager.Proxy.Credentials,
                                                      false, /*turn on 'tunneling'*/
                                                      false, /*sendWholeUri*/
                                                      HTTPManager.Proxy.NonTransparentForHTTPS);
#endif
#endif
        }

        #endregion

        #region Request Callbacks

#if (!UNITY_WEBGL || UNITY_EDITOR)
        private void OnInternalRequestCallback(HTTPRequest req, HTTPResponse resp)
        {
            string reason = string.Empty;

            switch (req.State)
            {
                // The request finished without any problem.
                case HTTPRequestStates.Finished:
                    if (resp.IsSuccess || resp.StatusCode == 101)
                    {
                        HTTPManager.Logger.Information("WebSocket", string.Format("Request finished. Status Code: {0} Message: {1}", resp.StatusCode.ToString(), resp.Message));

                        return;
                    }
                    else
                        reason = string.Format("Request Finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        resp.StatusCode,
                                                        resp.Message,
                                                        resp.DataAsText);
                    break;

                // The request finished with an unexpected error. The request's Exception property may contain more info about the error.
                case HTTPRequestStates.Error:
                    reason = "Request Finished with Error! " + (req.Exception != null ? ("Exception: " + req.Exception.Message + req.Exception.StackTrace) : string.Empty);
                    break;

                // The request aborted, initiated by the user.
                case HTTPRequestStates.Aborted:
                    reason = "Request Aborted!";
                    break;

                // Ceonnecting to the server is timed out.
                case HTTPRequestStates.ConnectionTimedOut:
                    reason = "Connection Timed Out!";
                    break;

                // The request didn't finished in the given time.
                case HTTPRequestStates.TimedOut:
                    reason = "Processing the request Timed Out!";
                    break;

                default:
                    return;
            }

            if (OnError != null)
                OnError(this, req.Exception);

            if (OnErrorDesc != null)
                OnErrorDesc(this, reason);

            if (OnError == null && OnErrorDesc == null)
                HTTPManager.Logger.Error("WebSocket", reason);
        }

        private void OnInternalRequestUpgraded(HTTPRequest req, HTTPResponse resp)
        {
            webSocket = resp as WebSocketResponse;

            if (webSocket == null)
            {
                if (OnError != null)
                    OnError(this, req.Exception);

                if (OnErrorDesc != null)
                {
                    string reason = string.Empty;
                    if (req.Exception != null)
                        reason = req.Exception.Message + " " + req.Exception.StackTrace;

                    OnErrorDesc(this, reason);
                }

                return;
            }

            webSocket.WebSocket = this;

            if (this.Extensions != null)
            {
                for (int i = 0; i < this.Extensions.Length; ++i)
                {
                    var ext = this.Extensions[i];

                    try
                    {
                        if (ext != null && !ext.ParseNegotiation(webSocket))
                            this.Extensions[i] = null; // Keep extensions only that succesfully negotiated
                    }
                    catch (Exception ex)
                    {
                        HTTPManager.Logger.Exception("WebSocket", "ParseNegotiation", ex);

                        // Do not try to use a defective extension in the future
                        this.Extensions[i] = null;
                    }
                }
            }

            if (OnOpen != null)
            {
                try
                {
                    OnOpen(this);
                }
                catch(Exception ex)
                {
                    HTTPManager.Logger.Exception("WebSocket", "OnOpen", ex);
                }
            }

            webSocket.OnText = (ws, msg) =>
            {
                if (OnMessage != null)
                    OnMessage(this, msg);
            };

            webSocket.OnBinary = (ws, bin) =>
            {
                if (OnBinary != null)
                    OnBinary(this, bin);
            };

            webSocket.OnClosed = (ws, code, msg) =>
            {
                if (OnClosed != null)
                    OnClosed(this, code, msg);
            };

            if (OnIncompleteFrame != null)
                webSocket.OnIncompleteFrame = (ws, frame) =>
                {
                    if (OnIncompleteFrame != null)
                        OnIncompleteFrame(this, frame);
                };

            if (StartPingThread)
                webSocket.StartPinging(Math.Max(PingFrequency, 100));

            webSocket.StartReceive();
        }
#endif

#endregion

#region Public Interface

        /// <summary>
        /// Start the opening process.
        /// </summary>
        public void Open()
        {
            if (requestSent || InternalRequest == null)
                return;

            if (this.Extensions != null)
            {
                try
                {
                    for (int i = 0; i < this.Extensions.Length; ++i)
                    {
                        var ext = this.Extensions[i];
                        if (ext != null)
                            ext.AddNegotiation(InternalRequest);
                    }
                }
                catch(Exception ex)
                {
                    LoggerManager.Logger.Exception("WebSocket", "Open", ex);
                }
            }

            InternalRequest.Send();
            requestSent = true;
        }

        /// <summary>
        /// It will send the given message to the server in one frame.
        /// </summary>
        public void Send(string message)
        {
            if (!IsOpen)
                return;

            webSocket.Send(message);
        }

        /// <summary>
        /// It will send the given data to the server in one frame.
        /// </summary>
        public void Send(byte[] buffer)
        {
            if (!IsOpen)
                return;

            webSocket.Send(buffer);
        }

        /// <summary>
        /// Will send count bytes from a byte array, starting from offset.
        /// </summary>
        public void Send(byte[] buffer, ulong offset, ulong count)
        {
            if (!IsOpen)
                return;

            webSocket.Send(buffer, offset, count);
        }

        /// <summary>
        /// It will send the given frame to the server.
        /// </summary>
        public void Send(WebSocketFrame frame)
        {
            if (IsOpen)
                webSocket.Send(frame);
        }

        /// <summary>
        /// It will initiate the closing of the connection to the server.
        /// </summary>
        public void Close()
        {
            if (!IsOpen)
                return;

            webSocket.Close();
        }

        /// <summary>
        /// It will initiate the closing of the connection to the server sending the given code and message.
        /// </summary>
        public void Close(UInt16 code, string message)
        {
            if (!IsOpen)
                return;

            webSocket.Close(code, message);
        }

        public static byte[] EncodeCloseData(UInt16 code, string message)
        {
            //If there is a body, the first two bytes of the body MUST be a 2-byte unsigned integer
            // (in network byte order) representing a status code with value /code/ defined in Section 7.4 (http://tools.ietf.org/html/rfc6455#section-7.4). Following the 2-byte integer,
            // the body MAY contain UTF-8-encoded data with value /reason/, the interpretation of which is not defined by this specification.
            // This data is not necessarily human readable but may be useful for debugging or passing information relevant to the script that opened the connection.
            int msgLen = Encoding.UTF8.GetByteCount(message);
            using (MemoryStream ms = new MemoryStream(2 + msgLen))
            {
                byte[] buff = BitConverter.GetBytes(code);
                if (BitConverter.IsLittleEndian)
                    Array.Reverse(buff, 0, buff.Length);

                ms.Write(buff, 0, buff.Length);

                buff = Encoding.UTF8.GetBytes(message);
                ms.Write(buff, 0, buff.Length);

                return ms.ToArray();
            }
        }

        #endregion

        #region Private Helpers

        private string GetSecKey(object[] from)
        {
            byte[] keys = new byte[16];
            int pos = 0;

            for (int i = 0; i < from.Length; ++i)
            {
                byte[] hash = BitConverter.GetBytes((Int32)from[i].GetHashCode());

                for (int cv = 0; cv < hash.Length && pos < keys.Length; ++cv)
                    keys[pos++] = hash[cv];
            }

            return Convert.ToBase64String(keys);
        }

        #endregion
    }
}
