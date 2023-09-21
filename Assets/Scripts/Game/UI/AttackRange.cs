using System;
using Prefeb;
using UnityEngine;

namespace Game.UI
{
    public class AttackRange:MonoBehaviour
    {
        [SerializeField,Header("PlayerUnit")] public PlayerUnit PlayerUnit;

        // 怪物检测玩家是否进入攻击范围
        private void OnTriggerEnter2D(Collider2D other)
        {
            PlayerUnit fromUnit = other.GetComponent<PlayerUnit>();
            if(fromUnit == null) return;
            if(fromUnit.IsEnemy) return;
            
            // 开始攻击
            PlayerUnit.IsPlayerInAttackRange = true;
            PlayerUnit.CaseSkills();
        }
        
        //玩家离开攻击范围
        private void OnTriggerExit2D(Collider2D other)
        {
            PlayerUnit fromUnit = other.GetComponent<PlayerUnit>();
            if(fromUnit == null) return;
            if(fromUnit.IsEnemy) return;
            
            // 停止攻击
            PlayerUnit.IsPlayerInAttackRange = false;
            PlayerUnit.StopCastSkills();
        }
    }
}