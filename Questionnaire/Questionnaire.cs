using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Rand = System.Random;
using UnityEngine.UI;
public class Questionnaire : MonoBehaviour
{
    private enum Objects { Questions, Border, Text, Answer, Child}
    public GameObject bigBorder;
    public GameObject smallBorder;
    private GameObject border;
    private Transform[] questions = new Transform[3];
    private Transform answer;
    private Dictionary<string, Dictionary<string, int>> AnswerPoints = new Dictionary<string, Dictionary<string, int>>();
    private int selLoc = 0;
    public Questions questionPack;
    private int questionIndex = 0;
    private bool canMove = true;
    public int Score = 0;
    private Vector3[] originalPos = new Vector3[3];
    private Vector3[] thirdLocOp = new Vector3[2];
    private Animator childAnim;
    private bool thirdLeft = false;
    public GameObject Self;
    public GameObject Player;

    void Start()
    {
        
        
        for (int i = 0; i < questions.Length; i++)
        {
            
            questions[i] = transform.Find(Objects.Questions.ToString()).GetChild(i);
        }
        

        answer = transform.Find(Objects.Answer.ToString());
        childAnim = transform.Find(Objects.Child.ToString()).GetComponent<Animator>();
        
        for (int i = 0; i < questions.Length; i++)
        {
            originalPos[i] = questions[i].localPosition;
        }

        thirdLocOp = new Vector3[] {questions[2].localPosition,
                                    new Vector3(-questions[2].localPosition.x, questions[2].localPosition.y, questions[2].localPosition.z)};
        AnswerPoints = ChangeQuestions(questionIndex);

        MoveCursor();
    }


