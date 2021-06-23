using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KasperDev.Dialogue.Example
{
    public class FaceTheCamera : MonoBehaviour
    {
        private Camera mainCamera;

        private void Awake()
        {
            mainCamera = Camera.main;
        }

        void Update()
        {
            transform.LookAt(mainCamera.transform);
            transform.Rotate(0, 180, 0);
        }
    }
}