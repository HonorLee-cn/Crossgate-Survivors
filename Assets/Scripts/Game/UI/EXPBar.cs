using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 角色经验条
    public class EXPBar:MonoBehaviour
    {
        [SerializeField,Header("当前角色等级")] public Text PlayerLevel;
        [SerializeField,Header("当前角色经验条")] public Slider PlayerEXPBar;
        
        private int _maxValue;
        private int _value;

        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                PlayerEXPBar.maxValue = value;
            }
        }
        
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                PlayerEXPBar.value = value;
            }
        }
        
    }
}