using System;
using System.Collections.Generic;
using Controller;
using Game.UI;
using Prefeb;
using Reference;
using UnityEngine;

namespace GameData
{
    public class SkillSetting
    {
        // 技能接口
        public interface ISkill
        {
            // 技能初始化 需主动调用 InitBase(PlayerUnit playerUnit, SkillInfo skillInfo) 以正确初始化
            public void Init(PlayerUnit playerUnit, SkillInfo skillInfo);
            // 技能释放处理 基类调用Cast()方法后会自动调用
            public void CastSkill();
            // 技能升级处理
            public void LevelUp();
            // 技能CD完成处理
            public void OnCDComplate();
            // 获取技能对应等级参数数值
            public Dictionary<string,object> GetLevelParams(int level);
        }

        // 技能基类
        public abstract class SkillBase:MonoBehaviour,ISkill
        {
            // 绑定单位
            [NonSerialized] public PlayerUnit PlayerUnit;
            // 关联技能数据
            [NonSerialized] public SkillInfo SkillInfo;
            // 绑定技能对象
            [NonSerialized] public SkillItem SkillItem;
            // 技能CD列表
            [NonSerialized] public List<CircleCD> CircleCDs = new List<CircleCD>();
            // 技能主要CD
            [NonSerialized] public CircleCD MainCircleCD;
            // 技能CD
            [NonSerialized] public float CD;
            // 技能消耗
            [NonSerialized] public int MPCost;

            // 技能是否可用
            [NonSerialized] public bool IsAvailable = true;
            // 技能是否是默认攻击技能
            [NonSerialized] public bool IsAtkSkill;
            // 技能是否是得意技技能
            [NonSerialized] public bool IsSpecialSkill;
            // 技能是否自动释放
            [NonSerialized] public bool IsAudoSkill;

            // 技能等级
            private int _level;
            public int Level
            {
                get => _level;
                set
                {
                    _level = value;
                    CD = SkillInfo.CD - Level * SkillInfo.CD * SkillInfo.CDReduceRatePerLevel;
                    MPCost = (int) (SkillInfo.MPCost + Level * SkillInfo.MPCost * SkillInfo.MPCostPerLevel);
                    if (SkillItem != null)
                    {
                        SkillItem.Level = value;
                    }
                }
            }
            
            // 技能参数 可被解析至技能说明信息
            public Dictionary<string, object> Params;

            // 技能初始化
            public void InitBase(PlayerUnit playerUnit, SkillInfo skillInfo)
            {
                PlayerUnit = playerUnit;
                SkillInfo = skillInfo;
                Params = new Dictionary<string, object>(SkillInfo.Params);
                Level = 0;
                MPCost = skillInfo.MPCost;
                CD = skillInfo.CD;
                IsAvailable = true;
                IsAudoSkill = skillInfo.IsAutoSkill;

                // 初始化技能CD处理
                CircleCD circleCd = Instantiate(GlobalReference.Prefeb.CircleCD_Prefeb, transform);
                circleCd.OnCDComplete += OnCDComplate;
                circleCd.OnCDComplete += () =>
                {
                    IsAvailable = true;
                    if (IsAudoSkill) Cast();
                };
                CircleCDs.Add(circleCd);
                // 作为主CD控制器,当前技能所有的CD处理以MainCD为准
                MainCircleCD = circleCd;
            }

            // 技能释放
            public void Cast()
            {
                if(CD==0) return;
                if (IsAvailable == false)
                {
                    GameController.AddFloatingText(PlayerUnit.transform.position, "技能冷却中", Color.white);
                    return;
                }
                if (MPCost > 0)
                {
                    if (MPCost > PlayerUnit.Attr.MP)
                    {
                        GameController.AddFloatingText(PlayerUnit.transform.position, "魔法不足", Color.white);
                        return;
                    }
                    PlayerUnit.Attr.MP -= MPCost;
                }
                // 开始CD
                StartCD();
                // 执行子类技能释放处理
                CastSkill();
            }

            // 重置技能
            public void ResetSkill()
            {
                Level = 0;
                Params = new Dictionary<string, object>(SkillInfo.Params);
            }

            // 开始CD
            public void StartCD()
            {
                if(CD==0) return;
                if(!IsAvailable) return;
                IsAvailable = false;
                
                //对所有已绑定CD控制器进行CD处理
                for (var i = 0; i < CircleCDs.Count; i++)
                {
                    CircleCDs[i].Stop();
                    CircleCDs[i].StartCD(CD);
                }
            }

