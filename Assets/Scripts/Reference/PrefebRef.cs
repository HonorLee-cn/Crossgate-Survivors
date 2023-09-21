using Game.UI;
using Prefeb;
using UnityEngine;

namespace Reference
{
    public class PrefebRef:MonoBehaviour
    {
        [SerializeField,Header("人物预制体")] public PlayerUnit PlayerUnit;
        [SerializeField,Header("地面物件预制体")] public MapUnit MapUnit;
        [SerializeField,Header("人物选择预制体")] public UserSelector UserSelector;
        [SerializeField,Header("关卡选择预制体")] public LevelSelector LevelSelector;
        [SerializeField,Header("交互物体预制体")] public InteractiveUnit InteractiveUnit;
        [SerializeField, Header("技能列表Item")] public SkillItem SkillItem_Prefeb;
        [SerializeField,Header("循环CD预制件")] public CircleCD CircleCD_Prefeb;
        [SerializeField,Header("掉落物预制件")] public LootItem LootItem_Prefeb;
        [SerializeField,Header("装备物预制件")] public EquipItem EquipItem_Prefeb;
    }
}