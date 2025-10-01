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
    public class SkyIsland : Subworld
    {
        public override int Width => 600;
        public override int Height => 400;

        public override bool ShouldSave => false;
        public override bool NoPlayerSaving => true;

        public override List<GenPass> Tasks => new List<GenPass>()
        {
            new PassLegacy("Создание острова", GenerateIsland)
        };

        private void GenerateIsland(GenerationProgress progress, GameConfiguration configuration)
        {
            progress.Message = "Генерация небесного острова...";

            int islandCenterX = Width / 2;
            int islandCenterY = Height / 2;

            for (int x = islandCenterX - 60; x < islandCenterX + 60; x++)
            {
                for (int y = islandCenterY - 20; y < islandCenterY + 20; y++)
                {
                    if (Vector2.Distance(new Vector2(x, y), new Vector2(islandCenterX, islandCenterY)) < 40)
                    {
                        WorldGen.PlaceTile(x, y, TileID.Cloud);
                    }
                }
            }

            // озеро на острове
            for (int x = islandCenterX - 20; x < islandCenterX + 20; x++)
            {
                for (int y = islandCenterY - 5; y < islandCenterY + 5; y++)
                {
                    var tile = Main.tile[x, y];
                    tile.LiquidType = LiquidID.Water;
                    tile.LiquidAmount = 255;
                }
            }
        }

        public override void OnEnter()
        {
            Main.NewText("Вы оказались на Небесном острове!", Color.LightSkyBlue);
        }

        public override void OnExit()
        {
            Main.NewText("Вы покинули Небесный остров", Color.LightGray);
        }
    }
}