using System.Collections.Generic;

namespace GameData
{
    // 未完成
    public class BuffSetting
    {
        public class Buff
        {
            public BuffInfo BuffInfo;
            public int Level;
            public Buff(BuffInfo buffInfo, int level)
            {
                BuffInfo = buffInfo;
                Level = level;
            }
        }
        public class BuffInfo
        {
            public int ID;
            public string Name;
            public float Duration;
            public float DurationPerLevel;
            public float EffectCD;
            public float EffectCDReducePerLevel;
            public float Effect;
            public float EffectPerLevel;
            public int MaxLevel;
        }

        public static List<BuffInfo> BuffInfos = new List<BuffInfo>()
        {
            new BuffInfo()
            {
                ID = 0,
                Name = "中毒",
                Duration = 5f,
                DurationPerLevel = 1f,
                EffectCD = 2f,
                EffectCDReducePerLevel = 0.1f,
                Effect = 5,
                EffectPerLevel = 1,
                MaxLevel = 10
            }
        };
    }
}