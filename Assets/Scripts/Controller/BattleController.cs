using System;
using System.Collections.Generic;
using CGTool;
using DG.Tweening;
using Game.UI;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;
using Util;
using Random = UnityEngine.Random;

namespace Controller
{
    // 战斗回合控制器
    public static class BattleController
    {
        // 战斗数据引用
        public static BattleData BattleData;
        // 关卡数据引用
        public static LevelSetting.LevelInfo LevelInfo;
        
        // BOSS计数
        private static int _BossCount = 0;
        // BOSS血条组
        private static List<BOSSBar> _bossBars = new List<BOSSBar>();
        // BOSS盟友生成计数
        // private static int _BossAllySpawnCount = 0;
        
        // 延时器
        // 回合自动计时器
        private static Tween _autoRoundTweener;
        // 是否生成干扰敌人
        private static bool _spawnDisturbEnemy;
        // 生成干扰敌人计时器
        private static Tween _spawnDisturbEnemyTweener;
        // 现有NPC单位
        private static InteractiveUnit _NPCUnit;
        // 生成NPC计时器
        private static Tween _spawnNPCTweener;

        // 战场清理
        public static void Clear()
        {
            _BossCount = 0;
            foreach (BOSSBar bossBar in _bossBars)
            {
                GameObject.Destroy(bossBar.gameObject);
            }
            _bossBars.Clear();
            _spawnDisturbEnemy = false;
            _autoRoundTweener?.Kill();
            _spawnDisturbEnemyTweener?.Kill();
            _spawnNPCTweener?.Kill();
            foreach (Transform child in GlobalReference.UI.UnitContainer.transform)
            {
                PlayerUnit unit = child.GetComponent<PlayerUnit>();
                if (unit != null)
                {
                    unit.AnimePlayer.Stop();
                    GameObject.Destroy(unit.AnimePlayer);
                    unit.EffectPlayer.Stop();
                    GameObject.Destroy(unit.EffectPlayer);
                    foreach (SkillSetting.SkillBase unitPlayerSkill in unit.PlayerSkills)
                    {
                        GameObject.Destroy(unitPlayerSkill);
                    }
                }
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in GlobalReference.UI.MapUnitContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in GlobalReference.UI.BattleUI.SkillList.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in GlobalReference.UI.BattleUI.RoundPanel.BOSSContainer.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in GlobalReference.UI.BattleUI.BOSSPanel.Container.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
            foreach (Transform child in GlobalReference.UI.BattleUI.EquipList.transform)
            {
                GameObject.Destroy(child.gameObject);
            }
        }
        
        // 初始化战场
        public static void InitRounds()
        {
            // 引用战斗数据
            BattleData = GlobalReference.BattleData;
            // 引用关卡数据
            LevelInfo = BattleData.LevelInfo;
            
            // 初始化回合
            GlobalReference.UI.BattleUI.RoundPanel.CurrenRound.value = -1;
            // 最大回合数
            GlobalReference.UI.BattleUI.RoundPanel.CurrenRound.maxValue = LevelInfo.RoundInfos.Count - 1;
            // 开启干扰敌人生成
            _spawnDisturbEnemy = true;

            // 初始化回合队列
            BattleData.BattleRounds = new Queue<LevelSetting.RoundInfo>();
            
            // 初始化左侧回合导航
            int bossYHeight = 420 / LevelInfo.RoundInfos.Count;
            for (int i = 0; i < LevelInfo.RoundInfos.Count; i++)
            {
                LevelSetting.RoundInfo roundInfo = LevelInfo.RoundInfos[i];
                BattleData.BattleRounds.Enqueue(roundInfo);

                if (roundInfo.BossInfos != null && roundInfo.BossInfos.Count>0)
                {
                    GameObject bossBox = GameObject.Instantiate(GlobalReference.UI.BattleUI.RoundPanel.BOSSBox_Prefeb,
                        GlobalReference.UI.BattleUI.RoundPanel.BOSSContainer.transform);
                    bossBox.GetComponent<RectTransform>().anchoredPosition =
                        new Vector2(0, i * bossYHeight + bossYHeight);
                    for (var n = 0; n < roundInfo.BossInfos.Count; n++)
                    {
                        LevelSetting.BossInfo bossInfo = roundInfo.BossInfos[n];
                        RoundBOSS boss = GameObject.Instantiate(GlobalReference.UI.BattleUI.RoundPanel.BOSS_Prefeb,
                            bossBox.transform);
                        boss.Player.Serial = EnemySetting.EnemyInfos[bossInfo.EnemyID].Serial;
                        boss.Player.changeDirection(Anime.DirectionType.South);
                        boss.Player.Pause();
                    }
                }
            }

            // 监听装备变化 - 右下装备列表
            BattleData.OnEquipItemChange += () =>
            {
                // 清空装备列表
                foreach (Transform child in GlobalReference.UI.BattleUI.EquipList.transform)
                {
                    GameObject.Destroy(child.gameObject);
                }
                // 重新生成装备列表
                foreach (var battleDataEquipItem in BattleData.EquipItems)
                {
                    if(battleDataEquipItem.Value==null) continue;
                    ItemSetting.Item item = battleDataEquipItem.Value;
                    EquipItem equipItem = GameObject.Instantiate(GlobalReference.Prefeb.EquipItem_Prefeb,
                        GlobalReference.UI.BattleUI.EquipList.transform);
                    equipItem.SetItem(item);
                }
            };
            
            // 回合自动切换计时开始
            DOTween.Sequence().SetDelay(1f).onComplete = NextRound;
            
            // 如果关卡设定中存在NPC,则开始NPC生成计时器
            if(LevelInfo.NPCInfos!=null && LevelInfo.NPCInfos.Count>0)
            {
                _spawnNPCTweener = DOTween.Sequence();
                _spawnNPCTweener.SetDelay(60).onComplete = SpawnNPC;
            }
        }

        // 生成NPC
        public static void SpawnNPC()
        {
            // 如果已经存在NPC,则删除NPC
            if (_NPCUnit != null)
            {
                GameObject.Destroy(_NPCUnit.gameObject);
                _NPCUnit = null;
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, "奇怪的人一眨眼消失了",
                    Color.green,18);
            }
            else
            {
                // 生成随机NPC
                InteractiveSetting.InteractiveInfo interactiveInfo = LevelInfo.NPCInfos[Random.Range(0, LevelInfo.NPCInfos.Count)];
                _NPCUnit = GameObject.Instantiate(GlobalReference.Prefeb.InteractiveUnit,
                    GlobalReference.UI.UnitContainer.transform);
                // 随机生成位置
                int x = Random.Range(5, LevelInfo.MapWidth-5);
                int y = Random.Range(5, LevelInfo.MapHeight-5);
                Vector3 pos = Toolkit.TileLocationToPosition(new Vector2Int(x, y));
                _NPCUnit.transform.position = pos;
                _NPCUnit.SpriteSerial = interactiveInfo.Serial;
                _NPCUnit.Type = interactiveInfo.Type;
                // 生成提示
                GameController.AddFloatingText(GlobalReference.BattleData.PlayerUnit.transform.position, "战场上出现了奇怪的人",
                    Color.green,18);
            }

            // 重新生成计时器
            _spawnNPCTweener = DOTween.Sequence();
            _spawnNPCTweener.SetDelay(60).onComplete = SpawnNPC;
        }

