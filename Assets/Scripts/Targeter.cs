using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    [Tooltip("The Collider component of the Targeter. Can be a box or sphere collider.")]
    public Collider col;

    //List of all the enemies within the targeter:
    [HideInInspector] public List<Enemy> enemies = new List<Enemy>();

    //Return true if there are any targets:
    public bool TargetsAreAvailable
    {
        get
        {
            return enemies.Count > 0;
        }
    }


    public void SetRange(int range)
    {
        //Try to cast to a box collider:
        BoxCollider boxCol = col as BoxCollider;

        if (boxCol != null )
        {
            //We multiply range by 2 to make sure hte targeter covers a space 'rage' units in any direction.
            boxCol.center = new Vector3(range * 2, 30, range * 2);

            //Shift the Y position of the center up by half the height:
            boxCol.center = new Vector3(0, 15, 0);
        }
        else
        {
            //If it wasnt a box collider, try to cast to a sphere collider:
            SphereCollider sphereCol = col as SphereCollider;

            if ( sphereCol != null)
            {
                //Sphere collider radius is the distance from the center to the  edge.
                sphereCol.radius = range;
            }
            else
            {
                Debug.LogWarning("Collider for Targeter was not a box or sphere collider.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        var enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemies.Add(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var enemy = other.gameObject.GetComponent<Enemy>();
        if (enemy != null)
        {
            enemies.Remove(enemy);
        }
    }
}
