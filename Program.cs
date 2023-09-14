using RPCServer.Server;
using System;
using RPCServer.SQL;

namespace RPCServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            //序列化和反序列化测试
            //byte[] s = RpcSerialize.Serialize(new object[] { 1, true, new Vector2[] { new Vector2(1, 2) }, new Vector2(11, 22) });
            //object[] o = RpcSerialize.Deserialize(s);
            //Console.WriteLine(RpcSerialize.GetObjectString(o));

            //SQL测试
            //Console.WriteLine(SQLOperation.RegistUser("admin", "admin"));
            //Console.WriteLine(SQLOperation.CheckUser("123"));


            NetServer server = new NetServer();
            server.Run(6666);
            Console.ReadKey();
        }
    }
}
