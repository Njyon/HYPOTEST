using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using MathPlus; //Using math plus!

public class colorDemoScript : MonoBehaviour {

	float t; // Timer so the colors dont change crazy fast!

	public GameObject normal; //Color demo Squares
	public GameObject vivid; //-

	public Slider vividness; //Sliders to adjust value
	public Slider damping; //-

	int vividnessInt; //Stored int
	int dampingInt; //-

	public Text vividText; //Text above sliders
	public Text dampingText; //-

	void Update () {
		//Read the sliders!
		vividnessInt = (int)vividness.value;
		dampingInt = (int)damping.value;

		//Update the display Text!
		vividText.text = "Vividness: " + vividnessInt.ToString();
		dampingText.text = "Damping: " + dampingInt.ToString ();

		t -= Time.deltaTime; //Lower the timer
		if (t <= 0) { // If the timer reaches 0 chang the color!
			t = 0.5f; //Reset timer

			normal.GetComponent<Image>().color = mathPlus.RandomColor (); //Change the colors of the demo Squares
			vivid.GetComponent<Image>().color = mathPlus.RandomSmartColor (vividnessInt, dampingInt);
		}
	}
}
