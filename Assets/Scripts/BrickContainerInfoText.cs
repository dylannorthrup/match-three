using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrickContainerInfoText : MonoBehaviour {

  private BrickContainer meMum;
  private Text info;

  // Use this for initialization
  void Start () {
    info = GetComponent<Text> ();
    meMum = transform.parent.gameObject.GetComponent<BrickContainer> ();
  }
	
  // Update is called once per frame
  void Update () {
    info.text = "BrickContainer Info Text" +
    "\nRows: " + meMum.rows +
    "\nCols: " + meMum.cols +
    "";
    
  }
}
