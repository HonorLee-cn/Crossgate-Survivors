using System;
using UnityEngine;

namespace Game.UI
{
    // 自定义鼠标 一般用不到
    public class Mouse:MonoBehaviour
    {
        private void Start()
        {
            
        }

        private void OnEnable()
        {
            Cursor.visible = false;
        }

        private void Update()
        {
            //替换原有鼠标并跟随鼠标
            Vector3 mousePosition = Input.mousePosition;

            // 将屏幕位置转换为世界空间坐标
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            // 设置鼠标的位置
            transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }

        private void OnDisable()
        {
            Cursor.visible = true;
        }
    }
}