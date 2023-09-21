using System;
using System.Reflection;
using Prefeb;
using UnityEngine;

namespace GameData
{
    /** 这是游戏统一的角色属性类
     *  前一版本为了方便,使用全反射和全递归方式处理,但会造成严重的性能问题
     *  具体可参考backup文件了解
     *  所以现在改为单个属性的get,set方法,并且使用委托监听来处理属性变化事件
     */
    
    // 基础属性
    public class Info
    {
        public int MaxHP;
        public int MaxMP;
        public int ATK;
        public int DEF;
        public int SPD;
        public int Level;
        public int NextLevelEXP;
    }

    // 属性类
    public class Attr
    {
        //基础数值
        public static class ValueBase
        {
            public static int HP = 20;
            public static int MP = 20;
            public static int ATK = 20;
            public static int DEF = 10;
            public static int SPD = 40;
            public static int EXP = 50;
            public static int Loot_Exp = 10;
            public static int Loot_Coin = 10;
            public static float Loot_HP = 5;
            public static float Loot_MP = 5;
        }

        // 当前等级
        private int _Level;
        public int Level {
            get => _Level;
            set
            {
                _Level = value;
                OnAttrChange?.Invoke("Level",_Level,this);
            }
        }
        
        // 下一等级经验
        private int _NextLevelEXP;
        public int NextLevelEXP {
            get => _NextLevelEXP;
            set
            {
                _NextLevelEXP = value;
                OnAttrChange?.Invoke("NextLevelEXP",_NextLevelEXP,this);
            }
        }
        
        //当前经验
        private int _EXP;
        public int EXP {
            get => _EXP;
            set
            {
                _EXP = value;
                if (value >= NextLevelEXP)
                {
                    value = value - NextLevelEXP;
                    _EXP = value;
                    _unit.LevelUp();
                    // OnAttrChange?.Invoke("Level", Level, this);
                    // OnAttrChange?.Invoke("NextLevelEXP", NextLevelEXP, this);
                }
                OnAttrChange?.Invoke("EXP",_EXP,this);
            }
        }

        // 攻击力
        // (不建议在其他地方直接修改当前值,而是通过改变Base,Addon,Addon_Rate来重新计算改变当前值)
        // 其他属性同理
        private int _ATK;
        public int ATK {
            get => _ATK;
            set
            {
                _ATK = value;
                OnAttrChange?.Invoke("ATK",_ATK,this);
            }
        }
        
