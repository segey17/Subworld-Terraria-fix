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
    public class SkyKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Небесный Ключ");
            // Tooltip.SetDefault("Переносит на парящий в небесах остров\nМирное место для отдыха и созерцания\nПравый клик - подняться на остров\nЛевый клик - спуститься обратно");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.buyPrice(gold: 2);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item9;
            Item.consumable = false;
            Item.useTurn = true;
        }

        public override bool? UseItem(Player player)
        {
            if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
            {
                // Спускаемся с острова
                SubworldSystem.Exit();
                Main.NewText("Вы спустились с Небесного Острова", Color.LightBlue);
            }
            else
            {
                // Поднимаемся на остров
                SubworldSystem.Enter<Subworlds.SkyIsland>();
            }

            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            // Можно использовать в любое время
            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
            {
                tooltips.Add(new TooltipLine(Mod, "SkyStatus", "Вы находитесь на Небесном Острове")
                {
                    OverrideColor = Color.LightBlue
                });
                tooltips.Add(new TooltipLine(Mod, "ExitHint", "Используйте для спуска")
                {
                    OverrideColor = Color.Orange
                });
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "EnterHint", "Используйте для подъема на остров")
                {
                    OverrideColor = Color.LightGreen
                });
                tooltips.Add(new TooltipLine(Mod, "PeacefulHint", "Идеальное место для отдыха")
                {
                    OverrideColor = Color.LightCyan
                });
            }

            // Показываем текущее время дня
            string timeOfDay = Main.dayTime ? "День" : "Ночь";
            tooltips.Add(new TooltipLine(Mod, "TimeInfo", $"Время: {timeOfDay}")
            {
                OverrideColor = Main.dayTime ? Color.Yellow : Color.DarkBlue
            });
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cloud, 10);
            recipe.AddIngredient(ItemID.FallenStar, 3);
            recipe.AddIngredient(ItemID.Feather, 5);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddTile(TileID.SkyMill);
            recipe.Register();
        }

        public override void UpdateInventory(Player player)
        {
            // Даем игроку небесные баффы на острове
            if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
            {
                player.AddBuff(BuffID.Featherfall, 2); // Медленное падение
                player.AddBuff(BuffID.Swiftness, 2); // Ускорение

                // Увеличиваем высоту прыжка
                player.jumpSpeedBoost += 2f;
            }
        }

        public override void HoldItem(Player player)
        {
            // Создаем небесные эффекты вокруг игрока
            if (Main.rand.NextBool(8))
            {
                Vector2 position = player.Center + new Vector2(Main.rand.Next(-40, 40), Main.rand.Next(-40, 40));

                if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
                {
                    // На острове - облачные частицы
                    Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.Cloud, 0f, -1f, 100, Color.White, 1.2f);
                    dust.noGravity = true;
                }
                else
                {
                    // В обычном мире - звездные частицы
                    Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.Enchanted_Gold, 0f, 0f, 100, Color.LightBlue, 0.8f);
                    dust.noGravity = true;
                }
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Добавляем небесное свечение
            if (SubworldSystem.IsActive<Subworlds.SkyIsland>())
            {
                // На острове - голубое свечение
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi * i / 4);
                    spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position + offset, frame, Color.LightBlue * 0.4f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }
            else
            {
                // В обычном мире - золотое свечение
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(1, 0).RotatedBy(MathHelper.TwoPi * i / 4);
                    spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position + offset, frame, Color.Gold * 0.3f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }

        public override void PostDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Добавляем пульсирующий эффект
            float pulse = 1f + (float)Math.Sin(Main.GameUpdateCount * 0.05f) * 0.1f;
            Color glowColor = SubworldSystem.IsActive<Subworlds.SkyIsland>() ? Color.LightBlue : Color.Gold;

            if (Main.GameUpdateCount % 120 < 60)
            {
                spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position, frame, glowColor * 0.3f, 0f, origin, scale * pulse, SpriteEffects.None, 0f);
            }
        }

        public override void PostUpdate()
        {
            // Создаем падающие звезды вокруг предмета когда он на земле
            if (Main.rand.NextBool(60))
            {
                Vector2 position = Item.Center + new Vector2(Main.rand.Next(-50, 50), -50);
                Dust dust = Dust.NewDustDirect(position, 1, 1, DustID.Enchanted_Gold, 0f, 2f, 100, Color.White, 1.0f);
                dust.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            // Предмет всегда немного светится
            return Color.Lerp(lightColor, Color.White, 0.3f);
        }
    }
}
