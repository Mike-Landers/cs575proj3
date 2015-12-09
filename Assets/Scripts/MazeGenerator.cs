using UnityEngine;
using System.Collections;
using System;

#if UNITY_IPHONE
using UnityEngine.iOS;
#endif

public class MazeGenerator : MonoBehaviour {

   public Transform WallHorizontal;
   public Transform WallVertical;

   private GameObject[,] wallsHorizontal;
   private GameObject[,] wallsVertical;
   private int gridWidth;
   private bool[,] cellVisited;
   private int currentCellX;
   private int currentCellY;
   private int neighborCellX;
   private int neighborCellY;
   private int numCellsVisited;
   private int[,] neighborCells;
   private int numAvailNeighborCells;
   private int totalNumCells;
   private Stack cellsXToRevisit;
   private Stack cellsYToRevisit;
   private System.Random rand;
   private int chosenNeighbor;
   private int[] availNeighborCells;
   private int randIndex;
   private int frameCount;


   public MazeGenerator () {
      gridWidth = 16;
      wallsHorizontal = new GameObject[gridWidth-1, gridWidth];
      wallsVertical = new GameObject[gridWidth, gridWidth-1];
      cellVisited = new bool[gridWidth, gridWidth];
      currentCellX = 0;
      currentCellY = 0;
      neighborCellX = 0;
      neighborCellY = 0;
      numCellsVisited = 0;
      neighborCells = new int[4, 3]; // {top, right, bot, left}
      numAvailNeighborCells = 0;
      totalNumCells = gridWidth * gridWidth;
      cellsXToRevisit = new Stack();
      cellsYToRevisit = new Stack();
      chosenNeighbor = 0;
      availNeighborCells = new int[3]; // max 3 available neighbors in a square grid system
      randIndex = 0;
      frameCount = 0;
   }

   void Start() {
      //Debug.Log ("starting maze generator");

      // get references to grid of horizontal prefab walls and initially set them inactive
      for(int i = 0; i < gridWidth-1; i++) {
         for(int j = 0; j < gridWidth; j++) {
            wallsHorizontal[i, j] = GameObject.Find("WallHorizontal (" + i.ToString() + ") (" + j.ToString() + ")");
            wallsHorizontal[i, j].SetActive(false);
         }
      }
      
      // get references to grid of vertical prefab walls
      for(int i = 0; i < gridWidth; i++) {
         for(int j = 0; j < gridWidth-1; j++) {
            wallsVertical[i, j] = GameObject.Find("WallVertical (" + i.ToString() + ") (" + j.ToString() + ")");
            wallsVertical[i, j].SetActive(false);
         }
      }

      SetCellsUnvisited ();

      //for (int y = 0; y < 5; y++) {
      //   for (int x = 0; x < 5; x++) {
            //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //cube.AddComponent<Rigidbody>();
            //cube.transform.position = new Vector3(x, y, 0);
      //   }
      //}
      //Instantiate(WallHorizontal, new Vector3(-13.5f, 0.5f, -12.6f), Quaternion.identity);
      //Instantiate(WallVertical, new Vector3(-12.6f, 0.5f, -13.5f), Quaternion.identity);
   }

   private void ShowAllWalls() {
      for(int i = 0; i < gridWidth-1; i++) {
         for(int j = 0; j < gridWidth; j++) {
            wallsHorizontal[i, j].SetActive(true);
         }
      }
      for(int i = 0; i < gridWidth; i++) {
         for(int j = 0; j < gridWidth-1; j++) {
            wallsVertical[i, j].SetActive(true);
         }
      }
   }

   private void SetCellsUnvisited() {
      for(int i = 0; i < gridWidth; i++) {
         for(int j = 0; j < gridWidth; j++) {
            cellVisited[i, j] = false;
         }
      }
   }