        // 进入下一回合
        public static void NextRound()
        {
            // 如果没有回合了，就结束游戏(胜利)
            if (BattleData.BattleRounds.Count == 0)
            {
                if (BattleData.LastEnemyCount == 0) GameController.EndGame();
                return;
            }

            // 左侧导航前进
            GlobalReference.UI.BattleUI.RoundPanel.CurrenRound.DOValue(
                GlobalReference.UI.BattleUI.RoundPanel.CurrenRound.value + 1, 0.5f);
            
            // 获取下一回合数据
            BattleData.CurrentRound = BattleData.BattleRounds.Dequeue();
            // 如果有普通敌人，就生成普通敌人
            if (BattleData.CurrentRound.EnemyIDs != null)
            {
                for (var i = 0; i < BattleData.CurrentRound.EnemyIDs.Length; i++)
                {
                    SpawnEnemy(BattleData.CurrentRound.EnemyIDs[i], BattleData.CurrentRound.SpawnCountPerEnemy,
                        BattleData.CurrentRound.Level);
                }
            }
            // BattleData.LastEnemyCount += BattleData.CurrentRound.EnemyIDs.Length * BattleData.CurrentRound.SpawnCountPerEnemy;
            // 如果有Boss数据，就生成Boss
            if (BattleData.CurrentRound.BossInfos != null)
            {
                for (var i = 0; i < BattleData.CurrentRound.BossInfos.Count; i++)
                {
                    LevelSetting.BossInfo bossInfo = BattleData.CurrentRound.BossInfos[i];
                    SpawnEnemy(bossInfo.EnemyID, 1, bossInfo.Level, bossInfo);
                }
            }
            
            // 如果有回合时间限制，就生成下一回合计时器
            if (BattleData.CurrentRound.Duration > 0)
            {
                _autoRoundTweener = DOTween.Sequence();
                _autoRoundTweener.SetDelay(BattleData.CurrentRound.Duration).onComplete = NextRound;
            }

            // 如果当前回合有设定新加入额外干扰敌人，就加入待生成干扰敌人列表
            if (BattleData.CurrentRound.ExtraDisturbEnemyIDs != null &&
                BattleData.CurrentRound.ExtraDisturbEnemyIDs.Length > 0)
            {
                BattleData.DisturbEnemyIDs.AddRange(BattleData.CurrentRound.ExtraDisturbEnemyIDs);
            }

            // 如果当前回合有设定新加入额外干扰敌人，就生成干扰敌人计时器
            if (_spawnDisturbEnemyTweener == null && BattleData.CurrentRound.ExtraDisturbEnemyIDs.Length>0)
            {
                _spawnDisturbEnemyTweener = DOTween.Sequence();
                _spawnDisturbEnemyTweener.SetDelay(60f).onComplete = SpawnDisturbEnemy;    
            }
            
        }

