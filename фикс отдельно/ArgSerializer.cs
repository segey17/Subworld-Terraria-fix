using Terraria.ModLoader;
using System.IO;
using System.Collections.Generic;

namespace SubworldMPBridge
{
    public static class ArgSerializer
    {
        // Универсальная сериализация аргументов
        public static void WriteArgs(ModPacket packet, params object[] args)
        {
            packet.Write((byte)args.Length);
            foreach (var arg in args)
            {
                switch (arg)
                {
                    case int i: packet.Write((byte)0); packet.Write(i); break;
                    case float f: packet.Write((byte)1); packet.Write(f); break;
                    case double d: packet.Write((byte)2); packet.Write(d); break;
                    case bool b: packet.Write((byte)3); packet.Write(b); break;
                    case string s: packet.Write((byte)4); packet.Write(s); break;
                    default: packet.Write((byte)255); break;
                }
            }
        }

        public static object[] ReadArgs(BinaryReader reader)
        {
            int count = reader.ReadByte();
            List<object> results = new List<object>();
            for (int i = 0; i < count; i++)
            {
                byte type = reader.ReadByte();
                switch (type)
                {
                    case 0: results.Add(reader.ReadInt32()); break;
                    case 1: results.Add(reader.ReadSingle()); break;
                    case 2: results.Add(reader.ReadDouble()); break;
                    case 3: results.Add(reader.ReadBoolean()); break;
                    case 4: results.Add(reader.ReadString()); break;
                    default: results.Add(null); break;
                }
            }
            return results.ToArray();
        }
    }
}
