using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.IO;

namespace SubworldSupport
{
    public class ExampleIntegration : ModSystem
    {
        public static bool IsPlayerInOurSubworld(Player player)
        {
            return SubworldSystem.IsActive<Subworlds.BossArena>() ||
                   SubworldSystem.IsActive<Subworlds.MiniDungeon>() ||
                   SubworldSystem.IsActive<Subworlds.SkyIsland>();
        }

        public static string GetCurrentSubworldName()
        {
            if (SubworldSystem.IsActive<Subworlds.BossArena>())
                return "BossArena";
            else if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
                return "MiniDungeon";
            else if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
                return "SkyIsland";
            else
                return "MainWorld";
        }

        public static bool SafeTeleportToSubworld(Player player, string subworldType)
        {
            if (player.hostile || player.dead)
            {
                Main.NewText("Нельзя телепортироваться во время боя или смерти!", Color.Red);
                return false;
            }

            switch (subworldType.ToLower())
            {
                case "arena":
                    if (!player.HasItem(ModContent.ItemType<Items.BossArenaKey>()))
                    {
                        Main.NewText("У вас нет Ключа от Арены Боссов!", Color.Red);
                        return false;
                    }
                    SubworldSystem.Enter<Subworlds.BossArena>();
                    return true;

                case "dungeon":
                    if (!player.HasItem(ModContent.ItemType<Items.DungeonKey>()))
                    {
                        Main.NewText("У вас нет Ключа от Мини-Данжа!", Color.Red);
                        return false;
                    }
                    SubworldSystem.Enter<Subworlds.MiniDungeon>();
                    return true;

                case "sky":
                    if (!player.HasItem(ModContent.ItemType<Items.SkyKey>()))
                    {
                        Main.NewText("У вас нет Небесного Ключа!", Color.Red);
                        return false;
                    }
                    SubworldSystem.Enter<Subworlds.SkyIsland>();
                    return true;

                default:
                    return false;
            }
        }

        public override void PostUpdateWorld()
        {
            if (IsPlayerInOurSubworld(Main.LocalPlayer))
            {
                HandleSubworldEnemies();
            }

            HandleSubworldSpecificLogic();
        }

        private void HandleSubworldEnemies()
        {
            foreach (NPC npc in Main.npc)
            {
                if (npc.active && npc.type != NPCID.None)
                {
                    if (SubworldSystem.IsActive<Subworlds.BossArena>() && npc.boss)
                    {
                        npc.damage = (int)(npc.damage * 1.2f);
                    }

                    if (SubworldSystem.IsActive<Subworlds.MiniDungeon>() && !npc.boss)
                    {
                        npc.velocity *= 0.8f;
                    }

                    if (SubworldSystem.IsActive<Subworlds.SkyIsland>() && npc.damage > 0)
                    {
                        npc.active = false;
                    }
                }
            }
        }

        private void HandleSubworldSpecificLogic()
        {
            Player player = Main.LocalPlayer;

            if (SubworldSystem.IsActive<Subworlds.BossArena>())
            {
                if (player.statLife < player.statLifeMax)
                    player.lifeRegen += 2;

                if (Main.rand.NextBool(30))
                {
                    Vector2 pos = player.Center + new Vector2(Main.rand.Next(-100, 100), Main.rand.Next(-100, 100));
                    Dust.NewDust(pos, 1, 1, DustID.Torch, 0f, 0f, 100, Color.Orange, 0.8f);
                }
            }

            if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
            {
                // Даём ночное зрение вместо несуществующего player.torch
                player.AddBuff(BuffID.NightOwl, 2);

                if (Main.rand.NextBool(60))
                {
                    Vector2 pos = player.Center + new Vector2(Main.rand.Next(-50, 50), Main.rand.Next(-50, 50));
                    Dust.NewDust(pos, 1, 1, DustID.MagicMirror, 0f, 0f, 100, Color.Purple, 0.6f);
                }
            }

            if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
            {
                player.moveSpeed += 0.2f;

                if (Main.rand.NextBool(20))
                {
                    Vector2 pos = player.Center + new Vector2(Main.rand.Next(-80, 80), Main.rand.Next(-80, 80));
                    Dust.NewDust(pos, 1, 1, DustID.Cloud, 0f, -1f, 100, Color.White, 1.0f);
                }
            }
        }
    }