        // 生成干扰敌人
        public static void SpawnDisturbEnemy()
        {
            //如果禁止生产干扰敌人
            if (!_spawnDisturbEnemy) return;
            
            //随机生成1-2个干扰敌人种类
            List<int> randomId = new List<int>();
            int disturbEnemyCount = UnityEngine.Random.Range(1, 3);
            for (int i = 0; i < disturbEnemyCount; i++)
            {
                int id = BattleData.DisturbEnemyIDs[UnityEngine.Random.Range(0, BattleData.DisturbEnemyIDs.Count)];
                randomId.Add(id);
            }

            // 生成随机位置 四角及中心位置
            List<Vector3Int> spawnLocs = new List<Vector3Int>()
            {
                new Vector3Int(2, 2),
                new Vector3Int(LevelInfo.MapWidth-2, 2),
                new Vector3Int(2, LevelInfo.MapHeight-2),
                new Vector3Int(LevelInfo.MapWidth-2, LevelInfo.MapHeight-2),
                new Vector3Int(LevelInfo.MapWidth/2, LevelInfo.MapHeight/2),
            };
            //根据种类数量在对应位置生成
            List<Vector3Int> spawnLoc = new List<Vector3Int>(disturbEnemyCount);
            for (int i = 0; i < disturbEnemyCount; i++)
            {
                int index = UnityEngine.Random.Range(0, spawnLocs.Count);
                spawnLoc.Add(spawnLocs[index]);
                spawnLocs.RemoveAt(index);
            }
            //在对应位置生成
            for (int i = 0; i < disturbEnemyCount; i++)
            {
                SpawnEnemy(randomId[i], 10, BattleData.CurrentRound.Level, null, spawnLoc[i]);
            }

            // 重新生成计时器
            _spawnDisturbEnemyTweener = DOTween.Sequence();
            _spawnDisturbEnemyTweener.SetDelay(60f).onComplete = SpawnDisturbEnemy;
        }

        // 当敌人死亡时处理
        public static void OnEnemyDead(PlayerUnit unit)
        {
            // 数据更新
            GlobalReference.BattleData.TotalKillCount++;
            GlobalReference.UI.BattleUI.PlayerDatas.KILL.text = GlobalReference.BattleData.TotalKillCount.ToString();
            GlobalReference.BattleData.EnemyUnits.Remove(unit);
            BattleData.LastEnemyCount--;
            
            // 如果是BOSS，就停止生成随身小怪
            if (unit.IsBoss)
            {
                _BossCount--;
                unit.BossInfo.StopSpawnAllies();
                
                //短时间开启全场自动拾取掉落物
                GlobalReference.BattleData.AutoLoot = true;
                DOTween.Sequence().SetDelay(1f).onComplete = () =>
                {
                    GlobalReference.BattleData.AutoLoot = false;
                };
                // 最后一回合BOSS死亡后不再生成干扰
                if (_BossCount == 0 && BattleData.BattleRounds.Count==0)
                {
                    _spawnDisturbEnemyTweener.Kill();
                    _spawnDisturbEnemy = false;
                }
            }
            
            // 如果当前回合敌人全部死亡，就清除回合计时器并直接进入下一回合
            if (BattleData.LastEnemyCount == 0)
            {
                if (_autoRoundTweener != null)
                {
                    _autoRoundTweener.Kill();
                    _autoRoundTweener = null;
                }
                DOTween.Sequence().SetDelay(3f).onComplete = () =>
                {
                    NextRound();    
                };
            }
        }

