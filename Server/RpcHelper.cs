using System;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.Runtime.Serialization.Formatters.Binary;

namespace RPCServer.Server
{
    public class RpcHelper
    {
        private static MyDictionary<string, RpcMethodInfo> rpcDict = new MyDictionary<string, RpcMethodInfo>();
        public static byte[] Packet(string func, params object[] data)
        {
            RpcModel rtn = new RpcModel(func, false, data);
            BingRpcReturn(func, rtn);//绑定回调消息
            return rtn.pack;
        }
        public static byte[] Packet(byte cmd, string func, params object[] data)
        {
            RpcModel rtn = new RpcModel(cmd, func, false, data);
            BingRpcReturn(func, rtn);//绑定回调消息
            return rtn.pack;
        }
        public static void unPacket(NetClient client, int length)
        {
            //压入待处理的数据
            client.buffer.Push(client.receiveBuffer, length);
            byte[] takeData = client.buffer.Take();
            while (takeData != null)
            {
                RpcModel rtn = new RpcModel(takeData);
                //执行调用的远程方法
                if (!string.IsNullOrEmpty(rtn.func))
                {
                    //进行CRC验证
                    byte CRC = CRCHelper.CRC8(Encoding.UTF8.GetBytes(rtn.func));
                    if (CRC.Equals(rtn.CRC))
                        InvokeRpc(rtn.func, client, rtn.data);
                    else
#if Server

                        Console.WriteLine($"远程方法[{rtn.func}]校验失败");
#else
                        Debug.Log($"远程方法[{rtn.func}]校验失败");
#endif
                }
                if (rtn.func != RpcInnerCmd.Heart.ToString() && rtn.func != RpcInnerCmd.RpcMove.ToString())
                {
#if Server
                    //Console.WriteLine($"收到消息:CMD={rtn.cmd},FUNC={rtn.func},DATA={JsonConvert.SerializeObject(rtn.data)}");
#else
                    //   Debug.Log($"收到消息:CMD={rtn.cmd},FUNC={rtn.func},DATA={JsonConvert.SerializeObject(rtn.data)}");
#endif
                }
                takeData = client.buffer.Take();
            }
        }
        /// <summary>
        /// 绑定Rpc方法的返回值
        /// </summary>
        public static void BingRpcReturn(string func, RpcModel rtn)
        {
            if (rpcDict.ContainsKey(func))
            {
                rpcDict[func].rtn = rtn;
            }
        }
        public static RpcModel GetRpcReturn(string func)
        {
            if (rpcDict.ContainsKey(func))
                return rpcDict[func].rtn;
            else
            {
                //Debug.Log("获取返回值失败");
                return new RpcModel();
            }
        }
        public static void AddRpcHandler(object target)
        {
            var members = target.GetType().GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var info in members)
            {
                if (info.MemberType == MemberTypes.Method)
                {
                    var attributes = info.GetCustomAttributes(typeof(RpcF), true);
                    if (attributes.Length > 0)
                    {
                        // 找到带有RpcF标记的方法
                        var methodName = info.Name;
                        var methodInfo = target.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                        if (methodInfo != null && !rpcDict.ContainsKey(methodName))
                        {
                            rpcDict.Add(methodName, new RpcMethodInfo(target, methodInfo, null));
                        }
                        else if (rpcDict.ContainsKey(methodName))
                        {
#if Server
                            Console.WriteLine($"Rpc方法[{methodName}]已被注册");
#else
                            Debug.Log($"Rpc方法[{methodName}]已被注册");

#endif
                        }
                    }
                }
            }
        }
        public static void RemoveRpcHandler(object target)
        {
            List<string> keysToDelete = rpcDict
                .Where(m => m.Value.target.GetType().Equals(target.GetType()))
                .Select(pair => pair.Key)
                .ToList();
            foreach (var key in keysToDelete) { rpcDict.Remove(key); }
        }
        public static void InvokeRpc(string methodName, NetClient client, params object[] data)
        {
            if (rpcDict.ContainsKey(methodName))
            {
                var methodInfo = rpcDict[methodName];
                var parameterTypes = methodInfo.mothod.GetParameters().Select(p => p.ParameterType).ToArray();
                // 确保传递的参数数量和类型与目标方法的参数数量和类型匹配
#if Server
                if (parameterTypes.Length - 1 == data.Length)
                {
                    //最前面加一个Client参数
                    object[] parm = new object[data.Length + 1];
                    parm[0] = client;
                    Array.Copy(data, 0, parm, 1, data.Length);
                    // 检测参数类型是否匹配
                    bool parametersMatch = true;
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (parm[i] == null || !parameterTypes[i].IsInstanceOfType(parm[i]))
                        {
                            parametersMatch = false;
                            // 输出目标类型和期望类型的字符串
                            string targetType = parameterTypes[i].FullName;
                            string expectedType = parm[i] == null ? "null" : parm[i].GetType().FullName;
                            Console.WriteLine($"RPC方法[{methodName}]参数类型不匹配: 目标类型 {targetType}, 输入类型 {expectedType}");
                            break;
                        }
                    }
                    if (parametersMatch)
                        methodInfo.rtn = methodInfo.mothod.Invoke(methodInfo.target, parm) as RpcModel;
                }
                else
                {
                    Console.WriteLine($"RPC方法[{methodName}]参数数量不匹配");
                }
#else
                if (parameterTypes.Length == data.Length)
                {
                    // 检测参数类型是否匹配
                    bool parametersMatch = true;
                    for (int i = 0; i < parameterTypes.Length; i++)
                    {
                        if (data[i] == null || !parameterTypes[i].IsInstanceOfType(data[i]))
                        {
                            parametersMatch = false;
                            // 输出目标类型和期望类型的字符串
                            string targetType = parameterTypes[i].FullName;
                            string expectedType = data[i] == null ? "null" : data[i].GetType().FullName;
                            Console.WriteLine($"RPC方法[{methodName}]参数类型不匹配: 目标类型{targetType}, 输入类型 {expectedType}");
                            break;
                        }
                    }
                    if (parametersMatch)
                        methodInfo.rtn = methodInfo.mothod.Invoke(methodInfo.target, data) as RpcModel;
                }
                else
                {
                    Console.WriteLine($"RPC方法[{methodName}]参数数量不匹配");
                }
#endif
            }
            else
            {
#if Server
                Console.WriteLine($"RPC方法[{methodName}]不存在");
#else
                Debug.LogError($"RPC方法[{methodName}]不存在");
#endif
            }
        }
    }
    public class RpcMethodInfo
    {
        public object target;
        public MethodInfo mothod;
        public RpcModel rtn;
        public RpcMethodInfo(object target, MethodInfo mothod, RpcModel rtn)
        {
            this.target = target;
            this.mothod = mothod;
            this.rtn = rtn;
        }
    }
    public class RpcF : Attribute { }
    public class RpcModel
    {
        public int length;
        public byte cmd;
        public byte CRC;
        public string func;
        public object[] data;
        public byte[] pack;//封包的数据
        public bool IsCompleted;
        public bool isBigData;//大文件标记，如果是大文件的话就不处理了
        private int parsIndex;//参数读取索引
        public RpcModel() { }
        public RpcModel(bool IsCompleted, params object[] data)
        {
            this.cmd = NetCmd.Inner;
            this.IsCompleted = IsCompleted;
            this.data = data;
        }
        public RpcModel(string func, bool IsCompleted = false, params object[] data)
        {
            this.cmd = NetCmd.Inner;
            this.func = func;
            this.data = data;
            this.IsCompleted = IsCompleted;
            PackData();
        }
        public RpcModel(byte cmd, string func, bool IsCompleted = false, params object[] data)
        {
            this.cmd = cmd;
            this.func = func;
            this.data = data;
            this.IsCompleted = IsCompleted;
            PackData();
        }
        void PackData()
        {
            byte[] funcBytes = Encoding.UTF8.GetBytes(func);
            //byte[] otaBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(data));
            byte[] otaBytes = RpcSerialize.Serialize(data);
            this.CRC = CRCHelper.CRC8(funcBytes); // 校正码跟方法名绑定
            this.length = sizeof(int) * 3 + sizeof(byte) * 2 + sizeof(bool) + funcBytes.Length + otaBytes.Length;
            this.isBigData = this.length > 1024;

            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                writer.Write(length);
                writer.Write(cmd);
                writer.Write(CRC);
                writer.Write(isBigData);
                // 写入字符串 func 的字节数组及其长度
                writer.Write(funcBytes.Length);
                writer.Write(funcBytes);
                // 写入字符串 ota 的字节数组及其长度
                writer.Write(otaBytes.Length);
                writer.Write(otaBytes);
                pack = stream.ToArray();
                //#if Server
                //                Console.WriteLine("封包数据：" + RpcSerialize.ByteArrayToHexString(pack));
                //#else
                //                Debug.Log("封包数据：" + RpcSerialize.ByteArrayToHexString(pack));
                //#endif
            }
        }
        void UnPackData()
        {
            if (pack == null)
            {
                cmd = 0;
                func = "";
                data = new object[0];
                return;
            }
            using (MemoryStream stream = new MemoryStream(pack))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                try
                {
                    //读取包长度
                    int totalLength = reader.ReadInt32();
                    if (stream.Length >= totalLength)
                    {
                        cmd = reader.ReadByte();//1
                        CRC = reader.ReadByte();//1
                        isBigData = reader.ReadBoolean();//1
                        int funcStrLength = reader.ReadInt32();//4
                        byte[] funcStrBytes = reader.ReadBytes(funcStrLength);//4
                        func = Encoding.UTF8.GetString(funcStrBytes);//4
                                                                     // 读取字符串长度和字符串数据
                        int jsonStrLength = reader.ReadInt32();//4
                        byte[] jsonStrBytes = reader.ReadBytes(jsonStrLength);//6
                        //data = JsonConvert.DeserializeObject<object[]>(Encoding.UTF8.GetString(jsonStrBytes));
                        data = RpcSerialize.Deserialize(jsonStrBytes);
                    }
                }
                catch (EndOfStreamException ex)
                {
                    // 处理异常
#if Server
                    Console.WriteLine($"解包异常: {ex.Message}");
#else
                    Debug.Log($"解包异常: {ex.Message}");
#endif
                }
            }
        }
        public RpcModel(byte[] data)
        {
            this.pack = data;
            UnPackData();
        }

        #region 参数提取
        public T To<T>()
        {
            var t = (T)data[parsIndex];
            parsIndex++;
            return t;
        }
        public T As<T>() where T : class
        {
            var t = data[parsIndex] as T;
            parsIndex++;
            return t;
        }
        public byte AsByte { get => To<byte>(); }
        public sbyte AsSbyte { get => To<sbyte>(); }
        public bool AsBoolen { get => To<bool>(); }
        public short AsShort { get => To<short>(); }
        public ushort AsUshort { get => To<ushort>(); }
        public char AsChar { get => To<char>(); }
        public int AsInt { get => To<int>(); }
        public uint AsUint { get => To<uint>(); }
        public float AsFloat { get => To<float>(); }
        public long AsLong { get => To<long>(); }
        public ulong AsUlong { get => To<ulong>(); }
        public double AsDouble { get => To<double>(); }
        public string AsString { get => As<string>(); }
        #endregion
    }
    public enum ChatType : byte
    {
        map, online, person
    }
    public class RpcInnerCmd
    {
        public const string RpcMove = "RpcMove";
        public const string PlayerEnter = "PlayerEnter";
        public const string PlayerExit = "PlayerExit";
        public const string Heart = "Heart";
        public const string EnterScene = "EnterScene";
        public const string Login = "Login";
        public const string Regist = "Regist";
        public const string Chat = "Chat";
        public const string PlayerClear = "PlayerClear";
        public const string QuitLogin = "QuitLogin";
    }
    public class NetCmd
    {
        public const byte Inner = 1;//如果是内部的
        public const byte CallRpc = 2;//角色自身
        public const byte SendRT = 3;//角色所在场景
        public const byte Online = 4;//所有在线角色
    }
    public class BufferPool
    {
        private MemoryStream bufferStream = new MemoryStream();
        private readonly object SyncRoot = new object();

        public void Push(byte[] data, int length)
        {
            //压入一段需要处理的数据
            byte[] realData = new byte[length];
            Array.Copy(data, realData, length);
            //Console.WriteLine("压包数据：" + RpcSerialize.ByteArrayToHexString(realData));
            lock (SyncRoot)
            {
                //bufferStream.Write(data, 0, length);
                bufferStream.Write(data, 0, length);
            }
        }
        public byte[] Take()
        {
            lock (SyncRoot)
            {
                // 检查缓冲区中是否有足够的数据用于解析一个完整的数据包
                if (bufferStream.Length >= sizeof(int))
                {
                    bufferStream.Position = 0;
                    int packetLength = BitConverter.ToInt32(bufferStream.GetBuffer(), 0);
                    // 检查 packetLength 是否是有效的值，防止溢出
                    if (packetLength > 0 && packetLength <= bufferStream.Length)
                    {
                        byte[] packetData = new byte[packetLength];
                        bufferStream.Read(packetData, 0, packetLength);

                        // 清除已提取的数据
                        int remain = (int)bufferStream.Length - packetLength;
                        if (remain > 0)
                        {
                            byte[] remainData = new byte[remain];
                            bufferStream.Read(remainData, 0, remain);
                            bufferStream.SetLength(0);
                            bufferStream.Write(remainData, 0, remain);
                        }
                        else
                        {
                            // 如果没有剩余数据，则清空整个缓冲区
                            bufferStream.SetLength(0);
                        }
                        bufferStream.Position = 0;

                        //Console.WriteLine($"取出数据：" + RpcSerialize.ByteArrayToHexString(packetData));
                        //Console.WriteLine($"剩余数据[{remainData.Length}]：" + RpcSerialize.ByteArrayToHexString(remainData));

                        return packetData;
                    }
                }
                return null; // 没有足够的数据来解析一个完整的数据包
            }
        }
    }
    public static class CRCHelper
    {
        /// <summary>
        /// CRC校验代码表, 用户可自行改变CRC校验码, 直接改源代码
        /// 客户端和服务器检验码必须一致, 否则识别失败
        /// </summary>
        public static readonly byte[] CRCCode = new byte[]
        {
            0x2d, 0x9e, 0x2e, 0xbe, 0x29, 0x5e, 0x0e, 0x64, 0x30, 0xcb, 0xe5, 0xce, 0x0c, 0x4e,
            0xe8, 0x4d, 0x87, 0xf0, 0x14, 0xcd, 0x24, 0x3a, 0x4a, 0xe7, 0x73, 0x75, 0x3d, 0x85,
            0xa7, 0xde, 0x95, 0x23, 0x25, 0x07, 0x11, 0x1d, 0x82, 0x28, 0x33, 0x2c, 0xeb, 0xa5,
            0x31, 0xf3, 0x91, 0xf6, 0x5c, 0x69, 0xf5, 0xa3, 0x32, 0x26, 0xd7, 0x84, 0x3e, 0x49,
            0x77, 0xbb, 0x3b, 0xfc, 0x9b, 0xfd, 0xc0, 0xb0, 0x08, 0xb4, 0x62, 0xe4, 0x8e, 0xa6,
            0xb9, 0x18, 0xef, 0xc6, 0x46, 0xe0, 0x90, 0x20, 0x27, 0x1b, 0x72, 0xc7, 0xf2, 0xdb,
            0x71, 0x03, 0x7e, 0x00, 0x35, 0x53, 0x4c, 0xe2, 0x63, 0x55, 0x61, 0x4b, 0x9a, 0x93,
            0x02, 0xab, 0xd9, 0x3c, 0xbd, 0xf9, 0x47, 0x42, 0x09, 0xad, 0x70, 0x1a, 0xc5, 0x2a,
            0xb8, 0x34, 0xd0, 0x81, 0xe9, 0xae, 0x60, 0x10, 0x4f, 0x74, 0xb7, 0x76, 0xe3, 0xfb,
            0xe6, 0xc9, 0x6b, 0xdf, 0x3f, 0x12, 0xa8, 0xec, 0xcf, 0x05, 0x1c, 0xc8, 0x98, 0x51,
            0x21, 0x5d, 0x41, 0x45, 0x94, 0xd1, 0xe1, 0x52, 0x67, 0xea, 0x8b, 0xd5, 0x0d, 0x01,
            0x97, 0x83, 0xbf, 0x17, 0xbc, 0x40, 0xb1, 0x89, 0x79, 0x7a, 0x16, 0xfe, 0xff, 0x54,
            0x80, 0x5b, 0x43, 0x13, 0xf1, 0xfa, 0x5f, 0x57, 0x50, 0xee, 0x44, 0x92, 0xca, 0x15,
            0x9f, 0xf7, 0x56, 0x65, 0x9c, 0xdd, 0x5a, 0xc2, 0x86, 0xd3, 0xf8, 0x06, 0xa0, 0x58,
            0xa1, 0x6a, 0x39, 0x59, 0xd2, 0xf4, 0x0f, 0x6c, 0x6f, 0x1f, 0xd8, 0x68, 0x19, 0xb2,
            0x0a, 0x48, 0x6d, 0xa4, 0x8d, 0xa2, 0x37, 0x66, 0x04, 0x22, 0x0b, 0x9d, 0xb6, 0x78,
            0x36, 0x7d, 0xb3, 0xdc, 0x96, 0x8a, 0xda, 0x7c, 0xba, 0x8c, 0x8f, 0xac, 0x2f, 0x6e,
            0x7f, 0xcc, 0x38, 0x2b, 0x99, 0xaf, 0xc3, 0xd6, 0xc1, 0xd4, 0xc4, 0xaa, 0x7b, 0x88,
            0xed, 0x1e, 0xb5, 0xa9,
        };

        public static byte CRC8(byte[] buffer)
        {
            return CRC8(buffer, 0, buffer.Length);
        }

        public static byte CRC8(byte[] buffer, int off, int len)
        {
            byte crc = 0;
            for (int i = off; i < len; i++)
            {
                crc ^= CRCCode[crc ^ buffer[i]];
            }
            return crc;
        }

        public unsafe static byte CRC8(byte* buffer, int off, int len)
        {
            byte crc = 0;
            for (int i = off; i < len; i++)
            {
                var value = buffer[i];
                crc ^= CRCCode[crc ^ value];
            }
            return crc;
        }

        public unsafe static byte CRC8(byte* buffer, int off, int len, byte mask)
        {
            byte crc = 0;
            for (int i = off; i < len; i++)
            {
                var value = buffer[i];
                crc = (byte)(crc ^ CRCCode[crc ^ value] ^ mask);
            }
            return crc;
        }
    }
    public class RpcSerialize
    {
        private static MyDictionary<Type, ushort> serializeType1s = new MyDictionary<Type, ushort>();
        private static MyDictionary<ushort, Type> serializeTypes = new MyDictionary<ushort, Type>();
        static RpcSerialize()
        {
            Init();
        }
        public static void Init()
        {
            AddBaseType<short>();
            AddBaseType<int>();
            AddBaseType<long>();
            AddBaseType<ushort>();
            AddBaseType<uint>();
            AddBaseType<ulong>();
            AddBaseType<float>();
            AddBaseType<double>();
            AddBaseType<bool>();
            AddBaseType<char>();
            AddBaseType<string>();
            AddBaseType<byte>();
            AddBaseType<sbyte>();
            AddBaseType<DateTime>();
            AddBaseType<decimal>();
            AddBaseType<DBNull>();
            AddBaseType<Type>();
            //基础序列化数组
            AddBaseType<short[]>();
            AddBaseType<int[]>();
            AddBaseType<long[]>();
            AddBaseType<ushort[]>();
            AddBaseType<uint[]>();
            AddBaseType<ulong[]>();
            AddBaseType<float[]>();
            AddBaseType<double[]>();
            AddBaseType<bool[]>();
            AddBaseType<char[]>();
            AddBaseType<string[]>();
            AddBaseType<byte[]>();
            AddBaseType<sbyte[]>();
            AddBaseType<DateTime[]>();
            AddBaseType<decimal[]>();
            //基础序列化List
            AddBaseType<List<short>>();
            AddBaseType<List<int>>();
            AddBaseType<List<long>>();
            AddBaseType<List<ushort>>();
            AddBaseType<List<uint>>();
            AddBaseType<List<ulong>>();
            AddBaseType<List<float>>();
            AddBaseType<List<double>>();
            AddBaseType<List<bool>>();
            AddBaseType<List<char>>();
            AddBaseType<List<string>>();
            AddBaseType<List<byte>>();
            AddBaseType<List<sbyte>>();
            AddBaseType<List<DateTime>>();
            AddBaseType<List<decimal>>();
            //基础序列化List
            AddBaseType<List<short[]>>();
            AddBaseType<List<int[]>>();
            AddBaseType<List<long[]>>();
            AddBaseType<List<ushort[]>>();
            AddBaseType<List<uint[]>>();
            AddBaseType<List<ulong[]>>();
            AddBaseType<List<float[]>>();
            AddBaseType<List<double[]>>();
            AddBaseType<List<bool[]>>();
            AddBaseType<List<char[]>>();
            AddBaseType<List<string[]>>();
            AddBaseType<List<byte[]>>();
            AddBaseType<List<sbyte[]>>();
            AddBaseType<List<DateTime[]>>();
            AddBaseType<List<decimal[]>>();
            //其他可能用到的
            AddBaseType<Vector2>();
            AddBaseType<Vector2[]>();
            AddBaseType<List<Vector2>>();
            AddBaseType<Vector3>();
            AddBaseType<Vector3[]>();
            AddBaseType<List<Vector3>>();
            AddBaseType<Vector4>();
            AddBaseType<Vector4[]>();
            AddBaseType<List<Vector4>>();
            AddBaseType<Rect>();
            AddBaseType<Rect[]>();
            AddBaseType<List<Rect>>();
            AddBaseType<Quaternion>();
            AddBaseType<Quaternion[]>();
            AddBaseType<List<Quaternion>>();
            AddBaseType<GameObject>();
            AddBaseType<GameObject[]>();
            AddBaseType<List<GameObject>>();
            AddBaseType<Transform>();
            AddBaseType<Transform[]>();
            AddBaseType<List<Transform>>();
            AddBaseType<Color>();
            AddBaseType<Color[]>();
            AddBaseType<List<Color>>();
            AddBaseType<Color32>();
            AddBaseType<Color32[]>();
            AddBaseType<List<Color32>>();
            AddBaseType<Bounds>();
            AddBaseType<Bounds[]>();
            AddBaseType<List<Bounds>>();
            //框架操作同步用到
            //AddSerializeType<Operation>();
            //AddSerializeType<Operation[]>();
            //AddSerializeType<OperationList>();
        }
        public static void AddBaseType<T>()
        {
            var type = typeof(T);
            if (serializeType1s.ContainsKey(type)) return;
            serializeTypes.Add((ushort)serializeTypes.Count, type);
            serializeType1s.Add(type, (ushort)serializeType1s.Count);
        }
        private static ushort TypeToIndex(Type type)
        {
            //类型转索引
            if (serializeType1s.TryGetValue(type, out ushort typeHash))
                return typeHash;
            throw new KeyNotFoundException($"没有注册[{type}]类型的序列化对象");
        }
        public static Type IndexToType(ushort typeIndex)
        {
            if (serializeTypes.TryGetValue(typeIndex, out Type type))
                return type;
            return null;
        }
        public static byte[] Serialize(object[] data)
        {
            using (MemoryStream stream = new MemoryStream())
            using (BinaryWriter writer = new BinaryWriter(stream))
            {
                foreach (object obj in data)
                {
                    Type type = obj == null ? typeof(DBNull) : obj.GetType();
                    writer.Write(TypeToIndex(type));
                    if (obj == null) continue;
                    //WriteObject(writer, obj, type);
                    writer.Write(JsonConvert.SerializeObject(obj,
                        new JsonSerializerSettings
                        {
                            Converters = new List<JsonConverter> {
                                new Vector2Converter(),
                                new Vector3Converter(),
                                new Vector4Converter(),
                                new ColorConverter(),
                                new Color32Converter(),
                                new QuaternionConverter(),
                                new TransformConverter(),
                                new GameObjectConverter(),
                            }
                        }));
                }
                return stream.ToArray();
            }
        }
        private static void WriteObject(BinaryWriter writer, object value, Type type)
        {
            if (type == typeof(string))
            {
                writer.Write((string)value);
            }
            else if (type == typeof(int))
            {
                writer.Write((int)value);
            }
            else if (type == typeof(bool))
            {
                writer.Write((bool)value);
            }
            else if (type == typeof(Vector2))
            {
                Vector2 w = (Vector2)value;
                writer.Write(w.x);
                writer.Write(w.y);
            }
            else if (type == typeof(Vector3))
            {
                Vector3 w = (Vector3)value;
                writer.Write(w.x);
                writer.Write(w.y);
                writer.Write(w.z);
            }
            else if (type.IsArray || type.IsGenericType)
            {
            }
            else
            {
            }
        }
        public static object[] Deserialize(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(stream))
            {
                List<object> deserializedObjects = new List<object>();
                while (stream.Position < stream.Length)
                {
                    ushort typeIndex = reader.ReadUInt16();
                    Type type = IndexToType(typeIndex);
                    if (type == null)
                    {
                        // 处理未知类型
#if Server
                        Console.WriteLine("反序列化未知类型");
#else
                        Debug.Log("反序列化未知类型");
#endif
                    }
                    else if (type == typeof(DBNull))
                    {
                        deserializedObjects.Add(null);
                    }
                    else
                    {
                        //deserializedObjects.Add(ReadObject(reader, type));
                        deserializedObjects.Add(JsonConvert.DeserializeObject(reader.ReadString(), type));
                        // 然后使用 Activator.CreateInstance 或其他方式构造对象
                    }
                }
                return deserializedObjects.ToArray();
            }
        }
        private static object ReadObject(BinaryReader reader, Type type)
        {
            if (type == typeof(string))
            {
                return (string)reader.ReadString();
            }
            else if (type == typeof(int))
            {
                return (int)reader.ReadInt32();
            }
            else if (type == typeof(bool))
            {
                return (bool)reader.ReadBoolean();
            }
            else if (type == typeof(Vector2))
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                return new Vector2(x, y);
            }
            else if (type == typeof(Vector3))
            {
                float x = reader.ReadSingle();
                float y = reader.ReadSingle();
                float z = reader.ReadSingle();
                return new Vector3(x, y, z);
            }
            else if (type.IsArray || type.IsGenericType)
            {
            }
            else
            {
            }
            return null;
        }
        public static string GetObjectString(object[] data)
        {
            if (data == null) return "null";
            return string.Join(",", data);
        }
        public static string ByteArrayToHexString(byte[] byteArray)
        {
            StringBuilder hex = new StringBuilder(byteArray.Length * 2);
            foreach (byte b in byteArray)
            {
                hex.AppendFormat("{0:X2} ", b);
            }
            return hex.ToString();
        }
    }
}