            // 获取技能对应等级参数数值
            public string GetLevelDesc(int level)
            {
                // 从子类获取技能等级参数
                Dictionary<string,object> levelParams = GetLevelParams(level);
                // 格式化
                string desc = SkillInfo.AbilityDescTpl;
                foreach (var param in levelParams)
                {
                    desc = desc.Replace($"{{{param.Key}}}", "<color=#FFFA28>" + param.Value + "</color>");
                }

                return desc;
            }
            
            // 获取技能效果说明
            public string AbilityDesc
            {
                get
                {
                    var desc = SkillInfo.AbilityDescTpl;
                    foreach (var param in Params)
                    {
                        desc = desc.Replace($"{{{param.Key}}}", "<color=#FFFA28>" + param.Value + "</color>");
                    }

                    return desc;
                }
            }
            
            // 获取技能下一级效果说明
            public string AbilityLevelUpDesc
            {
                get
                {
                    var desc = SkillInfo.AbilityLevelUpDescTpl;
                    Dictionary<string,object> levelParams = GetLevelParams(Level+1);
                    foreach (var param in levelParams)
                    {
                        desc = desc.Replace($"{{{param.Key}}}", "<color=#FFFA28>" + param.Value + "</color>");
                    }

                    return desc;
                }
            }
            
            // 抽象初始化方法 - 子类需实现
            public abstract void Init(PlayerUnit playerUnit, SkillInfo skillInfo);
            // 抽象技能释放方法 - 子类需实现
            public abstract void CastSkill();
            // 抽象技能CD完成方法 - 子类需实现
            public abstract void OnCDComplate();
            // 抽象技能升级方法 - 子类需实现
            public abstract void LevelUp();
            // 抽象获取技能等级参数方法 - 子类需实现
            public abstract Dictionary<string,object> GetLevelParams(int level);
            
        }

        public class SkillInfo
        {
            // 技能ID
            public int ID;
            // 技能名称
            public string Name;
            // 技能简介
            public string Desc;
            public int Icon;
            // 技能是否为得意技,得意技不会自动释放,且不会被其他角色升级时选中
            public bool IsSpecial;
            // 是否可学
            public bool IsLearnable = true;
            // 是否自动释放
            public bool IsAutoSkill;
            // 默认技能冷却CD
            public float CD = 10f;
            // 技能自动释放CD每级降低百分比
            public float CDReduceRatePerLevel = 0.02f;
            // 技能自定义参数
            public Dictionary<string, object> Params;
            // 技能能力说明模板
            public string AbilityDescTpl;
            // 技能升级提升说明模板
            public string AbilityLevelUpDescTpl;
            // 技能最大等级
            public int MaxLevel;
            // 所属技能ID,在未获得该技能时,该技能不会在升级时被选中
            public int BelongSkillID;
            // 技能MP消耗基础值
            public int MPCost;
            // 技能MP消耗每级增加百分比
            public float MPCostPerLevel = 0.2f;

            // 技能能力说明生成
            public string AbilityDesc
            {
                get
                {
                    var desc = AbilityDescTpl;
                    foreach (var param in Params)
                    {
                        desc = desc.Replace($"{{{param.Key}}}", "<color=#FFFA28>" + param.Value + "</color>");
                    }

                    return desc;
                }
            }
            // 技能升级提升说明生成
            public string AbilityLevelUpDesc
            {
                get
                {
                    var desc = AbilityLevelUpDescTpl;
                    foreach (var param in Params)
                    {
                        desc = desc.Replace($"{{{param.Key}}}", "<color=#FFFA28>" + param.Value + "</color>");
                    }

                    return desc;
                }
            }
            
        }

        // 获取对应ID技能信息
        public static SkillInfo GetSkillInfo(int id)
        {
            return SkillInfos.Find(skillInfo => skillInfo.ID == id);
        }
        
