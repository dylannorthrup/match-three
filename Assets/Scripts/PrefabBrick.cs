using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabBrick : MonoBehaviour {
  public float colorChangePeriod = 100f; // Speed for automatic color changing
  public bool testColorChanging = false; // Toggle for automatic color changing
  public float mvmtTestDelay = 50f;      // Delay for checking if we're moving or not

  static private int numColors = 8;      // This is the upper bound of colors
  private int colorsAllowed = numColors; // This can be modifiable per level to make more colors
  private int colorIndex = 0;            // We want to use this for matching logic
  private float nextColorChange = 0.0f;  // Used when doing automatic color changing
  private Color[] colors = new Color[numColors];
  private Material materialColored;
  private Color currentColor;
  private Rigidbody rb;                  // Used for determining if this object is in motion
  private bool moving = true;            // Used for signalling if this object is in motion
  static private Vector3 stationary = new Vector3(0,0,0);  // Comparison vector (maybe not needed?)?
  static private float moveCheckTimer = 0.0f;   // Used for smoothing out movement check logic
  private int myCol;
  private BrickContainer meMum;

	// Use this for initialization
	void Start () {
    // Posible brick colors
    {
      colors[0] = Color.yellow;
      colors[1] = Color.red;
      colors[2] = Color.green;
      colors[3] = new Color(255, 165, 0);
      colors[4] = Color.blue;
      colors[5] = Color.magenta;
      colors[6] = Color.grey;
      colors[7] = Color.white;
    }
    ChangeColor ();

    // Set the rigidbody of this object so we can know when we're moving
    // or not
    rb = GetComponent<Rigidbody>();

    // Do this so we can refer to it later
    meMum = transform.parent.gameObject.GetComponent<BrickContainer>();
 	}

  public void setMyCol(int i) {
    myCol = i;
  }

  public int getMyRow() {
    return (int)(transform.position.y + 0.5);
  }

  public int getMyCol() {
    return myCol;
  }

  // So we can trigger this from the controller
  public void activateColorChanging() {
    testColorChanging = true;
  }

  // Something to test and set whether or not we're moving
  void isWeMoving() {
    if(Time.time < mvmtTestDelay) {
      return;
    }
    if(rb.velocity == stationary && moving == true) {
      if (Time.time > moveCheckTimer) {
        moveCheckTimer += mvmtTestDelay;
      } else {
        Debug.Log ("Block at " + rb.position + " Stopped moving");
        moving = false;
      }
    } 
    if(rb.velocity != stationary && moving == false){
      if (Time.time > moveCheckTimer) {
        moveCheckTimer += mvmtTestDelay;
      } else {
        Debug.Log ("Block at " + rb.position + " started moving");
        moving = true;
      }
    }
  }

  public Color getMyColor() {
    return currentColor;
  }

  // And a way for other things to see if we're moving
  public bool weBeMoving() {
    return moving;
  }

  // Update is called once per frame
  void Update () {
    isWeMoving ();

    // Used for testing color changing.
    if (testColorChanging) {
      if (Time.time > nextColorChange) {
        nextColorChange += colorChangePeriod;
        ChangeColor ();
      }
    }
  }

  // Logic for what to do when this object goes away
  void goByeBye() {
    Debug.LogError ("OH NOE! Going away at column: " + getMyCol() + " and row " + myCol);
    transform.SetParent (transform.parent.parent);
    Destroy (gameObject);
    meMum.whichRowNeedsANewBrick ();
  }

  // Something to allow us to dynamically change the color on the brick
  void ChangeColor() {
    int _range = colorBoundsCheck (colorsAllowed);
    ChangeColor (_range);
  }
  void ChangeColor(int range=3) {
    Debug.Log ("Using " + range + " for range check");
    int _range = colorBoundsCheck(range);
    Debug.Log ("Using " + _range + " for color after doing range check");
    colorIndex = Random.Range (0, _range);
    Debug.Log ("Random number chosen was " + colorIndex);
    Color newColor = colors[colorIndex];
    GetComponent<Renderer> ().material.color = newColor;
    currentColor = newColor;
  }

  // A simple bounds checker for making sure we don't go into uncharted 
  // color territory
  int colorBoundsCheck(int range) {
    if(range < 1) {
      return 1;
    }
    if(range > colors.Length + 1) {
      return colors.Length + 1;
    }
    return range;
  }
	
}
