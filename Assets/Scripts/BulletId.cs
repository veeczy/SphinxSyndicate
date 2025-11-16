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
    public bool timedDestroy = true;
    public float destroyTime = 5f;
    public string[] surfaceTags;
    public GameObject[] impactPrefabs;
    void Awake()
    {
        if(timedDestroy)
        StartCoroutine("destroy");
    }
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject != sender && !simpleMode)
        {
            for (int i = 0; i < surfaceTags.Length; i++)
            {
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
    IEnumerator destroy()
    {
        yield return new WaitForSeconds(destroyTime);
            Destroy(gameObject);
    }
}


