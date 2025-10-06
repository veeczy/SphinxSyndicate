using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletId : MonoBehaviour
{
    public GameObject sender;
    private Collider senderCollider;
    public bool simpleMode = false; //Script only returns sender
    public int dmg;
    public bool impactDestroy = true;
    public string[] surfaceTags;
    public GameObject[] impactPrefabs;
    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log(col.gameObject + " : " + sender);
        if (col.gameObject != sender && !simpleMode)
        {
            for (int i = 0; i < surfaceTags.Length; i++)
            {
                //Debug.Log(col.gameObject.tag + " | " + surfaceTags[i] + " | " + col.gameObject.CompareTag(surfaceTags[i]));
                if (col.gameObject.CompareTag(surfaceTags[i]))
                {
                    Debug.DrawRay(transform.position, transform.forward, Color.white);
                    if (impactPrefabs != null)
                        Instantiate(impactPrefabs[i], transform.position, transform.rotation);

                }
                if (impactDestroy)
                    Destroy(gameObject);
            }

        }
    }
}


