using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    /// <summary>
    /// Points on the fuse that the sparks travel down.
    /// </summary>
    public Transform[] fusePoints;
    public float explosionForce, explosionRadius, upwardExplosionForceModifier;
    /// <summary>
    /// Modify via duration in fuseParts
    /// </summary>
    private float fuseTime;
    public bool isLit;
    public ParticleSystem explosionParts, fuseParts;
    public Animator animator;
    private GameObject player;
    private Rigidbody rb;
    /// <summary>
    /// The lowest possible speed the bomb needs to be moving at to stick into the ground when it hits a collider.
    /// </summary>
    [SerializeField] private float minSpeedToStickBomb;
    /// <summary>
    /// How far into the ground the bomb will stick itself.
    /// </summary>
    [SerializeField] private float distToStickInGround;
    private SphereCollider sphereCollider;
    private bool exploding;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        fuseTime = fuseParts.main.duration;
        //                  cam   
        player = transform.parent.gameObject;
    }

    
    private void OnCollisionEnter(Collision collision)
    {
        //If the bomb has been thrown                    and it's moving fast enough to stick 
        if (this.transform.parent == null && Mathf.Abs(collision.relativeVelocity.magnitude) >= minSpeedToStickBomb)
        {
            StickIntoGround(collision.GetContact(0).point);
            Explode();
        }
    }

    public virtual IEnumerator BombRoutine()
    {
        //play fuse sparks
        fuseParts.Play();
        //get the timestamps for each section of fuse
        float[] sectionTimes = GetFuseSectionTimes(GetFuseLength());
        float timer = 0;
        int currentSection = 1;
        while(timer < fuseTime)
        {
            //change current section
            if (timer >= sectionTimes[currentSection])
            {
                currentSection++;
            }
            MoveSparks(timer, currentSection, sectionTimes);
            timer += Time.deltaTime;
            yield return null;
        }
        if (exploding) { yield break; }
        Explode();
        yield return null;
    }
    
    /// <summary>
    /// Move sparks down the fuse based on the ratio of elapsed time to fuse time, and the number of sections the fuse has.
    /// </summary>
    protected void MoveSparks(float elapsed, int fuseSection, float[] sectionTimes)
    {
        if (exploding) { return; }
        fuseParts.transform.position = Vector3.Lerp(fusePoints[fuseSection-1].position, fusePoints[fuseSection].position, 
            (elapsed - sectionTimes[fuseSection - 1]) / (sectionTimes[fuseSection] - sectionTimes[fuseSection - 1]));
    }
    /// <summary>
    /// Returns the total length of the fuse. Only works if there are at least 2 fuse points.
    /// </summary>
    /// <returns></returns>
    protected float GetFuseLength()
    {
        float toReturn = 0;
        for (int i = 1; i < fusePoints.Length; i++)
        {
            toReturn += Vector3.Distance(fusePoints[i - 1].position, fusePoints[i].position);
        }
        return toReturn;
    }
    /// <summary>
    /// Returns an array with the timestamps associated with each fusePoint.
    /// </summary>
    /// <returns></returns>
    protected float[] GetFuseSectionTimes(float fuseLength)
    {
        float[] toReturn = new float[fusePoints.Length];
        toReturn[0] = 0;
        for (int i = 1; i < toReturn.Length; i++)
        {
            toReturn[i] = toReturn[i - 1] + (Vector3.Distance(fusePoints[i - 1].position, fusePoints[i].position) / fuseLength) * fuseTime;
        }
        return toReturn;
    }
    protected virtual void Explode()
    {
        exploding = true;
        if (this.transform.parent != null)
        {
            //kill player and break
            player.GetComponent<ThrowBomb>().BlowUpPlayer();
        }
        isLit = false;
        fuseParts.Stop();
        //explosionParts.Play();
        //do force math here:
        Collider[] colliders;
        colliders = Physics.OverlapSphere(sphereCollider.center, explosionRadius);
        foreach (Collider col in colliders)
        {
            if (col.gameObject.TryGetComponent<Rigidbody>(out Rigidbody body))
            {
                body.AddExplosionForce(explosionForce, this.transform.position, explosionRadius, upwardExplosionForceModifier, ForceMode.Impulse);
            }
        }
        Destroy(this.gameObject);
    }
    protected void StickIntoGround(Vector3 contactPoint)
    {
        rb.isKinematic = true;
        //Vector3 direction = (contactPoint - sphereCollider.center).normalized;
        //transform.position = transform.position + direction * distToStickInGround;
        transform.position = contactPoint;
        sphereCollider.enabled = false;
    }
}
