using System;
using CGTool;
using GameData;
using Reference;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Graphic = UnityEngine.UI.Graphic;

namespace Prefeb
{
    // 商店道具
    public class ShopItem:MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
    {
        [SerializeField,Header("道具名称")] public Text ItemName;
        [SerializeField,Header("道具图片")] public Image ItemImage;
        [SerializeField, Header("道具价格")] public Text ItemPrice;
        [SerializeField, Header("道具按钮")] public Button BuyButton;
        [SerializeField, Header("售罄按钮")] public Button SoldButton;
        private ItemSetting.Item _item;

        public ItemSetting.Item Item
        {
            get => _item;
            set
            {
                _item = value;
                ItemName.text = _item.Name;
                ItemImage.sprite = _item.Sprite;
                ItemImage.SetNativeSize();
                ItemPrice.text = _item.Price.ToString();
            }
        }

        private void Update()
        {
            if(GlobalReference.BattleData.Money < _item.Price)
                BuyButton.interactable = false;
            else
                BuyButton.interactable = true;
        }

        // 购买
        public void Buy()
        {
            if(GlobalReference.BattleData.Money < _item.Price) return;
            GlobalReference.BattleData.Money -= _item.Price;
            if (_item.Type == ItemSetting.Type.Bottle)
            {
                if(_item.Name=="血瓶")
                    GlobalReference.BattleData.RedBottleCount++;
                else
                    GlobalReference.BattleData.BlueBottleCount++;
                return;
            }

            if (GlobalReference.BattleData.EquipItems[_item.Type]!=null)
            {
                GlobalReference.BattleData.EquipItems[_item.Type].Sell();
            }
            GlobalReference.BattleData.EquipItems[_item.Type] = _item;
            _item.Equip();
            SoldButton.gameObject.SetActive(true);
            BuyButton.gameObject.SetActive(false);
            GlobalReference.BattleData.EquipChanged();
        }


        // 鼠标移入时显示道具信息及当前装备信息
        public void OnPointerEnter(PointerEventData eventData)
        {
            GlobalReference.UI.BattleUI.InteractiveAction.ItemInfo.Item = _item;
            GlobalReference.UI.BattleUI.InteractiveAction.ItemInfo.gameObject.SetActive(true);
            if (_item.Type!=ItemSetting.Type.Bottle && GlobalReference.BattleData.EquipItems[_item.Type] != null)
            {
                GlobalReference.UI.BattleUI.InteractiveAction.EquipInfo.Item = GlobalReference.BattleData.EquipItems[_item.Type];
                GlobalReference.UI.BattleUI.InteractiveAction.EquipInfo.gameObject.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            GlobalReference.UI.BattleUI.InteractiveAction.ItemInfo.gameObject.SetActive(false);
            GlobalReference.UI.BattleUI.InteractiveAction.EquipInfo.gameObject.SetActive(false);
        }
    }
}