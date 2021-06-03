using UnityEngine;

namespace Endo
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Header("メインカメラと位置を同期させるカメラ")]
        private Camera[] followCameras;

        private Transform _playerTrf;
        private Vector3   _offsetPos;

        private void Start()
        {
            foreach (Camera cam in followCameras)
            {
                cam.transform.rotation = transform.rotation;
            }

            _playerTrf = GameObject.FindGameObjectWithTag("Player").transform;
            _offsetPos = transform.position - _playerTrf.position;
        }

        private void Update()
        {
            transform.position = _playerTrf.position + _offsetPos;
            
            foreach (Camera cam in followCameras)
            {
                cam.transform.position = transform.position;
            }
        }
    }
}
