using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using Terraria.IO;
using Terraria.GameContent.Generation;

namespace SubworldSupport.Subworlds
{
    public class BossArena : Subworld
    {
        public override int Width => 1200;
        public override int Height => 600;

        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new PassLegacy("Создание арены", GenerateArena)
        };

        private void GenerateArena(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Генерация арены боссов...";

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        WorldGen.PlaceTile(x, y, TileID.Obsidian);
                    }
                }
            }

            for (int y = 150; y < Height - 50; y += 100)
            {
                for (int x = 50; x < Width - 50; x++)
                {
                    WorldGen.PlaceTile(x, y, TileID.Platforms);
                }
            }
        }

        public override void OnEnter()
        {
            Main.NewText("Добро пожаловать на Арену Боссов!", Color.OrangeRed);
            Player player = Main.LocalPlayer;
            player.statLife = player.statLifeMax2;
            player.statMana = player.statManaMax2;

            for (int i = player.buffType.Length - 1; i >= 0; i--)
            {
                if (player.buffType[i] > 0 && Main.debuff[player.buffType[i]])
                {
                    player.DelBuff(i);
                }
            }
        }

        public override void OnExit()
        {
            Main.NewText("Вы покинули Арену Боссов", Color.Orange);
        }

        public override void Update()
        {
            Player player = Main.LocalPlayer;
            if (player.statLife < player.statLifeMax2)
                player.lifeRegen += 2;
            if (player.statMana < player.statManaMax2)
                player.manaRegenBonus += 2;

            if (Main.rand.NextBool(40))
            {
                Vector2 pos = player.Center + new Vector2(Main.rand.Next(-120, 120), Main.rand.Next(-120, 120));
                Dust.NewDust(pos, 1, 1, DustID.Torch, 0f, 0f, 100, Color.OrangeRed, 1f);
            }
        }
    }
}