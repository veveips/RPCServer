using System;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

namespace RPCServer.Server
{
    public class NetClient
    {
        public Socket Client;
        public bool login;
        public BufferPool buffer = new BufferPool();
        public byte[] receiveBuffer = new byte[1024];
        public bool receivedHeartbeat = true;

        public string ip => Client.RemoteEndPoint.ToString();
        public int UID { get; set; }
        public int PID { get; set; }
        public string SceneName { get; set; }

        public void OnEnter()
        {
            Console.WriteLine($"用户[{ip}]UID[{UID}]连接服务器");
        }

        #region 客户端发消息
        public void Send(string func, params object[] data)
        {
            if (!Client.Connected) return;
            try
            {
                Client.Send(RpcHelper.Packet(func, data));
            }
            catch (Exception ex)
            {
#if Server
                Console.WriteLine($"发送消息时发生异常: {ex.Message}");
#else
            Debug.Log($"发送消息时发生异常: {ex.Message}");
#endif
            };

        }
        public void Send(byte cmd, string func, params object[] data)
        {
            if (!Client.Connected) return;
            try
            {
                Client.Send(RpcHelper.Packet(cmd, func, data));
            }
            catch (Exception ex)
            {
#if Server
                Console.WriteLine($"发送消息时发生异常: {ex.Message}");
#else
            Debug.Log($"发送消息时发生异常: {ex.Message}");
#endif
            }
        }
        public async UniTask<RpcModel> Call(string func, params object[] data)
        {
            if (!Client.Connected) return null;
            try
            {
                Client.Send(RpcHelper.Packet(func, data));
                RpcModel rtn = new RpcModel();
                var timeout = (uint)Environment.TickCount + (uint)5000; //默认超时5秒
                while ((uint)Environment.TickCount < timeout)
                {
                    await UniTask.Yield();
                    rtn = RpcHelper.GetRpcReturn(func);
                    if (rtn.IsCompleted)
                        goto J;
                }
            J: return rtn;
            }
            catch (Exception ex)
            {
#if Server
                Console.WriteLine($"发送消息时发生异常: {ex.Message}");
#else
            Debug.Log($"发送消息时发生异常: {ex.Message}");
#endif
                return null;
            };
        }
        #endregion
    }
}