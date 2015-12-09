using UnityEngine;
using System.Collections;
using System;

public class CameraController : MonoBehaviour {

   public GameObject player;
   public Camera mainCamera;

   private Vector3 offset;
   private Vector3 currentPosition;

   // Use this for initialization
   void Start () {
      offset = transform.position - player.transform.position;
   }

   // Update is called once per frame - guaranteed to run after all items have been processed in Update()
   void LateUpdate () {
      currentPosition = player.transform.position;

      // if phone is tilted to landscape mode
      if (Screen.orientation == ScreenOrientation.Landscape) {
         mainCamera.fieldOfView = 30;

         //mainCamera.dtransform.position.y = 35;
         //mainCamera.transform.position.z = -25;

         // limit x axis to [-2.5, 2.5]
         if (currentPosition.x > 2.5f) {
            currentPosition.x = 2.5f;
         }
         else if (currentPosition.x < -2.5f) {
            currentPosition.x = -2.5f;
         }

         currentPosition.z = -2.0f;
      }
      // portrait
      else {
         mainCamera.fieldOfView = 65;
         //mainCamera.transform.position.y = 25;
         //mainCamera.transform.position.z = -18;

         currentPosition.x = 0.0f;

         // limit z axis to [-2.0, 1.0]
         if (currentPosition.z > 1.0f) {
            currentPosition.z = 1.0f;
         }
         else if (currentPosition.z < -6.0f) {
            currentPosition.z = -6.0f;
         }

      }

      transform.position = currentPosition + offset;

   }
}
