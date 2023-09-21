using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 统一血蓝条
    public class HpMpBar:MonoBehaviour
    {
        [SerializeField,Header("当前角色血蓝量")] public Text ValueText;
        [SerializeField,Header("当前角色血蓝条")] public Slider ValueBar;

        private int _maxValue;
        private int _value;
        
        public int MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                ValueBar.maxValue = value;
                ValueText.text = _value + "/" + _maxValue;
            }
        }
        
        public int Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueBar.value = value;
                ValueText.text = _value + "/" + _maxValue;
            }
        }
    }
}