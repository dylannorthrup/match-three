using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickContainer : MonoBehaviour {
  private int rows;
  private int cols;
  private LevelController meMum;
 
	// Use this for initialization
	void Start () {
    meMum = transform.parent.gameObject.GetComponent<LevelController>();
	}

  public void setRows(int _rows) {
    rows = _rows;
  }

  public void setCols(int _cols) {
    cols = _cols;
  }

  int currentBrickTotal() {
    return transform.childCount;
  }

  // Look through the columns and find out if any of them need a new brick
  public int whichRowNeedsANewBrick() {
    int[] bricksInColumn = new int[cols];
    foreach (int i in bricksInColumn) {
      bricksInColumn[i] = 0;
    }
    PrefabBrick[] allChildren = GetComponentsInChildren<PrefabBrick>();
    foreach (PrefabBrick brick in allChildren) {
      int _row = brick.getMyRow ();
      Debug.Log("Index of " + _row + " when setting up array");
      bricksInColumn [_row]++;
      // do what you want with the transform
    }
    for(int i=0; i < cols; i++) {
      try {
        if(bricksInColumn[i] < rows) {
          meMum.cleanupOnAisle(i);
        }
      }
      catch (Exception e) {
        Debug.LogError ("Checking " + i + " on array of length " + bricksInColumn.Length + " against rows of " + rows);
        throw e;
      }
    }
    return -1;
  }

	
	// Update is called once per frame
	void Update () {
		
	}
}
