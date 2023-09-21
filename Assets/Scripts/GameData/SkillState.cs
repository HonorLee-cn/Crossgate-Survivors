using UnityEngine;

namespace GameData
{
    // 技能状态,技能被释放时,会附加在具有碰撞体的物体上,并在接触到敌方目标是将数据传递给敌方目标
    public class SkillState:MonoBehaviour
    {
        // 技能伤害
        public int Damage;
        // 击退距离
        public int HitBackDistance = 1;
        // 是否来自怪物伤害
        public bool IsFromEnemy;
        // 目标对象受击音效
        public int TargetAudio;
        // 是否允许暴击
        public bool CriticalAble = true;
        // 是否无视对方防御
        public bool IgnoreDefense = false;
    }
}