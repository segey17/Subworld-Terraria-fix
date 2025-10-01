using System;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace SubworldSupport.Items
{
    public class DungeonKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ключ от Мини-Данжа");
            // Tooltip.SetDefault("Открывает портал в таинственный подземный лабиринт\nПолон ловушек, врагов и сокровищ\nПравый клик - войти в данж\nЛевый клик - выйти из данжа");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.useTime = 40;
            Item.useAnimation = 40;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.buyPrice(gold: 3);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Unlock;
            Item.consumable = false;
            Item.useTurn = true;
        }

        public override bool? UseItem(Player player)
        {
            if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
            {
                // Выходим из данжа
                SubworldSystem.Exit();
                Main.NewText("Вы покинули Мини-Данж", Color.Purple);
            }
            else
            {
                // Входим в данж
                SubworldSystem.Enter<Subworlds.MiniDungeon>();
            }

            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            // Проверяем, есть ли у игрока факелы (нужны для данжа)
            if (!SubworldSystem.IsActive<Subworlds.MiniDungeon>() && !player.HasItem(ItemID.Torch))
            {
                Main.NewText("Вам нужны факелы для исследования данжа!", Color.Yellow);
                return false;
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
            {
                tooltips.Add(new TooltipLine(Mod, "DungeonStatus", "Вы находитесь в Мини-Данже")
                {
                    OverrideColor = Color.Purple
                });
                tooltips.Add(new TooltipLine(Mod, "ExitHint", "Используйте для выхода из данжа")
                {
                    OverrideColor = Color.Orange
                });
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "EnterHint", "Используйте для входа в данж")
                {
                    OverrideColor = Color.LightGreen
                });

                // Показываем требования
                if (!Main.LocalPlayer.HasItem(ItemID.Torch))
                {
                    tooltips.Add(new TooltipLine(Mod, "RequirementWarning", "Требуется: Факелы")
                    {
                        OverrideColor = Color.Red
                    });
                }
                else
                {
                    tooltips.Add(new TooltipLine(Mod, "RequirementMet", "Требования выполнены")
                    {
                        OverrideColor = Color.LightGreen
                    });
                }
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BlueBrick, 50);
            recipe.AddIngredient(ItemID.Bone, 10);
            recipe.AddIngredient(ItemID.Cobweb, 20);
            recipe.AddIngredient(ItemID.ShadowOrb, 1);
            recipe.AddTile(TileID.DemonAltar);
            recipe.Register();

            // Альтернативный рецепт для миров с багрянцем
            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.BlueBrick, 50);
            recipe2.AddIngredient(ItemID.Bone, 10);
            recipe2.AddIngredient(ItemID.Cobweb, 20);
            recipe2.AddIngredient(ItemID.CrimsonHeart, 1);
            recipe2.AddTile(TileID.DemonAltar);
            recipe2.Register();
        }

        public override void UpdateInventory(Player player)
        {
            // Даем игроку ночное зрение в данже
            if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
            {
                player.AddBuff(BuffID.NightOwl, 2);
                player.AddBuff(BuffID.Spelunker, 2); // Показывает сокровища
            }
        }

        public override void HoldItem(Player player)
        {
            // Создаем магический эффект вокруг игрока когда держит ключ
            if (Main.rand.NextBool(10))
            {
                Vector2 position = player.Center + new Vector2(Main.rand.Next(-30, 30), Main.rand.Next(-30, 30));
                Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.MagicMirror, 0f, 0f, 100, Color.Purple, 0.8f);
                dust.noGravity = true;
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Добавляем мистическое свечение
            if (SubworldSystem.IsActive<Subworlds.MiniDungeon>())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(1.5f, 0).RotatedBy(MathHelper.TwoPi * i / 4);
                    spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position + offset, frame, Color.Purple * 0.4f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Добавляем анимированные искры
            if (Main.GameUpdateCount % 60 < 30)
            {
                float glowScale = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.1f) * 0.1f;
                spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position, frame, Color.Purple * 0.3f, 0f, origin, scale * glowScale, SpriteEffects.None, 0f);
            }
        }
    }
}
