using UnityEngine;

namespace Util
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform target;  // 跟随目标的Transform
        public float smoothSpeed = 0.5f;  // 跟随的平滑速度
        public bool smooth = false;  // 是否平滑跟随
        public bool roamMode = false;  // 漫游模式
        public Vector3 roamPosition = Vector3.zero;  // 漫游模式下的相机位置
        public Vector3 roamOriginPosition;  // 漫游模式下的相机原始位置
        public float roamRadius = 500f;  // 漫游模式下的漫游半径
        

        private void LateUpdate()
        {
            UpdatePosition();
            
        }

        public void UpdatePosition()
        {
            if (roamMode)
            {
                //漫游模式下围绕OriginPosition进行随机目标roamPosition的漫游,漫游范围最大不超过roamRadius半径,到达roamPosition后会重新随机目标
                //移动方式为固定速度移动,速度为固定0.1f
                if (roamPosition == Vector3.zero)
                {
                    roamPosition = roamOriginPosition + Random.insideUnitSphere * roamRadius;
                }
                if (Vector3.Distance(transform.position, roamPosition) < 1f)
                {
                    roamPosition = roamOriginPosition + Random.insideUnitSphere * roamRadius;
                }
                Vector3 distance = roamPosition - transform.position;
                if (distance.magnitude > 1f)
                {
                    Vector3 smoothedPosition = Vector3.Lerp(transform.position, roamPosition, 0.001f);  // 平滑插值计算相机位置
                
                
                    transform.position = smoothedPosition;
                }
                else transform.position = roamPosition;  // 更新相机位置
            }
            else
            {
                if (target == null) return;
                Vector3 desiredPosition = target.position;  // 获取跟随目标的位置
                desiredPosition.z = -10;  // 设置相机的z轴位置
            
                Vector3 distance = desiredPosition - transform.position;
                // Debug.Log();
            
                if (smooth && distance.magnitude > 1f)
                {
                    Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);  // 平滑插值计算相机位置
                
                
                    transform.position = smoothedPosition;
                }
                else transform.position = desiredPosition;  // 更新相机位置
            }

        }
    }
}
