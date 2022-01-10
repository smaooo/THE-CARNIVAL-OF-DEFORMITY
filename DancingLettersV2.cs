using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using Rand = System.Random;
using System.Linq;

public class DancingLettersV2 : MonoBehaviour
{
    [System.Serializable]
    public struct Row
    {
        public Transform[] letters;
    }

    private enum Objects { Sentence, HangMan, LetterBorder, GroupBorder, Cross, EndingLEtterBorder, EndingGroupBorder, Screen}

    private string answer = "THEMERCILESSPLEASER";
    private Transform[,] originalLetters = new Transform[6,5];
    private Transform[,] letters = new Transform[6, 5];
    public List<Transform> Letters1D = new List<Transform>();
    public Row[] rows;
    private Transform Sentence;
    private Transform HangMan;
    private Transform Screen;
    public Texture LetterBorder;
    public Texture GroupBorder;
    public Texture Cross;
    public Texture EndingLetterBorder;
    public Texture EndingGroupBorde;
    public Texture[] Dashes;
    private Dictionary<Transform, List<Transform>> pairs = new Dictionary<Transform, List<Transform>>();
    private Rand rand = new Rand();
    private int[] location = new int[2] { 0, 0 };
    private int lives = 2;
    private bool canMove = true;
    public GameObject WinLose;
    private int[] prevLoc = new int[2];
    private int recursion = 0;
    private TentLogic tentLogic;
    void Start()
    {
        tentLogic = FindObjectOfType<TentLogic>();
        Sentence = transform.Find(Objects.Sentence.ToString());
        HangMan = transform.Find(Objects.HangMan.ToString());
        Screen = transform.Find(Objects.Screen.ToString());

        originalLetters = FillArray();
        letters = originalLetters.Clone() as Transform[,];

        RandomizeDashes();
        
        pairs = Pairing();

        location.CopyTo(prevLoc, 0);
        MoveCursor(0,0);
    
        

    }


