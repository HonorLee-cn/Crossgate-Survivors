using System;
using System.Collections.Generic;
using GameData;
using Prefeb;
using Reference;
using UnityEngine;

namespace Game.UI
{
    // 商店界面
    public class ShopPanel:MonoBehaviour
    {
        [SerializeField,Header("道具信息")] public ShopPanel_ItemInfo ItemInfo;
        [SerializeField,Header("装备信息")] public ShopPanel_ItemInfo EquipInfo;
        [SerializeField,Header("道具列表")] public Transform ItemList;
        [SerializeField,Header("道具预制体")] public ShopItem Item_Prefeb;
        
        public List<ItemSetting.Item> ItemListData;
        
        private void Start()
        {
        }

        private void OnEnable()
        {
            Time.timeScale = 0;
            ItemListData = new List<ItemSetting.Item>();
            // 默认增加血瓶 魔瓶
            ItemListData.Add(ItemSetting.GetBottleItem());
            ItemListData.Add(ItemSetting.GetBottleItem(true));
            // 加入三个随机装备
            for (int i = 0; i < 3; i++)
            {
                ItemSetting.Item item =
                    ItemSetting.GetRandomItem(GlobalReference.BattleData.PlayerUnit.Attr.Level / 2);
                ItemListData.Add(item);
            }
            // 生成道具列表
            for (int i = 0; i < ItemListData.Count; i++)
            {
                ShopItem shopItem = Instantiate(Item_Prefeb, ItemList);
                shopItem.Item = ItemListData[i];

            }
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
            for (int i = 0; i < ItemList.childCount; i++)
            {
                Destroy(ItemList.GetChild(i).gameObject);
            }
        }

        public void Close()
        {
            transform.parent.gameObject.SetActive(false);
        }
    }
}