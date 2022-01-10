using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
public class SceneDialogueSystem : MonoBehaviour
{

    public enum Convo { Provider, ChildTeacher, CTFailure, CTFinish, CTSuccess, CutScene, GuardExit, InitialGuard, InterveneY, PasswordCorrect, Ending1, Ending2, Ending3, Ending4, None}
    public Conversations ProviderConvo;
    public Conversations ChildTeacherConvo;
    public Conversations CTFailureConvo;
    public Conversations CTFinishConvo;
    public Conversations CTSuccessConvo;
    public Conversations CutSceneConvo;
    public Conversations GuardExitConvo;
    public Conversations InitialGuardConvo;
    public Conversations InterveneYConvo;
    public Conversations PasswordCorrect;
    public Conversations Ending1Convo;
    public Conversations Ending2Convo;
    public Conversations Ending3Convo;
    public Conversations Ending4Convo;

    public GameObject Child;
    public GameObject Teacher;
    public GameObject Provider;
    public GameObject Guard;
    
    private HashSet<GameObject> participants = new HashSet<GameObject>(); 
    
    public GameObject canvas;
    private TextMeshProUGUI canvasText;
    public bool running = false;
    public int lineIndex = 0;
    private List<Sprite> sprites = new List<Sprite>();
    private List<string> lines = new List<string>();
    private MainSceneCharacter character;
    private string curLine = "";
    private bool printing = false;
    private bool stop = false;
    private TypeWriter typeWriter;

    private void Start()
    {
        character = FindObjectOfType<MainSceneCharacter>();
        canvasText = canvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        typeWriter = FindObjectOfType<TypeWriter>();
    }

