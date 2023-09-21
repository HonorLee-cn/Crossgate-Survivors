using System.Collections.Generic;
using CGTool;
using Prefeb;
using Reference;
using UnityEngine;

namespace GameData
{
    // 装备道具设定
    public class ItemSetting
    {
        // 装备类型
        public enum Type
        {
            Bottle,
            Hand,
            Armor,
            Jewelry,
            // Necklace,
            // Ring,
            Shoes,
            // Hat,
            // Shield
        }

        // 装备类型对应名称
        public static Dictionary<Type, string> TypeName = new Dictionary<Type, string>()
        {
            [Type.Bottle] = "药水",
            [Type.Hand] = "主手",
            [Type.Armor] = "护甲",
            [Type.Jewelry] = "饰品",
            // [Type.Necklace] = "项链",
            // [Type.Ring] = "戒指",
            [Type.Shoes] = "鞋子",
            // [Type.Hat] = "帽子",
            // [Type.Shield] = "副手",
        };

        // 附加值
        public class Addon
        {
            public AddonType Type;
            public int Min;
            public int Max;
        }
        
        // 附加值类型
        public enum AddonType
        {
            MaxHP_Rate,
            MaxMP_Rate,
            ATK,
            ATK_Rate,
            DEF,
            DEF_Rate,
            SPD,
            SPD_Rate,
            Loot_Exp_Rate,
            Loot_Coin_Rate,
            Critical_Rate,
        }

        // 附加值类型对应基础值
        public static Dictionary<AddonType, int> AddonBase = new Dictionary<AddonType, int>()
        {
            [AddonType.MaxHP_Rate] = 2,
            [AddonType.MaxMP_Rate] = 2,
            [AddonType.ATK] = 10,
            [AddonType.ATK_Rate] = 8,
            [AddonType.DEF] = 8,
            [AddonType.DEF_Rate] = 5,
            [AddonType.SPD] = 5,
            [AddonType.SPD_Rate] = 2,
            [AddonType.Loot_Exp_Rate] = 10,
            [AddonType.Loot_Coin_Rate] = 10,
            [AddonType.Critical_Rate] = 1,
        };

        // 附加值类型对应名称
        public static Dictionary<AddonType, string> AddonName = new Dictionary<AddonType, string>()
        {
            [AddonType.MaxHP_Rate] = "最大生命值",
            [AddonType.MaxMP_Rate] = "最大魔法值",
            [AddonType.ATK] = "攻击力",
            [AddonType.ATK_Rate] = "攻击力",
            [AddonType.DEF] = "防御力",
            [AddonType.DEF_Rate] = "防御力",
            [AddonType.SPD] = "速度",
            [AddonType.SPD_Rate] = "速度",
            [AddonType.Loot_Exp_Rate] = "获取经验",
            [AddonType.Loot_Coin_Rate] = "获取金币",
            [AddonType.Critical_Rate] = "暴击率",
        };
        
        // 物品信息
        public class ItemInfo
        {
            // 图档编号
            public uint Serial;
            // 等级
            public int Level;
            // 名称
            public string Name;
            // 描述
            public string Desc;
            // 功能描述模板
            public string AbilityDescTpl;
            // 类型
            public Type Type;
            // 默认附加属性
            public List<AddonType> DefaultAddon;
        }

        // 物品类
        public class Item:ItemInfo
        {
            // 物品的附加属性
            public Dictionary<AddonType, int> Addon;
            // 物品的价格
            public int Price;
            // 物品功能描述
            public string AbilityDesc;
            // 物品颜色(白绿蓝紫黄)
            public Color Color = Color.white;
            // 绑定的玩家单位
            private PlayerUnit _playerUnit;
            // 物品的图片
            public Sprite Sprite;
            
