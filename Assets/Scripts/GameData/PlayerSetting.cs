using System;
using System.Collections.Generic;

namespace GameData
{
    // 角色职业设定
    public static class PlayerSetting
    {
        public class PlayerInfo
        {
            public uint Serial;
            public uint WeaponSerial;
            public String Name;
            public String Desc;
            public String Skill;
            public int ATKSkill;
            public int SpecialSkill;
            public float ATK;
            public float HP;
            public float MP;
            public float DEF;
            public float SPD;
        }

        
        public class PlayerGrowBase
        {
            public float HP;
            public float MP;
            public float ATK;
            public float DEF;
            public float SPD;
        }
        // 角色成长设定
        public static PlayerGrowBase PlayerGrow = new PlayerGrowBase()
        {
            HP  = 0.06f,
            MP  = 0.02f,
            ATK = 0.02f,
            DEF = 0.03f,
            SPD = 0.01f,
        };
        
        public static List<PlayerInfo> PlayerInfos = new List<PlayerInfo>()
        {
            new PlayerInfo()
            {
                Serial = 105053,
                Name = "剑士",
                Desc = "一把圣剑披荆斩棘",
                Skill = "弧月斩",
                
                ATKSkill = 0,   //普攻
                SpecialSkill = 15,   //弧月斩
                ATK = 5,
                HP = 4,
                MP = 2.6f,
                DEF = 4,
                SPD = 3,
            },
            new PlayerInfo()
            {
                Serial = 106126,
                WeaponSerial = 20814,
                Name = "弓箭手",
                Desc = "看我乱箭齐发",
                Skill = "乱射",
                
                ATKSkill = 1,   //射箭
                SpecialSkill = 2,   //乱射
                ATK = 4,
                HP = 3,
                MP = 2.6f,
                DEF = 2.8f,
                SPD = 5,
            },
            // new PlayerInfo()
            // {
            //     Serial = 106279,
            //     Name = "魔法师",
            //     Desc = "见识一下魔法的力量吧",
            //     Skill = "冰冻术",
            //     LevelScale = 1.2f,
            //     ATK = 5,
            //     HP = 2,
            //     MP = 5,
            //     DEF = 1,
            //     SPD = 2,
            // },
            // new PlayerInfo()
            // {
            //     Serial = 105272,
            //     Name = "传教士",
            //     Desc = "请赐予我神圣的祝福!",
            //     Skill = "治疗术",
            //     LevelScale = 1.2f,
            //     ATK = 2,
            //     HP = 5,
            //     MP = 4,
            //     DEF = 2,
            //     SPD = 2,
            // },
            // new PlayerInfo()
            // {
            //     Serial = 105352,
            //     Name = "气功师",
            //     Desc = "气破万物，波澜不惊",
            //     Skill = "气功弹",
            //     LevelScale = 1.2f,
            //     ATK = 4,
            //     HP = 4,
            //     MP = 2,
            //     DEF = 3,
            //     SPD = 2,
            // },
            // new PlayerInfo()
            // {
            //     Serial = 106352,
            //     Name = "封印师",
            //     Desc = "宝宝乖~姐姐给你吃糖",
            //     Skill = "封印",
            //     LevelScale = 1.2f,
            //     ATK = 2,
            //     HP = 3,
            //     MP = 4,
            //     DEF = 3,
            //     SPD = 3,
            // },
        };
    }
}