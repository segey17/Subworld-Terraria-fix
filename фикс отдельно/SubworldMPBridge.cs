using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using SubworldLibrary;

namespace SubworldMPBridge
{
    public class SubworldMPBridge : Mod
    {
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            string subworldName = reader.ReadString();

            if (!string.IsNullOrEmpty(subworldName))
            {
                if (Main.netMode == NetmodeID.Server)
                {
                    Logger.Info($"[Server] Entering subworld {subworldName}");
                    SubworldSystem.Enter(subworldName);
                }
                else if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    Logger.Info($"[Client] Entering subworld {subworldName}");
                    SubworldSystem.Enter(subworldName);
                }
            }
        }
    }
}