            // 初始化
            public Item(ItemInfo itemInfo)
            {
                Serial = itemInfo.Serial;
                GraphicInfoData graphicInfoData = GraphicInfo.GetGraphicInfoDataBySerial((uint) Serial);
                GraphicData graphicData =
                    CGTool.Graphic.GetGraphicData(graphicInfoData);
                Sprite = graphicData.Sprite;
                Level = itemInfo.Level;
                Name = itemInfo.Name;
                Desc = itemInfo.Desc;
                AbilityDescTpl = itemInfo.AbilityDescTpl;
                AbilityDesc = AbilityDescTpl;
                Type = itemInfo.Type;
                _playerUnit = GlobalReference.BattleData.PlayerUnit;
                
                // 生成附加属性
                if (Type == Type.Bottle)
                {
                    // 药水统一价格100
                    Price = 100;
                }
                else
                {
                    // 生成随机属性
                    Addon = new Dictionary<AddonType, int>();
                    // 默认1-2属性
                    int MaxAddon = Random.Range(1, 2);
                    // 20%概率3属性
                    if (Random.Range(1, 5) == 1)
                    {
                        MaxAddon = 3;
                        Color = Color.green;
                    }
                    // 5%概率4属性
                    if (Random.Range(1, 20) == 1)
                    {
                        MaxAddon = 4;
                        Color = new Color(0, 100, 255, 1);
                    }
                    // 2%概率5属性
                    if (Random.Range(1, 50) == 1)
                    {
                        MaxAddon = 5;
                        Color = Color.yellow;
                    }
                    // 1%概率6属性
                    if (Random.Range(1, 100) == 1)
                    {
                        MaxAddon = 6;
                        Color = Color.magenta;
                    }
                    // 生成属性值
                    List<AddonType> addonTypes = new List<AddonType>();
                    List<AddonType> defaultTypes = new List<AddonType>()
                    {
                        AddonType.MaxHP_Rate,
                        AddonType.MaxMP_Rate,
                        AddonType.ATK,
                        AddonType.ATK_Rate,
                        AddonType.DEF,
                        AddonType.DEF_Rate,
                        AddonType.SPD,
                        AddonType.SPD_Rate,
                        AddonType.Loot_Exp_Rate,
                        AddonType.Loot_Coin_Rate,
                        AddonType.Critical_Rate,
                    };
                    // 生成属性列表
                    for (int i = 0; i < MaxAddon; i++)
                    {
                        AddonType addonType = defaultTypes[Random.Range(0, defaultTypes.Count)];
                        defaultTypes.Remove(addonType);
                        addonTypes.Add(addonType);
                    }
                    // 如果装备存在固定属性，则固定属性必定出现
                    if (itemInfo.DefaultAddon != null)
                    {
                        AddonType addonType = itemInfo.DefaultAddon[Random.Range(0, itemInfo.DefaultAddon.Count)];
                        addonTypes.Remove(addonType);
                        int value = (int) (Random.Range(AddonBase[addonType] * Level / 2f,
                            AddonBase[addonType] * Level * 2f));
                        Addon.Add(addonType, value);
                        MaxAddon--;
                    }
                    // 生成剩余属性
                    for(int i =0;i<MaxAddon;i++)
                    {
                        AddonType addonType = addonTypes[Random.Range(0, addonTypes.Count)];
                        addonTypes.Remove(addonType);
                        int value = (int) (Random.Range(AddonBase[addonType] * Level / 2f,
                            AddonBase[addonType] * Level * 2f));
                        Addon.Add(addonType, value);
                    }
                    
                    // 生成描述
                    if (AbilityDesc == "")
                    {
                        foreach (var keyValuePair in Addon)
                        {
                            string name = AddonName[keyValuePair.Key];
                            string value = keyValuePair.Value.ToString();
                            if(keyValuePair.Key==AddonType.MaxHP_Rate || keyValuePair.Key==AddonType.MaxMP_Rate || keyValuePair.Key.ToString().Contains("_Rate")) value += "%";
                            AbilityDesc += name + "  <color=#ffb000>+" + value + "</color>\n";
                        }
                    }

                    // 生成价格
                    Price = itemInfo.Level * 100 + MaxAddon * 50;
                }
                
            }
            
            // 装备物品
            public void Equip()
            {
                ChangeValue();
            }

            // 卸下物品
            public void Sell()
            {
                ChangeValue(true);
            }

