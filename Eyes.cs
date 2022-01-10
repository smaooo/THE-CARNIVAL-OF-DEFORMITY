using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rand = System.Random;


public class Eyes : MonoBehaviour
{

    private Transform boundary;
    private Vector3[] areaBounds = new Vector3[2];
    public GameObject[] initialEyes;
    private GameObject[] eyes;
    public float inverseSpeed = 8;
    private VisualSnow visualSnow;
    private List<Transform> locs = new List<Transform>();
    public bool move = false;

    void Start()
    {
        visualSnow = FindObjectOfType<VisualSnow>();
        //initialEyes = new GameObject[2] { transform.GetChild(0).gameObject, transform.GetChild(1).gameObject };
        boundary = transform.parent.Find("Boundary");
        areaBounds[0] = boundary.GetComponent<EdgeCollider2D>().bounds.min + new Vector3(1,1,0);
        areaBounds[1] = boundary.GetComponent<EdgeCollider2D>().bounds.max - new Vector3(1,1,0);
        for (int i = 0; i < transform.childCount; i++)
        {
            locs.Add(transform.GetChild(i));
        }

        Rand rand = new Rand();
        //int r = rand.Next(4, 7);
        int r = 10;
        eyes = new GameObject[r];
        for (int i = 0; i < r; i++)
        {
            Vector3 loc = new Vector3(Random.Range(areaBounds[0].x, areaBounds[1].x), Random.Range(areaBounds[0].y, areaBounds[1].y ), 0);
            eyes[i] = Instantiate(initialEyes[rand.Next(0, 2)],transform);
            eyes[i].transform.localPosition = eyes[i].transform.InverseTransformPoint(loc);
            eyes[i].GetComponent<Animator>().speed = Random.Range(0.5f, 1.2f);
            //eyes[i].transform.localPosition = new Vector3(eyes[i].transform.localPosition.x, eyes[i].transform.localPosition.y, 0f);
            eyes[i].transform.localPosition = locs[i].transform.localPosition;

        }
    }


    void Update()
    {
        if (move)
        {
            if (!visualSnow.finished)
            {
                foreach(GameObject g in eyes)
                {
                    StartCoroutine(MoveEyes(g));
                }

            }
            else
            {
            
                foreach (GameObject g in eyes)
                {
                    StartCoroutine(HideEye(g));
                }
            }

        }
    }

    private IEnumerator MoveEyes(GameObject obj)
    {
        yield return null;
        Vector2 loc = new Vector2(Random.Range(areaBounds[0].x+1, areaBounds[1].x-1), Random.Range(areaBounds[0].y+1, areaBounds[1].y-1));
        loc = obj.transform.InverseTransformDirection(loc);
        Rand rand = new Rand();
        int r = rand.Next(0, 2);
        if(r == 0)
        {
            loc = -loc;
        }
        obj.GetComponent<Rigidbody2D>().AddForce(obj.transform.TransformDirection(loc.normalized/inverseSpeed), ForceMode2D.Impulse);
        //obj.transform.localPosition = new Vector3(obj.transform.localPosition.x, obj.transform.localPosition.y, 0f);


        yield return new WaitForSeconds(5);
        //while (obj.transform.position != loc)
        //{
            
        //    //yield return new WaitForEndOfFrame();
        //    //obj.transform.position = Vector3.MoveTowards(obj.transform.position, loc, 0.01f);
        //    //print(obj.transform.position);
        //    //print(loc);
        //}


    }

    private IEnumerator HideEye(GameObject obj)
    {
        obj.GetComponent<Rigidbody2D>().Sleep();
        yield return new WaitForSeconds(Random.Range(1f, 2f));
        obj.SetActive(false);
    }
}
