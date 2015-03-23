using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyState : MonoBehaviour {

	public bool playerSeen;
	public float angleOfView = 110f;
	
	NavMeshAgent nma;
	Transform player;
	public Transform bullet;
	public Transform FiringPin;
	public Transform[] waypoints;
	int waypointsIndex = 0;
	public float moveSpeed;
	public float playerDistance;
	public float rotationDamping;
	public float waitTimer = 1f;
	float searchTime, searchTimer = 4f;
	float closeTime, closeTimer = 2f;
	float waitTime;
	SphereCollider col;
	int bulTime = 5;


	enum States //our states 
	{
		StartGame,
		Patrol,
		Seen,
		Tracking,
		Fighting,
	}
	States currentState = States.StartGame; //create an instance of the enum and set it's default to Initialize

	void Start()
	{
		nma = GetComponent<NavMeshAgent>();
		col = GetComponent<SphereCollider>();
	}
	
	void Update () 
	{
		switch(currentState) //pass in the current state
		{
		case States.StartGame:
			StartGame();
			break;
		case States.Patrol:
			Patrol ();
			break;
		case States.Seen:
			Seen ();
			break;
		case States.Tracking:
			Tracking ();
			break;
		case States.Fighting:
			Fighting ();
			break;
		}
	}
	void StartGame()
	{
		transform.renderer.material.color = Color.white;
		player  = GameObject.FindWithTag ("Player").transform;
		col = GetComponent<SphereCollider>();
		Debug.Log ("Start");
		currentState = States.Patrol;
	}

	void Patrol()
	{
		playerSeen = false;
		playerDistance = Vector3.Distance (player.position, transform.position);
		transform.renderer.material.color = Color.green;
		nma.destination = waypoints[waypointsIndex].position; //destination is a member of navmeshagent
		if(nma.remainingDistance < nma.stoppingDistance)
		{
			waitTime += Time.deltaTime;
			if(waitTime >= waitTimer)
			{
				//waypointIndex = (waypointIndex + 1) % 4; //does same thing as if structure below
				if(waypointsIndex == waypoints.Length - 1)
				{
					waypointsIndex = 0;
				}
				else
				{
					waypointsIndex++;
				}
				waitTime = 0f;
			}
		}
		ColliderCheck();

		//Debug.Log("Patrol");
	}
	
	void Seen()
	{
		playerDistance = Vector3.Distance (player.position, transform.position);
		if(playerDistance < 10f)
		{
		transform.renderer.material.color = Color.blue;
		//Debug.Log ("seen");
		Quaternion rotation = Quaternion.LookRotation (player.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);

		searchTime+=Time.deltaTime;
		if(searchTime >= searchTimer && playerSeen == true)
		{
				searchTime = 0;
				currentState = States.Tracking;
		}
		}
		else
		{
			currentState = States.Patrol;
		}
	}	
	
	void Tracking()
	{
		playerDistance = Vector3.Distance (player.position, transform.position);
		transform.renderer.material.color = Color.red;
		//Debug.Log("Tracking");
		if(playerDistance < 10f)
		{
			if(playerDistance > 4f)
			{
				FollowPlayer();
				if(playerDistance < 4.5f)
				{
					currentState = States.Fighting;

				}
			}
		}
		else if(playerDistance > 10f)
		{
			currentState = States.Seen;
		}
		/*else
		{
			currentState = States.Searching;
		}*/

	}
	
	void Fighting()
	{
		playerDistance = Vector3.Distance (player.position, transform.position);
		transform.renderer.material.color = Color.black;
		if(playerDistance < 5)
		{
			if(playerDistance > 3)
			{
			FollowPlayer();
			closeTime+=Time.deltaTime;
			if(closeTime >= closeTimer && playerSeen == true)
			{
				closeTime = 0;
				Instantiate(bullet, FiringPin.transform.position, transform.rotation);
				Debug.Log ("Shooting");
				//currentState = States.Tracking;
			}
			//Instantiate(bullet, FiringPin.transform.position, transform.rotation);
			}
		}
		else
			currentState = States.Seen;

	}
	void FollowPlayer()
	{
		transform.Translate (Vector3.forward * moveSpeed * Time.deltaTime);
		Quaternion rotation = Quaternion.LookRotation (player.position - transform.position);
		transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * rotationDamping);
	}

	/*void CollisionDetection()
	{
		playerDistance = Vector3.Distance (player.position, transform.position);
		if (playerDistance <= 5)
		{
			currentState = States.Seen;
		}
	}*/
	/*void OnTriggerEnter (Collider other) 
	{
		if (other.transform == player)
		{
			playerSeen = false;

			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			if(angle < angleOfView * 0.5f)
			{					
				Debug.Log("see ya");
				RaycastHit hit;
				
				// ... and if a raycast towards the player hits something...
				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, col.radius))
				{
					// ... and if the raycast hits the player...
					if(hit.collider.gameObject == player)
					{
						// ... the player is in sight.
						playerSeen = true;
					}
				}
			}
			if(playerSeen = true)
			{
				currentState = States.Seen;
			}

		}
	}*/
	void ColliderCheck()
	{
		Collider [] overCollider = Physics.OverlapSphere (transform.position, 10f);
		for(int i = 0; i < overCollider.Length; i++)
		{
			if(overCollider[i].gameObject.tag == "Player")
			{
				playerSeen = true;
				/*playerSeen = false;
				
				Vector3 direction = transform.position - transform.position;
				float angle = Vector3.Angle(direction, transform.forward);
				if(angle < angleOfView * 0.5f)
				{					
					Debug.Log("see ya");
					RaycastHit hit;
					
					// ... and if a raycast towards the player hits something...
					if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, 10))
					{
						// ... and if the raycast hits the player...
						if(hit.collider.gameObject == player)
						{
							// ... the player is in sight.
							playerSeen = true;
						}
					}*/
				}
				if(playerSeen == true)
				{
					currentState = States.Seen;
				}
			else if (overCollider[i].gameObject.tag != "Player")
			{
				playerSeen = false;
			}
			}
		}
}
	/*void OnTriggerExit (Collider other)
	{
		// If the player leaves the trigger zone...
		if(other.gameObject == player)
			// ... the player is not in sight.
			playerSeen = false;
		//currentState = States.Patrol;*/