using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Util;

namespace Prefeb
{
    // BOSS血条
    public class BOSSBar:MonoBehaviour
    {
        [SerializeField,Header("BOSS血条")] private Slider BOSSHPBar;
        [SerializeField,Header("BOSS名称")] private Text BOSSName;
        [SerializeField,Header("BOSS血量")] private Text BOSSHP;

        private PlayerUnit _playerUnit;
        public Guid ID;
        
        public PlayerUnit PlayerUnit
        {
            get => _playerUnit;
            set
            {
                _playerUnit = value;
                ID = _playerUnit.ID;
                _playerUnit.Attr.OnAttrChange += (prop, change,attr) =>
                {
                    BOSSHP.text = Toolkit.FormatNumberWithKMB(attr.HP);
                    BOSSHPBar.value = attr.HP;
                    BOSSHPBar.maxValue = attr.MaxHP;
                };
                _playerUnit.OnDead += OnUnitDead;
            }
        }
        
        public string Name
        {
            get => BOSSName.text;
            set => BOSSName.text = "BOSS: " + value;
        }

        private void OnUnitDead(PlayerUnit playerUnit)
        {
            Destroy(gameObject);
        }
    }
}