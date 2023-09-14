using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Linq;
using UnityEngine;
using RPCServer.SQL;
using Newtonsoft.Json;

namespace RPCServer.Server
{
    public class NetServer<Client, Scene> where Client : NetClient, new() where Scene : NetScene<Client>, new()
    {
        System.Random random = new System.Random(DateTime.Now.Millisecond);
        public Socket socket;
        private bool isRunning;
        public List<Client> Clients = new List<Client>();
        public List<Client> onlineClients => Clients.Where(m => m.login).ToList();
        private readonly object SyncRoot = new object();//同步锁对象
        public Dictionary<string, Scene> Scenes = new Dictionary<string, Scene>();
        private int nextUID = 1; // 初始UID
        /// <summary>
        /// 启动服务
        /// </summary>
        public void Run(int port)
        {
            InitServer();
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.NoDelay = true;
            isRunning = true;
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            socket.Listen(5000);
            // 注册远程方法
            RpcHelper.AddRpcHandler(this);
            // 添加自定义可序列化类
            RpcSerialize.AddBaseType<MyCharacter>();
            RpcSerialize.AddBaseType<MyBagData>();
            RpcSerialize.AddBaseType<FriendsData>();
            RpcSerialize.AddBaseType<ChatType>();
            RpcSerialize.AddBaseType<ChatMsg>();
            RpcSerialize.AddBaseType<ChatMsgList>();
            // 启动接受客户线程
            Thread acceptThread = new Thread(AcceptClients);
            acceptThread.Start();
            // 启动心跳检测线程
            //Thread heartbeatThread = new Thread(HeartbeatCheck);
            //heartbeatThread.Start();
            //启动接受消息的线程
            //Thread acceptMsgThread = new Thread(AcceptMessage);
            //acceptMsgThread.Start();
            Console.WriteLine("服务器已启动...");
        }
        /// <summary>
        /// 服务器初始化
        /// </summary>
        public void InitServer()
        {
            DataManager.Instance.Init();
            CreateScenes(new string[] { "地图A", "地图B" });
        }
        /// <summary>
        /// 停止服务
        /// </summary>
        public void StopServer()
        {
            isRunning = false;
            // 关闭服务器socket
            if (socket != null)
                socket.Close();
        }

        /// <summary>
        /// 定期向所有客户端发送心跳消息
        /// </summary>
        private void HeartbeatCheck()
        {
            while (true)
            {
                for (int i = 0; i < Clients.Count; i++)
                {
                    //先设置成false，当收到心跳回调的时候会设置成true的
                    Clients[i].receivedHeartbeat = false;
                    try
                    {
                        // 发送心跳消息给客户端
                        Clients[i].Send(RpcInnerCmd.Heart);
                    }
                    catch (Exception) { }
                }
                Thread.Sleep(1000); // 5秒钟发送一次心跳检测
                for (int i = 0; i < Clients.Count; i++)
                {
                    if (!Clients[i].receivedHeartbeat)
                    {
                        Console.WriteLine($"心跳检测：用户UID[{Clients[i].UID}]断开连接");
                        try
                        {
                            for (int j = 0; j < Clients.Count; j++)
                            {
                                //通知其他已登录的客户有用户断开连接
                                if (Clients[j] == Clients[i] || !Clients[j].login) continue;
                                Clients[j].Send(RpcInnerCmd.PlayerExit, Clients[i].UID);
                            }
                        }
                        catch (Exception) { }
                        Clients[i].Client.Disconnect(false);
                        Clients.Remove(Clients[i]);
                    }
                }
            }
        }

