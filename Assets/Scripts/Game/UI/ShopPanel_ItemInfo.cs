using GameData;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 商店内道具信息框  当前选择 当前装备 公用
    public class ShopPanel_ItemInfo:MonoBehaviour
    {
        [SerializeField,Header("物品名称")] public Text ItemName;
        [SerializeField,Header("物品图片")] public Image ItemImage;
        [SerializeField,Header("物品类型")] public Text ItemType;
        [SerializeField,Header("物品简介")] public Text ItemInfo;
        [SerializeField,Header("物品属性")] public Text ItemAttr;

        private ItemSetting.Item _item;
        
        public ItemSetting.Item Item
        {
            get => _item;
            set
            {
                _item = value;
                ItemName.text = _item.Name;
                ItemName.color = _item.Color;
                ItemImage.sprite = _item.Sprite;
                ItemImage.SetNativeSize();
                ItemType.text = ItemSetting.TypeName[_item.Type];
                ItemInfo.text = _item.Desc;
                ItemAttr.text = _item.AbilityDesc;
            }
        }
    }
}