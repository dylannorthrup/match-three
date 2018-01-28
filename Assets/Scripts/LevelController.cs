using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour {
  public Transform brick;
  public int depth = 10;
  public int xOffset = 0;
  public int yOffset = 4;
  public int stackWidth = 4;
  public int stackHeight = 8;
  public bool brickColorChanging = false;
  public int newBrickHeight = 20;
  private int totalBricks;
  private int currentBricks;
  private BrickContainer bc;
  private Camera cam;

	// Use this for initialization
	void Start () {
    totalBricks = stackWidth * stackHeight;

    // Need this for onClick stuff
    cam = transform.Find ("MainCamera").GetComponent<Camera> ();

    // Need this for brick checking logic stuff
    bc = gameObject.GetComponentInChildren<BrickContainer>();
    bc.setCols (stackWidth);
    bc.setRows (stackHeight);

    // Create new bricks
    for (int y = 0; y < stackHeight; y++) {
      for (int x = 0; x < stackWidth; x++) {
        makeBrick (x, y);
      }
    }

    // Something to allow easier triggering of color change testing
    if (brickColorChanging) {
      GameObject[] bricks;
      bricks = GameObject.FindGameObjectsWithTag ("Brick");
      foreach(GameObject brick in bricks) {
        PrefabBrick b = brick.GetComponent<PrefabBrick> ();
        b.activateColorChanging ();
      }
    }
	}

  public void cleanupOnAisle(int i) {
    Debug.Log ("Doing Cleanup on Aisle " + i);
    makeBrick (i, newBrickHeight);
  }
    
  private void makeBrick(int x, int y) {
    PrefabBrick b;
    Transform bt = Instantiate(brick, new Vector3(xOffset + x, yOffset + y, depth), 
      Quaternion.identity);
    bt.SetParent (bc.transform);
    b = bt.GetComponent<PrefabBrick> ();
    b.setMyCol (x);
    totalBricks++;
  }

  public void moarBricks(Vector3 location) {
    // We want to offset this by the maximum height of the stack.
    location.y += (float)stackHeight + (float)0.5;
    Instantiate (brick, location, Quaternion.identity);
  }
	
  // Update is called once per frame
	void Update () {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = cam.ScreenPointToRay (Input.mousePosition);
      RaycastHit hit;

      if (Physics.Raycast (ray, out hit)) {
        if (hit.transform.CompareTag("Brick")) {
          hit.transform.BroadcastMessage ("goByeBye");
//          Debug.Log ("Hit a brick");
//        } else {
//          Debug.Log ("Did not hit a brick");
        }
      }
    }

	}
}
