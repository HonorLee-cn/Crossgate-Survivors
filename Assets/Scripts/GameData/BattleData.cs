using System.Collections.Generic;
using Prefeb;
using Reference;
using Util;

namespace GameData
{
    // 每进入关卡时生成的战斗数据
    public class BattleData
    {
        // 关卡信息
        public LevelSetting.LevelInfo LevelInfo;
        // 玩家信息
        public PlayerSetting.PlayerInfo PlayerInfo;
        // 回合队列
        public Queue<LevelSetting.RoundInfo> BattleRounds;
        // 当前回合信息
        public LevelSetting.RoundInfo CurrentRound;
        // 怪物种类击杀数量(当前未使用)
        public Dictionary<int,int> EnemyKillCount;
        // 所有怪物
        public List<PlayerUnit> EnemyUnits = new List<PlayerUnit>();
        // 干扰怪物ID序列
        public List<int> DisturbEnemyIDs = new List<int>();
        
        // 场上掉落物
        public List<LootItem> DropItems = new List<LootItem>();
        
        // 掉落物经验拾取倍率
        public int Loot_ExpRate = 100;
        // 掉落物金币拾取倍率
        public int Loot_CoinRate = 100;
        // 是否开启自动拾取
        public bool AutoLoot;
        // 暴击倍率
        public float CriticalRate = 10f;
        // Miss倍率(尚未使用)
        public float MissRate = 10f;

        // 当前交互对象
        public InteractiveUnit InteractiveUnit = null;
        
        // 装备信息
        public Dictionary<ItemSetting.Type,ItemSetting.Item> EquipItems = new Dictionary<ItemSetting.Type, ItemSetting.Item>()
        {
            [ItemSetting.Type.Hand] = null,
            [ItemSetting.Type.Armor] = null,
            [ItemSetting.Type.Jewelry] = null,
            // [ItemSetting.Type.Necklace] = null,
            // [ItemSetting.Type.Ring] = null,
            [ItemSetting.Type.Shoes] = null,
            // [ItemSetting.Type.Hat] = null,
            // [ItemSetting.Type.Shield] = null,
        };
        // 装备改变事件
        public delegate void EquipItemChange();
        public event EquipItemChange OnEquipItemChange;

        // 装备改变事件 - 装备购买时自动调用触发回调
        public void EquipChanged()
        {
            OnEquipItemChange?.Invoke();
        }
        
        // 学习点数
        private int _learnPoint = 0;
        public int LearnPoint
        {
            get => _learnPoint;
            set
            {
                _learnPoint = value;
                if (!GlobalReference.UI.BattleUI.LearnSkill.gameObject.activeSelf)
                {
                    GlobalReference.UI.BattleUI.LearnSkill.gameObject.SetActive(true);    
                }
            }
        }

        // 场上剩余敌人数量
        private int _lastEnemyCount = 0;
        public int LastEnemyCount
        {
            get => _lastEnemyCount;
            set
            {
                _lastEnemyCount = value;
                GlobalReference.UI.BattleUI.CurrentEnemy.text = _lastEnemyCount.ToString();
            }
        }
        
        // 玩家信息
        public PlayerUnit PlayerUnit;
        // 玩家金钱
        private int _money;
        public int Money
        {
            get => _money;
            set
            {
                _money = value;
                GlobalReference.UI.BattleUI.CurrentCoin.text = Toolkit.FormatNumberWithCommas(_money);
            }
        }

        // 累计金钱
        private int _totalMoney;
        public int TotalMoney
        {
            get => _totalMoney;
            set
            {
                _totalMoney = value;
                GlobalReference.UI.BattleUI.PlayerDatas.COIN.text = Toolkit.FormatNumberWithCommas(_totalMoney);
            }
        }
        
        // 累计杀敌数
        private int _totalKillCount;
        public int TotalKillCount
        {
            get => _totalKillCount;
            set
            {
                _totalKillCount = value;
                GlobalReference.UI.BattleUI.PlayerDatas.KILL.text = Toolkit.FormatNumberWithCommas(_totalKillCount);
            }
        }
        
        // public int TotalExp;
        // public int TotalDamage;
        
        // 红瓶数量
        private int _redBottleCount = 0;
        public int RedBottleCount
        {
            get => _redBottleCount;
            set
            {
                _redBottleCount = value;
                GlobalReference.UI.BattleUI.HpBottle.Count = _redBottleCount;
            }
        }
        
        // 蓝瓶数量
        private int _blueBottleCount = 0;
        public int BlueBottleCount
        {
            get => _blueBottleCount;
            set
            {
                _blueBottleCount = value;
                GlobalReference.UI.BattleUI.MpBottle.Count = _blueBottleCount;
            }
        }
    }
}