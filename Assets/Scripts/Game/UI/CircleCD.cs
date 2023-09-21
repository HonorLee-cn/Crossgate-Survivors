using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    // 全局统一的CD处理器
    public class CircleCD:MonoBehaviour
    {
        [SerializeField,Header("冷却时间")] private Text cdTimeText;
        [SerializeField,Header("冷却效果ImageMask")] private Image MaskImage;
        [SerializeField,Header("对象Image")] private Image CoverImage;
        [SerializeField,Header("底层Image")] private Image BGImage;
        
        // 冷却完成事件
        public delegate void CDComplete();
        public event CDComplete OnCDComplete;
        
        // 是否可用
        private bool _available = true;
        public bool Available => _available;
        
        // 冷却时间
        private float _cdTime = 0;
        private float _cd = 0;

        
        // 设置技能图标
        public void SetItem(Sprite sprite)
        {
            CoverImage.sprite = sprite;
            if (BGImage != null) BGImage.sprite = sprite;
        }

        // 初始化显示
        private void Start()
        {
            if (MaskImage != null) MaskImage.fillAmount = 1;
            if (cdTimeText != null) cdTimeText.text = "";
        }

        // 开始CD
        public void StartCD(float cdTime)
        {
            _cdTime = cdTime;
            _cd = 0;
            _available = false;
            if(MaskImage!=null) MaskImage.fillAmount = 0;
            InvokeRepeating("countDown", 0, 0.1f);
        }

        // 停止CD
        public void Stop()
        {
            _available = true;
            CancelInvoke("countDown");
            if (cdTimeText != null)cdTimeText.text = "";
            if(MaskImage!=null) MaskImage.fillAmount = 1;
        }

        // 倒计时
        private void countDown()
        {
            _cd += 0.1f;
            if (_cd >= _cdTime)
            {
                _available = true;
                CancelInvoke("countDown");
                if (cdTimeText != null)cdTimeText.text = "";
                if(MaskImage!=null) MaskImage.fillAmount = 1;
                OnCDComplete?.Invoke();
                return;
            }

            if (cdTimeText != null) cdTimeText.text = Math.Round((_cdTime - _cd), 1) + "s";
            if (MaskImage != null) MaskImage.fillAmount = _cd / _cdTime;
        }
    }
}