   private void GetAvailNeighborCells() {
      // neighborCells[0] = top
      // neighborCells[1] = right
      // neighborCells[2] = bot
      // neighborCells[3] = left

      // reset numAvailNeighborCells
      numAvailNeighborCells = 0;

      // check for top neighbor
      if (currentCellY < 15) {
         // if top neighbor not visited
         if (!cellVisited [currentCellX, currentCellY + 1]) {
            availNeighborCells[numAvailNeighborCells] = 0; // 0 = top
            numAvailNeighborCells++;
            neighborCells[0,0] = 1; // indicate neighbor exists and not visited
            neighborCells[0,1] = currentCellX; // neighbor cell x value
            neighborCells[0,2] = currentCellY + 1; // neighbor cell y value
         }
         else {
            // indicate top neighbor has been visited
            neighborCells[0,0] = 0;
         }
      }
      else {
         // indicate top neighbor does not exist
         neighborCells[0,0] = 0;
      }

      // check for right neighbor
      if (currentCellX < 15) {
         // if right neighbor not visited
         if (!cellVisited [currentCellX + 1, currentCellY]) {
            availNeighborCells[numAvailNeighborCells] = 1; // 1 = right
            numAvailNeighborCells++;
            neighborCells[1,0] = 1; // indicate neighbor exists and not visited
            neighborCells[1,1] = currentCellX + 1; // neighbor cell x value
            neighborCells[1,2] = currentCellY; // neighbor cell y value
         }
         else {
            // indicate right neighbor has been visited
            neighborCells[1,0] = 0;
         }
      }
      else {
         // indicate right neighbor does not exist
         neighborCells[1,0] = 0;
      }

      // check for bot neighbor
      if (currentCellY > 0) {
         // if bot neighbor not visited
         if (!cellVisited [currentCellX, currentCellY - 1]) {
            availNeighborCells[numAvailNeighborCells] = 2; // 2 = bot
            numAvailNeighborCells++; 
            neighborCells[2,0] = 1; // indicate neighbor exists and not visited
            neighborCells[2,1] = currentCellX; // neighbor cell x value
            neighborCells[2,2] = currentCellY - 1; // neighbor cell y value
         }
         else {
            // indicate bot neighbor has been visited
            neighborCells[2,0] = 0;
         }
      }
      else {
         // indicate bot neighbor does not exist
         neighborCells[2,0] = 0;
      }

      // check for left neighbor
      if (currentCellX > 0) {
         // if left neighbor not visited
         if (!cellVisited [currentCellX - 1, currentCellY]) {
            availNeighborCells[numAvailNeighborCells] = 3; // 3 = left
            numAvailNeighborCells++;
            neighborCells[3,0] = 1; // indicate neighbor exists and not visited
            neighborCells[3,1] = currentCellX - 1; // neighbor cell x value
            neighborCells[3,2] = currentCellY; // neighbor cell y value
            //Debug.Log("Left neighbor of (" + currentCellX + ", " + currentCellY + ") exists!");
         }
         else {
            // indicate left neighbor has been visited
            neighborCells[3,0] = 0;
            //Debug.Log("Left neighbor of (" + currentCellX + ", " + currentCellY + ") already visited / marked as unavailable");
         }
      }
      else {
         // indicate left neighbor does not exist
         neighborCells[3,0] = 0;
         //Debug.Log("Left neighbor of (" + currentCellX + ", " + currentCellY + ") does not exist / marked as unavailable");
      }
   }

