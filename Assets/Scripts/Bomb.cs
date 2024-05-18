using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Bomb : MonoBehaviour
{
    /// <summary>
    /// Points on the fuse that the sparks travel down.
    /// </summary>
    public Transform[] fusePoints;
    public float fuseTime, explosionForce, explosionRadius;
    public ParticleSystem explosionParts, fuseParts;

    public virtual IEnumerator BombRoutine()
    {
        //get the timestamps for each section of fuse
        float[] sectionTimes = GetFuseSectionTimes(GetFuseLength());
        float timer = 0;
        int currentSection = 1;

        while(timer < fuseTime)
        {
            MoveSparks(timer, currentSection, sectionTimes);
            timer += Time.deltaTime;
        }

        yield return null;
    }
    
    /// <summary>
    /// Move sparks down the fuse based on the ratio of elapsed time to fuse time, and the number of sections the fuse has.
    /// </summary>
    protected void MoveSparks(float elapsed, int fuseSection, float[] sectionTimes)
    {
        fuseParts.transform.position = Vector3.Lerp(fusePoints[fuseSection-1].position, fusePoints[fuseSection].position, 
            elapsed / (sectionTimes[fuseSection] - sectionTimes[fuseSection - 1]));
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
            //prevTimestamp + (thisSectionLength / totalLength) * totalTime 
            toReturn[i] = toReturn[i - 1] + (Vector3.Distance(fusePoints[i - 1].position, fusePoints[i].position) / fuseLength) * fuseTime;
        }
        return toReturn;
    }
    protected virtual void Explode()
    {
        explosionParts.Play();
    }
}
