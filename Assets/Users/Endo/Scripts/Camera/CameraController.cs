using UnityEngine;

namespace Endo
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Header("メインカメラと位置を同期させるカメラ")]
        private Camera[] followCameras;

        private void Update()
        {
            FollowCamerasToMe();
        }

        /// <summary>
        /// 対象のカメラを自身に追従させる
        /// </summary>
        private void FollowCamerasToMe()
        {
            foreach (Camera cam in followCameras)
            {
                Transform targetTrf = cam.transform;
                
                targetTrf.SetPositionAndRotation(transform.position, transform.rotation);
            }
        }
    }
}
