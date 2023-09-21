using System;
using System.Collections.Generic;

namespace GameData
{
    // 怪物设定
    public static class EnemySetting
    {
        public class EnemyInfo:PlayerSetting.PlayerInfo
        {
            // 怪物ID
            public int ID;
            // 怪物技能列表与对应等级  int[]{SkillID,Level}
            public List<int[]> Skills = new List<int[]>(){new int[]{0,1}};
        }

        // 怪物成长设定
        public static PlayerSetting.PlayerGrowBase EnemyGrow = new PlayerSetting.PlayerGrowBase()
        {
            HP  = 0.045f,
            MP  = 0.1f,
            ATK = 0.02f,
            DEF = 0.03f,
            SPD = 0.01f,
        };

        public static List<EnemyInfo> EnemyInfos = new List<EnemyInfo>()
        {
            new EnemyInfo()
            {
                ID = 0,
                Serial = 101800,
                Name = "哥布林",
                Desc = "哥布林是一种矮小的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 2.5f,
                DEF = 2.5f,
                SPD = 2f,
                HP = 3.5f,
                MP = 5,
                Skills = new List<int[]>()
                {
                    new int[]{0,1},
                    new int[]{10,1}
                }
            },
            new EnemyInfo()
            {
                ID = 1,
                Serial = 101801,
                Name = "红帽哥布林",
                Desc = "哥布林是一种矮小的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 2.5f,
                DEF = 2.5f,
                SPD = 2f,
                HP = 4f,
                MP = 5,
                Skills = new List<int[]>()
                {
                    new int[]{0,1},
                    new int[]{10,1}
                }
            },
            new EnemyInfo()
            {
                ID = 2,
                Serial = 101400,
                Name = "树精",
                Desc = "树精是一种生活在森林中的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 2.5f,
                DEF = 4,
                SPD = 1.5f,
                HP = 5f,
                MP = 5,
                Skills = new List<int[]>()
                {
                    new int[]{0,1},
                    new int[]{10,1}
                }
            },
            new EnemyInfo()
            {
                ID = 3,
                Serial = 101201,
                Name = "使魔",
                Desc = "使魔是一种生活在森林中的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 2.5f,
                DEF = 2.5f,
                SPD = 3f,
                HP = 2.4f,
                MP = 5,
                Skills = new List<int[]>()
                {
                    new int[]{0,1},
                    new int[]{8,1}
                }
            },
            new EnemyInfo()
            {
                ID = 4,
                Serial = 101242,
                Name = "小蝙蝠(红)",
                Desc = "小蝙蝠是一种生活在森林中的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 2.5f,
                DEF = 2.5f,
                SPD = 3f,
                HP = 2.4f,
                MP = 5
            },
            new EnemyInfo()
            {
                ID = 5,
                Serial = 101240,
                Name = "小蝙蝠(绿)",
                Desc = "小蝙蝠是一种生活在森林中的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 2.5f,
                DEF = 2.5f,
                SPD = 2.4f,
                HP = 2.5f,
                MP = 0
            },
            new EnemyInfo()
            {
                ID = 6,
                Serial = 101313,
                Name = "蜜蜂",
                Desc = "蜜蜂是一种生活在森林中的怪物，它们喜欢在森林中出没，有时也会出现在城镇附近。",
                
                ATK = 5f,
                DEF = 1,
                SPD = 4f,
                HP = 2.5f,
                MP = 5,
                Skills = new List<int[]>()
                {
                    new int[]{0,1},
                    new int[]{9,1},
                }
            },
            new EnemyInfo()
            {
                ID = 7,
                Serial = 101020,
                Name = "老鼠",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 2.5f,
                DEF = 1f,
                SPD = 4f,
                HP = 2.5f,
                MP = 5
            },
            new EnemyInfo()
            {
                ID = 8,
                Serial = 101100,
                Name = "丧尸",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 3f,
                DEF = 3f,
                SPD = 1.5f,
                HP = 5f,
                MP = 0
            },
            new EnemyInfo()
            {
                ID = 9,
                Serial = 101120,
                Name = "小石魔像",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 2f,
                DEF = 2f,
                SPD = 2f,
                HP = 0.3f,
                MP = 0
            },
            new EnemyInfo()
            {
                ID = 10,
                Serial = 101250,
                Name = "大蝙蝠",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 2f,
                DEF = 2f,
                SPD = 2.5f,
                HP = 0.5f,
                MP = 0
            },
            new EnemyInfo()
            {
                ID = 11,
                Serial = 101330,
                Name = "蜘蛛",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 2f,
                DEF = 1f,
                SPD = 4f,
                HP = 0.5f,
                MP = 0
            },
            new EnemyInfo()
            {
                ID = 12,
                Serial = 101500,
                Name = "史莱姆",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 2f,
                DEF = 3f,
                SPD = 1.5f,
                HP = 0.8f,
                MP = 0
            },
            new EnemyInfo()
            {
                ID = 13,
                Serial = 101500,
                Name = "史莱姆",
                Desc = "喜欢在阴暗角落里穿行的生物。",
                
                ATK = 2f,
                DEF = 3f,
                SPD = 1.5f,
                HP = 0.8f,
                MP = 0
            },
        };

    }
}