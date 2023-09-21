using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 角色数据面板
    public class PlayerDatas:MonoBehaviour
    {
        [SerializeField,Header("当前ATK")] public Text ATK;
        [SerializeField,Header("当前DEF")] public Text DEF;
        [SerializeField,Header("当前SPD")] public Text SPD;
        [SerializeField,Header("总杀敌")] public Text KILL;
        [SerializeField,Header("总金币")] public Text COIN;
    }
}