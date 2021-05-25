using UnityEngine;

namespace Endo
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Header("メインカメラと位置を同期させるカメラ")]
        private Camera[] followCameras;

        private void Update()
        {
            foreach (Camera cam in followCameras)
            {
                cam.transform.position = transform.position;
            }
        }
    }
}