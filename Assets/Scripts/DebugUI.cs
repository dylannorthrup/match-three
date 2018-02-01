using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DebugUI : MonoBehaviour {

  Text text;
  LevelController meMumsMum;

  // Use this for initialization
  void Start () {
    text = GetComponent<Text> ();
    meMumsMum = transform.parent.parent.gameObject.GetComponent<LevelController> ();
		
  }
	
  // Update is called once per frame
  void Update () {
    text.text = "Level Controller DebugUI" +
    "\nHeight should be " + meMumsMum.stackHeight +
    "\nWidth should be " + meMumsMum.stackWidth +
    "\nBricks in action: " + meMumsMum.getTotalBricks ();
		
  }
}
