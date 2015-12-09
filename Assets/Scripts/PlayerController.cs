using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class PlayerController : MonoBehaviour {

   public float speed;
   public Text mazesCompletedText;
   public GameObject player;
   public Color[] playerColors;

   private int numMazesCompleted;
   private Rigidbody rb;
   private GameObject pickupCubeTopLeft;
   private GameObject pickupCubeBotRight;
   private MazeGenerator mazeGen;
   private Renderer playerRenderer;
   private int colorFromIndex;
   private int colorToIndex;
   private float lerpValue;
   private Material colliderMaterial;
   private Component colliderWall;
   private Renderer wallRenderer;
   private int randSeed;
   private System.Random rand;
   private GameObject collObject;
   private Renderer collRenderer;

   public PlayerController() {
      numMazesCompleted = 0;
      lerpValue = 0.0f;
      colorFromIndex = 0;
      colorToIndex = 1;
      rand = new System.Random ();
      //Screen.orientation = ScreenOrientation.LandscapeRight;
   }

   // Use this for initialization
   void Start () {
      //Debug.Log ("starting player controller");
      rb = GetComponent<Rigidbody>();
      SetNumMazesCompletedText();
      pickupCubeTopLeft = GameObject.Find("PickUpCube (0)");
      pickupCubeBotRight = GameObject.Find("PickUpCube (1)");
      pickupCubeTopLeft.SetActive(true);
      pickupCubeBotRight.SetActive(false);
      mazeGen = GameObject.FindObjectOfType (typeof(MazeGenerator)) as MazeGenerator;
      playerRenderer = player.GetComponent(typeof(Renderer)) as Renderer;
      playerRenderer.material.color = Color.blue;
   }

   // Update is called once per frame
   void Update () {
      //if (player.transform.position.y < -10.0f) {
      //   player.transform.position.Set(0.0f,5.0f,0.0f);
      //};

      UpdatePlayerColor ();
   }

   // FixedUpdate is called before performing physics calculations / Update; fixed time between calls
   void FixedUpdate () {
      float moveHorizontal = Input.GetAxis ("Horizontal");
      float moveVertical = Input.GetAxis ("Vertical");

      Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);

      rb.AddForce (movement * speed);
   }

   void OnTriggerEnter (Collider other) {
      if (other.gameObject.CompareTag ("PickUpCube")) {
         // get a new random seed used to create a particular maze (want to save seeds of particularly good random mazes)
         randSeed = rand.Next();

         // update mazes completed text
         numMazesCompleted++;
         SetNumMazesCompletedText();

         // swap active finish pickup objects between top-left and bot-right
         if(pickupCubeTopLeft.activeSelf) {
            pickupCubeTopLeft.SetActive(false);
            pickupCubeBotRight.SetActive(true);
         }
         else {
            pickupCubeTopLeft.SetActive(true);
            pickupCubeBotRight.SetActive(false);
         }

         // generate a new maze using a coroutine
         StartCoroutine(mazeGen.CreateMaze(randSeed));
      }
   }

   void OnCollisionEnter(Collision col) {
      collObject = col.gameObject;

      // if the object is not tagged as "Ground"
      if (!collObject.CompareTag ("Ground")) {
         collRenderer = collObject.GetComponent (typeof(Renderer)) as Renderer;
         collRenderer.material.color = playerRenderer.material.color;
         ;
      }

      //Debug.Log ("collision object type: " + collObject.tag.ToString() + ", alpha = " + collRenderer.material.color.a);
   }

   void UpdatePlayerColor() {
      // changes color of player material gradually over time
      playerRenderer.material.color = Color.Lerp (playerColors[colorFromIndex], playerColors[colorToIndex], lerpValue);
      if (lerpValue < 1.0f) {
         // gradually increment lerpValue to 1.0f
         lerpValue += 0.001f;
      }
      else {
         // reset lerpValue to 0
         lerpValue = 0.0f;

         // update player color indices
         colorFromIndex = colorToIndex;
         if (colorToIndex < 6) { //playerColors.Length) {
            colorToIndex++;
         }
         else {
            colorToIndex = 0;
         }
      }
   }

   void SetNumMazesCompletedText() {
      //mazesCompletedText.text = "Mazes Completed: " + numMazesCompleted.ToString ();
      mazesCompletedText.text = "Maze ID: " + randSeed;
   }
}
