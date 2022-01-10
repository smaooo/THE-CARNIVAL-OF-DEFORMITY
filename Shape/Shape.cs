using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Shape : MonoBehaviour
{
    private enum Objects { Character, Levels}
    private enum Passages { Left, Middle, Right}
    private enum Levels { Level1, Level2, Level3, Level4}
    private Levels currentLevel;
    private Transform character;
    private Animator charAnim;
    private Rigidbody characterRB;
    private Vector3 velocity;
    private Vector3 prevVel = new Vector3(0, 1, 0);
    public float CharacterSpeed = 1;
    private Transform[] levels = new Transform[4];
    private Vector3 charPos;
    public GameObject fader;
    public float CameraFadeSpeed = 1;
    public float WaitDurationinFade = 0.8f;
    private Stack<KeyCode> inputs = new Stack<KeyCode>();
    private KeyCode prevLastKey;
    public Material CamMaterial;
    private float angleOffset = 0;
    public float offsetSpeed = 1;
    private TentLogic tentLogic;


    private void Start()
    {
        //CamMaterial.SetInt("_MG4", 1);   
        //GetComponent<BlitManager>().MG4ActiveBlit();
        tentLogic = FindObjectOfType<TentLogic>();
        character = transform.Find(Objects.Character.ToString());
        charAnim = character.GetComponent<Animator>();
        characterRB = character.GetComponent<Rigidbody>();
        currentLevel = Levels.Level1;
        charPos = character.position;
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i] = transform.Find(Objects.Levels.ToString()).GetChild(i);
        }
    }

    private void Update()
    {

        Debug.DrawRay(character.position, character.right, Color.red);
        angleOffset += Time.deltaTime * offsetSpeed;
        CamMaterial.SetFloat("_AngleOffset", angleOffset);

        if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && character.GetComponent<MG4Character>().coliderObj != null)
        {
            CheckAnswer(character.GetComponent<MG4Character>().coliderObj.name);
        }

        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            inputs.Push(KeyCode.W);
            MoveCharacter();

        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            inputs.Push(KeyCode.D);
            MoveCharacter();

        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            inputs.Push(KeyCode.S);
            MoveCharacter();

        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            inputs.Push(KeyCode.A);
            MoveCharacter();

        }


        if (inputs.Count > 0)
        {
            MoveCharacter();


            if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
            {
                if (inputs.Contains(KeyCode.W))
                {
                    KeyCode[] tmp = new KeyCode[inputs.Count];
                    inputs.CopyTo(tmp, 0);

                    inputs.Clear();
                    foreach (KeyCode k in tmp)
                    {
                        if (k != KeyCode.W)
                        {
                            inputs.Push(k);
                        }
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow))
            {
                if (inputs.Contains(KeyCode.D))
                {
                    KeyCode[] tmp = new KeyCode[inputs.Count];
                    inputs.CopyTo(tmp, 0);
                    inputs.Clear();
                    
                    foreach (KeyCode k in tmp)
                    {
                        if (k != KeyCode.D)
                        {
                            inputs.Push(k);
                        }
                    }
                }
            }
            else if (Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.DownArrow))
            {
                if (inputs.Contains(KeyCode.S))
                {
                    KeyCode[] tmp = new KeyCode[inputs.Count];
                    inputs.CopyTo(tmp, 0); 
                    inputs.Clear();
                    foreach (KeyCode k in tmp)
                    {
                        if (k != KeyCode.S)
                        {
                            inputs.Push(k);
                        }
                    }

                }
            }
            else if (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))
            {
                if (inputs.Contains(KeyCode.A))
                {
                    KeyCode[] tmp = new KeyCode[inputs.Count];
                    inputs.CopyTo(tmp, 0);
                    inputs.Clear();
                    foreach (KeyCode k in tmp)
                    {
                        if (k != KeyCode.A)
                        {
                            inputs.Push(k);
                        }
                    }
                }

            }
           
        }
        if (!Input.anyKey)
        {
            inputs.Clear();

        }


    }


    private void MoveCharacter()
    {


        KeyCode lastKey = inputs.Peek();
        
        
        switch (lastKey)
        {
            case KeyCode.W:
                prevLastKey = lastKey;
                charAnim.SetFloat("MoveF", 0.5f);

                break;

            case KeyCode.D:
                prevLastKey = lastKey;
                charAnim.SetFloat("MoveF", 1);

                break;

            case KeyCode.S:
                prevLastKey = lastKey;
                charAnim.SetFloat("MoveF", 0.8f);

                break;

            case KeyCode.A:
                prevLastKey = lastKey;
                charAnim.SetFloat("MoveF", 1);

                break;
        }

        if (prevLastKey == KeyCode.D || prevLastKey == KeyCode.A)
        {
            velocity = character.right * Input.GetAxisRaw("Horizontal");
        }
        else if (prevLastKey == KeyCode.W || prevLastKey == KeyCode.S)
        {
            velocity = character.up * Input.GetAxisRaw("Vertical");
        }

        charAnim.SetFloat("MoveX", -velocity.x);
        charAnim.SetFloat("MoveY", velocity.y);
        

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1
                || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {

            charAnim.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
            charAnim.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
        }
        if (character.GetComponent<MG4Character>().collided && prevVel == velocity)
        {
            prevVel = velocity;
            charAnim.SetFloat("MoveX", 0);
            charAnim.SetFloat("MoveY", 0);
        }

        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {

            characterRB.velocity = velocity * CharacterSpeed;
        }
        else
        {
            characterRB.velocity = Vector3.zero;
        }

    }

    private void CheckAnswer(string objName)
    {
        switch (currentLevel)
        {
            case Levels.Level1:
                if (objName.Equals(Passages.Middle.ToString()))
                {
                    currentLevel = Levels.Level2;
                    StartCoroutine(FadeCamera());

                }
                else
                {
                    currentLevel = Levels.Level1;
                    StartCoroutine(FadeCamera());


                }
                break;

            case Levels.Level2:
                if (objName.Equals(Passages.Left.ToString()))
                {
                    currentLevel = Levels.Level3;
                    StartCoroutine(FadeCamera());

                }
                else
                {
                    currentLevel = Levels.Level1;
                    StartCoroutine(FadeCamera());

                }
                break;

            case Levels.Level3:
                if (objName.Equals(Passages.Right.ToString()))
                {
                    currentLevel = Levels.Level4;
                    StartCoroutine(FadeCamera());

                }
                else
                {
                    currentLevel = Levels.Level1;
                    StartCoroutine(FadeCamera());

                }
                break;

            case Levels.Level4:
                if (objName.Equals(Passages.Left.ToString()))
                {
                    tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);

                }
                else
                {
                    currentLevel = Levels.Level1;
                    StartCoroutine(FadeCamera());

                    //ChangeLevel(currentLevel = Levels.Level1);
                }
                break;
        }
    }

    private void ChangeLevel()
    {

        foreach (Transform t in levels)
        {
            t.gameObject.SetActive(false);
        }

        switch (currentLevel)
        {
            case Levels.Level1:
                character.position = charPos;
                levels[0].gameObject.SetActive(true);        
                break;

            case Levels.Level2:
                character.position = charPos;
                levels[1].gameObject.SetActive(true);
                break;

            case Levels.Level3:
                character.position = charPos;
                levels[2].gameObject.SetActive(true);
                break;

            case Levels.Level4:
                character.position = charPos;
                levels[3].gameObject.SetActive(true);
                break;
        }

        

    }

    private IEnumerator FadeCamera()
    {
        float timer = 0;
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * CameraFadeSpeed;
            fader.GetComponent<RawImage>().color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 1), timer);
            if (fader.GetComponent<RawImage>().color == new Color(0,0,0,1))
            {
                break;
            }
        }

        yield return new WaitForSeconds(WaitDurationinFade);
        timer = 0;
        ChangeLevel();
        while (true)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime * CameraFadeSpeed;
            fader.GetComponent<RawImage>().color = Color.Lerp(new Color(0, 0, 0, 1), new Color(0, 0, 0, 0), timer);
            if (fader.GetComponent<RawImage>().color == new Color(0,0,0,0))
            {
                break;
            }
        }
    }

    
}
