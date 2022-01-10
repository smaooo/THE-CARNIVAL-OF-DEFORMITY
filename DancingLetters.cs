using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Rand = System.Random;
using UnityEngine.UI;


public class DancingLetters : MonoBehaviour
{
    public Sprite[] dashes;
    private Transform letterBox;
    private List<Transform> letterRow = new List<Transform>();
    private Transform[,] letters = new Transform[6,5];
    Dictionary<Transform, List<Transform>> pairs = new Dictionary<Transform, List<Transform>>();
    private int[] letterLoc = new int[] { 0, 0 };
    private string answer = "THEMERCILESSPLEASER";
    private List<Transform> selected = new List<Transform>();
    public Sprite letterBorder;
    public Sprite groupBorder;
    public Sprite cross;
    private Transform sentence;
    private Transform hangMan;
    private bool lost = false;
    public Sprite EndingLBorder;
    public Sprite EndingGBorder;
    private bool canMove = true;
    List<Transform> chars = new List<Transform>();
    private TentLogic tentLogic;
    private int lives = 2;
    public GameObject originalSentence;
    public GameObject originalLetterBox;
    private Transform screen;
    public GameObject WinLose;
    private int recursionIndex = 0;

    private void ResetGame()
    {
        letterRow = new List<Transform>();
        letters = new Transform[6, 5];
        letterLoc = new int[] { 0, 0 };
        selected = new List<Transform>();
        lost = false;
        canMove = true;
        chars = new List<Transform>();

        Destroy(letterBox.gameObject);
        Destroy(sentence.gameObject);

        sentence = Instantiate(originalSentence, transform).transform;
        sentence.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        sentence.GetComponent<RectTransform>().offsetMax = Vector2.zero;
        letterBox = Instantiate(originalLetterBox, transform).transform;
        letterBox.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        letterBox.GetComponent<RectTransform>().offsetMax= Vector2.zero;
        sentence.name = "Sentence";
        letterBox.name = "LetterBox";

        sentence.gameObject.SetActive(true);
        letterBox.gameObject.SetActive(true);

        for (int i = 0; i < hangMan.childCount; i++)
        {
            hangMan.GetChild(i).gameObject.SetActive(false);
        }
        for (int s = 0; s < sentence.childCount; s++)
        {
            chars.Add(sentence.GetChild(s));
        }
        RandomizeDashes();
        FillArray();

        Pairing();
        selected = Selection(KeyCode.None, letterLoc, new bool[] { false, false });

    }