    void Update()
    {
        
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)|| Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveCursor();     

            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) {
                if (questionIndex < ((questions.Length * 4)+5) ) 
                {
                    StartCoroutine(Select());
                }

                if (questionIndex >= ((questions.Length * 4) + 5)) 
                {
                    

                }
            }
        }
        
    }

    private void MoveCursor()
    {
        foreach (Transform t in questions)
        {
            if (t.Find(Objects.Border.ToString()) != null)
            {
                Destroy(t.Find(Objects.Border.ToString()).gameObject);
            }
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            selLoc++;
            childAnim.SetTrigger("GoBack");

        }
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            selLoc--;
            childAnim.SetTrigger("GoBack");

        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (thirdLeft)
            {
                selLoc -= 2;

            }
            else
            {
                if (selLoc == 2)
                {
                    selLoc = 1;
                }
                else
                {
                    selLoc = 2;
                }
            }
            childAnim.SetTrigger("GoBack");
            
        }
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (thirdLeft)
            {
                selLoc += 2;

            }
            else
            {
                if (selLoc == 2)
                {
                    selLoc = 1;
                }
                else
                {
                    selLoc = 2;
                }
            }
            childAnim.SetTrigger("GoBack");
        }

        if (selLoc > 2)
        {
            selLoc = 0;
        }

        else if (selLoc < 0)
        {
            selLoc = 2;
        }
        Instantiate(border, questions[selLoc]).name = Objects.Border.ToString();

        childAnim.SetFloat("X", Mathf.Clamp(questions[selLoc].GetComponent<RectTransform>().localPosition.x, -1,1));
        childAnim.SetFloat("Y", Mathf.Clamp(questions[selLoc].GetComponent<RectTransform>().localPosition.y,-1,1));
    }

    private Dictionary<string, Dictionary<string, int>> ChangeQuestions(int index)
    {
        Dictionary<string, Dictionary<string, int>> curQues = new Dictionary<string, Dictionary<string, int>>();
        Rand rand = new Rand();
        if (index < questionPack.question.Length)
        {
            Questions.Question qu = questionPack.question[index];

            curQues.Add(qu.FirstQuestion, new Dictionary<string, int> { { "answer", qu.FirtsAnswer ? 1 : 0 }, { "point", qu.FirstPoint } });
            curQues.Add(qu.SecondQuestion, new Dictionary<string, int> { { "answer", qu.SecondAnswer? 1 : 0 }, { "point", qu.SecondPoint } });
            curQues.Add(qu.ThirdQuestion, new Dictionary<string, int> { { "answer", qu.ThirdAnswer? 1 : 0 }, { "point", qu.ThirdPoint} });
            border = smallBorder;
            for (int i = 0; i < questions.Length; i++)
            {
                if (i < questions.Length - 1)
                {
                    questions[i].localPosition = new Vector3(Random.Range(originalPos[i].x - 30, originalPos[i].x + 30),
                                                        Random.Range(originalPos[i].y - 40, originalPos[i].y + 40), questions[i].position.z);

                }

                else
                {
                    int j = rand.Next(thirdLocOp.Length);
                    questions[i].localPosition = new Vector3(Random.Range(thirdLocOp[j].x - 30, thirdLocOp[j].x + 30),
                                                        Random.Range(thirdLocOp[j].y - 40, thirdLocOp[j].y + 40), thirdLocOp[j].z);
                    thirdLeft = j == 0 ? true : false;
                }
            }

            List<Transform> tmp = new List<Transform>();
            tmp = questions.ToList();
            tmp = tmp.OrderBy(x => rand.Next()).ToList();
            for (int i = 0; i < questions.Length; i++)
            {
                
                tmp[i].Find(Objects.Text.ToString()).GetComponent<TextMeshProUGUI>().text = curQues.Keys.ToList()[i];
                
            }

        }

        else if (index == questionPack.question.Length)
        {
            for (int i = 1; i < questions.Length; i++)
            {
                questions[i].gameObject.SetActive(false);
            }

            questions[0].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -239f);
            questions[0].GetComponent<RectTransform>().sizeDelta = new Vector2(848.941f, 227.372f);
            questions[0].GetComponent<RawImage>().color = new Color32(0, 0, 0, 236);
            questions[0].Find("Text").GetComponent<TextMeshProUGUI>().enableAutoSizing = false;
            questions[0].Find("Text").GetComponent<TextMeshProUGUI>().fontSize = 45;
            questions[0].Find("Text").GetComponent<RectTransform>().sizeDelta = new Vector2(848.94f, 200);
            //border.GetComponent<RectTransform>().anchoredPosition = new Vector2(-14.0547f, 234f);
            //border.GetComponent<RectTransform>().sizeDelta = new Vector2(1143.68f, 768f);
            border = bigBorder;



            Questions.LastQuestion qu = questionPack.lastQuestion;
            
            curQues.Add(qu.Question, new Dictionary<string, int> { { "answer", qu.Answer ? 1 : 0 }, { "point", qu.Point } });

            questions[0].Find(Objects.Text.ToString()).GetComponent<TextMeshProUGUI>().text = curQues.Keys.ToList()[0];
        }

        else
        {
            //tell player what the score is
            Player.GetComponent<MainSceneCharacter>().MG0Score = Score;
            // tell player to set this to invactive
            Player.GetComponent<MainSceneCharacter>().MiniGame0Playing = false;
        }




        return curQues;
    }

    private IEnumerator Select()
    {
        childAnim.SetTrigger("GoBack");
        childAnim.SetFloat("X", 0);
        childAnim.SetFloat("Y", 0);
        canMove = false;
        Dictionary<string, int> curAnsPoint = AnswerPoints[questions[selLoc].Find(Objects.Text.ToString()).GetComponent<TextMeshProUGUI>().text];

        answer.gameObject.SetActive(true);

        

        //answer.GetComponent<TextMeshProUGUI>().text = curAnsPoint["answer"] == 1 ? "TRUE" : "FALSE";
        yield return StartCoroutine(ShowAnswer(curAnsPoint["answer"]));
        Score += curAnsPoint["point"];

        yield return new WaitForSeconds(0.5f);

        answer.gameObject.SetActive(false);
        questionIndex++;
        AnswerPoints = ChangeQuestions(questionIndex);
        

        canMove = true;
        selLoc = 0;
        MoveCursor();

    }

    private IEnumerator ShowAnswer(int state)
    {
        Animator anim = answer.GetComponent<Animator>();

        switch (state)
        {
            case 1:
                anim.SetTrigger("true");
                break;

            case 0:
                anim.SetTrigger("false");
                break;
        }

        yield return new WaitForSeconds(10/60f);
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
        anim.SetTrigger("Play");

        yield return new WaitForSeconds(0.5f);

        anim.SetTrigger("Back");

        anim.ResetTrigger("true");
        anim.ResetTrigger("false");
        anim.ResetTrigger("Back");
        anim.ResetTrigger("Play");
    }
}