            // 改变绑定对象属性值
            private void ChangeValue(bool reduce = false)
            {
                int x = reduce ? -1 : 1;
                foreach (var keyValuePair in Addon)
                {
                    AddonType type = keyValuePair.Key;
                    int value = keyValuePair.Value * x;
                    switch (type)
                    {
                        case AddonType.ATK:
                            _playerUnit.Attr.ATK_Addon += value;
                            break;
                        case AddonType.ATK_Rate:
                            _playerUnit.Attr.ATK_Addon_Rate += value;
                            break;
                        case AddonType.DEF:
                            _playerUnit.Attr.DEF_Addon += value;
                            break;
                        case AddonType.DEF_Rate:
                            _playerUnit.Attr.DEF_Addon_Rate += value;
                            break;
                        case AddonType.SPD:
                            _playerUnit.Attr.SPD_Addon += value;
                            break;
                        case AddonType.SPD_Rate:
                            _playerUnit.Attr.SPD_Addon_Rate += value;
                            break;
                        case AddonType.MaxHP_Rate:
                            _playerUnit.Attr.MaxHP_Addon_Rate += value;
                            break;
                        case AddonType.MaxMP_Rate:
                            _playerUnit.Attr.MaxMP_Addon_Rate += value;
                            break;
                        case AddonType.Loot_Coin_Rate:
                            GlobalReference.BattleData.Loot_CoinRate += value;
                            break;
                        case AddonType.Loot_Exp_Rate:
                            GlobalReference.BattleData.Loot_CoinRate += value;
                            break;
                        case AddonType.Critical_Rate:
                            GlobalReference.BattleData.CriticalRate += value;
                            break;
                    }
                }
            }
        }

        // 获取随机道具
        public static Item GetRandomItem(int Level)
        {
            int levelMin = Level - 1;
            int levelMax = Level + 1;
            if (levelMin < 1) levelMin = 1;
            if (levelMax > 10) levelMax = 10;
            List<ItemInfo> itemInfos = ItemInfos.FindAll(itemInfo => itemInfo.Level >= levelMin && itemInfo.Level <= levelMax);
            return new Item(itemInfos[Random.Range(0, itemInfos.Count)]);
        }

        // 生成药水
        public static Item GetBottleItem(bool MPBottle = false)
        {
            ItemInfo itemInfo = ItemInfos[MPBottle ? 1 : 0];
            return new Item(itemInfo);
        }
        