        // 攻击力基础值
        private int _ATK_Base;
        public int ATK_Base {
            get => _ATK_Base;
            set
            {
                _ATK_Base = value;
                ATK = _ATK_Base + _ATK_Addon + _ATK_Base * _ATK_Addon_Rate / 100;
                OnAttrChange?.Invoke("ATK_Base",_ATK_Base,this);
            }
        }
        // 攻击力附加值
        private int _ATK_Addon;
        public int ATK_Addon {
            get => _ATK_Addon;
            set
            {
                _ATK_Addon = value;
                ATK = _ATK_Base + _ATK_Addon + _ATK_Base * _ATK_Addon_Rate / 100;
                OnAttrChange?.Invoke("ATK_Addon",_ATK_Addon,this);
            }
        }
        // 攻击力附加值百分比
        private int _ATK_Addon_Rate;
        public int ATK_Addon_Rate {
            get => _ATK_Addon_Rate;
            set
            {
                _ATK_Addon_Rate = value;
                ATK = _ATK_Base + _ATK_Addon + _ATK_Base * _ATK_Addon_Rate / 100;
                OnAttrChange?.Invoke("ATK_Addon_Rate",_ATK_Addon_Rate,this);
            }
        }
        private int _DEF;
        public int DEF {
            get => _DEF;
            set
            {
                _DEF = value;
                OnAttrChange?.Invoke("DEF",_DEF,this);
            }
        }
        private int _DEF_Base;
        public int DEF_Base {
            get => _DEF_Base;
            set
            {
                _DEF_Base = value;
                DEF = _DEF_Base + _DEF_Addon + _DEF_Base * _DEF_Addon_Rate / 100;
                OnAttrChange?.Invoke("DEF_Base",_DEF_Base,this);
            }
        }
        private int _DEF_Addon;
        public int DEF_Addon {
            get => _DEF_Addon;
            set
            {
                _DEF_Addon = value;
                DEF = _DEF_Base + _DEF_Addon + _DEF_Base * _DEF_Addon_Rate / 100;
                OnAttrChange?.Invoke("DEF_Addon",_DEF_Addon,this);
            }
        }
        private int _DEF_Addon_Rate;
        public int DEF_Addon_Rate {
            get => _DEF_Addon_Rate;
            set
            {
                _DEF_Addon_Rate = value;
                DEF = _DEF_Base + _DEF_Addon + _DEF_Base * _DEF_Addon_Rate / 100;
                OnAttrChange?.Invoke("DEF_Addon_Rate",_DEF_Addon_Rate,this);
            }
        }
        private int _SPD;
        public int SPD {
            get => _SPD;
            set
            {
                _SPD = value;
                OnAttrChange?.Invoke("SPD",_SPD,this);
            }
        }
        private int _SPD_Base;
        public int SPD_Base {
            get => _SPD_Base;
            set
            {
                _SPD_Base = value;
                SPD = _SPD_Base + _SPD_Addon + _SPD_Base * _SPD_Addon_Rate / 100;
                OnAttrChange?.Invoke("SPD_Base",_SPD_Base,this);
            }
        }
        private int _SPD_Addon;
        public int SPD_Addon {
            get => _SPD_Addon;
            set
            {
                _SPD_Addon = value;
                SPD = _SPD_Base + _SPD_Addon + _SPD_Base * _SPD_Addon_Rate / 100;
                OnAttrChange?.Invoke("SPD_Addon",_SPD_Addon,this);
            }
        }
        private int _SPD_Addon_Rate;
        public int SPD_Addon_Rate {
            get => _SPD_Addon_Rate;
            set
            {
                _SPD_Addon_Rate = value;
                SPD = _SPD_Base + _SPD_Addon + _SPD_Base * _SPD_Addon_Rate / 100;
                OnAttrChange?.Invoke("SPD_Addon_Rate",_SPD_Addon_Rate,this);
            }
        }

        private int _HP;
        public int HP {
            get => _HP;
            set
            {
                
                if (value > MaxHP) value = MaxHP;
                if (value < 0) value = 0;
                _HP = value;
                OnAttrChange?.Invoke("HP",_HP,this);
            }
        }
        private int _MaxHP;
        public int MaxHP {
            get => _MaxHP;
            set
            {
                _MaxHP = value;
                if(_HP > _MaxHP) HP = _MaxHP;
                OnAttrChange?.Invoke("MaxHP",_MaxHP,this);
            }
        }
        private int _MaxHP_Base;
        public int MaxHP_Base {
            get => _MaxHP_Base;
            set
            {
                _MaxHP_Base = value;
                MaxHP = _MaxHP_Base + _MaxHP_Addon + _MaxHP_Base * _MaxHP_Addon_Rate / 100;
                OnAttrChange?.Invoke("MaxHP_Base",_MaxHP_Base,this);
            }
        }
        private int _MaxHP_Addon;
        public int MaxHP_Addon {
            get => _MaxHP_Addon;
            set
            {
                _MaxHP_Addon = value;
                MaxHP = _MaxHP_Base + _MaxHP_Addon + _MaxHP_Base * _MaxHP_Addon_Rate / 100;
                OnAttrChange?.Invoke("MaxHP_Addon",_MaxHP_Addon,this);
            }
        }
        private int _MaxHP_Addon_Rate;
        public int MaxHP_Addon_Rate {
            get => _MaxHP_Addon_Rate;
            set
            {
                _MaxHP_Addon_Rate = value;
                MaxHP = _MaxHP_Base + _MaxHP_Addon + _MaxHP_Base * _MaxHP_Addon_Rate / 100;
                OnAttrChange?.Invoke("MaxHP_Addon_Rate",_MaxHP_Addon_Rate,this);
            }
        }

