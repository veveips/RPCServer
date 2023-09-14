using RPCServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServer.SQL
{
    public class DataManager
    {
        public static DataManager Instance = new DataManager();
        public DataManager() { if (Instance == null) Instance = this; }
        public List<BagItem> allitems = new List<BagItem>();
        public void Init()
        {
            LoadItems();
        }
        public void LoadItems()
        {
            string[] items = new string[] { "技能书籍", "幸运结", "震天鼓", "普通鼓", "黄金稻草" };
            string[] icon = new string[] { "00001", "00002", "00003", "00004", "00005" };
            for (int i = 0; i < items.Length; i++)
            {
                allitems.Add(new BagItem(items[i], icon[i]));
                Console.WriteLine($"加载道具[{items[i]}]");
            }
        }
    }
}
