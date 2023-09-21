using System.Collections.Generic;
using Controller;
using DG.Tweening;

namespace GameData
{
    // 关卡设定
    public static class LevelSetting
    {
        // BOSS信息
        public class BossInfo
        {
            // Boss信息
            public int EnemyID;
            public string NamePrefix = "";
            public int Level;
            public List<int[]> Skills;
            // 子怪物
            public int[] Allies;    // 小兵EnemyID序列
            public int SpawnPerAlly = 10;   // 每个小兵生成数量
            public float AlliesSpawnInterval = 60f; // 小兵生成间隔
            public int AlliesSpawnTimes = 10;   // 小兵生成次数
            public int AlliesSpawnCount = 0;    // 小兵生成次数计数
            public Tween SpawnTweener;  // 小兵生成计时器
            // 属性加成
            public Dictionary<string, float> AttrRate;
            
            // 开始生成小兵(战斗控制器自动调用)
            public void StartSpawnAllies(int level)
            {
                AlliesSpawnCount = 0;
                if (Allies != null && Allies.Length > 0)
                {
                    SpawnAllies(level);    
                }
                
            }
            
            // 停止生成小兵(战斗控制器自动调用)
            public void StopSpawnAllies()
            {
                SpawnTweener?.Kill();
            }
            
            // 生成小兵
            private void SpawnAllies(int level)
            {
                AlliesSpawnCount++;
                for (var i = 0; i < Allies.Length; i++)
                {
                    int allyID = Allies[i];
                    BattleController.SpawnEnemy(allyID, SpawnPerAlly, level);
                }

                SpawnTweener = DOTween.Sequence();
                SpawnTweener.SetDelay(AlliesSpawnInterval).onComplete = () =>
                {
                    if (AlliesSpawnCount < AlliesSpawnTimes)
                    {
                        SpawnAllies(level);
                    }
                };
            }
            
            
        }

        // 回合信息
        public class RoundInfo
        {
            // 回合等级
            public int Level;
            // 回合怪物ID序列
            public int[] EnemyIDs;
            // 回合怪物生成数量
            public int SpawnCountPerEnemy;
            public float Duration;  // -1则表示无限时间,当最后一个敌人死亡时进行下一关 >=0则表示持续时间,当时间到达时自动进行下一关
            // BOSS信息
            public List<BossInfo> BossInfos;
            
            // 干扰怪物列表,此列表会加入当前关卡的所有干扰列表中
            // 干扰怪物会随机在当前地图的四角和中心位置刷出,不受回合数限制,直到标记为最后波次内的BOSS全部死亡为止
            public int[] ExtraDisturbEnemyIDs;
        }
        
        // 关卡信息
        public class LevelInfo
        {
            // 关卡名称
            public string Name;
            // 封面图
            public int CoverIndex;
            // 背景音
            public int Audio;
            // 难度等级(暂无实际效果)
            public int Difficulty;
            // 地图宽度
            public int MapWidth;
            // 地图高度
            public int MapHeight;
            // 使用调色板编号
            public int PaletIndex;
            // 地板图档序列
            public List<uint> GroundSerials;
            // 装饰物图档序列
            public List<uint> DecorationsSerials;
            // 单位图档序列
            public List<uint> UnitSerials;
            // 边界图档序列
            public uint BorderSerial;
            // 回合信息
            public List<RoundInfo> RoundInfos;
            // NPC信息
            public List<InteractiveSetting.InteractiveInfo> NPCInfos;
        }