        private int _MP;
        public int MP {
            get => _MP;
            set
            {
                if (value > MaxMP) value = MaxMP;
                if (value < 0) value = 0;
                _MP = value;
                OnAttrChange?.Invoke("MP",_MP,this);
            }
        }
        private int _MaxMP;
        public int MaxMP {
            get => _MaxMP;
            set
            {
                _MaxMP = value;
                if(_MP > _MaxMP) MP = _MaxMP;
                OnAttrChange?.Invoke("MaxMP",_MaxMP,this);
            }
        }
        private int _MaxMP_Base;
        public int MaxMP_Base {
            get => _MaxMP_Base;
            set
            {
                _MaxMP_Base = value;
                MaxMP = _MaxMP_Base + _MaxMP_Addon + _MaxHP_Base * _MaxMP_Addon_Rate / 100;
                OnAttrChange?.Invoke("MaxMP_Base",_MaxMP_Base,this);
            }
        }
        private int _MaxMP_Addon;
        public int MaxMP_Addon {
            get => _MaxMP_Addon;
            set
            {
                _MaxMP_Addon = value;
                MaxMP = _MaxMP_Base + _MaxMP_Addon + _MaxHP_Base * _MaxMP_Addon_Rate / 100;
                OnAttrChange?.Invoke("MaxMP_Addon",_MaxMP_Addon,this);
            }
        }
        private int _MaxMP_Addon_Rate;
        public int MaxMP_Addon_Rate {
            get => _MaxMP_Addon_Rate;
            set
            {
                _MaxMP_Addon_Rate = value;
                MaxMP = _MaxMP_Base + _MaxMP_Addon + _MaxHP_Base * _MaxMP_Addon_Rate / 100;
                OnAttrChange?.Invoke("MaxMP_Addon_Rate",_MaxMP_Addon_Rate,this);
            }
        }

        private int _Dodge;
        public int Dodge {
            get => _Dodge;
            set
            {
                if (value > _MaxDodge) value = _MaxDodge;
                if (value < 0) value = 0;
                _Dodge = value;
                OnAttrChange?.Invoke("Dodge",_Dodge,this);
            }
        }
        private int _MaxDodge;
        public int MaxDodge {
            get => _MaxDodge;
            set
            {
                if (value < _Dodge)
                {
                    _Dodge = value;
                    OnAttrChange?.Invoke("Dodge", _Dodge, this);
                }
                _MaxDodge = value;
                OnAttrChange?.Invoke("MaxDodge",_MaxDodge,this);
            }
        }
        private float _DodgeCD_Base;
        public float DodgeCD_Base {
            get => _DodgeCD_Base;
            set
            {
                _DodgeCD_Base = value;
            }
        }
        private int _DodgeCD_Addon_Rate;
        public int DodgeCD_Addon_Rate {
            get => _DodgeCD_Addon_Rate;
            set
            {
                _DodgeCD_Addon_Rate = value;
                DodgeCD = _DodgeCD_Base - _DodgeCD_Base * _DodgeCD_Addon_Rate / 100;
                OnAttrChange?.Invoke("DodgeCD_Addon_Rate",_DodgeCD_Addon_Rate,this);
            }
        }
        private float _DodgeCD;
        public float DodgeCD {
            get => _DodgeCD;
            set
            {
                // DodgeCD = _DodgeCD_Base - _DodgeCD_Base * _DodgeCD_Addon_Rate / 100;
                _DodgeCD = value;
            }
        }
        

        // 属性变化通知
        public delegate void AttrChange(string prop,int value,Attr attr);
        public event AttrChange OnAttrChange;
        
        // 属性绑定对象
        private PlayerUnit _unit;

        // 属性初始化
        public Attr(PlayerUnit unit)
        {
            _unit = unit;
            MaxDodge = 1;
            Dodge = 1;
            DodgeCD_Base = 5f;
            DodgeCD_Addon_Rate = 0;
        }

        // 设定基础属性
        public void InitBase(PlayerSetting.PlayerInfo info)
        {
            ATK_Base = (int) (info.ATK * ValueBase.ATK);
            DEF_Base = (int) (info.DEF * ValueBase.DEF);
            SPD_Base = (int) (info.SPD * ValueBase.SPD);
            MaxHP_Base = (int) (info.HP * ValueBase.HP);
            MaxMP_Base = (int) (info.MP * ValueBase.MP);
            HP = MaxHP_Base;
            MP = MaxMP_Base;
            NextLevelEXP = ValueBase.EXP;
        }
        
        // 反射获取属性值
        public object GetPropertyValue(string propertyName)
        {
            Type type = GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(this, null);
            }
            else
            {
                return null;
            }
        }
        
        // 反射设置属性值
        public void SetPropertyValue(string propertyName, object value)
        {
            Type type = GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);

            if (propertyInfo != null)
            {
                propertyInfo.SetValue(this, value, null);
            }
        }

    }
}