        // 道具信息
        public static List<ItemInfo> ItemInfos = new List<ItemInfo>()
        {
            new ItemInfo()
            {
                Serial = 26225,
                Level = 0,
                Name = "血瓶",
                Desc = "干了这瓶红，能苟就苟着！",
                Type = Type.Bottle,
                AbilityDescTpl = "恢复<color=#ffb000>10%</color>最大生命值",
            },
            new ItemInfo()
            {
                Serial = 26228,
                Level = 0,
                Name = "蓝瓶",
                Desc = "干了这瓶蓝，多苟一会儿是一会儿",
                Type = Type.Bottle,
                AbilityDescTpl = "恢复<color=#ffb000>10%</color>最大魔法值",
            },
            // Weapon
            new ItemInfo()
            {
                Serial = 21000,
                Level = 1,
                Name = "破损的匕首",
                Desc = "一把破损的匕首，看起来很不值钱",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 21248,
                Level = 2,
                Name = "勒手的刺环",
                Desc = "这玩意怎么看都看起来会伤到自己",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 26089,
                Level = 3,
                Name = "破木头",
                Desc = "路上捡的~你看~很直吧!",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 26018,
                Level = 4,
                Name = "吉他",
                Desc = "摇滚~滚~~滚~~~",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 26545,
                Level = 5,
                Name = "纸拍",
                Desc = "这东西打人不疼吧",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 26668,
                Level = 6,
                Name = "香肠",
                Desc = "再动!再动我甩你一脸!",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 14192,
                Level = 7,
                Name = "话筒",
                Desc = "我要唱歌！我就是闪亮的MVP！",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 27220,
                Level = 8,
                Name = "长虫",
                Desc = "啊呸~还没伤到敌就先把自己咬了",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 26303,
                Level = 9,
                Name = "王霸之剑",
                Desc = "你说..这玩意为啥叫王霸的剑？！",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            new ItemInfo()
            {
                Serial = 26300,
                Level = 10,
                Name = "水龙之剑",
                Desc = "这玩意才叫剑嘛！",
                Type = Type.Hand,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.ATK,AddonType.ATK_Rate}
            },
            // Armor
            new ItemInfo()
            {
                Serial = 26391,
                Level = 1,
                Name = "树叶",
                Desc = "这玩意能挡住什么？",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 26506,
                Level = 2,
                Name = "兽皮",
                Desc = "原始人的衣服，你懂的",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 26342,
                Level = 3,
                Name = "内裤？",
                Desc = "这玩意是谁的内裤？！",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 26341,
                Level = 4,
                Name = "披风",
                Desc = "套脖子漏胸,套腰上漏底..",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 26510,
                Level = 5,
                Name = "海盗头戴",
                Desc = "这是从哪个抢到头上扒下来的吧",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 21659,
                Level = 6,
                Name = "小背心",
                Desc = "这才是大老爷们该穿的衣服！",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 21624,
                Level = 7,
                Name = "木质胸甲",
                Desc = "家里木桶是不是买多了？",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 21503,
                Level = 8,
                Name = "绿帽子",
                Desc = "拿走!拿走!我看不得这玩意？",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 21700,
                Level = 9,
                Name = "袍子",
                Desc = "好歹~好歹是件衣服不是~？",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            new ItemInfo()
            {
                Serial = 26336,
                Level = 10,
                Name = "水龙盔甲",
                Desc = "这才是顶级防具！！！",
                Type = Type.Armor,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.DEF,AddonType.DEF_Rate}
            },
            // 饰品
            new ItemInfo()
            {
                Serial = 27019,
                Level = 1,
                Name = "大葱",
                Desc = "阿拉擦擦~阿比比拉比比~",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 26538,
                Level = 2,
                Name = "(棉)丝袜",
                Desc = "你说圣诞老人他会来么？",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 27062,
                Level = 3,
                Name = "过期牛奶",
                Desc = "牛奶常伴吾身!百毒不侵",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 26004,
                Level = 4,
                Name = "花鼓",
                Desc = "打起手鼓唱起歌~",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 25850,
                Level = 5,
                Name = "草编戒指",
                Desc = "你就是拿这个糊弄女孩子？",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 25422,
                Level = 6,
                Name = "钻石耳坠",
                Desc = "放大镜呢？我放大镜呢？",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 25673,
                Level = 7,
                Name = "狗牙项链",
                Desc = "我猜你这是刚去狗洞从狗嘴里拔的吧",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 25230,
                Level = 8,
                Name = "星星脖圈",
                Desc = "~你看~我美么~",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 25897,
                Level = 9,
                Name = "脖圈",
                Desc = "兄弟！带上这个一起健身吧",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            new ItemInfo()
            {
                Serial = 25663,
                Level = 10,
                Name = "水晶项链",
                Desc = "珠光宝气,也不知道是不是真的",
                Type = Type.Jewelry,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.MaxHP_Rate,AddonType.MaxMP_Rate}
            },
            //鞋子
            new ItemInfo()
            {
                Serial = 21843,
                Level = 1,
                Name = "破鞋",
                Desc = "这只是个破鞋，不是那个破鞋",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21811,
                Level = 2,
                Name = "草拖鞋",
                Desc = "这质量你是从刘皇叔那儿买的么",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21876,
                Level = 3,
                Name = "树丛鞋",
                Desc = "纯天然!绿色环保!",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21881,
                Level = 4,
                Name = "小飞鞋",
                Desc = "好看是挺好看,就是跑不起来",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21813,
                Level = 5,
                Name = "棉鞋",
                Desc = "暖和倒是挺暖和，但现在是夏天啊",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21818,
                Level = 6,
                Name = "特制鞋",
                Desc = "鞋子好是好..就是太挤脚了",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21805,
                Level = 7,
                Name = "古旧的铁靴",
                Desc = "这是从大英博物馆偷出来的吧",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21883,
                Level = 8,
                Name = "呱呱鞋",
                Desc = "走路呱呱叫与叽叽鞋有异曲同工之妙",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21867,
                Level = 9,
                Name = "骚红之鞋",
                Desc = "就你勾引大嫂啊连鞋子都穿这么骚",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
            new ItemInfo()
            {
                Serial = 21849,
                Level = 10,
                Name = "速跑鞋",
                Desc = "这脚合不合鞋,只有鞋知道",
                Type = Type.Shoes,
                AbilityDescTpl = "",
                DefaultAddon = new List<AddonType>(){AddonType.SPD,AddonType.SPD_Rate}
            },
        };
    }
}