        // 生成敌人 怪物ID 生成数量 生成等级 包含BOSS信息 指定中心位置
        public static void SpawnEnemy(int EnemyID, int count,int level,LevelSetting.BossInfo bossInfo = null,Vector3Int midLoc = default)
        {
            for (int i = 0; i < count; i++)
            {
                // 增加怪物数量计数
                BattleData.LastEnemyCount++;
                // 生成怪物单位
                EnemySetting.EnemyInfo enemyInfo = EnemySetting.EnemyInfos[EnemyID];
                PlayerUnit unit = MonoBehaviour.Instantiate(GlobalReference.Prefeb.PlayerUnit,
                    GlobalReference.UI.UnitContainer.transform);
                BattleData.EnemyUnits.Add(unit);
                // 如果没有指定中心位置，就使用玩家位置
                Vector3Int loc = midLoc != default
                    ? midLoc
                    : Toolkit.GetTilePosition(BattleData.PlayerUnit.gameObject.transform.position);


                // 获取当前人物位置的周边超出屏幕位置,但不超过地图Tilemap尺寸范围(2 < x < levelinfo.MapWidth,2 < y < levelinfo.MapHeight)的随机位置
                Vector3Int randomLoc = Toolkit.GetRandomLoc(loc, (int) LevelInfo.MapWidth, (int) LevelInfo.MapHeight);
                // 将随机位置转换为世界坐标
                Vector3 randomWorldLoc = Toolkit.TileLocationToPosition((Vector2Int) randomLoc);
                // 初始化相关数据
                unit.gameObject.tag = "EnemyUnit";
                unit.gameObject.transform.position = randomWorldLoc;
                unit.ID = Guid.NewGuid();
                unit.IsEnemy = true;
                unit.AnimePlayer.PaletIndex = BattleData.LevelInfo.PaletIndex;
                if (bossInfo != null) unit.IsBoss = true;
                
                _BossCount += unit.IsBoss ? 1 : 0;
                unit.OnDead += OnEnemyDead;
                unit.PlayerInfo = enemyInfo;
                unit.Serial = enemyInfo.Serial;
                
                // 初始化怪物属性
                unit.InitAttr();
                
                // 初始化怪物技能
                enemyInfo.Skills.ForEach(skillID =>
                {
                    unit.LearnSkill(skillID[0], skillID[1]);
                });
                
                // 如果是BOSS，就初始化BOSS信息
                if (unit.IsBoss)
                {
                    unit.BossInfo = bossInfo;
                    
                    // BOSS血条生成
                    BOSSBar bossBar = MonoBehaviour.Instantiate(GlobalReference.UI.BattleUI.BOSSBar_Prefeb,
                        GlobalReference.UI.BattleUI.BOSSPanel.Container.transform);
                    bossBar.ID = unit.ID;
                    bossBar.PlayerUnit = unit;
                    bossBar.Name = bossInfo.NamePrefix + enemyInfo.Name;
                    
                    _bossBars.Add(bossBar);
                    unit.LevelUpTo(bossInfo.Level);
                    
                    // BOSS血量100倍
                    unit.Attr.MaxHP *= 100;
                    
                    // int x = bossInfo.Level / 10;
                    // if (x > 0) unit.Attr.MaxHP *= x * 10;
                    
                    unit.Attr.HP = unit.Attr.MaxHP;

                    // BOSS技能
                    if(bossInfo.Skills!=null)
                    {
                        bossInfo.Skills.ForEach(skillID =>
                        {
                            unit.LearnSkill(skillID[0], skillID[1]);
                        });
                    }
                    
                    // BOSS属性加成
                    if(bossInfo.AttrRate!=null)
                    {
                        foreach (var keyValuePair in bossInfo.AttrRate)
                        {
                            int val = (int) unit.Attr.GetPropertyValue(keyValuePair.Key);
                            unit.Attr.SetPropertyValue(keyValuePair.Key, (int) (val * keyValuePair.Value - val));
                        }
                    }
                    
                    // 开启BOSS召唤小怪
                    bossInfo.StartSpawnAllies(BattleData.CurrentRound.Level);
                }
                else
                {
                    // 升级普通怪物等级至当前地图等级
                    unit.LevelUpTo(BattleData.CurrentRound.Level);
                }

            }
            
        }
    }
}