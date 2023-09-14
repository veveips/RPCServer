using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServer.Server
{
    public class ChatMsgList
    {
        public List<ChatMsg> list = new List<ChatMsg>();
        public ChatMsgList() { }
    }
    public class ChatMsg
    {
        public ChatType type;
        public int pid;
        public string name;
        public string msg;
        public ChatMsg(ChatType type, int pid, string name, string msg)
        {
            this.type = type;
            this.pid = pid;
            this.name = name;
            this.msg = msg;
        }
        public ChatMsg() { }
    }
    public class FriendsData
    {
        public List<FriendItem> list = new List<FriendItem>();
        public FriendsData() { }
    }
    public class FriendItem
    {
        public int pid;
        public string icon;
        public string name;
        private int v;

        public FriendItem() { }

        public FriendItem(int pid, string name, string icon)
        {
            this.pid = pid;
            this.name = name;
            this.icon = icon;
        }
    }
    public class FriendsPID
    {
        public List<int> pids = new List<int>();
        public FriendsPID() { }
    }
    public class MyBagData
    {
        public List<BagItem> list { get; set; }
        public MyBagData() { list = new List<BagItem>(); }
    }
    public class BagItem
    {
        public string name;
        public string icon;
        public int num;
        public BagItem() { } // 添加无参数的默认构造函数
        public BagItem(string name, string icon, int num)
        {
            this.name = name;
            this.icon = icon;
            this.num = num;
        }
        public BagItem(string name, string icon)
        {
            this.name = name;
            this.icon = icon;
        }

        public void SetNum(int num)
        {
            this.num = num;
        }
    }
    public class MyCharacter
    {
        public string icon;
        public string name;
        public string hero;
        public int attack;
        public int defense;
        public int blood;
        public int blue;
        public int speed;
    }
}
