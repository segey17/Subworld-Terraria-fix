using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace SubworldSupport
{
    public class SubworldPlayer : ModPlayer
    {
        public override void OnEnterWorld()
        {
            Main.NewText("Добро пожаловать в мир с Subworld Support Mod!", Color.LightGreen);

            // Проверим и выдадим достижение
            if (!SubworldAchievements.HasUnlocked("Первый вход"))
            {
                SubworldAchievements.Unlock("Первый вход");
            }
        }
    }
}
