using System;
using System.Collections.Generic;
using Prefeb;
using UnityEngine;

namespace Game.UI
{
    // 人物闪避点数UI
    public class DodgePoints:MonoBehaviour
    {
        [SerializeField,Header("闪避预制")] public GameObject DodgePoint_Prefeb;
        
        // 闪避点数对象列表
        private List<GameObject> _dodgePoints = new List<GameObject>();
        
        // 当前可用闪避点数
        private int _availableDodgePoints = 1;

        private void Awake()
        {
            MaxDodge = 1;
        }

        // 设置最大闪避点数
        public int MaxDodge
        {
            get => _dodgePoints.Count;
            set
            {
                if (_dodgePoints.Count > 0)
                {
                    foreach (GameObject dodgePoint in _dodgePoints)
                    {
                        Destroy(dodgePoint);
                    }
                    _dodgePoints.Clear();
                }
                // 建立闪避点数图像提示
                for (int i = 0; i < value; i++)
                {
                    GameObject dodgePoint = Instantiate(DodgePoint_Prefeb, transform);
                    _dodgePoints.Add(dodgePoint);
                    if(i<_availableDodgePoints)
                        dodgePoint.gameObject.SetActive(true);
                }
            }
        }
        
        // 设置当前可用闪避点数
        public int AvailableDodgePoint
        {
            get => _availableDodgePoints;
            set
            {
                _availableDodgePoints = value;
                for (int i = 0; i < _dodgePoints.Count; i++)
                {
                    if (i < _availableDodgePoints)
                    {
                        _dodgePoints[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        _dodgePoints[i].gameObject.SetActive(false);
                    }
                }
            }
        }
        
        
    }
}