    void Update()
    {
        if (canMove)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                
                location.CopyTo(prevLoc, 0);
                recursion = 0;
                MoveCursor(-1, 0);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                location.CopyTo(prevLoc, 0);
                recursion = 0;
                MoveCursor(0, 1);
            }
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            {
                location.CopyTo(prevLoc, 0);
                recursion = 0;
                MoveCursor(1, 0);
            }

            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                location.CopyTo(prevLoc, 0);
                recursion = 0;
                MoveCursor(0, -1);
            }
            else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                Select();
            }

        }
    }

    private Transform[,] FillArray()
    {
        Transform[,] array = new Transform[letters.GetLength(0), letters.GetLength(1)];

        for (int i = 0; i < rows.Length; i++)
        {
            for (int j = 0; j < rows[i].letters.Length; j++)
            {
                array[i, j] = rows[i].letters[j];
            }
        }

        return array;
    }

    private void CreateObject(Texture texture, Transform parent, string name)
    {
        GameObject obj = new GameObject();
        obj.name = name;
        obj.AddComponent<RectTransform>();
        obj.AddComponent<RawImage>();
        obj.GetComponent<RawImage>().texture = texture;
        obj.GetComponent<RawImage>().SetNativeSize();
        obj.transform.SetParent(parent);
        obj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        obj.GetComponent<RectTransform>().localPosition = Vector3.zero;
        obj.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;
        obj.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
    }

    private void RandomizeDashes()
    {
        Rand rand = new Rand();
        for (int d = 0; d < Sentence.childCount; d++)
        {
            int index = rand.Next(Dashes.Length);
            Sentence.GetChild(d).GetComponent<RawImage>().texture = Dashes[index];
        }
    }

    private Dictionary<Transform, List<Transform>> Pairing()
    {
        Dictionary<Transform, List<Transform>> dict = new Dictionary<Transform, List<Transform>>();

        for (int i = 0; i < letters.GetLength(0); i++)
        {
            for (int j = 0; j < letters.GetLength(1); j++)
            {
                if (letters[i,j] != null)
                {
                    
                    List<Transform> tmp = new List<Transform>();
                    int seed = rand.Next(0, 5);
                    int max = 0;
                    max = seed == 4 ? 2 : 1;
                    int m = 0;
                    while (m < max)
                    {
                        int index0;
                        int index1;
                        do
                        {
                            index0 = rand.Next(letters.GetLength(0));
                            index1 = rand.Next(letters.GetLength(1));

                        }
                        while (letters[index0, index1] == null);
                        if (letters[index0, index1] != letters[i,j])
                        {
                            tmp.Add(letters[index0, index1]);
                            m++;
                        }
                    }
                    dict.Add(letters[i, j], tmp);
                }
            }
        }
        return dict;
    }

    private static bool hasGBorder(Transform t)
    {
        return t.Find(Objects.GroupBorder.ToString()) != null ? true : false;
    }
    private static bool hasLBorder(Transform t)
    {
        return t.Find(Objects.LetterBorder.ToString()) != null ? true : false;
    }
    private void RemoveChilds()
    {
        foreach (Transform t in Letters1D.FindAll(hasGBorder))
        {
            Destroy(t.Find(Objects.GroupBorder.ToString()).gameObject);
        }
        foreach (Transform t in Letters1D.FindAll(hasLBorder))
        {
            Destroy(t.Find(Objects.LetterBorder.ToString()).gameObject);
        }
    }

    private void AddLetterBorder(Transform letter)
    {
        CreateObject(LetterBorder, letter, Objects.LetterBorder.ToString());
    }
    private void AddGroupBorder(List<Transform> group)
    {
        foreach (Transform t in group)
        {
            CreateObject(GroupBorder, t, Objects.GroupBorder.ToString());
        }
    }

    private void MoveCursor(int indexVertical, int indexHorizontal)
    {
        
        //location.CopyTo(prevLoc, 0);
        int[] nearest = FindNearest(indexVertical, indexHorizontal);
        location[0] += indexVertical;
        location[1] += indexHorizontal;
        if (location[0] == letters.GetLength(0))
        {
            location[0] = 0;
        }
        else if (location[0] < 0)
        {
            location[0] = letters.GetLength(0) - 1;
        }
        if (location[1] == letters.GetLength(1))
        {
            location[1] = 0;
        }
        else if (location[1] < 0)
        {
            location[1] = letters.GetLength(1) - 1;
        }
        
        

        if (letters[location[0], location[1]] == null)
        {
            recursion++;
            MoveCursor(indexVertical, indexHorizontal);
            return;
        }    
        else
        {
            if (recursion > 1)
            {

                int x;
                int y;
                if (prevLoc[0] == letters.GetLength(0) - 1 && location[0] == 0) x = letters.GetLength(0);
                else if (prevLoc[0] == 0 && location[0] == letters.GetLength(0) - 1) x = -1;
                else x = location[0];
                if (prevLoc[1] == letters.GetLength(1) - 1 && location[1] == 0) y = letters.GetLength(1);
                else if (prevLoc[1] == 0 && location[1] == letters.GetLength(1) - 1) y = -1;
                else y = location[1];
                int x2;
                int y2;
                if (prevLoc[0] == letters.GetLength(0) - 1 && nearest[0] == 0) x2 = letters.GetLength(0);
                else if (prevLoc[0] == 0 && nearest[0] == letters.GetLength(0) - 1) x2 = -1;
                else x2 = nearest[0];
                if (prevLoc[1] == letters.GetLength(1) - 1 && nearest[1] == 0) y2 = letters.GetLength(1);
                else if (prevLoc[1] == 0 && nearest[1] == letters.GetLength(1) - 1) y2 = -1;
                else y2 = nearest[1];
                float n = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - x2), 2) + Mathf.Pow((prevLoc[1] - y2), 2));
                float p = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - x), 2) + Mathf.Pow((prevLoc[1] - y), 2));
                //print(prevLoc[0].ToString() + " " + prevLoc[1].ToString());
                //print("N: " + nearest[0].ToString() + " " + nearest[1].ToString() + " " + (n).ToString());
                //print("L: " + location[0].ToString() + " " + location[1].ToString() + " " + p.ToString());
                //print(location.Equals(prevLoc));
                if (n< p || (prevLoc[0] == location[0] && prevLoc[1] == location[1]))
                {
                    nearest.CopyTo(location, 0);
                }

            }
            RemoveChilds();
            AddLetterBorder(letters[location[0], location[1]]);
            AddGroupBorder(pairs[letters[location[0], location[1]]]);


        }

    }

    private List<Transform> GetGroup()
    {
        List<Transform> list = new List<Transform>();
        list.Add(letters[location[0], location[1]]);
        foreach (Transform t in pairs[letters[location[0], location[1]]])
        {
            list.Add(t);
        }
        return list;
    }
    private void Select()
    {
        List<Transform> group = GetGroup();
        Transform selected = group[rand.Next(group.Count)];

        string selectedCharacter = selected.name;

        int strIndex = 0;
        bool first = true;

        strIndex = answer.IndexOf(selectedCharacter, strIndex);
        if (strIndex == -1)
        {
            if (first)
            {
                first = false;
                if (AddHangMan())
                {
                    return;
                }
                
                    
            }
            if (selected.childCount > 0)
            {
                ChangeBorders(selected);
            }

        }
        else
        {

                
            if (selected.childCount > 0)
            {
                ChangeBorders(selected);
            }
            DasheToLetter(selected);
        }

        if (CheckSentence())
        {
            canMove = false;

            StartCoroutine(WinLoseSign("Win"));

        }

        NullLetter(selected);
        StartCoroutine(MoveToNext());
        
    }

    private bool CheckSentence()
    {
        for (int i = 0; i < Sentence.childCount; i++)
        {
            if (Sentence.GetChild(i).name != Sentence.GetChild(i).GetComponent<RawImage>().texture.name)
            {
                return false;
            }
        }
        
        return true;
    }

    private IEnumerator MoveToNext()
    {
        yield return new WaitForSeconds(0.5f);

        pairs = Pairing();
        FindNextSpot();
        RemoveChilds();
        AddLetterBorder(letters[location[0], location[1]]);
        AddGroupBorder(pairs[letters[location[0], location[1]]]);


    }
    private void FindNextSpot()
    {
        for (int i = location[1]; i < letters.GetLength(1); i++)
        {
            if (letters[location[0], i] != null) {
                location[1] = i;
                
                return;
            }
        }
        for (int i = location[0]; i < letters.GetLength(0); i++)
        {
            if (letters[i, location[1]] != null)
            {
                location[0] = i;
                return;
            }
        }
        for (int i = location[1]; i > -1; i--)
        {
            if (letters[location[0], i] != null)
            {
                location[1] = i;
                return;
            }
        }
        for (int i = location[0]; i > -1; i--)
        {
            if (letters[i, location[1]] != null)
            {
                location[0] = i;
                return;
            }
        }
    }

    private int[] FindNearest(int v, int h)
    {
        int[] s1 = new int[2];
        int[] s2 = new int[2];
        if (v == 1)
        {
            for (int i = location[0]; i < letters.GetLength(0); i++)
            {
                for (int j = 1; j < letters.GetLength(1) / 2; j++)
                {
                    if (letters[i, location[1] + j < letters.GetLength(1) ? location[1] + j : 0] != null)
                    {
                        s1 = new int[] { i, location[1] + j < letters.GetLength(1) ? location[1] + j : 0 };
                        //return new int[] { i, location[1] + 1 < letters.GetLength(1) ? location[1] + 1 : 0 };
                    }
                    else if (letters[i, location[1] - j >= 0 ? location[1] - j : letters.GetLength(1) - 1] != null)
                    {
                        s2 = new int[] { i, location[1] - 1 >= 0 ? location[1] - j : letters.GetLength(1) - 1 };
                        //return new int[] { i, location[1] - 1 >= 0 ? location[1] - 1 : letters.GetLength(1) - 1 };
                    }
                    if (letters[s1[0], s1[1]] != null && letters[s2[0], s2[1]] != null)
                    {
                        float a1 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s1[0]), 2) + Mathf.Pow((prevLoc[1] - s1[1]), 2));
                        float a2 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s2[0]), 2) + Mathf.Pow((prevLoc[1] - s2[1]), 2));
                        return (a1 < a2 ? s1 : s2);

                    }
                    else if (letters[s1[0], s1[1]] != null) {
                        return s1;
                    }
                    else if (letters[s2[0], s2[1]] != null)
                    {
                        return s2;
                    }

                }
            }
        }

        else if (v == -1)
        {
            for (int i = location[0]; i > -1; i--)
            {
                for (int j = 1; j < letters.GetLength(1) / 2; j++)
                {
                    if (letters[i, location[1] + j < letters.GetLength(1) ? location[1] + j : 0] != null)
                    {
                        s1 = new int[] { i, location[1] + j < letters.GetLength(1) ? location[1] + j : 0 };
                        //return new int[] { i, location[1] + 1 < letters.GetLength(1) ? location[1] + 1 : 0 };
                    }
                    else if (letters[i, location[1] - j >= 0 ? location[1] - j : letters.GetLength(1) - 1] != null)
                    {
                        s2 = new int[] { i, location[1] - j >= 0 ? location[1] - j : letters.GetLength(1) - 1 };
                        //return new int[] { i, location[1] - 1 >= 0 ? location[1] - 1 : letters.GetLength(1) - 1 };
                    }
                    if (letters[s1[0], s1[1]] != null && letters[s2[0], s2[1]] != null)
                    {
                        float a1 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s1[0]), 2) + Mathf.Pow((prevLoc[1] - s1[1]), 2));
                        float a2 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s2[0]), 2) + Mathf.Pow((prevLoc[1] - s2[1]), 2));
                        return (a1 < a2 ? s1 : s2);

                    }
                    else if (letters[s1[0], s1[1]] != null)
                    {
                        return s1;
                    }
                    else if (letters[s2[0], s2[1]] != null)
                    {
                        return s2;
                    }

                }
            }

        }

        if (h == 1)
        {
            for (int i = location[1]; i < letters.GetLength(1); i++)
            {
                for (int j = 1; j < letters.GetLength(0); j++)
                {
                    if (letters[location[0] + j < letters.GetLength(0) ? location[0] + j : 0,i] != null)
                    {
                        s1 = new int[] { location[0] + j < letters.GetLength(0) ? location[0] + j : 0, i};
                        //return new int[] { location[0] + 1 < letters.GetLength(0) ? location[0] + 1 : 0, i};
                    }
                    else if (letters[location[0] - j >= 0 ? location[0] - j : letters.GetLength(0) - 1, i] != null)
                    {
                        s2 = new int[] { location[0] - j >= 0 ? location[0] - j : letters.GetLength(0) - 1,i};
                        //return new int[] { location[0] - 1 >= 0 ? location[0] - 1 : letters.GetLength(0) - 1,i};
                    }
                    if (letters[s1[0], s1[1]] != null && letters[s2[0], s2[1]] != null)
                    {
                        float a1 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s1[0]), 2) + Mathf.Pow((prevLoc[1] - s1[1]), 2));
                        float a2 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s2[0]), 2) + Mathf.Pow((prevLoc[1] - s2[1]), 2));
                        return (a1 < a2 ? s1 : s2);

                    }
                    else if (letters[s1[0], s1[1]] != null)
                    {
                        return s1;
                    }
                    else if (letters[s2[0], s2[1]] != null)
                    {
                        return s2;
                    }

                }
            }

        }

        else if (h == -1)
        {
            for (int i = location[1]; i > -1; i--)
            {
                for (int j = 1; j < letters.GetLength(0); j++)
                {
                    if (letters[location[0] + j < letters.GetLength(0) ? location[0] + j : 0, i] != null)
                    {
                        s1 = new int[] { location[0] + j < letters.GetLength(0) ? location[0] + j : 0, i };
                        //return new int[] { location[0] + 1 < letters.GetLength(0) ? location[0] + 1 : 0, i };
                    }
                    else if (letters[location[0] - j >= 0 ? location[0] - j : letters.GetLength(0) - 1, i] != null)
                    {
                        s2 = new int[] { location[0] - j >= 0 ? location[0] - j : letters.GetLength(0) - 1, i };
                        //return new int[] { location[0] - 1 >= 0 ? location[0] - 1 : letters.GetLength(0) - 1, i };
                    }
                    if (letters[s1[0], s1[1]] != null && letters[s2[0], s2[1]] != null)
                    {
                        float a1 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s1[0]), 2) + Mathf.Pow((prevLoc[1] - s1[1]), 2));
                        float a2 = Mathf.Sqrt(Mathf.Pow((prevLoc[0] - s2[0]), 2) + Mathf.Pow((prevLoc[1] - s2[1]), 2));
                        return (a1 < a2 ? s1 : s2);

                    }
                    else if (letters[s1[0], s1[1]] != null)
                    {
                        return s1;
                    }
                    else if (letters[s2[0], s2[1]] != null)
                    {
                        return s2;
                    }

                }
            }
        }
        return new int[] { -1, -1 };
    }
    private void DasheToLetter(Transform letter)
    {
        for (int i = 0; i < Sentence.childCount; i++)
        {
            if (letter.name == Sentence.GetChild(i).name)
            {
                Sentence.GetChild(i).GetComponent<RawImage>().texture = letter.GetComponent<RawImage>().texture;
            }
        }
        
    }
    private void NullLetter(Transform letter)
    {
        for (int i = 0; i < letters.GetLength(0);  i++)
        {
            for (int j = 0; j < letters.GetLength(1); j++)
            {
                if (letters[i,j] == letter)
                {
                    letters[i, j] = null;
                }
            }
        }
    }
    private void ChangeBorders(Transform letter)
    {
        for (int i = 0; i < letter.childCount; i++)
        {
            StartCoroutine(LetterCrossing(letter.GetChild(i)));
        }
    }
    private bool AddHangMan()
    {
        for (int i = 0; i < HangMan.childCount; i++)
        {
            if (!HangMan.GetChild(i).gameObject.activeSelf)
            {
                HangMan.GetChild(i).gameObject.SetActive(true);

                if (i == HangMan.childCount - 1)
                {
                    if (lives == 0)
                    {

                        Screen.GetChild(lives).gameObject.SetActive(false);
                        StartCoroutine(WinLoseSign("Lose"));
                        return true;
                    }
                    else
                    {
                        StartCoroutine(PrepareReset());
                        Screen.GetChild(lives).gameObject.SetActive(false);
                        lives--;
                        return true;
                    }
                }
                
                break;
            }
        }
        return false;
    }

    private IEnumerator LetterCrossing(Transform l)
    {
        yield return null;
        canMove = false;
        Transform parent = l.parent;
        if (l.name == Objects.LetterBorder.ToString())
        {
            l.GetComponent<RawImage>().texture = EndingLetterBorder;

            yield return new WaitForSeconds(0.5f);
            CreateObject(Cross, parent, Objects.Cross.ToString());

        }
        else if (l.name == Objects.GroupBorder.ToString())
        {
            l.GetComponent<RawImage>().texture = EndingGroupBorde;

            yield return new WaitForSeconds(0.5f);

            CreateObject(Cross, parent, Objects.Cross.ToString());
            //AddObject(cross, "Cross", parent);
            //Destroy(l.gameObject);

        }
        canMove = true;
    }
    private void ResetGame()
    {

        location = new int[2] { 0, 0 };
        canMove = true;

        RandomizeDashes();

        for (int i = 0; i < Letters1D.Count; i++)
        {
            if (Letters1D[i].childCount > 0)
            {
                
                for (int j = 0; j < Letters1D[i].childCount; j++)
                {
                    Destroy(Letters1D[i].GetChild(j).gameObject);
                }
                
            }
        }
        
        letters = originalLetters.Clone() as Transform[,];

        for (int i = 0; i < HangMan.childCount; i++)
        {
            HangMan.GetChild(i).gameObject.SetActive(false);
        }
        RandomizeDashes();
        pairs = Pairing();
        location.CopyTo(prevLoc, 0);
        MoveCursor(0, 0);
    }
    private IEnumerator PrepareReset()
    {
        yield return new WaitForSeconds(1f);
        ResetGame();
    }
    private IEnumerator WinLoseSign(string state)
    {
        yield return null;
        switch (state)
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