        /// <summary>
        /// 接受消息线程
        /// </summary>
        private void AcceptMessage(Client client)
        {
            while (isRunning)
            {
                try
                {
                    int bytesRead = client.Client.Receive(client.receiveBuffer);
                    if (bytesRead > 0)
                    {
                        //Console.WriteLine($"收到长度{bytesRead}");
                        RpcHelper.unPacket(client, bytesRead);
                    }
                }
                catch (SocketException)
                {
                    // 处理异常，比如客户端断开连接
                    //Console.WriteLine($"用户[{client.UID}]连接关闭");
                }
            }
        }
        /// <summary>
        /// 接受用户线程
        /// </summary>
        public void AcceptClients()
        {
            while (isRunning)
            {
                try
                {
                    lock (SyncRoot)
                    {
                        var client = new Client();
                        client.Client = socket.Accept();
                        client.UID = AllocateUID();
                        client.OnEnter();
                        Clients.Add(client);
                        Thread receiveThread = new Thread(() => AcceptMessage(client));
                        receiveThread.Start();
                    }
                }
                catch (SocketException) { }
            }
        }
        private int AllocateUID()
        {
            lock (Clients)
            {
                int uid = nextUID;
                nextUID++;
                return uid;
            }
        }
        #region 场景相关
        /// <summary>
        /// 登录游戏进入场景
        /// </summary>
        private void LoginHandle(Client client, string SceneName)
        {
            client.login = true;
            EnterScene(client, SceneName);
        }
        public void CreateScenes(string[] scenes)
        {
            for (int i = 0; i < scenes.Length; i++)
            {
                CreateScene(scenes[i]);
            }
        }
        public void CreateScene(string SceneName)
        {
            Scene scene = new Scene();
            scene.Name = SceneName;
            //OnSceneGroupSet(scene);//场景打组
            //scene.onSerializeOpt = OnSerializeOpt;//绑定同步事件
            //scene.onSerializeRpc = OnSerializeRPC;//绑定RPC事件
            Scenes.Add(SceneName, scene);
        }
        #endregion
        #region 服务器广播消息
        public void Send(string func, params object[] data)
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Send(func, data);
            }
        }
        public void Send(byte cmd, string func, params object[] data)
        {
            for (int i = 0; i < Clients.Count; i++)
            {
                Clients[i].Send(cmd, func, data);
            }
        }
        #endregion
        #region 自定义远程方法
        [RpcF]
        public void Login(Client client, string account, string pwd)
        {
            //Console.WriteLine($"用户[{client.UID}]登录 => 账号：{account} 密码：{pwd}");
            int rtn = SQLOperation.CheckUser(account, out int pid, pwd);
            //Console.WriteLine($"登录返回值：{rtn}");
            if (rtn == -1)
            {
                client.Send(RpcInnerCmd.Login, (byte)1, "账号或密码错误");
            }
            else if (rtn == -2)
            {
                client.PID = pid;
                client.Send(RpcInnerCmd.Login, (byte)2, "暂无角色");
            }
            else
            {
                client.PID = pid;//绑定角色PID
                client.Send(RpcInnerCmd.Login, (byte)0, "登录成功");
                LoginHandle(client, "地图A");
            }
        }
        [RpcF]
        public void Regist(Client client, string account, string pwd)
        {
            //Console.WriteLine($"用户[{client.UID}]注册 => 账号：{account} 密码：{pwd}");
            int pid = SQLOperation.RegistUser(account, pwd);
            if (pid == -1)
            {
                client.Send(RpcInnerCmd.Regist, (byte)1, "账号已存在");
            }
            else
            {
                client.Send(RpcInnerCmd.Regist, (byte)0, "注册成功");
            }
        }
        [RpcF]
        public void QuitLogin(Client client)
        {
            client.Send(RpcInnerCmd.PlayerClear);
            client.Send(RpcInnerCmd.QuitLogin);
            client.login = false;
            client.PID = -1;
            //通知其他玩家
            List<Client> notice = Clients.Where(m => m.login && m.SceneName == client.SceneName).ToList();
            client.SceneName = null;
            for (int i = 0; i < notice.Count; i++)
            {
                notice[i].Send(RpcInnerCmd.PlayerExit, client.UID);
            }
        }
        [RpcF]
        public void CreateHero(Client client, string hero, string name)
        {
            MyCharacter data = new MyCharacter();
            data.name = name;
            data.hero = hero;
            data.icon = hero;
            data.blood = GetRandom(1, 100);
            data.blue = GetRandom(1, 100);
            data.speed = GetRandom(1, 100);
            data.attack = GetRandom(1, 100);
            data.defense = GetRandom(1, 100);

            MyBagData bag = new MyBagData();
            int num = GetRandom(3, 10);
            for (int i = 0; i < num; i++)
            {
                //随机从数据配置中加入道具
                bag.list.Add(DataManager.Instance.allitems[GetRandom(0, DataManager.Instance.allitems.Count)]);
                bag.list[bag.list.Count - 1].SetNum(GetRandom(1, 100));
            }

            int code = SQLOperation.CreateHero(client.PID, JsonConvert.SerializeObject(data), JsonConvert.SerializeObject(bag), name);

            client.Send("CreateHero", (byte)code, code == 0 ? "创建成功" : "创建失败");
            //创建成功直接进入游戏
            LoginHandle(client, "地图A");
        }
        [RpcF]
        public void Chat(Client client, ChatType order, int PID, string msg)
        {
            //Console.WriteLine($"转发聊天[{order}]:[{msg}]");
            if (order == ChatType.map)
            {
                List<Client> notice = Clients.Where(m => m.login && m.SceneName == client.SceneName).ToList();
                for (int i = 0; i < notice.Count; i++)
                {
                    ChatMsg msgdata = new ChatMsg(ChatType.map, PID, SQLOperation.GetJsonData<MyCharacter>(client.PID, "info").name, msg);
                    notice[i].Send(RpcInnerCmd.Chat, msgdata);
                }
            }
            if (order == ChatType.online)
            {
                List<Client> notice = Clients.Where(m => m.login).ToList();
                for (int i = 0; i < notice.Count; i++)
                {
                    ChatMsg msgdata = new ChatMsg(ChatType.online, PID, SQLOperation.GetJsonData<MyCharacter>(client.PID, "info").name, msg);
                    notice[i].Send(RpcInnerCmd.Chat, msgdata);
                }
            }
            if (order == ChatType.person)
            {
                //私人聊天
                //加入数据库
                SQLOperation.InsertChat(client.PID, PID, msg);
                //如果在线的话直接转发
                //Console.WriteLine($"[{msgdata.name}]发送消息:[{msgdata.msg}]");
                client.Send(RpcInnerCmd.Chat, new ChatMsg(ChatType.person, PID, SQLOperation.GetJsonData<MyCharacter>(client.PID, "info").name, msg));
                Clients.Find(m => m.PID == PID)?.Send(RpcInnerCmd.Chat, new ChatMsg(ChatType.person, client.PID, SQLOperation.GetJsonData<MyCharacter>(client.PID, "info").name, msg));
            }
        }
        [RpcF]
        public void GetAllChat(Client client, int pid)
        {
            //Console.WriteLine("上下文请求");
            client.Send("GetAllChat", SQLOperation.GetChats(client.PID, pid));
        }
        [RpcF]
        public void GetPlayerInfo(Client client)
        {
            client.Send("GetPlayerInfo", SQLOperation.GetJsonData<MyCharacter>(client.PID, "info"));
        }
        [RpcF]
        public void GetPlayerBag(Client client)
        {
            client.Send("GetPlayerBag", SQLOperation.GetJsonData<MyBagData>(client.PID, "bag"));
        }
        #region 好友相关
        [RpcF]//同意添加好友
        public void AgreeFriend(Client client, int pid)
        {
            //将双方的消息都加入数据库
            FriendsPID pids = SQLOperation.GetJsonData<FriendsPID>(client.PID, "friend");
            if (pids == null) pids = new FriendsPID();
            if (pids.pids.Any(m => m == pid)) return;
            pids.pids.Add(pid);
            SQLOperation.SaveJsonData(client.PID, "friend", JsonConvert.SerializeObject(pids));
            pids = SQLOperation.GetJsonData<FriendsPID>(pid, "friend");
            if (pids == null) pids = new FriendsPID();
            if (pids.pids.Any(m => m == client.PID)) return;
            pids.pids.Add(client.PID);
            SQLOperation.SaveJsonData(pid, "friend", JsonConvert.SerializeObject(pids));
            //将申请消息删除
            pids = SQLOperation.GetJsonData<FriendsPID>(client.PID, "freq");
            pids.pids.RemoveAll(m => m == pid);
            SQLOperation.SaveJsonData(client.PID, "freq", JsonConvert.SerializeObject(pids));
            //返回添加好友回调
            client.Send("AgreeFriend", true);
            //如果在线的话就发一条好友通过消息
            Clients.Find(m => m.PID == pid)?.Send("AgreeFriend", false);
        }
        [RpcF]//检测是否有好友申请
        public void CheckFriendReq()
        {
        }
        [RpcF]//发送好友申请
        public void SendFriendReq(Client client, int pid)
        {
            Console.WriteLine("发送添加好友");
            //将申请消息添加到数据库
            FriendsPID pids = SQLOperation.GetJsonData<FriendsPID>(pid, "freq");
            if (pids == null) pids = new FriendsPID();
            if (pids.pids.Any(m => m == client.PID)) return;
            pids.pids.Add(client.PID);
            SQLOperation.SaveJsonData(pid, "freq", JsonConvert.SerializeObject(pids));
            //如果在线的话就发一条好友申请消息
            Client target = Clients.Find(m => m.PID == pid);
            if (target != null) target.Send("HaveFriendReq");
        }
        [RpcF]//查询好友
        public void SearchFriend(Client client, int pid)
        {
            FriendsData data = new FriendsData();
            if (client.PID == pid)
            {
                //这里要排除自身PID
                client.Send("SearchFriend", data);
                return;
            }
            MyCharacter info = SQLOperation.GetJsonData<MyCharacter>(pid, "info");
            if (info != null) data.list.Add(new FriendItem(pid, info.name, info.icon));
            client.Send("SearchFriend", data);
        }
        [RpcF]//获得好友列表信息
        public void GetFriends(Client client)
        {
            FriendsPID pids = SQLOperation.GetJsonData<FriendsPID>(client.PID, "friend");
            FriendsData data = new FriendsData();
            for (int i = 0; i < pids?.pids.Count; i++)
            {
                MyCharacter info = SQLOperation.GetJsonData<MyCharacter>(pids.pids[i], "info");
                data.list.Add(new FriendItem(pids.pids[i], info.name, info.icon));
            }
            client.Send("GetFriends", data);
        }
        [RpcF]//获得好友申请消息
        public void GetFriReq(Client client)
        {
            FriendsPID pids = SQLOperation.GetJsonData<FriendsPID>(client.PID, "freq");
            FriendsData data = new FriendsData();
            for (int i = 0; i < pids?.pids.Count; i++)
            {
                MyCharacter info = SQLOperation.GetJsonData<MyCharacter>(pids.pids[i], "info");
                data.list.Add(new FriendItem(pids.pids[i], info.name, info.icon));
            }
            client.Send("GetFriReq", data);
        }
        #endregion
        #endregion
        #region 内部远程方法
        [RpcF]
        public void EnterScene(Client client, string SceneName)
        {
            //检测地图是否存在
            if (!Scenes.ContainsKey(SceneName))
            {
                Console.WriteLine($"地图[{SceneName}]不存在");
                return;
            }
            //通知退出上一个地图
            string LastSceneName = client.SceneName;
            client.SceneName = SceneName;
            //通知本机玩家需要删除所有对象
            client.Send(RpcInnerCmd.PlayerClear);
            List<Client> delnotice = Clients.Where(m => m.login && m.SceneName == LastSceneName).ToList();
            for (int i = 0; i < delnotice.Count; i++)
            {
                //通知其他玩家本机角色退出场景
                //Console.WriteLine($"通知客户[{delnotice[i].UID}]玩家[{client.UID}]退出场景");
                delnotice[i].Send(RpcInnerCmd.PlayerExit, client.UID);
            }
            Console.WriteLine($"用户[{client.UID}]进入[{SceneName}]");
            //对应的地图所有用户
            List<Client> notice = Clients.Where(m => m.login && m.SceneName == SceneName).ToList();
            MyCharacter data = SQLOperation.GetJsonData<MyCharacter>(client.PID, "info");
            for (int i = 0; i < notice.Count; i++)
            {
                //通知其他角色
                if (notice[i] == client) continue;
                //Console.WriteLine($"用户[{client.UID}]进入游戏，通知其他客户[{notice[i].UID}]创建角色[{client.UID}][false]");
                notice[i].Send(RpcInnerCmd.PlayerEnter, client.UID, false, SceneName, data.name, data.icon);
            }
            //通知本机角色
            for (int i = 0; i < notice.Count; i++)
            {
                bool isLocal = notice[i] == client;
                data = SQLOperation.GetJsonData<MyCharacter>(notice[i].PID, "info");
                //Console.WriteLine($"用户[{client.UID}]进入游戏，通知本机客户[{client.UID}]创建角色[{uid}][{isLocal}]");
                client.Send(RpcInnerCmd.PlayerEnter, notice[i].UID, isLocal, SceneName, data.name, data.icon);
            }
        }
        [RpcF]
        public void Heart(Client client)
        {
            //心跳包回调
            client.receivedHeartbeat = true;
        }
        [RpcF]
        public void RpcMove(Client client, Vector3 pos)
        {
            //转发给给场景的其他客户端
            List<Client> cliens = Clients.Where(m => m.UID != client.UID && m.SceneName == client.SceneName).ToList();
            for (int i = 0; i < cliens.Count; i++)
            {
                cliens[i].Send(RpcInnerCmd.RpcMove, client.UID, pos);
            }
        }
        #endregion

        public int GetRandom(int min, int max)
        {
            int randomNumber = random.Next(min, max);
            return randomNumber;
        }
    }
    public class NetServer : NetServer<NetClient, NetScene<NetClient>> { }
}