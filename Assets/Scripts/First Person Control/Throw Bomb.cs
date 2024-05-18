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
    private bool isBombPrimed;
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

    [Header("Prefab References")]
    /// <summary>
    /// Bomb that explodes a certain amount of time after being lit.
    /// </summary>
    public Bomb timerBomb;
    /// <summary>
    /// Bomb that explodes on contact after being thrown.
    /// </summary>
    public Bomb impactBomb;
    /// <summary>
    /// The type of bomb that will be spawned in the player's hand. Change between levels?
    /// </summary>
    public Bomb bombPrefab;

    private void Awake()
    {
        //references
        rb = this.GetComponent<Rigidbody>();

        //instantiations
    }
    private void Update()
    {
        if (hasBomb)
        {
            //Check inputs:
            //prime bomb
            if (Input.GetMouseButtonDown(0) && !isBombPrimed)
            {
                isBombPrimed = true;
                StartCoroutine(heldBomb.BombRoutine());
            }
            //throw bomb
            else if (isBombPrimed && Input.GetMouseButtonUp(0))
            {

                heldBomb = null;
                isBombPrimed = false;
            }
        }
        else if (!waitingForBomb)
        {
            StartCoroutine(SpawnNewBomb(bombPrefab));
        }
    }
    /// <summary>
    /// Waits for timeToRespawnBomb before giving the player a new bomb
    /// </summary>
    /// <param name="bombPrefab"></param>
    /// <returns></returns>
    private IEnumerator SpawnNewBomb(Bomb bombPrefab)
    {
        waitingForBomb = true;
        yield return new WaitForSeconds(timeToRespawnBomb);
        heldBomb = Instantiate(bombPrefab.gameObject, holdPos).GetComponent<Bomb>();
        waitingForBomb = false;
        yield return null;
    }
    private void Throw() {}
}
