using UnityEngine;

public class DestroyExplosion : MonoBehaviour {

    public float ExplosionForce = 100f;
    [Tooltip("effects to instantiate when the thing goes boom. These effects should destroy themselves as soon as possible")]
    public GameObject[] ParticleEffectPrefabs;

    //for debug purposes, check keyboard for explosion command:
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Boom();
        }
    }

    public void Boom()
    {
        Rigidbody[] parts = GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody part in parts)
        {
            part.isKinematic = false;
            Vector3 explosionDirection = Random.onUnitSphere * ExplosionForce;
            explosionDirection.y = Mathf.Abs(explosionDirection.y);
            part.AddForce(explosionDirection, ForceMode.Impulse);
            part.AddTorque(Random.insideUnitSphere, ForceMode.Impulse);
            //Destroy(part.gameObject, 10f);
        }
        Destroy(this.gameObject, 10f);

        //instantiate each of the particle effects. They should take care of removing themselves whenever appropriate
        foreach (GameObject fx in this.ParticleEffectPrefabs)
        {
            Instantiate(fx, transform.position, transform.rotation);
        }
    }


}
