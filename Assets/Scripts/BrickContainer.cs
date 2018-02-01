using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickContainer : MonoBehaviour {
  public int matchThreshold = 4;

  public int rows { get; set; }

  public int cols { get; set; }

  private LevelController meMum;
  private bool stuffHasMoved;
  private bool[,] checkBlackList;
 
  // Use this for initialization
  void Start () {
    meMum = transform.parent.gameObject.GetComponent<LevelController> ();
  }

  int currentBrickTotal () {
    return transform.childCount;
  }

  // Go through all bricks and see if any of them are moving
  bool allBricksSettled () {
    foreach (PrefabBrick brick in getAllChildBlocks()) {
      if (brick.weBeMoving ()) {
        resetTheChecker ();
        return false;
      }
    }
    return true;
  }

  void resetTheChecker () {
    stuffHasMoved = true;
  }

  // Helper to make things smaller
  PrefabBrick[] getAllChildBlocks () {
    PrefabBrick[] allChildren = GetComponentsInChildren<PrefabBrick> ();
    return allChildren;
  }

  // Look through the columns and find out if any of them need a new brick
  public int whichRowNeedsANewBrick () {
    int[] bricksInColumn = new int[cols];
    foreach (int i in bricksInColumn) {
      bricksInColumn [i] = 0;
    }
    foreach (PrefabBrick brick in getAllChildBlocks()) {
      int _col = brick.getMyCol () - 1;
      Debug.Log ("Index of " + _col + " when setting up array");
      bricksInColumn [_col]++;
      // do what you want with the transform
    }
    for (int i = 0; i < cols; i++) {
      try {
        if (bricksInColumn [i] < rows) {
          meMum.cleanupOnAisle (i);
        }
      } catch (Exception e) {
        Debug.LogError ("Checking " + i + " on array of length " + bricksInColumn.Length + " against rows of " + rows);
        throw e;
      }
    }
    return -1;
  }

  List<int[]> checkNeighbors (List<int[]> brickList, PrefabBrick[,] bProxy, Color myColor, int x, int y) {
    int[] foo = new int[] { x, y };
    brickList.Add (foo);
    Debug.LogWarning ("Checking [" + x + "," + y + "]");
    if (x > 0 && !checkBlackList [x - 1, y]) {
      brickList = check (brickList, bProxy, myColor, x - 1, y);
    }
    if (x < checkBlackList.GetLength (0) - 1 && !checkBlackList [x + 1, y]) {
      brickList = check (brickList, bProxy, myColor, x + 1, y);
    }
    if (y > 0 && !checkBlackList [x, y - 1]) {
      brickList = check (brickList, bProxy, myColor, x, y - 1);
    }
    return brickList;
  }

  // Okay, this is the fundamental check method. It returns an array of ints that'll
  // get translated into x/y co-ordinates later on. If the color of the PrefabBrick
  // at bProxy[x][y] is the same as what we're looking at, add ourselves to the brickAry,
  // do checkLeft and checkDown, then return the results.
  List<int[]> check (List<int[]> brickList, PrefabBrick[,] bProxy, Color myColor, int x, int y) {
    // Make sure the brick we're checking is actually a brick.
    Color targetColor;
    try {
      // Try to get match for the thing we're looking at
      targetColor = bProxy [x, y].getMyColor ();
    } catch (System.IndexOutOfRangeException) {
      // If it was out of range of the proxy array, just return what we were given
      return brickList;
    }
    // If they don't match, simply return what we were given
    if (targetColor != myColor) {
      return brickList;
    }
    checkBlackList [x, y] = true;
    brickList = checkNeighbors (brickList, bProxy, myColor, x, y);
    return brickList;
  }

  void doMatchLogic () {
    // If stuff hasn't moved yet, just return
    if (!stuffHasMoved) {
      return;
    }
    // We will set up a two dimensional array with the colors of the bricks
    PrefabBrick[,] brickProxy = new PrefabBrick [rows, cols];
    bool[,] deleteQueue = new bool[rows, cols];
    foreach (PrefabBrick brick in getAllChildBlocks()) {
      int row = brick.getMyRow ();
      int col = brick.getMyCol ();
      brickProxy [row - 1, col - 1] = brick;
    }
    // We also want to set up a checkBlackList so we can be
    // a little more efficient about checking things.
    checkBlackList = new bool[rows, cols];
    // Now that we've built that up, let's compare colors
    List<int[]> brickList;
    for (int i = rows - 1; i >= 0; i--) {
      for (int j = cols - 1; j >= 0; j--) {
        if (checkBlackList [i, j] == true) {
          continue;
        }
        brickList = new List<int[]> ();
        brickList = checkNeighbors (brickList, brickProxy, brickProxy [i, j].getMyColor (), i, j);
        // Remove any duplicate entries from the bricklist.
        brickList = brickList.Distinct ().ToList ();
        // Print out some info about the bricklist.
        String foo = "";
        foreach (int[] thing in brickList) {
          foo = foo + "[" + thing [0] + "," + thing [1] + "]=";
        }
        Debug.LogWarning ("After doing check that started with [" + i + "," + j + "], the brickList was " + brickList.Count + " elements long: " + foo);
        // Okay, we should have a proper brickAry with all matched brick locations
        // If the array is larger than the match threshold, add them to the delete queue
        if (brickList.Count >= matchThreshold) {
          Debug.LogError ("brickList was long enough. Adding those bricks to the deleteQueue");
          // Add these to the delete queue if we got enough.
          foreach (int[] entry in brickList) {
            int x = entry [0];
            int y = entry [1];
            deleteQueue [x, y] = true;
          }
          foo = "Delete queue is: ";

          //    return;
          // TODO: SOMETHING IS MESSED UP HERE IN THE CHECK!!!! 
          // TODO: Somehow deleteQueue is getting out of this loop without us detecting things properly
          // TODO: and printing out its contents.  Something to look at another day
          for (int k = 0; i < deleteQueue.GetUpperBound (0); k++) {
            for (int l = 0; j < deleteQueue.GetUpperBound (1); l++) {
              if (deleteQueue [k, l] == true) {
                foo = foo + "[" + k + "," + l + "]=";
              }
            }
          }
          Debug.LogWarning (foo);

        }
//        brickList.ForEach ((int[,]) => {
//          int x = 
//          int y = decodeYFromPoint (intPoint);
//          checkBlackList [x, y] = true;
//          if (startDeleting) {
//            deleteQueue [x, y] = true;
//          }
//        });
      }
    }

    String frack = "Delete queue is: ";

//    return;
    for (int i = 0; i < deleteQueue.GetUpperBound (0); i++) {
      for (int j = 0; j < deleteQueue.GetUpperBound (1); j++) {
        if (deleteQueue [i, j]) {
          frack = frack + "[" + i + "," + j + "]=";
          brickProxy [i, j].goByeBye ();
        }
      }
    }
    Debug.LogWarning (frack);
  }
	
  // Update is called once per frame
  void Update () {
    // Do quick check for whether the bricks are static and in place before doing 
    // the heavyweight logic for matching
    if (allBricksSettled ()) {
      doMatchLogic ();
      stuffHasMoved = false;
    }
  }
}
