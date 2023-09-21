using System;
using System.Collections.Generic;
using CGTool;
using Controller;
using DG.Tweening;
using GameData;
using Reference;
using UnityEngine;

namespace Prefeb
{
    // 掉落物品
    public class LootItem:MonoBehaviour
    {
        [SerializeField,Header("Sprite")] public SpriteRenderer SpriteRenderer;
        [SerializeField,Header("SpriteShadow")] public RectTransform Shadow;
        public LootSetting.LootInfo LootInfo;
        public int _level;
        public bool _isFlying;
        
        public static Queue<LootSetting.LootInfo> LootQueue = new Queue<LootSetting.LootInfo>();
        public static bool _isLooting;


        private void Update()
        {
            if (_isFlying) return;
            if(GlobalReference.BattleData.AutoLoot) FlyToPlayer();
        }

        // 拾取时触发对应效果
        public static void Loot()
        {
            if(_isLooting) return;
            if (LootQueue.Count == 0)
            {
                _isLooting = false;
                return;
            }
            LootSetting.LootInfo lootInfo = LootQueue.Dequeue();
            PlayerUnit playerUnit = GlobalReference.BattleData.PlayerUnit;
            switch (lootInfo.Type)
            {
                // 魔石 -> 金币
                case LootSetting.LootType.Stone:
                    int coin = Attr.ValueBase.Loot_Coin * lootInfo.Level;
                    //产生随机范围 0.8-1.2
                    coin = (int)(coin * UnityEngine.Random.Range(0.8f, 1.2f));
                    coin = coin * GlobalReference.BattleData.Loot_CoinRate / 100;
                    GlobalReference.BattleData.Money += coin;
                    GlobalReference.BattleData.TotalMoney += coin;
                    break;
                // 宝石 -> 经验
                case LootSetting.LootType.Jawel:
                    int exp = Attr.ValueBase.Loot_Exp * lootInfo.Level * GlobalReference.BattleData.Loot_ExpRate /
                              100;
                    exp = (int)(exp * UnityEngine.Random.Range(0.8f, 1.2f));
                    playerUnit.Attr.EXP += exp;
                    break;
                // 红宝石 -> HP
                case LootSetting.LootType.HP:
                    int hp = (int)(playerUnit.Attr.MaxHP * lootInfo.Level * (Attr.ValueBase.Loot_HP/100f));
                    playerUnit.Attr.HP += hp;
                    GameController.AddFloatingText(
                        playerUnit.transform.position + new Vector3(0, 50),
                        hp.ToString(), Color.green);
                    break;
                // 蓝宝石 -> MP
                case LootSetting.LootType.MP:
                    int mp = (int)(playerUnit.Attr.MaxMP * lootInfo.Level * (Attr.ValueBase.Loot_MP/100f));
                    playerUnit.Attr.MP += mp;
                    GameController.AddFloatingText(
                        playerUnit.transform.position + new Vector3(0, 50),
                        mp.ToString(), Color.blue);
                    break;
                // 红药水 -> HP瓶
                case LootSetting.LootType.HPBottle:
                    GlobalReference.BattleData.RedBottleCount++;
                    break;
                // 蓝药水 -> MP瓶
                case LootSetting.LootType.MPBottle:
                    GlobalReference.BattleData.BlueBottleCount++;
                    break;
            }
            Loot();
        }
        

        // 初始化效果
        public void Init(LootSetting.LootType lootType,int level)
        {
            LootInfo = LootSetting.GetLootInfo(lootType, level);
            _level = level;
            GraphicInfoData graphicInfoData = GraphicInfo.GetGraphicInfoDataBySerial((uint) LootInfo.Serial);
            GraphicData graphicData =
                Graphic.GetGraphicData(graphicInfoData, GlobalReference.BattleData.LevelInfo.PaletIndex);
            SpriteRenderer.sprite = graphicData.Sprite;
            Shadow.localPosition = new Vector3(0, -SpriteRenderer.sprite.texture.height / 2f + 3, 0);
        }

        // 玩家拾取范围碰撞体接近时,自动飞近玩家
        private void OnTriggerEnter2D(Collider2D other)
        {
            if(!other.CompareTag("PlayUnit")) return;
            if(LootInfo.Type!=LootSetting.LootType.Box) FlyToPlayer();
        }

        //飞向角色,如果角色移动中,则持续飞向角色
        public void FlyToPlayer()
        {
            _isFlying = true;
            GetComponent<BoxCollider2D>().enabled = false;
            InvokeRepeating(nameof(_fly), 0, 0.01f);
        }

        //飞向角色,自动修正位置
        private void _fly()
        {
            Vector3 targetPos = GlobalReference.BattleData.PlayerUnit.transform.position;
            Vector3 dir = targetPos - transform.position;
            float distance = dir.magnitude;
            float speed = 5f;
            
            if (distance < 0.3f)
            {
                CancelInvoke(nameof(_fly));
                LootQueue.Enqueue(LootInfo);
                Loot();
                Destroy(gameObject);
            }
            else
            {
                transform.position += dir.normalized * (distance > speed ? speed : distance);
            }
            
        }

        
    }
}