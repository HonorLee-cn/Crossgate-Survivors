using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    //光环效果
    public class UnitLight:MonoBehaviour
    {
        public SpriteRenderer _Renderer;
        public Dictionary<Guid,int> IncreaseValue = new Dictionary<Guid, int>();

        private void Awake()
        {
            _Renderer = GetComponent<SpriteRenderer>();
        }

        public void Show(Color color = default)
        {
            if (color != default) _Renderer.color = color;
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}