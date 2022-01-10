using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rand = System.Random;
public class VisualSnow : MonoBehaviour
{

    private enum Objects { Character, Lantern, Boundary}
    public Transform character;
    private Rigidbody2D characterRB;
    private Animator charAnim;
    private Transform lantern;
    private List<Transform> lanternParts = new List<Transform>();
    private List<bool> partsIn = new List<bool>();
    private Transform boundary;
    private Vector3[] areaBounds = new Vector3[2];
    private Vector2 velocity;
    public Sprite lanternTop;
    public float speed = 1.5f;
    private TentLogic tentLogic;
    private int lives = 2;
    public GameObject[] hearts;
    public bool finished = false;
    private Eyes eyeClass;

    void Start()
    {
        eyeClass = FindObjectOfType<Eyes>();
        character = transform.Find(Objects.Character.ToString());
        characterRB = character.GetComponent<Rigidbody2D>();
        charAnim = character.GetComponent<Animator>();
        lantern = transform.Find(Objects.Lantern.ToString());
        boundary = transform.Find(Objects.Boundary.ToString());
        tentLogic = FindObjectOfType<TentLogic>();
        
        for (int i = 0; i < lantern.childCount; i++)
        {
            lanternParts.Add(lantern.GetChild(i));
            partsIn.Add(false);
        }
        areaBounds[0] = boundary.GetComponent<EdgeCollider2D>().bounds.min;
        areaBounds[1] = boundary.GetComponent<EdgeCollider2D>().bounds.max;
        
        //ScatterObjects();
    }

    void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && character.GetComponent<MG3Character>().coliderObj != null)
        {
            PickObject(character.GetComponent<MG3Character>().coliderObj);
            
        } 


        if (!partsIn.Contains(false) && !finished)
        {
            StartCoroutine(TurnLightOn());
        }

        if (finished)
        {
            characterRB.Sleep();
        }
        
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        velocity = character.up * Input.GetAxisRaw("Vertical") + character.right * Input.GetAxisRaw("Horizontal");
        charAnim.SetFloat("MoveX", velocity.x);
        charAnim.SetFloat("MoveY", velocity.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1
                || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {
            eyeClass.move = true;

            charAnim.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
            charAnim.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
        }
        
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            characterRB.velocity = velocity * speed;
        }
        else
        {
            characterRB.velocity = Vector2.zero;
        }
    }

    private void PickObject(GameObject obj)
    {
        lantern.Find(obj.name).gameObject.SetActive(true);
        Destroy(obj);
        
        foreach (Transform t in lanternParts)
        {
            if (t.name == obj.name)
            {
                partsIn[lanternParts.IndexOf(t)] = true;
            }
        }
    }

    private void ScatterObjects()
    {
        Rand rand = new Rand();
        List<Vector3> prevlocs = new List<Vector3>();
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag("Collectable"))
            {
                
                Vector3 loc = new Vector3(Random.Range(areaBounds[0].x + 1, areaBounds[1].x-1), Random.Range(areaBounds[0].y+1, areaBounds[1].y-1), -0.1f);
                if (prevlocs.Count > 1)
                {
                    foreach(Vector3 v in prevlocs)
                    {
                        if (Mathf.Abs(v.x - loc.x) < 1 || Mathf.Abs(v.y - loc.y) < 1)
                        {
                            loc = new Vector3(Random.Range(areaBounds[0].x + 1, areaBounds[1].x-1), Random.Range(areaBounds[0].y+1, areaBounds[1].y-1), -0.1f);

                        }
                    }
                    prevlocs.Add(loc);
                }
                else
                {
                    prevlocs.Add(loc);
                }

                transform.GetChild(i).position = loc;
                //Random.seed = rand.Next(1000);
            }
        }
    }



    private IEnumerator TurnLightOn()
    {
        finished = true;
        yield return new WaitForSeconds(0.8f);
        lantern.Find("TOP").GetComponent<SpriteRenderer>().sprite = lanternTop;
        //tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);
        StartCoroutine(PlayWinLoseAnim("Win"));
    }

    public void ReduceLife()
    {
        if (!finished)
        {
            Destroy(hearts[lives]);
            lives--;
            if (lives < 0)
            {
                StartCoroutine(PlayWinLoseAnim("Lose"));
                return;
            }

        }
    }

    private IEnumerator PlayWinLoseAnim(string state)
    {
        
        finished = true;
        
        switch (state)
        {
            case "Win":
                transform.Find("WinLose").GetChild(0).GetComponent<Animator>().SetTrigger("Win");
                yield return new WaitForSeconds(transform.Find("WinLose").GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                character.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                transform.Find("WinLose").GetChild(0).GetComponent<Animator>().SetTrigger("Back");
                yield return new WaitForSeconds(transform.Find("WinLose").GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);
                break;

            case "Lose":
                transform.Find("WinLose").GetChild(0).GetComponent<Animator>().SetTrigger("Lose");
                yield return new WaitForSeconds(transform.Find("WinLose").GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                character.gameObject.SetActive(false);
                yield return new WaitForSeconds(0.2f);
                transform.Find("WinLose").GetChild(0).GetComponent<Animator>().SetTrigger("Back");
                yield return new WaitForSeconds(transform.Find("WinLose").GetChild(0).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Lose);
                break;



        }
    }
}
