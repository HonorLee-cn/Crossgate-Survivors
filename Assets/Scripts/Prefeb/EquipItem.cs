using GameData;
using UnityEngine;
using UnityEngine.UI;

namespace Prefeb
{
    //装备道具
    public class EquipItem:MonoBehaviour
    {
        [SerializeField,Header("图片")] public Image Image;
        [SerializeField,Header("名称")] public Text Name;

        public void SetItem(ItemSetting.Item item)
        {
            Image.sprite = item.Sprite;
            Name.text = item.Name;
        }
    }
}