    public class SubworldGlobalItem : GlobalItem
    {
        public override void UpdateInventory(Item item, Player player)
        {
            if (item.buffType > 0 && ExampleIntegration.IsPlayerInOurSubworld(player))
            {
                if (player.HasBuff(item.buffType))
                {
                    int buffIndex = player.FindBuffIndex(item.buffType);
                    if (buffIndex >= 0)
                        player.buffTime[buffIndex] = (int)(player.buffTime[buffIndex] * 1.1f);
                }
            }
        }

        public override bool CanUseItem(Item item, Player player)
        {
            if (item.type == ItemID.MagicMirror || item.type == ItemID.RecallPotion)
            {
                if (ExampleIntegration.IsPlayerInOurSubworld(player))
                {
                    Main.NewText("Этот предмет нельзя использовать в подмире!", Color.Red);
                    return false;
                }
            }

            return base.CanUseItem(item, player);
        }
    }

    public class SubworldGlobalNPC : GlobalNPC
    {
        public override bool PreAI(NPC npc)
        {
            if (ExampleIntegration.IsPlayerInOurSubworld(Main.LocalPlayer))
            {
                if (SubworldSystem.IsActive<Subworlds.BossArena>() && npc.boss)
                {
                    npc.velocity *= 1.1f;
                }

                if (SubworldSystem.IsActive<Subworlds.MiniDungeon>() && !npc.boss && Main.rand.NextBool(1800))
                {
                    Vector2 newPos = npc.Center + new Vector2(Main.rand.Next(-200, 200), Main.rand.Next(-200, 200));
                    npc.Center = newPos;

                    for (int i = 0; i < 20; i++)
                        Dust.NewDust(npc.Center, npc.width, npc.height, DustID.MagicMirror, 0f, 0f, 100, Color.Purple, 1.2f);
                }
            }

            return base.PreAI(npc);
        }

        public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot)
        {
            if (ExampleIntegration.IsPlayerInOurSubworld(Main.LocalPlayer))
            {
                if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
                {
                    npcLoot.Add(ItemDropRule.Common(ItemID.GoldCoin, 2, 1, 3));
                }
            }
        }
    }

    public class BossChecklistIntegration : ModSystem
    {
        public override void PostSetupContent()
        {
            if (ModLoader.TryGetMod("BossChecklist", out Mod bossChecklistMod))
            {
                // Интеграция placeholder — доработать при необходимости
            }
        }
    }

    public class SubworldEvent : ModSystem
    {
        public static bool skyIslandStorm = false;
        private int stormTimer = 0;

        public override void PostUpdateWorld()
        {
            if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
                HandleSkyStorm();
        }

        private void HandleSkyStorm()
        {
            if (!skyIslandStorm && Main.rand.NextBool(18000))
            {
                skyIslandStorm = true;
                stormTimer = 3600;
                Main.NewText("Небесная буря началась!", Color.LightBlue);
            }

            if (skyIslandStorm)
            {
                stormTimer--;

                if (Main.rand.NextBool(10))
                {
                    Vector2 lightningPos = new Vector2(Main.rand.Next(0, 1000 * 16), Main.rand.Next(0, 100 * 16));
                    Dust.NewDust(lightningPos, 1, 1, DustID.Electric, 0f, 10f, 100, Color.LightBlue, 2.0f);
                }

                Main.windSpeedCurrent = 0.8f;

                if (stormTimer <= 0)
                {
                    skyIslandStorm = false;
                    Main.NewText("Небесная буря закончилась", Color.LightGreen);
                    Main.windSpeedCurrent = 0.1f;
                }
            }
        }
    }
}