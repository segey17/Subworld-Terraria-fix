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
    public class MiniDungeon : Subworld
    {
        public override int Width => 800;
        public override int Height => 600;

        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new PassLegacy("Создание подземелья", GenerateDungeon)
        };

        private void GenerateDungeon(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Генерация подземелья...";

            for (int x = 50; x < Width - 50; x++)
            {
                for (int y = 100; y < Height - 50; y++)
                {
                    if (Main.rand.NextBool(20))
                        WorldGen.PlaceTile(x, y, TileID.Stone);
                    if (Main.rand.NextBool(50))
                        WorldGen.PlaceWall(x, y, WallID.Stone);
                }
            }

            // немного лавы внизу
            for (int x = 100; x < Width - 100; x++)
            {
                for (int y = Height - 60; y < Height - 20; y++)
                {
                    var tile = Main.tile[x, y];
                    tile.LiquidType = LiquidID.Lava;
                    tile.LiquidAmount = 255;
                }
            }
        }

        public override void OnEnter()
        {
            Main.NewText("Вы вошли в Подземелье!", Color.MediumPurple);
        }

        public override void OnExit()
        {
            Main.NewText("Вы вышли из Подземелья", Color.Gray);
        }
    }
}