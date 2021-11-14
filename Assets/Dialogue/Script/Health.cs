using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Health : MonoBehaviour
{
	public float maxHealthPoint = 10f;

	public float HealthPoint;

	public bool invincible = false;

	public bool dead = false;

	//public Animator animator;

	//public GameObject StrawberryPrefab;

	private void Start()
	{
		HealthPoint = maxHealthPoint;
		//animator = gameObject.GetComponent<Animator>();
	}

	private void Update()
	{
		if (HealthPoint <= 0)
		{
			if (name == "Madeline")
			{
				dead = true;
				GetComponent<PlayerMovement>().canMove = false;
				GetComponent<PlayerMovement>().Animation("Player_Death");
			}
			else if(name == "Spider")
			{
				if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Spider_Death"))
				{
					GetComponent<Animator>().Play("Spider_Death", 0, 0);
				}
				if (!dead)
					StartCoroutine(DelayedDeath(1.0f));
			}
			else if (name == "Flower")
			{
				if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("Flower_Death"))
				{
					GetComponent<Animator>().Play("Flower_Death", 0, 0);
				}
				if (!dead)
					StartCoroutine(DelayedDeath(1.0f));
			}
			else if (name == "Blood King")
			{
				if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("BloodKing_Death"))
				{
					GetComponent<Animator>().Play("BloodKing_Death", 0, 0);
				}
				if (!dead)
					StartCoroutine(DelayedDeath(1.0f));
			}

			else
			{
				//if (!animator.GetCurrentAnimatorStateInfo(0).IsName(""))
				//GetComponent<PlayerMovement>().Animation("");
				StartCoroutine(DelayedDeath(1.0f));
			}
		}

		if (HealthPoint > maxHealthPoint)
		{
			HealthPoint = maxHealthPoint;
		}

	}


	IEnumerator DelayedDeath(float delay)
	{
		dead = true;
		yield return new WaitForSeconds(delay);
		Instantiate(GameObject.Find("Strawberry"), transform.position + new Vector3(Random.Range(-1,1), Random.Range(1, 2)), Quaternion.identity);
		//Destroy(gameObject);
		GameObject.FindGameObjectWithTag("Player").GetComponent<Health>().HealthPoint++;
	}

}