    private void Update()
    {
        if (running)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (printing)
                {
                    StopCoroutine(RunConversation(Convo.None));
                    StopCoroutine(PrintLetters(""));
                    canvasText.text = curLine;
                    //lineIndex++;
                    printing = false;
                    //typing = false;
                    stop = true;

                }
                else if (running && !printing && lineIndex < lines.Count && running)
                {
                
                    StartCoroutine(RunConversation(Convo.None));

                }
                else
                {

                    running = false;
                    canvas.SetActive(false);
                }
            }

        }
    }
    public IEnumerator RunConversation(Convo convo)
    {
        
        if (lineIndex == lines.Count && running)
        {
            running = false;
            canvas.SetActive(false);
            yield break;
        }
        if (!running)
        {
            sprites.Clear();
            lines.Clear();
            participants.Clear();
            running = true;
            canvas.SetActive(true);
            lineIndex = 0;

            Tuple<List<string>, List<Sprite>> tmp = SelectDialogue(convo);
            lines = tmp.Item1;
            sprites = tmp.Item2;

            if (convo == Convo.ChildTeacher) 
            {
                StartCoroutine(TeacherChildArgGuardthing());
            }
            
            
        }
        
        //typing = true;
        if (sprites[lineIndex] != null)
        {
            
            ChangeSprite(sprites[lineIndex]);
        }
        
        else
        {
            foreach (GameObject obj in participants)
            {
                obj.GetComponent<SpriteRenderer>().color = new Color32(52, 52, 52, 255);
                
            }
        }
        printing = true;
        
        yield return StartCoroutine(PrintLetters(curLine = lines[lineIndex]));
        printing = false;
        //typing = false;
        stop = false;
        lineIndex++;



    }

    private IEnumerator TeacherChildArgGuardthing() 
    {
        yield return new WaitWhile(() => lineIndex < 12);
        Guard.GetComponent<Animator>().Play("GuardTeacherCA");
    }
    public Tuple<List<string>, List<Sprite>> SelectDialogue(Convo convo)
    {
        List<Sprite> sprites = new List<Sprite>();
        List<string> lines = new List<string>();

        switch (convo)
        {
            case Convo.ChildTeacher:
                foreach (Line line in ChildTeacherConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Teacher);
                participants.Add(Child);
                participants.Add(Guard);
                break;

            case Convo.CTFailure:
                foreach (Line line in CTFailureConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Teacher);
                break;

            case Convo.CTFinish:
                foreach (Line line in CTFinishConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Teacher);
                break;

            case Convo.CTSuccess:
                foreach (Line line in CTSuccessConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Teacher);
                break;

            case Convo.CutScene:
                foreach (Line line in CutSceneConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }

                break;

            case Convo.GuardExit:
                foreach (Line line in GuardExitConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;

            case Convo.InitialGuard:
                foreach (Line line in InitialGuardConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;

            case Convo.InterveneY:
                foreach (Line line in InterveneYConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Teacher);
                break;

            case Convo.Provider:
                foreach (Line line in ProviderConvo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);

                }
                participants.Add(Provider);
                break;

            case Convo.PasswordCorrect:
                foreach (Line line in PasswordCorrect.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;
            case Convo.Ending1:
                foreach (Line line in Ending1Convo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;

            case Convo.Ending2:
                foreach (Line line in Ending2Convo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;

            case Convo.Ending3:
                foreach (Line line in Ending3Convo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;

            case Convo.Ending4:
                foreach (Line line in Ending4Convo.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                participants.Add(Guard);
                break;
        }

       
        
        return Tuple.Create(lines, sprites);
    }


    private IEnumerator PrintLetters(string text)
    {
        canvasText.text = "";

        foreach (char c in text)
        {
            if (stop)
            {
                canvasText.text = text;
                break;
            }
            else
            {
                string tmp = canvasText.text;
                tmp = tmp + c.ToString();
                if (tmp.Length % 2 == 0)
                {
                    if (c != ' ')
                    {
                        typeWriter.PlaySound();

                    }

                }
                canvasText.text = tmp;
                yield return new WaitForSeconds(0.05f);

            }

        }
    }
    public void ChangeSprite(Sprite sprite)
    {
        if (sprite.name.Contains("CHILD"))
        {
            if (Child.GetComponent<Animator>().enabled)
            {
                Child.GetComponent<Animator>().enabled = false;
            }
            Child.GetComponent<SpriteRenderer>().sprite = sprite;
            Child.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            foreach (GameObject obj in participants)
            {
                if (obj != Child)
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color32(52, 52, 52, 255);
                }
            }
        }
        else if (sprite.name.Contains("TEACHER"))
        {
            if (Teacher.GetComponent<Animator>().enabled)
            {
                Teacher.GetComponent<Animator>().enabled = false;
            }
            Teacher.GetComponent<SpriteRenderer>().sprite = sprite;
            Teacher.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            foreach (GameObject obj in participants)
            {
                if (obj != Teacher)
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color32(52, 52, 52, 255);
                }
            }
        }

        else if (sprite.name.Contains("GUARD"))
        {
            if (Guard.GetComponent<Animator>().enabled)
            {
                //Guard.GetComponent<Animator>().enabled = false;
                MainSceneLayers SceneLayers = FindObjectOfType<MainSceneLayers>();
                //SceneLayers.GuardPlaying = true;

                StartCoroutine(GuardDisableHotFix());
            }
            Guard.GetComponent<SpriteRenderer>().sprite = sprite;
            Guard.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            foreach (GameObject obj in participants)
            {
                if (obj != Guard)
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color32(52, 52, 52, 255);
                }
            }
        }

        else if (sprite.name.Contains("PROVIDER"))
        {
            if (Provider.GetComponent<Animator>().enabled)
            {
                Provider.GetComponent<Animator>().enabled = false;
            }
            Provider.GetComponent<SpriteRenderer>().sprite = sprite;
            Provider.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            foreach (GameObject obj in participants)
            {
                if (obj != Provider)
                {
                    obj.GetComponent<SpriteRenderer>().color = new Color32(52, 52, 52, 255);
                }
            }
        }
    }

    private IEnumerator GuardDisableHotFix() 
    {
        yield return new WaitForSeconds(1f);
        //Guard.GetComponent<Animator>().enabled = false;

        MainSceneLayers SceneLayers = FindObjectOfType<MainSceneLayers>();
        //SceneLayers.GuardPlaying = false;
    }
}