        public static List<LevelInfo> LevelInfos = new List<LevelInfo>()
        {
            new LevelInfo()
            {
                Name = "白天的芙蕾雅森林",
                CoverIndex = 0,
                Audio = 210,
                MapWidth = 50,
                MapHeight = 50,
                PaletIndex = 0,
                Difficulty = 1,
                GroundSerials = new List<uint> { 5355, 5356, 5357, 5358, 5359, 5360 },
                DecorationsSerials = new List<uint>
                {
                    11087, 11088, 11098, 11099, 11100, 11101, 11625, 11647, 11646, 11650,
                    11651, 11652, 11653, 11654, 11696, 11697, 11698, 12100, 12103, 12105,
                    12107, 12108, 12169, 12170, 12171, 12175, 12176, 12177, 12178, 12179, 12180
                },
                UnitSerials = new List<uint>
                {
                    10015, 10016, 10017, 10018, 10019, 10020, 10021, 10022, 10023, 10058,
                    10059, 10060, 10061, 10062, 10063, 10064, 10065, 10066, 10067, 10068,
                    10069, 10070, 10071, 10072, 10073, 10074, 10085, 10086, 10087, 10088,
                    10089, 10090, 10091, 10092, 10093, 10094
                },
                NPCInfos = new List<InteractiveSetting.InteractiveInfo>()
                {
                    new InteractiveSetting.InteractiveInfo()
                    {
                        Type = InteractiveSetting.Type.Shop,
                        Serial = 14510,
                    },
                    new InteractiveSetting.InteractiveInfo()
                    {
                        Type = InteractiveSetting.Type.Doctor,
                        Serial = 14088,
                    },
                    new InteractiveSetting.InteractiveInfo()
                    {
                        Type = InteractiveSetting.Type.Teacher,
                        Serial = 14101,
                    },
                },
                BorderSerial = 10014,
                RoundInfos = new List<RoundInfo>()
                {
                    // new RoundInfo()
                    // {
                    //     Level = 10,
                    //     EnemyIDs = new int[]{0,1,2,3,4,5,6},
                    //     IsBoss = true,
                    //     BossNamePrefix = "疯狂的",
                    //     Duration = -1,
                    //     // BossAllies = new []{0,1},
                    //     SpawnCountPerEnemy = 5
                    // },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{3},
                        Duration = 60,
                        SpawnCountPerEnemy = 10,
                        ExtraDisturbEnemyIDs = new int[]{3}
                    },
                    new RoundInfo()
                    {
                        Level = 2,
                        EnemyIDs = new int[]{4,5},
                        Duration = 60,
                        SpawnCountPerEnemy = 10,
                        ExtraDisturbEnemyIDs = new int[]{4,5}
                    },
                    new RoundInfo()
                    {
                        Level = 3,
                        EnemyIDs = new int[]{0},
                        Duration = 60,
                        SpawnCountPerEnemy = 15,
                        ExtraDisturbEnemyIDs = new int[]{0}
                    },
                    new RoundInfo()
                    {
                        Level = 4,
                        Duration = -1, 
                        BossInfos = new List<BossInfo>()
                        {
                            new BossInfo()
                            {
                                Level = 5,
                                EnemyID = 0,
                                NamePrefix = "丢了帽子的",
                                Allies = new []{3,4,5},
                                Skills = new List<int[]>()
                                {
                                    new []{11,1},
                                },
                            }
                        },
                    },
                    new RoundInfo()
                    {
                        Level = 5,
                        EnemyIDs = new int[]{1},
                        Duration = 60,
                        SpawnCountPerEnemy = 15,
                        ExtraDisturbEnemyIDs = new int[]{1,7,13,12}
                    },
                    new RoundInfo()
                    {
                        Level = 6,
                        EnemyIDs = new int[]{0,1},
                        Duration = 60,
                        SpawnCountPerEnemy = 15
                    },
                    new RoundInfo()
                    {
                        Level = 7,
                        EnemyIDs = new int[]{0,1,3,4,5},
                        Duration = 60,
                        SpawnCountPerEnemy = 10
                    },
                    new RoundInfo()
                    {
                        Level = 8,
                        Duration = -1,
                        BossInfos = new List<BossInfo>()
                        {
                            new BossInfo()
                            {
                                Level = 9,
                                EnemyID = 1,
                                NamePrefix = "找帽子的",
                                Allies = new []{0,1,3,4,5},
                                Skills = new List<int[]>()
                                {
                                    new []{11,1},
                                },
                                SpawnPerAlly = 10
                            }
                        },
                    },
                    new RoundInfo()
                    {
                        Level = 9,
                        Duration = -1,
                        BossInfos = new List<BossInfo>()
                        {
                            new BossInfo()
                            {
                                Level = 10,
                                EnemyID = 0,
                                NamePrefix = "帽子失而复得的",
                                Allies = new []{0,1},
                                Skills = new List<int[]>()
                                {
                                    new []{12,1},
                                },
                                SpawnPerAlly = 15,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"DEF", 1.5f},
                                }
                            },
                            new BossInfo()
                            {
                                Level = 10,
                                EnemyID = 1,
                                NamePrefix = "跟着傻乐的",
                                Allies = new []{3,4,5},
                                Skills = new List<int[]>()
                                {
                                    new []{11,1},
                                },
                                SpawnPerAlly = 10,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"ATK", 1.5f},
                                }
                            }
                        }
                        
                    },
                    new RoundInfo()
                    {
                        Level = 10,
                        EnemyIDs = new int[]{2},
                        Duration = 60,
                        SpawnCountPerEnemy = 30,
                        ExtraDisturbEnemyIDs = new int[]{2}
                    },
                    new RoundInfo()
                    {
                        Level = 11,
                        EnemyIDs = new int[]{6},
                        Duration = 60,
                        SpawnCountPerEnemy = 30,
                        ExtraDisturbEnemyIDs = new int[]{6}
                    },
                    new RoundInfo()
                    {
                        Level = 12,
                        EnemyIDs = new int[]{0,1,3,4,5},
                        Duration = 60,
                        SpawnCountPerEnemy = 15
                    },
                    new RoundInfo()
                    {
                        Level = 13,
                        Duration = -1,
                        BossInfos = new List<BossInfo>()
                        {
                            new BossInfo()
                            {
                                Level = 14,
                                EnemyID = 2,
                                NamePrefix = "树杈被砍的",
                                Allies = new []{3,4,5,6},
                                Skills = new List<int[]>()
                                {
                                    new []{14,1},
                                },
                                SpawnPerAlly = 15,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"DEF", 1.5f},
                                    {"MaxHP", 1.5f},
                                }
                            }
                        }
                    },
                    new RoundInfo()
                    {
                        Level = 15,
                        EnemyIDs = new int[]{2},
                        Duration = 60,
                        SpawnCountPerEnemy = 30
                    },
                    new RoundInfo()
                    {
                        Level = 16,
                        EnemyIDs = new int[]{0,1},
                        Duration = 60,
                        SpawnCountPerEnemy = 20
                    },
                    new RoundInfo()
                    {
                        Level = 17,
                        EnemyIDs = new int[]{0,1},
                        Duration = 60,
                        SpawnCountPerEnemy = 20
                    },
                    new RoundInfo()
                    {
                        Level = 18,
                        Duration = -1,
                        BossInfos = new List<BossInfo>()
                        {
                            new BossInfo()
                            {
                                Level = 19,
                                EnemyID = 6,
                                NamePrefix = "蜂窝被掏了的",
                                Allies = new []{6},
                                Skills = new List<int[]>()
                                {
                                    new []{13,1},
                                },
                                SpawnPerAlly = 50,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"ATK", 1.5f},
                                    {"MaxHP", 1.5f},
                                }
                            }
                        }
                    },
                    new RoundInfo()
                    {
                        Level = 19,
                        EnemyIDs = new int[]{0,1,2,3,4,5,6},
                        Duration = 120,
                        SpawnCountPerEnemy = 10
                    },
                    new RoundInfo()
                    {
                        Level = 20,
                        Duration = -1,
                        BossInfos = new List<BossInfo>()
                        {
                            new BossInfo()
                            {
                                Level = 22,
                                EnemyID = 0,
                                NamePrefix = "复仇的",
                                Allies = new []{0},
                                Skills = new List<int[]>()
                                {
                                    new []{12,1},
                                },
                                SpawnPerAlly = 5,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"DEF", 1.5f},
                                }
                            },
                            new BossInfo()
                            {
                                Level = 22,
                                EnemyID = 1,
                                NamePrefix = "复仇的",
                                Allies = new []{1},
                                Skills = new List<int[]>()
                                {
                                    new []{11,1},
                                },
                                SpawnPerAlly = 5,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"ATK", 1.5f},
                                }
                            },
                            new BossInfo()
                            {
                                Level = 22,
                                EnemyID = 2,
                                NamePrefix = "复仇的",
                                Allies = new []{2},
                                Skills = new List<int[]>()
                                {
                                    new []{14,1},
                                },
                                SpawnPerAlly = 5,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"MaxHP", 1.5f},
                                    {"DEF",1.5f}
                                    
                                }
                            },
                            new BossInfo()
                            {
                                Level = 22,
                                EnemyID = 6,
                                NamePrefix = "复仇的",
                                Allies = new []{1},
                                Skills = new List<int[]>()
                                {
                                    new []{13,1},
                                },
                                SpawnPerAlly = 5,
                                AttrRate = new Dictionary<string, float>()
                                {
                                    {"ATK", 1.5f},
                                    {"MaxHP",1.5f}
                                }
                            },
                        }
                    },
                }
            },
            new LevelInfo()
            {
                Name = "地下灵堂(装修中)",
                CoverIndex = 1,
                Audio = 213,
                MapWidth = 100,
                MapHeight = 100,
                PaletIndex = 2,
                Difficulty = 1,
                GroundSerials = new List<uint> { 7820,7821,7822,7823,7824,7825,7826,7827,7828,7829,7830,7831,7832,7833,7834,7834 },
                DecorationsSerials = new List<uint>
                {
                    11055,11056, 11057, 11058,11100,11101,12136,12137,12138,12139,12140,12141,12142,12143,12144,12145,
                    12146,12147,12148,12149,12150,12151,12152,12153,12154,12155,
                    15177,15178,15179,15180,15181,15182,15183,15184,15185,15186,15187,15188,15189,15190,15191,15192,
                    15193,15194,15195,15196,15197,15198,15199,15200,15201,15202,15203,15204,15205,15206,15207,15208,
                },
                UnitSerials = new List<uint>
                {
                    11017,11018,11035,11036,11061,11062,11566,11567,11568,11569,11570,11571,11572,11573,11574,14522,14523,14524,14525,16197,16198,16199,16200,17680
                },
                BorderSerial = 17680,
                RoundInfos = new List<RoundInfo>()
                {
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{7},
                        Duration = 60,
                        SpawnCountPerEnemy = 5
                    },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{8},
                        Duration = 60,
                        SpawnCountPerEnemy = 5
                    },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{9},
                        Duration = 60,
                        SpawnCountPerEnemy = 5
                    },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{10},
                        Duration = 60,
                        SpawnCountPerEnemy = 5
                    },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{11},
                        Duration = 60,
                        SpawnCountPerEnemy = 5
                    },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{12},
                        Duration = 60,
                        SpawnCountPerEnemy = 5
                    },
                    new RoundInfo()
                    {
                        Level = 1,
                        EnemyIDs = new int[]{13},
                        Duration = -1,
                        SpawnCountPerEnemy = 5
                    },
                }
            }
        };

    }
}