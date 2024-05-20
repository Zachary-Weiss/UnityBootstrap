using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBomb : MonoBehaviour
{
    private Rigidbody rb;
    /// <summary>
    /// True if the bomb is ready to be thrown. When true, the fuse should start counting down and 
    /// the bomb should start moving back
    /// </summary>
    private bool isBombLit;
    /// <summary>
    /// True if the bomb is ready to be thrown.
    /// </summary>
    private bool canThrowBomb;
    /// <summary>
    /// True if the player is holding a bomb.
    /// </summary>
    private bool hasBomb;
    /// <summary>
    /// True if the player is not holding a bomb and the spawnNewBomb coroutine has already started.
    /// </summary>
    private bool waitingForBomb;
    /// <summary>
    /// The amount of time it takes for the player to gain a new bomb after throwing the last one.
    /// </summary>
    public float timeToRespawnBomb;
    /// <summary>
    /// The time it takes to get ready to throw the bomb.
    /// </summary>
    public float timeToPrepThrow;
    /// <summary>
    /// Impulse force applied to thrown bombs.
    /// </summary>
    [SerializeField] private float throwForce;
    /// <summary>
    /// Impulse force applied to lobbed bombs.
    /// </summary>
    [SerializeField] private float lobForce;
    /// <summary>
    /// The pos that the bomb is held in idly.
    /// </summary>
    public Transform holdPos;
    /// <summary>
    /// The pos that the bomb is held in when the player is ready to throw it.
    /// </summary>
    public Transform throwPos;
    /// <summary>
    /// Direction the bomb is thrown in.
    /// </summary>
    private Vector3 throwDir;
    /// <summary>
    /// The Bomb component of the bomb the player is holding.
    /// </summary>
    private Bomb heldBomb;
    /// <summary>
    /// Reference to the PlayerReset component of the block that the player spawns on. Run ResetPlayer(transform.parent) to reset.
    /// </summary>
    private PlayerReset resetBlock;
    /// <summary>
    /// GameObject of the reset block.
    /// </summary>
    public GameObject resetBlockObject;

    [Header("Prefab References")]
    /// <summary>
    /// Bomb that explodes a certain amount of time after being lit.
    /// </summary>
    public GameObject timerBomb;
    /// <summary>
    /// Bomb that explodes on contact after being thrown.
    /// </summary>
    public GameObject impactBomb;
    /// <summary>
    /// The type of bomb that will be spawned in the player's hand. Change between levels?
    /// </summary>
    public GameObject bombPrefab;

    private void Awake()
    {
        //references
        rb = this.GetComponent<Rigidbody>();
        resetBlock = resetBlockObject.GetComponent<PlayerReset>();
        //instantiations
    }
    private void Start()
    {
        //give the player a bomb
        StartCoroutine(SpawnNewBomb(timerBomb));
    }
    private void Update()
    {
        if (hasBomb)
        {
            //Check inputs:
            //prime bomb

            if (Input.GetMouseButtonDown(0) && !heldBomb.isLit || Input.GetMouseButtonDown(1))
            {
                
                heldBomb.isLit = true;
                StartCoroutine(PrepareToThrowBomb());
                StartCoroutine(heldBomb.BombRoutine());
            }
            //throw bomb
            else if (heldBomb.isLit && Input.GetMouseButtonUp(0))
            {
                Throw(throwForce);
                heldBomb = null;
                canThrowBomb = false;
                hasBomb = false;
            }
            //lob bomb
            else if (heldBomb.isLit && Input.GetMouseButtonUp(1))
            {
                Throw(lobForce);
                heldBomb = null;
                canThrowBomb = false;
                hasBomb = false;
            }
        }
        //if !hasBomb and !waitingForBomb...
        else if (!waitingForBomb)
        {
            StartCoroutine(SpawnNewBomb(timerBomb));
        }
    }
    /// <summary>
    /// Waits for timeToRespawnBomb before giving the player a new bomb
    /// </summary>
    /// <param name="bombPrefab"></param>
    /// <returns></returns>
    private IEnumerator SpawnNewBomb(GameObject prefab)
    {
        waitingForBomb = true;
        yield return new WaitForSeconds(timeToRespawnBomb);
        //change the argument to give the player a different bomb
        heldBomb = MakeBomb(prefab.GetComponent<Bomb>());
        hasBomb = true;
        waitingForBomb = false;
        yield return null;
    }
    private IEnumerator PrepareToThrowBomb()
    {
        float timer = 0;
        //wind up the throw
        while (timer < timeToPrepThrow)
        {
            //stop coroutine if you already threw the bomb
            if (heldBomb == null)
            {
                yield break;
            }
            heldBomb.gameObject.transform.position = Vector3.Lerp(holdPos.position, throwPos.position, timer / timeToPrepThrow);
            timer += Time.deltaTime;
            yield return null;
        }
        heldBomb.transform.position = throwPos.position;
        canThrowBomb = true;
        yield return null;
    }
    private Bomb MakeBomb(Bomb prefab)
    {
        Bomb bomb = Instantiate(prefab.gameObject, holdPos.position, holdPos.rotation, this.transform).GetComponent<Bomb>();
        return bomb;
    }
    private void Throw(float force) 
    {
        heldBomb.GetComponentInChildren<Animator>().enabled = false;
        heldBomb.GetComponentInChildren<Animator>().transform.position = heldBomb.transform.position;
        heldBomb.transform.parent = null;
        heldBomb.GetComponent<Rigidbody>().isKinematic = false;
        heldBomb.GetComponent<SphereCollider>().enabled = true;
        //need to calibrate throwForce, replace transform.forward with throwDir
        heldBomb.GetComponent<Rigidbody>().AddForce(transform.forward * force, ForceMode.Impulse);
    }
    /// <summary>
    /// Runs if the player is still holding a bomb when it explodes. 
    /// </summary>
    public void BlowUpPlayer()
    {
        hasBomb = false;
        heldBomb = null;
        resetBlock.ResetPlayer(this.transform.parent.gameObject);
    }
}