        // 技能库
        public static List<SkillInfo> SkillInfos = new List<SkillInfo>()
        {
            new SkillInfo()
            {
                ID = 0,
                Name = "挥砍",
                Desc = "剑指前方,所向披靡",
                Icon = 10139,
                CD = 2f,
                CDReduceRatePerLevel = 0.03f,
                MaxLevel = 10,
                IsAutoSkill = true,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 100,
                    ["PercentRatePerLevel"] = 5,
                    ["Distance"] = 10,
                    ["DistancePerLevel"] = 1,
                    ["Range"] = 90f,
                    ["RangePerLevel"] = 5f,
                    ["Count"] = 1,
                    ["CountPer3Level"] = 1,
                },
                AbilityDescTpl = "向面前距离{Distance}和{Range}度范围挥砍{Count}次，对范围内的敌人造成{Percent}%伤害。",
                AbilityLevelUpDescTpl ="每级增加{PercentRatePerLevel}%伤害，{DistancePerLevel}攻击距离，{RangePerLevel}攻击范围。" +
                                       "\n每3级增加{CountPer3Level}次挥砍" 
            },
            new SkillInfo()
            {
                ID = 1,
                Name = "弓术射击",
                Desc = "使用弓箭向远处目标射击",
                Icon = 10100,
                CD = 2f,
                CDReduceRatePerLevel = 0.03f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 100,
                    ["PercentRatePerLevel"] = 5,
                    ["Range"] = 50,
                    ["RangePerLevel"] = 5,
                    ["Speed"] = 50,
                    ["SpeedPerLevel"] = 5,
                    ["Count"] = 1,
                    ["Through"] = 0
                },
                AbilityDescTpl = "向目标方向以{Speed}单位速度发射一支箭矢，对击中的敌人造成{Percent}%伤害，最大距离{Range}。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%伤害，{RangePerLevel}射程，{SpeedPerLevel}速度。"
            },
            new SkillInfo()
            {
                ID = 2,
                Name = "乱射",
                Desc = "向远处射出大量的箭矢,让敌人沐浴在箭雨中",
                Icon = 10011,
                IsSpecial = true,
                CD = 10f,
                CDReduceRatePerLevel = 0.05f,
                MaxLevel = 10,
                MPCost = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 80,
                    ["PercentRatePerLevel"] = 5,
                    ["Count"] = 1,
                    ["CountPer3Level"] = 1,
                    ["Range"] = 50,
                    ["RangePerLevel"] = 5,
                    ["Speed"] = 50,
                    ["SpeedPerLevel"] = 5,
                    ["Rate"] = 1f
                },
                AbilityDescTpl = "每{Rate}秒向目标位置发射大量箭矢造成{Range}单位范围的{Percent}%伤害，最多发射{Count}次。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%伤害，{RangePerLevel}范围。" +
                                        "\n每3级增加{CountPer3Level}次发射次数。"
            },
            new SkillInfo()
            {
                ID = 3,
                Name = "健身达人",
                Desc = "力量!我渴望力量!",
                Icon = 10028,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 5,
                    ["PercentRatePerLevel"] = 1,
                },
                AbilityDescTpl = "基础伤害增加{Percent}%。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%基础伤害。"
            },
            new SkillInfo()
            {
                ID = 4,
                Name = "长跑爱好者",
                Desc = "你猜我跑得多快?",
                Icon = 10111,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 5,
                    ["PercentRatePerLevel"] = 1,
                },
                AbilityDescTpl = "增加{Percent}%移动速度。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%移动速度。"
            },
            new SkillInfo()
            {
                ID = 5,
                Name = "护卫",
                Desc = "我这脸犹如城墙",
                Icon = 10007,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 5,
                    ["PercentRatePerLevel"] = 1,
                },
                AbilityDescTpl = "增加{Percent}%基础防御。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%基础防御。"
            },
            new SkillInfo()
            {
                ID = 6,
                Name = "好学生",
                Desc = "经验!宝贵的经验!",
                Icon = 90065,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 0,
                    ["PercentRatePerLevel"] = 10,
                },
                AbilityDescTpl = "获取到的经验增加{Percent}%。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%。"
            },
            new SkillInfo()
            {
                ID = 7,
                Name = "财迷宝宝",
                Desc = "闪开!地上那些魔石都是我的!",
                Icon = 30075,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 0,
                    ["PercentRatePerLevel"] = 10,
                },
                AbilityDescTpl = "获取到的金币增加{Percent}%。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%。"
            },
            new SkillInfo()
            {
                ID = 8,
                Name = "冲啊",
                Desc = "看见前面没!冲!",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    
                },
                AbilityDescTpl = "怪物专用技能,短时间加快移动速度",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 9,
                Name = "我闪!",
                Desc = "闪避点满",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    
                },
                AbilityDescTpl = "怪物专用技能,Dash!!!!",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 10,
                Name = "我挡!",
                Desc = "嘿嘿~~防御我最棒",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    
                },
                AbilityDescTpl = "怪物专用技能,防御!!!",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 11,
                Name = "力量光环",
                Desc = "奋起吧!我的兄弟们!",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Light"] = 0,
                    ["Range"] = 1f,
                    ["RangePerLevel"] = 0.1f,
                },
                AbilityDescTpl = "为周边怪物增加20%攻击",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 12,
                Name = "防御光环",
                Desc = "奋起吧!我的兄弟们!",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Light"] = 1,
                    ["Range"] = 1f,
                    ["RangePerLevel"] = 0.1f,
                },
                AbilityDescTpl = "为周边怪物增加20%防御",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 13,
                Name = "速度光环",
                Desc = "奋起吧!我的兄弟们!",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Light"] = 2,
                    ["Range"] = 1f,
                    ["RangePerLevel"] = 0.1f,
                },
                AbilityDescTpl = "为周边怪物增加20%速度",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 14,
                Name = "生命光环",
                Desc = "奋起吧!我的兄弟们!",
                Icon = 30075,
                IsLearnable = false,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Light"] = 3,
                    ["Range"] = 1f,
                    ["RangePerLevel"] = 0.1f,
                },
                AbilityDescTpl = "为周边怪物增加20%最大生命",
                AbilityLevelUpDescTpl = "怪物专用技能"
            },
            new SkillInfo()
            {
                ID = 15,
                Name = "弧月斩",
                Desc = "一剑斩月，弧月斩！",
                Icon = 10078,
                IsSpecial = true,
                CD = 10f,
                CDReduceRatePerLevel = 0.03f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 80,
                    ["PercentRatePerLevel"] = 5,
                    ["Range"] = 50,
                    ["RangePerLevel"] = 5,
                    ["Speed"] = 50,
                    ["SpeedPerLevel"] = 5,
                    ["Count"] = 1,
                },
                AbilityDescTpl = "向目标方向以{Speed}速度挥舞出一道剑气，对{Range}范围内击中的敌人造成{Percent}%伤害。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%伤害，{RangePerLevel}范围，{SpeedPerLevel}速度。"
            },
            new SkillInfo()
            {
                ID = 16,
                Name = "武力全开",
                Desc = "暴击!给我狠狠的暴击!",
                Icon = 90035,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 10,
                    ["PercentRatePerLevel"] = 5,
                },
                AbilityDescTpl = "攻击造成伤害时,有{Percent}%造成暴击,暴击产生2倍伤害。",
                AbilityLevelUpDescTpl = "每级增加{PercentRatePerLevel}%。"
            },
            new SkillInfo()
            {
                ID = 17,
                Name = "飞毛腿",
                Desc = "你看我跑多快~",
                Icon = 30040,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Percent"] = 10,
                    ["PercentRatePerLevel"] = 1,
                    ["Count"] = 1,
                    ["CountLevel"] = 5
                },
                AbilityDescTpl = "降低躲避CD的{Percent}%时长，并增加闪避点数。",
                AbilityLevelUpDescTpl = "每级降低{PercentRatePerLevel}%，每{CountLevel}级增加{Count}闪避点数。"
            },
            new SkillInfo()
            {
                ID = 18,
                Name = "缓速光环",
                Desc = "在我的地盘这~你就得听我的",
                Icon = 90036,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Range"] = 50,
                    ["RangePerLevel"] = 5,
                    ["Effect"] = 10,
                    ["EffectPerLevel"] = 5
                },
                AbilityDescTpl = "对进入光环{Range}范围的敌人造成{Effect}%减速。",
                AbilityLevelUpDescTpl = "每级增大{RangePerLevel}范围，提高{EffectPerLevel}%减速效果。"
            },
            new SkillInfo()
            {
                ID = 19,
                Name = "灼热光环",
                Desc = "在火热地狱中哀嚎吧",
                Icon = 90035,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 10,
                Params = new Dictionary<string, object>()
                {
                    ["Range"] = 50,
                    ["RangePerLevel"] = 5,
                    ["Effect"] = 10,
                    ["EffectPerLevel"] = 1
                },
                AbilityDescTpl = "对进入光环{Range}范围的敌人每秒造成{Effect}%攻击力的伤害。",
                AbilityLevelUpDescTpl = "每级增大{RangePerLevel}范围，提高{EffectPerLevel}%伤害。"
            },
            new SkillInfo()
            {
                ID = 20,
                Name = "拿来吧你",
                Desc = "我的!都是我的!",
                Icon = 30076,
                CD = 0f,
                CDReduceRatePerLevel = 0f,
                MaxLevel = 20,
                Params = new Dictionary<string, object>()
                {
                    ["Range"] = 0,
                    ["RangePerLevel"] = 10
                },
                AbilityDescTpl = "拾取范围扩大{Range}%。",
                AbilityLevelUpDescTpl = "每级扩大{RangePerLevel}%范围。"
            },
        };
    }
}