using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AldursLab.WurmApi
{
    public static class MeditationPaths
    {
        // knowledge, insanity, power, love, hate
        static readonly string[] Level0 = { "Uninitiated" };
        static readonly string[] Level1 = { "Initiate" };
        static readonly string[] Level2 = { "Eager", "Disturbed", "Gatherer", "Nice", "Ridiculous" };
        static readonly string[] Level3 = { "Explorer", "Crazed", "Greedy", "Gentle", "Envious" };
        static readonly string[] Level4 = { "Sheetfolder", "Deranged", "Strong", "Warm", "Hateful" };
        static readonly string[] Level5 = { "Desertmind", "Sicko", "Released", "Goodhearted", "Finger" };
        static readonly string[] Level6 = { "Observer", "Mental", "Unafraid", "Giving", "Sheep" };
        static readonly string[] Level7 = { "Bookkeeper", "Psycho", "Brave", "Rock", "Snake" };
        static readonly string[] Level8 = { "Mud-dweller", "Beast", "Performer", "Splendid", "Shark" };
        static readonly string[] Level9 = { "Thought Eater", "Maniac", "Liberator", "Protector", "Infection" };
        static readonly string[] Level10 = { "Crooked", "Drooling", "Force", "Respectful", "Swarm" };
        static readonly string[] Level11 = { "Enlightened", "Gone", "Vibrant Light", "Saint", "Free" };
        static readonly string[] Level12 = { "12th Hierophant", "12th Eidolon", "12th Sovereign", "12th Deva", "12th Harbinger" };
        static readonly string[] Level13 = { "13th Hierophant", "13th Eidolon", "13th Sovereign", "13th Deva", "13th Harbinger" };
        static readonly string[] Level14 = { "14th Hierophant", "14th Eidolon", "14th Sovereign", "14th Deva", "14th Harbinger" };
        static readonly string[] Level15 = { "15th Hierophant", "15th Eidolon", "15th Sovereign", "15th Deva", "15th Harbinger" };
        static readonly string[] Level16 = { "16th Hierophant", "16th Eidolon", "16th Sovereign", "16th Deva", "16th Harbinger" };

        static readonly Dictionary<int, string[]> LevelToTitlesMap = new Dictionary<int, string[]>();
        static readonly Dictionary<int, int> LevelToCooldownInHoursMap = new Dictionary<int, int>();

        const int CooldownMax = 576;

        static MeditationPaths()
        {
            LevelToTitlesMap.Add(0, Level0);
            LevelToTitlesMap.Add(1, Level1);
            LevelToTitlesMap.Add(2, Level2);
            LevelToTitlesMap.Add(3, Level3);
            LevelToTitlesMap.Add(4, Level4);
            LevelToTitlesMap.Add(5, Level5);
            LevelToTitlesMap.Add(6, Level6);
            LevelToTitlesMap.Add(7, Level7);
            LevelToTitlesMap.Add(8, Level8);
            LevelToTitlesMap.Add(9, Level9);
            LevelToTitlesMap.Add(10, Level10);
            LevelToTitlesMap.Add(11, Level11);
            LevelToTitlesMap.Add(12, Level12);
            LevelToTitlesMap.Add(13, Level13);
            LevelToTitlesMap.Add(14, Level14);
            LevelToTitlesMap.Add(15, Level15);
            LevelToTitlesMap.Add(16, Level16);

            LevelToCooldownInHoursMap.Add(0, 0);
            LevelToCooldownInHoursMap.Add(1, 12);
            LevelToCooldownInHoursMap.Add(2, 24);
            LevelToCooldownInHoursMap.Add(3, 72);
            LevelToCooldownInHoursMap.Add(4, 144);
            LevelToCooldownInHoursMap.Add(5, 288);
            LevelToCooldownInHoursMap.Add(6, 576);
            LevelToCooldownInHoursMap.Add(7, 576);
            LevelToCooldownInHoursMap.Add(8, 576);
            LevelToCooldownInHoursMap.Add(9, 576);
            LevelToCooldownInHoursMap.Add(10, 576);
            LevelToCooldownInHoursMap.Add(11, 576);
            LevelToCooldownInHoursMap.Add(12, 576);
            LevelToCooldownInHoursMap.Add(13, 576);
            LevelToCooldownInHoursMap.Add(14, 576);
            LevelToCooldownInHoursMap.Add(15, 576);
            LevelToCooldownInHoursMap.Add(16, 576);
        }

        /// <summary>
        /// Finds a numeric level of the meditation path, if it was succesfully parsed out of the contents, -1 if not.
        /// Numbers reflect "Level" as listed in WurmPedia.
        /// </summary>
        /// <param name="logEntryContent"></param>
        /// <returns></returns>
        public static int FindLevel(string logEntryContent)
        {
            foreach (var item in LevelToTitlesMap)
            {
                foreach (string title in item.Value)
                {
                    if (Regex.IsMatch(logEntryContent, title))
                    {
                        return item.Key;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Returns number of hours required to reach this level. 
        /// If the level is higher, than largest level known by WurmApi, max cooldown value is returned.
        /// If level is invalid (eg -1), returns -1;
        /// </summary>
        /// <param name="nextMeditLevel"></param>
        /// <returns></returns>
        public static int GetCooldownHoursForLevel(int nextMeditLevel)
        {
            int result;
            if (LevelToCooldownInHoursMap.TryGetValue(nextMeditLevel, out result))
            {
                return result;
            }
            else if (nextMeditLevel > LevelToTitlesMap.Keys.Max())
            {
                return CooldownMax;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Returns the list of known meditation levels with arrays of matching titles.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<KeyValuePair<int, string[]>> GetLevelToTitlesPairs()
        {
            return LevelToTitlesMap.Select(pair => pair).ToArray();
        }
    }
}
