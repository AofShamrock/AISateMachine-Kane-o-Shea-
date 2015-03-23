using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {

	public float health;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	if(health < 0f)
		{
			//EnemyState.isPlayerAlive = false;
			transform.renderer.material.color = Color.black;
		}
	}
}
