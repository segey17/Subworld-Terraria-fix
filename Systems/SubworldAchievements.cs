using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System.Collections.Generic;


namespace SubworldSupport
{
    public class SubworldAchievements : ModSystem
    {
        private static HashSet<string> unlockedAchievements = new();

        // Сохраняем данные
        public override void SaveWorldData(TagCompound tag)
        {
            tag["UnlockedAchievements"] = new List<string>(unlockedAchievements);
        }

        // Загружаем данные
        public override void LoadWorldData(TagCompound tag)
        {
            unlockedAchievements = new HashSet<string>(tag.GetList<string>("UnlockedAchievements"));
        }

        public static void Unlock(string key)
        {
            if (!unlockedAchievements.Contains(key))
            {
                unlockedAchievements.Add(key);
                Main.NewText($"Открыто достижение: {key}!", Microsoft.Xna.Framework.Color.Gold);
            }
        }

        public static bool HasUnlocked(string key) => unlockedAchievements.Contains(key);
    }
}
