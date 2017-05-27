using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class FireAndForget : MonoBehaviour {

    public float Speed;
    public GameObject ExplosionPrefab;
    public MOBAUnit.DamageType DamageTyp;
    public HeroControl LaunchedBy;
    public float DamageAmount;
    public float distanceSQR = 2f;

    [HideInInspector]
    public MOBAUnit Target;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Target)
        {
            if ((Target.transform.position - transform.position).sqrMagnitude < distanceSQR)
            {
                SelfDestruct();
            }
            transform.LookAt(Target.transform);
        }
        transform.Translate(Vector3.forward * Speed * Time.deltaTime);


	}

    private void SelfDestruct()
    {
        LaunchedBy.AwardPoints((int) this.DamageAmount);
        Target.ReceiveDamage(this.DamageTyp, this.DamageAmount);
        Destroy(this.gameObject);
        Destroy(Instantiate(this.ExplosionPrefab, transform.position, Quaternion.identity), 5);

    }
    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("fireball collided with " + collision.gameObject.name);
        //SelfDestruct();
    }
}
