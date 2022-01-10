using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TentDialogueSystem : MonoBehaviour
{

    public Conversations Watcher;
    public Conversations Reader;
    public Conversations Know;
    public Conversations Catcher;
    public Conversations Betrayed;
    public Conversations Empty;

    public GameObject canvas;
    private TextMeshProUGUI canvasText;
    public bool running = false;
    private int lineIndex = 0;
    private List<Sprite> sprites = new List<Sprite>();
    private List<string> lines = new List<string>();
    private TentLogic tentLogic;
    private string curLine = "";
    private bool printing = false;
    private bool stop = false;
    private TypeWriter typeWriter;
    private void Start()
    {
        tentLogic = FindObjectOfType<TentLogic>();
        canvasText = canvas.transform.Find("Text").GetComponent<TextMeshProUGUI>();
        typeWriter = FindObjectOfType<TypeWriter>();
    }

    private void Update()
    {
        if (printing)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                //StopCoroutine(RunConversation(TentLogic.MGS.None));
                //StopCoroutine(PrintLetters(""));
                //canvasText.text = curLine;
                //lineIndex++;
                //printing = false;
                //tentLogic.typing = false;
                stop = true;

            }
        }
    }
    public IEnumerator RunConversation(TentLogic.MGS mgState)
    {
        if (lineIndex == lines.Count && running)
        {
            tentLogic.inDialogue = false;
            running = false;
            canvas.SetActive(false);
            yield break;
        }
        if (!running)
        {
            sprites.Clear();
            lines.Clear();

            running = true;
            canvas.SetActive(true);
            lineIndex = 0;
            if (tentLogic.GameStates[mgState])
            {
                Tuple<List<string>, List<Sprite>> tmp = SelectDialogue(mgState);
                lines = tmp.Item1;
                sprites = tmp.Item2;

            }
            else
            {

                lines.Add("...");
                sprites.Add(null);
            }
        }
        tentLogic.typing = true;
        if (sprites[lineIndex] != null)
        {
            tentLogic.ChangeSprite(sprites[lineIndex]);
        }
        else
        {
            if (tentLogic.GameStates[mgState])
            {
                tentLogic.BlackSprite();

            }
        }
        printing = true;
        yield return StartCoroutine(PrintLetters(curLine = lines[lineIndex]));
        printing = false;
        tentLogic.typing = false;
        stop = false;
        lineIndex++;
        

        
    } 

    
    public Tuple<List<string>, List<Sprite>> SelectDialogue(TentLogic.MGS mgState)
    {
        List<Sprite> sprites = new List<Sprite>();
        List<string> lines = new List<string>();
        
        switch (mgState)
        {
            case TentLogic.MGS.MG1:
                foreach(Line line in Reader.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                    
                }
                break;

            case TentLogic.MGS.MG2:
                foreach (Line line in Watcher.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);
                }
                break;

            case TentLogic.MGS.MG3:
                foreach (Line line in Catcher.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);

                }
                break;

            case TentLogic.MGS.MG4:
                foreach (Line line in Know.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);

                }
                break;

            case TentLogic.MGS.MG5:
                foreach (Line line in Betrayed.lines)
                {
                    lines.Add(line.text);
                    sprites.Add(line.sprite);

                }
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
    //private void ChangeCanvasTarget(TentLogic.MGS mgState)
    //{
    //    switch (mgState)
    //    {
    //        case TentLogic.MGS.MG1:
    //            canvas.GetComponent<Canvas>().rend
    //            break;

    //        case TentLogic.MGS.MG2:

    //            break;

    //        case TentLogic.MGS.MG3:

    //            break;

    //        case TentLogic.MGS.MG4:

    //            break;

    //        case TentLogic.MGS.MG5:

    //            break;
    //    }
    //}
}
