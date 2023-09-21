using System;
using DG.Tweening;
using UnityEngine;

namespace Base
{
    // 幻影效果
    public class GhostEffect : MonoBehaviour
    {
        //定位对象的SpriteRenderer
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private float _startTime = 0;
        private float _duration = 0;
        private float _clearDuration = 0;
        
        public void showGhost(float duration,float spawnTimeval=0.1f,float clearDuration=0.5f)
        {
            _duration = duration;
            _clearDuration = clearDuration;
            _startTime = Time.time;
            InvokeRepeating("drawGhost", 0, spawnTimeval);    
        }

        private void drawGhost()
        {
            if(Time.time-_startTime>_duration)
            {
                CancelInvoke("drawGhost");
                return;
            }
            GameObject ghost = new GameObject();
            ghost.transform.SetParent(this.transform.parent.transform);
            Vector3 pos = transform.localPosition;
            pos.z = pos.z + 1;
            ghost.transform.localPosition = pos;
            ghost.transform.localScale = transform.localScale;
            ghost.transform.localRotation = transform.localRotation;
            ghost.layer = transform.parent.gameObject.layer;
            SpriteRenderer ghostSpriteRenderer = ghost.AddComponent<SpriteRenderer>();
            ghostSpriteRenderer.sprite = _spriteRenderer.sprite;
            ghostSpriteRenderer.sortingOrder = _spriteRenderer.sortingOrder-1;
            ghostSpriteRenderer.sortingLayerName = _spriteRenderer.sortingLayerName;
            ghostSpriteRenderer.DOFade(0, _clearDuration).onComplete = () =>
            {
                Destroy(ghost);
            };

        }
    }
}