    void Start()
    {
        letterBox = transform.Find("LetterBox");
        sentence = transform.Find("Sentence");
        hangMan = transform.Find("HangMan");
        screen = transform.Find("Screen");
        
        tentLogic = FindObjectOfType<TentLogic>();

        for (int s = 0; s < sentence.childCount; s++)
        {
            chars.Add(sentence.GetChild(s));
        }
        RandomizeDashes();
        FillArray();
        
        Pairing();
        selected = Selection(KeyCode.None, letterLoc, new bool[] { false, false});

        
    }

    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.O))
        //{
        //    tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);
        //}

        if (!lost && canMove)
        {

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                recursionIndex = 0;
                selected = Selection(KeyCode.UpArrow, letterLoc, new bool[] { false, false });
                
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            {
                recursionIndex = 0;
                selected = Selection(KeyCode.DownArrow, letterLoc, new bool[] { false, false });
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
            {
                recursionIndex = 0;
                selected = Selection(KeyCode.RightArrow, letterLoc, new bool[] { false, false });
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
            {
                recursionIndex = 0;
                selected = Selection(KeyCode.LeftArrow, letterLoc, new bool[] { false, false });
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Select(selected);
            }
        }
    }

    private void RandomizeDashes()
    {
        Rand rand = new Rand();
        for (int d = 0; d < sentence.childCount; d++)
        {
            int index = rand.Next(dashes.Length);
            sentence.GetChild(d).GetComponent<RawImage>().texture = dashes[index].texture;
        }
    }
    private void FillArray()
    {
        if (letterBox != null)
        {
            for (int c = 0; c < letterBox.childCount; c++)
            {
                letterRow.Add(letterBox.GetChild(c));
            }

        }

        int index = 0;
        bool breakLoop = false;
        for (int i = 0; i < 6; i++)
        {
            
            for (int j = 0; j < 5; j++)
            {
                
                
                if (i == 4 && j == 3)
                {
                    break;
                }
                if (i == 5 && j == 3)
                {
                    breakLoop = true;
                    break;
                }
                
                letters[i, j] = letterRow[j + index];
                        
            }
            if (breakLoop)
            {
                break;
            }
            if (i > 3)
            {
                index += 3;
            }
            else
            { 
            index += 5;
            }
        }


      
        
    }
    private void Pairing()
    {
        pairs.Clear();
        //pairs = new Dictionary<Transform, List<Transform>>();
        Rand rand = new Rand();
        for (int i = 0; i < letterRow.Count; i++)
        {
            List<Transform> tmp = new List<Transform>();
            int seed = rand.Next(0, 5);
            int max = 0;
            if (seed == 4)
            {
                max = 2;
            }
            else
            {
                max = 1;
            }
            //int max = rand.Next(2,3);
            int m = 0;
            while (m < max)
            {
                int index = rand.Next(letterRow.Count);
                if (letterRow[index] != letterRow[i])
                {
                    tmp.Add(letterRow[index]);
                    m++;

                }
            }
            pairs.Add(letterRow[i], tmp);
           
        }
        
    }

    private List<Transform> Selection(KeyCode key, int[] prev, bool[] rev)
    {
        if (!rev.Equals(new bool[] { false, false }))
        {
            recursionIndex++;
        }

        
        //int[] prev = letterLoc;

        //for (int i = 0; i < letterBox.childCount; i++)
        //{
        //    Transform t = letterBox.GetChild(i);
        //    for (int j = 0; j < t.childCount; j++)
        //    {
        //        if (t.GetChild(j).name == "LBorder(Clone)")
        //        {
        //            Destroy(t.GetChild(j).gameObject);
        //        }
        //    }
        //}
        foreach (KeyValuePair<Transform, List<Transform>> kvp in pairs)
        {
            for (int i = 0; i < kvp.Key.childCount; i++)
            {
                Destroy(kvp.Key.GetChild(i).gameObject);
            }

            for (int i = 0; i < kvp.Value.Count; i++)
            {
                for (int j = 0; j < kvp.Value[i].childCount; j++)
                {
                    Destroy(kvp.Value[i].GetChild(j).gameObject);
                }
            }
        }

       
        if (key == KeyCode.RightArrow)
        {
            
            if (letterLoc[1] == 4)
            {
                letterLoc[1] = 0;
            }
            else
            {
                
                letterLoc[1]++;
            }

            if (letterLoc[0] > 3 && letterLoc[1] == 3)
            {
                
                letterLoc[1] = 0;
            }
           
        }

        else if (key == KeyCode.LeftArrow)
        {
           
            if (letterLoc[1] == 0)
            {
                if (letterLoc[0] > 3)
                {
                    
                    letterLoc[1] = 2;
                }
                else
                {
                    letterLoc[1] = 4;

                }
            }
            else
            {
                letterLoc[1]--;
            }
            
           
        }
        else if (key == KeyCode.UpArrow)
        {
           
            if (letterLoc[0] == 0)
            {
                letterLoc[0] = 5;
            }
            else
            {
                letterLoc[0]--;
            }

            if (letterLoc[1] > 2 && letterLoc[0] == 5)
            {
                letterLoc[0] = 3;
            }
            
        }
        else if (key == KeyCode.DownArrow)
        {

            
            if (letterLoc[0] == 5)
            {
                letterLoc[0] = 0;
            }
            else
            {
                letterLoc[0]++;
            }
            if (letterLoc[1] > 2 && letterLoc[0] == 4)
            {
                letterLoc[0] = 0;
            }
        }
     
        else if (key == KeyCode.ScrollLock)
        {
            bool[,] vis = new bool[letters.GetLength(0), letters.GetLength(1)];
            Queue<int[]> q = new Queue<int[]>();
            int[] drow;
            int[] dcol;
            if (rev[0])
            {
                drow = new int[] { 0, -1, -2, -3, 1, 2};

            }
            else
            {
                drow = new int[] { 0, 1, 2, 3, -1, -2};

            }
            if (rev[1])
            {
                dcol = new int[] {  0, -1, -2, 1, 2};

            }
            else
            {
                dcol = new int[] {  0, 1, 2, -1, -2};

            }

            q.Enqueue(new int[] { letterLoc[0], letterLoc[1] });
            vis[letterLoc[0], letterLoc[1]] = true;
            bool breakOut = false;

            while (q.Count != 0)
            {

                int[] cell = q.Peek();
                int x = cell[0];
                int y = cell[1];
                q.Dequeue();

                for (int i = 0; i < 6; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {

                        int adjx = x + drow[i];
                        int adjy = y + dcol[j];

                        if ((adjx > 0 && adjx < letters.GetLength(0)) &&
                            (adjy > 0 && adjy < letters.GetLength(1)) &&
                            letters[adjx, adjy] != null && letters[adjx,adjy].Find("Cross(Clone)") == null &&
                            !(new int[] { adjx, adjy }.Equals(prev)))
                        {
                            letterLoc[0] = adjx;
                            letterLoc[1] = adjy;
                            breakOut = true;
                            break;
                        }
                    }
                    if(breakOut)
                    {
                        break;
                    }
                }
                if (breakOut)
                {
                    break;
                }
            }
        }
        
        Transform tmp;
        tmp = letters[letterLoc[0], letterLoc[1]];

        if (recursionIndex > 3)
        {
            for (int j = 0; j < letters.GetLength(0); j++)
            {
                for (int k = 0; k < letters.GetLength(1); k++)
                {
                    if (j > 3 && k > 2)
                    {
                        break;
                    }
                    if (letters[j, k].Find("Cross(Clone)") == null)
                    {
                        letterLoc[0] = j;
                        letterLoc[1] = k;
                        tmp = letters[j, k];
                        break;
                    }
                }
            }
        }
        if (tmp == null)
        {
            selected = Selection(key, prev, key == KeyCode.DownArrow ? new bool[] { false, false } : new bool[] { false, true });
            return selected;
        }
         
        if (tmp.Find("Cross(Clone)") != null)
        {
            if (key == KeyCode.UpArrow || key == KeyCode.DownArrow)
            {
                for (int i = 0; i < letters.GetLength(0); i++)
                {
                        
                    Transform t = letters[i, letterLoc[1]];
                    if (t != tmp && t.Find("Cross(Clone)") == null && letterLoc[0] != i)
                    {
                        selected = Selection(key, prev, key == KeyCode.DownArrow ? new bool[] { false, false } : new bool[] { false, true });
                        return selected;
                            
                    }
                    else if (Math.Abs(letterLoc[0] - i) > 3)
                    {
                        selected = Selection(KeyCode.ScrollLock, prev, key == KeyCode.DownArrow ? new bool[] { false, false } : new bool[] { false, true });
                        return selected;

                    }
                }
            }
               
            else if (key == KeyCode.RightArrow || key == KeyCode.LeftArrow)
            {
                for (int i = 0; i < letters.GetLength(1); i++)
                {
                    Transform t = letters[letterLoc[0], i];
                    print(tmp);
                    if (t != tmp && t.Find("Cross(Clone)") == null && letterLoc[1] != i)
                    {
                        try
                        {
                            selected = Selection(key, prev, key == KeyCode.RightArrow ? new bool[] { false, false } : new bool[] { true, false});
                            return selected;

                        }
                        catch (StackOverflowException)
                        {
                            for (int j = 0; j < letters.GetLength(0); j++)
                            {
                                for (int k = 0; k < letters.GetLength(1); k++)
                                {
                                    if (letters[j,k].Find("Cross(Clone)") == null)
                                    {
                                        tmp = letters[j, k];
                                        break;
                                    }
                                }
                            }
                        }
                            
                    }
                    else if (Math.Abs(letterLoc[1] - i) > 3)
                    {
                                
                            selected = Selection(KeyCode.ScrollLock, prev, key == KeyCode.RightArrow ? new bool[] { false, false } : new bool[] { true, false});
                                
                                
                            return selected;
                    }
                }
            }
                

        }
        
        
        
        AddObject(letterBorder, "LBorder", tmp);
        List<Transform> group = new List<Transform>();
        List<Transform> tmpGroup = new List<Transform>();
        //tmpGroup = pairs[tmp];
        if (pairs.TryGetValue(tmp, out tmpGroup))
        {
            tmpGroup = pairs[tmp];
        }
        else
        {
            print(tmp);
        }
        //try
        //{

        //}
        //catch (KeyNotFoundException)
        //{
        //    //selected = Selection(KeyCode.LeftArrow, prev, key == KeyCode.LeftArrow ? new bool[] { false, false } : new bool[] { false, true});
        //    tmpGroup = FindSpot();
        //    //return selected;
        //}
        group.Add(tmp);
        foreach (Transform t in tmpGroup)
        {
            
            AddObject(groupBorder, "GBorder", t);
            if (t.Find("GBorder(Clone)") == null)
            {
                

            }
          
            if (t.childCount == 0)
            {
            }
            if (!group.Contains(t))
            {
                group.Add(t);

            }
        }

        return group;
    }

    private void Select(List<Transform> sel)
    {

        
        int index = 0;

        Rand ran = new Rand();
        index = ran.Next(sel.Count);
        Transform let = sel[index];
        string ch = let.name;
        List<int> locs = new List<int>();
        
        int strIndex = 0;
        bool first = true;
        do
        {

            strIndex = answer.IndexOf(ch, strIndex);
            if (strIndex == -1)
            {
                if (first)
                {
                    first = false;

                    for (int h = 0; h < hangMan.childCount; h++)
                    {
                        if (!hangMan.GetChild(h).gameObject.activeSelf)
                        {
                            hangMan.GetChild(h).gameObject.SetActive(true);
                            if (h == hangMan.childCount - 1)
                            {
                                lost = true;
                                if (lives == 0)
                                {

                                    StartCoroutine(WinLoseSign("Lose"));


                                }
                                else
                                {
                                    screen.GetChild(lives).gameObject.SetActive(false);
                                    lives--;
                                    StartCoroutine(PrepareReset());
                                    //return;
                                }
                            }
                            break;
                        }
                    }
                }
                if (let.childCount > 0)
                {
                    for (int c = 0; c < let.childCount; c++)
                    {
                        StartCoroutine(LetterCrossing(let.GetChild(c)));

                    }
                }
                letterRow.Remove(let);
                
                // CHECK HERE LATER
                if (let.Equals(sel[0]))
                {
                    StartCoroutine(Waiter("Other"));
                    if (CheckSentence())
                    {
                        canMove = false;
                        
                        StartCoroutine(WinLoseSign("Win"));
                        
                    }
                    return;
                }
                
                break;
            }
            else
            {

                first = false;
                locs.Add(strIndex);

                if (let.childCount > 0)
                {
                    for (int c = 0; c < let.childCount; c++)
                    {
                        StartCoroutine(LetterCrossing(let.GetChild(c)));
                        
                    }
                }

                foreach (Transform t in chars)
                {
                    if (let.name == t.name)
                    {
                        t.GetComponent<RawImage>().texture = let.GetComponent<RawImage>().texture;
                    }
                }
                
                letterRow.Remove(let);
                strIndex++;
                
            }
        }
        while (strIndex != -1);

        StartCoroutine(Waiter("Exact"));
        
        if (CheckSentence())
        {
            canMove = false;
            StartCoroutine(WinLoseSign("Win"));
            return;
        }

        
        
    }

    private List<Transform> FindSpot()
    {
        List<Transform> tmpGroup = new List<Transform>();
        for (int j = 0; j < letters.GetLength(0); j++)
        {
            for (int k = 0; k < letters.GetLength(1); k++)
            {
                if (j > 3 && k > 2)
                {
                    break;
                }
                if (letters[j, k].Find("Cross(Clone)") == null)
                {
                    letterLoc[0] = j;
                    letterLoc[1] = k;
                    if (pairs.TryGetValue(letters[j,k], out tmpGroup))
                    {

                        tmpGroup = pairs[letters[j, k]];
                    }

                    

                    break;
                }
            }
        }
        return tmpGroup;
    }
    private GameObject AddObject(Sprite sprite, string name, Transform parent)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        obj.AddComponent<RectTransform>();
        obj.AddComponent<RawImage>();
        obj.GetComponent<RawImage>().texture = sprite.texture;
        obj.GetComponent<RawImage>().SetNativeSize();
        Instantiate(obj, parent);
        Destroy(obj);
        return obj;
    }

    private IEnumerator LetterCrossing(Transform l)
    {
        yield return null;
        canMove = false;
        Transform parent = l.parent;
        if (l.name == "LBorder(Clone)")
        {
            l.GetComponent<RawImage>().texture = EndingLBorder.texture;

            yield return new WaitForSeconds(0.5f);

            AddObject(cross, "Cross", parent);
            Destroy(l.gameObject);

        }
        else if (l.name == "GBorder(Clone)")
        {
            l.GetComponent<RawImage>().texture = EndingGBorder.texture;

            yield return new WaitForSeconds(0.5f);

            AddObject(cross, "Cross", parent);
            Destroy(l.gameObject);

        }
        canMove = true;
    }

    private IEnumerator Waiter(string state)
    {
        yield return new WaitForSeconds(0.5f);

        switch (state)
        {
            case "Exact":

                Pairing();
                selected = Selection(KeyCode.None, letterLoc, new bool[] { false, false });
                break;

            case "Other":

                Pairing();
                try
                {
                    selected = Selection(KeyCode.RightArrow, letterLoc, new bool[] { false, false });

                }
                catch (NullReferenceException)
                {
                    selected = Selection(KeyCode.ScrollLock, letterLoc, new bool[] { false, false });



                }
                catch (KeyNotFoundException)
                {
                    selected = Selection(KeyCode.ScrollLock, letterLoc, new bool[] { false, false });
                }

                break;
        }
    }

    private bool CheckSentence()
    {
        foreach(Transform t in chars)
        {
            if (t.name != t.GetComponent<RawImage>().texture.name)
            {
                return false;
            }

            
            
        }

        return true;
    }

    private IEnumerator PrepareReset()
    {
        yield return new WaitForSeconds(1f);
        ResetGame();
    }

    private IEnumerator WinLoseSign(string state)
    {
        switch(state)
        {
            case "Win":
                WinLose.GetComponent<Animator>().SetTrigger("Win");
                yield return new WaitForSeconds(WinLose.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                yield return new WaitForSeconds(0.2f);

                //WinLose.GetComponent<Animator>().ResetTrigger("Win");
                WinLose.GetComponent<Animator>().SetTrigger("Back");
                tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);
                break;

            case "Lose":
                WinLose.GetComponent<Animator>().SetTrigger("Lose");
                yield return new WaitForSeconds(WinLose.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
                yield return new WaitForSeconds(0.2f);

                //WinLose.GetComponent<Animator>().ResetTrigger("Lose");
                WinLose.GetComponent<Animator>().SetTrigger("Back");
                tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Lose);
                break;
        }
    }
}

