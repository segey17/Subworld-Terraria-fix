using Microsoft.Xna.Framework;
using SubworldLibrary;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace SubworldSupport.Items
{
    public class BossArenaKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ключ от Арены Боссов");
            // Tooltip.SetDefault("Телепортирует в специальную арену для сражений с боссами\nПравый клик - войти в арену\nЛевый клик - выйти из арены");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.buyPrice(gold: 5);
            Item.rare = ItemRarityID.Orange;
            Item.UseSound = SoundID.Item44;
            Item.consumable = false;
            Item.useTurn = true;
        }

        public override bool? UseItem(Player player)
        {
            if (SubworldSystem.IsActive<Subworlds.BossArena>())
            {
                // Выходим из арены
                SubworldSystem.Exit();
                Main.NewText("Вы покинули Арену Боссов", Color.Orange);
            }
            else
            {
                // Входим в арену
                SubworldSystem.Enter<Subworlds.BossArena>();
            }

            return true;
        }

        public override bool AltFunctionUse(Player player)
        {
            return true;
        }

        public override bool CanUseItem(Player player)
        {
            // Нельзя использовать во время боя с боссом (кроме как в самой арене)
            if (!SubworldSystem.IsActive<Subworlds.BossArena>() && NPC.AnyNPCs(NPCID.EyeofCthulhu))
            {
                Main.NewText("Нельзя использовать во время боя с боссом!", Color.Red);
                return false;
            }

            return true;
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            if (SubworldSystem.IsActive<Subworlds.BossArena>())
            {
                tooltips.Add(new TooltipLine(Mod, "ArenaStatus", "Вы находитесь в Арене Боссов")
                {
                    OverrideColor = Color.Gold
                });
                tooltips.Add(new TooltipLine(Mod, "ExitHint", "Используйте для выхода из арены")
                {
                    OverrideColor = Color.Orange
                });
            }
            else
            {
                tooltips.Add(new TooltipLine(Mod, "EnterHint", "Используйте для входа в арену")
                {
                    OverrideColor = Color.LightGreen
                });
                tooltips.Add(new TooltipLine(Mod, "SafeHint", "Безопасное место для тренировки против боссов")
                {
                    OverrideColor = Color.LightBlue
                });
            }
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.GoldBar, 15);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.Register();
        }

        public override void UpdateInventory(Player player)
        {
            // Даем игроку небольшой бафф регенерации если ключ в инвентаре
            if (SubworldSystem.IsActive<Subworlds.BossArena>())
            {
                player.AddBuff(BuffID.Regeneration, 2);
            }
        }

        public override bool PreDrawInInventory(SpriteBatch spriteBatch, Vector2 position, Rectangle frame, Color drawColor, Color itemColor, Vector2 origin, float scale)
        {
            // Добавляем свечение если игрок в арене
            if (SubworldSystem.IsActive<Subworlds.BossArena>())
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = new Vector2(2, 0).RotatedBy(MathHelper.TwoPi * i / 4);
                    spriteBatch.Draw(ModContent.Request<Texture2D>(Texture).Value, position + offset, frame, Color.Gold * 0.3f, 0f, origin, scale, SpriteEffects.None, 0f);
                }
            }

            return true;
        }
    }
}