   private void ChoseNeighborAndHideWall() {
      // get a random int which represents a value 0 to number of neighbor cells - 1
      randIndex = rand.Next (0, numAvailNeighborCells); // if 1 avail neighbor this will always return 0
      chosenNeighbor = availNeighborCells[randIndex];

      //Debug.Log ("current cell = (" + currentCellX + ", " + currentCellY + ")");
      //Debug.Log ("numAvailNeighborCells = " + numAvailNeighborCells);
      //Debug.Log ("chosenNeighbor = " + chosenNeighbor);


      switch (chosenNeighbor) {
         case 0: // top
            // if top neighbor exists and has not been visited
            if(neighborCells[0,0] == 1) {
               neighborCellX = currentCellX;
               neighborCellY = currentCellY + 1;
               // hide wall on top of current cell
               wallsHorizontal[currentCellY, currentCellX].SetActive(false);
            }
            else {
               //Debug.Log("Error: top neighbor cell of (" + currentCellX + ", " + currentCellY + ") doesn't exist");
            }
            break;

         case 1: // right
            // if right neighbor exists and has not been visited
            if(neighborCells[1,0] == 1) {
               neighborCellX = currentCellX + 1;
               neighborCellY = currentCellY;
               // hide wall to right of current cell
               wallsVertical[currentCellY, currentCellX].SetActive(false);
            }
            else {
               //Debug.Log("Error: right neighbor cell of (" + currentCellX + ", " + currentCellY + ") doesn't exist");
            }
            break;

         case 2: // bot
            // if bot neighbor exists and has not been visited
            if(neighborCells[2,0] == 1) {
               neighborCellX = currentCellX;
               neighborCellY = currentCellY - 1;
               // hide wall below current cell
               wallsHorizontal[currentCellY - 1, currentCellX].SetActive(false);
            }
            else {
               //Debug.Log("Error: bot neighbor cell of (" + currentCellX + ", " + currentCellY + ") doesn't exist");
            }
            break;

         case 3: // left
            // if left neighbor exists and has not been visited
            if(neighborCells[3,0] == 1) {
               neighborCellX = currentCellX - 1;
               neighborCellY = currentCellY;
               // hide wall to left of current cell
               wallsVertical[currentCellY, currentCellX - 1].SetActive(false);
               //Debug.Log ("left neighbor of (" + currentCellX + ", " + currentCellY + ") hidden");
            }
            else {
               //Debug.Log("Error: left neighbor cell of (" + currentCellX + ", " + currentCellY + ") doesn't exist");
            }
            break;
      }
   }

   public IEnumerator CreateMaze (int randSeed) {
      #if UNITY_IPHONE
      Handheld.SetActivityIndicatorStyle(UnityEngine.iOS.ActivityIndicatorStyle.White);
      #elif UNITY_ANDROID
      Handheld.SetActivityIndicatorStyle(AndroidActivityIndicatorStyle.Small);
      #endif

      Handheld.StartActivityIndicator();

      // assign a new random number generator based on the seed provided for this particular maze 
      rand = new System.Random (randSeed);

      // reset all cells to unvisited
      SetCellsUnvisited ();

      // show complete wall grid
      ShowAllWalls ();

      // Depth first search starting at upper left = [0,15]
      currentCellX = 0;
      currentCellY = 15;
      cellVisited[0,15] = true;
      numCellsVisited = 1;

      // while the number of cells visited is less than gridWidth squared
      while(numCellsVisited < totalNumCells) {

         // determine number / location of neighbor cells
         GetAvailNeighborCells();

         // if current cell has any unvisited neighbors
         if(numAvailNeighborCells > 0) {
         
            // push current cell to stack if there are 2 or more neighbors
            if(numAvailNeighborCells > 1) {
               cellsXToRevisit.Push(currentCellX);
               cellsYToRevisit.Push(currentCellY);
            }

            // chose an available neighbor randomly and hide wall between current cell and chosen neighbor
            ChoseNeighborAndHideWall();

            // every 8th frame, wait for 1 frame so we see maze being generated vs freeze briefly upon hitting the pick-up cube
            if((frameCount++ & 0x7) == 0x7) {
               yield return null;
            }

            // make chosen neighbor the current cell and mark as visited
            currentCellX = neighborCellX;
            currentCellY = neighborCellY;
            cellVisited[currentCellX, currentCellY] = true;
            numCellsVisited++;
         }
         // else if stack is not empty
         else if (cellsXToRevisit.Count > 0) {
            // pop last cell added to the stack and assign it to the current cell
            currentCellX = (int)cellsXToRevisit.Pop();
            currentCellY = (int)cellsYToRevisit.Pop();
         }
         else {
            //Debug.Log("Error: 0 available neighbors and stack is empty before all cells marked visited");
            break;
         }
      }

      Handheld.StopActivityIndicator();

      yield return null;
   }
}
