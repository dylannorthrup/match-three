using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickContainer : MonoBehaviour {
  public int matchThreshold = 4;

  public int rows { get; set; }

  public int cols { get; set; }

  private LevelController meMum;
  private bool stuffHasMoved;
 
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

  // Okay, this is the fundamental check method. It returns an array of ints that'll
  // get translated into x/y co-ordinates later on. If the color of the PrefabBrick
  // at bProxy[x][y] is the same as what we're looking at, add ourselves to the brickAry,
  // do checkLeft and checkDown, then return the results.
  List<int> check (List<int> brickAry, PrefabBrick[,] bProxy, Color myColor, int x, int y) {
    // Get match for the thing we're looking at
    Color targetColor = bProxy [x, y].getMyColor ();
    // If they don't match, simply return what we were given
    if (targetColor != myColor) {
      return brickAry;
    }
//    int blah = encodePointAsInt (x, y);

    return brickAry;
  }

  // We want to encode x and y into a single int for easier storage
  int encodePointAsInt (int x, int y) {
    int retInt = x + (y * 100);
    return retInt;
  }

  // And corresponding decode methods
  int decodeXFromPoint (int i) {
    return i % 100;
  }

  int decodeYFromPoint (int j) {
    return (int)(j / 100);
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
    bool[,] checkBlackList = new bool[rows, cols];
    // Now that we've built that up, let's compare colors
    List<int> brickAry = new List<int> ();
    for (int i = rows - 1; i > 0; i--) {
      for (int j = cols - 1; j > 0; j--) {
        if (checkBlackList [i, j] == true) {
          continue;
        }
        brickAry.Add (encodePointAsInt (i, j));
        if (i > 1) {
          brickAry = check (brickAry, brickProxy, brickProxy [i, j].getMyColor (), i - 1, j);
        }
        if (j > 1) {
          brickAry = check (brickAry, brickProxy, brickProxy [i, j].getMyColor (), i - 1, j);
        }
        // Okay, we should have a proper brickAry with all matched brick locations
        // If the array is larger than the match threshold, set a boolean so we add them to the delete queue
        bool startDeleting = false;
        if (brickAry.Count >= matchThreshold) {
          startDeleting = true;
        }
        // Add all of the brick locations to the blacklist (so we don't check them again)
        brickAry.ForEach ((int intPoint) => {
          int x = decodeXFromPoint (intPoint);
          int y = decodeYFromPoint (intPoint);
          checkBlackList [x, y] = true;
          if (startDeleting) {
            deleteQueue [x, y] = true;
          }
        });
      }
    }

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
