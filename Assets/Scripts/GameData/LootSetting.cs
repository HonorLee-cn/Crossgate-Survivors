using System.Collections.Generic;
using UnityEngine;

namespace GameData
{
    // 掉落战利品设定
    public class LootSetting
    {
        // 掉落类型
        public enum LootType
        {
            Stone,
            Jawel,
            HP,
            MP,
            HPBottle,
            MPBottle,
            Box // 未使用
        }
        
        // 掉落物类型,等级及对应图档编号
        public static Dictionary<LootType,Dictionary<int,List<int>>> LootStore = new Dictionary<LootType, Dictionary<int, List<int>>>()
        {
            {
                //魔石
                LootType.Stone, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){27958,27959,27960,27961}},
                    {2,new List<int>(){27962,27963,27964,27965}},
                    {3,new List<int>(){27966,27967,27968,27969}},
                    {4,new List<int>(){27970,27971,27972,27973}},
                    {5,new List<int>(){27403,27404}},
                }
            },
            {
                //经验宝石
                LootType.Jawel, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){27977}},
                    {2,new List<int>(){27981}},
                    {3,new List<int>(){27984}},
                    {4,new List<int>(){27988}},
                    {5,new List<int>(){27352}},
                }
            },
            {
                //HP宝石
                LootType.HP, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){27976}},
                    {2,new List<int>(){27980}},
                    {3,new List<int>(){27985}},
                    {4,new List<int>(){27989}},
                    {5,new List<int>(){27989}},
                }
            },
            {
                //MP宝石
                LootType.MP, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){27974}},
                    {2,new List<int>(){27978}},
                    {3,new List<int>(){27982}},
                    {4,new List<int>(){27986}},
                    {5,new List<int>(){27986}},
                }
            },
            {
                //HP药水
                LootType.HPBottle, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){26218}},
                    {2,new List<int>(){26218}},
                    {3,new List<int>(){26218}},
                    {4,new List<int>(){26218}},
                    {5,new List<int>(){26218}},
                }
            },
            {
                //MP药水
                LootType.MPBottle, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){26219}},
                    {2,new List<int>(){26219}},
                    {3,new List<int>(){26219}},
                    {4,new List<int>(){26219}},
                    {5,new List<int>(){26219}},
                }
            },
            {
                //箱子
                LootType.Box, new Dictionary<int, List<int>>()
                {
                    {1,new List<int>(){27893,27894}},
                    {2,new List<int>(){27996,27997}},
                    {3,new List<int>(){27895,27896}},
                    {4,new List<int>(){27897,27898}},
                    {5,new List<int>(){27899,27900}},
                }
            },
        };
        
        // 掉落物信息
        public class LootInfo
        {
            public LootType Type;
            public int Level;
            public int Serial;
        }
        
        // 获取掉落物信息
        public static LootInfo GetLootInfo(LootType type, int level)
        {
            var lootInfo = new LootInfo();
            lootInfo.Type = type;
            lootInfo.Level = level;
            int maxLevel = 5;
            if (level > maxLevel)
            {
                level = maxLevel;
            }
            var lootStore = LootStore[type];
            var lootList = lootStore[level];
            var index = Random.Range(0, lootList.Count);
            lootInfo.Serial = lootList[index];
            return lootInfo;
        }

        // 获取随机掉落类型
        public static LootType GetRandomLootType()
        {
            // 掉落类型
            List<LootType> lootTypeList = new List<LootType>()
            {
                LootType.Jawel, LootType.Stone, LootType.HP, LootType.MP, LootType.HPBottle, LootType.MPBottle
            };
            // 掉落权重
            List<int> lootRateList = new List<int>()
            {
                50, 20, 3, 3, 1, 1
            };
            LootType lootType = LootType.Jawel;
            // 计算总权重
            int totalWeight = 0;

            foreach (int weight in lootRateList)
            {
                totalWeight += weight;
            }

            // 生成随机数
            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            // 根据随机数选择掉落物品
            int cumulativeWeight = 0;

            for (int i = 0; i < lootTypeList.Count; i++)
            {
                cumulativeWeight += lootRateList[i];

                if (randomValue < cumulativeWeight)
                {
                    lootType = lootTypeList[i];
                    break;
                }
            }
            return lootType;
        }
    }
}