using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Rand = System.Random;
using System.Linq;

public class Memory : MonoBehaviour
{
    public float MoveSpeed = 5;
    public float PictureSize = 50;
    private Transform journal;
    public Sprite[] rawPics;
    public GameObject picBoderFloor;
    private Dictionary<Transform, Transform> pictures = new Dictionary<Transform, Transform>();
    public GameObject picBorder;
    public GameObject wordBorder;
    private Transform zoomPage;
    private Transform zoomPic;
    private Transform sentences;
    private Transform wordPage;
    private Transform picPage;
    private List<Transform> picHolders = new List<Transform>();
    private List<Transform> wordHolders = new List<Transform>();
    private GameObject pic;
    private int selectedPic = 0;
    private int selectedWord = 0;
    private int prevLoc;
    private Stack<KeyCode> inputs = new Stack<KeyCode>();
    private KeyCode prevLastKey;
    private Vector3 velocity;
    private Vector3 prevVel = new Vector3(0, 1, 0);
    private Rigidbody charRB;
    public float CharacteSpeed = 1000;
    private enum GameState {Searching, Journal};
    private GameState gameState = GameState.Searching;

    private enum JournalState { WordPage, PicturePage, Zoom, Placed};
    private JournalState journalState = JournalState.WordPage;

    private enum Objects { Journal, WordPage, Sentences, PicturePage, ZoomPage, ZoomPicture, Picture, PClone, BClone , Temp, World, Character}

    private enum Movement { Right, Left, Up, Down}
    private Transform world;
    private Transform character;
    public Material SpriteMaterial;
    public GameObject MemoryCamera;
    private TentLogic tentLogic;
    private JournalState prevState;
    void Start()
    {
        journal = transform.Find(Objects.Journal.ToString());
        wordPage = journal.Find(Objects.WordPage.ToString());
        picPage = journal.Find(Objects.PicturePage.ToString());
        zoomPage = journal.Find(Objects.ZoomPage.ToString());
        zoomPic = zoomPage.Find(Objects.ZoomPicture.ToString());
        sentences = journal.Find(Objects.Sentences.ToString());
        tentLogic = FindObjectOfType<TentLogic>();

        for (int c = 0; c < picPage.childCount; c++)
        {
            
            pictures.Add(picPage.GetChild(c), null);
            picHolders.Add(picPage.GetChild(c));
        }
       
        for (int c = 0; c < wordPage.childCount; c++)
        {
            wordHolders.Add(wordPage.GetChild(c));
        }

        MainStateMachine(GameState.Searching);


        world = transform.Find(Objects.World.ToString());
        character = transform.Find(Objects.Character.ToString());
        
        charRB = character.GetComponent<Rigidbody>();
        charRB.Sleep();

        ScatterPictures();
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.O))
        {
            tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);

        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (journal.gameObject.activeSelf)
            {
                if (pic != null)
                {
                    RemoveChilds();

                }
                JournalStateMachine(journalState = JournalState.WordPage);
                MainStateMachine(gameState = GameState.Searching);
            }
            else
            {
                MainStateMachine(gameState = GameState.Journal);
            }
        }

        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (gameState == GameState.Journal)
            {

                if (journalState == JournalState.PicturePage || journalState == JournalState.Placed)
                {
                    prevState = journalState;
                    JournalStateMachine(journalState = JournalState.Zoom);
                }
                else if (journalState == JournalState.Zoom)
                {
                    MainStateMachine(gameState = GameState.Journal);
                }
            }

            
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            if (gameState == GameState.Journal)
            {
                MoveCursor(KeyCode.RightArrow);
            }
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            if (gameState == GameState.Journal)
            {
                MoveCursor(KeyCode.LeftArrow);
            }
        }

        else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (gameState == GameState.Searching && character.GetComponent<MG5Character>().picTexture != null)
            {
                CollectPicture();
                character.GetComponent<MG5Character>().col = null;
            }

            else if (gameState == GameState.Journal)
            {

                if ((journalState == JournalState.Zoom  && prevState != JournalState.Placed)|| journalState == JournalState.PicturePage)
                {
                    selectedWord = FindFirstEmpty(wordHolders, JournalState.WordPage);

                    pic = picHolders[selectedPic].Find(Objects.Picture.ToString()).gameObject;
                    JournalStateMachine(journalState = JournalState.WordPage);
                }

                else if (journalState == JournalState.Placed || (journalState == JournalState.Zoom && prevState == JournalState.Placed))
                {
                    CheckPictures();
                    print("Checked Picture");
                    if (pic == null)
                    {
                        prevLoc = selectedWord;
                        pic = wordHolders[selectedWord].Find(Objects.PClone.ToString()).gameObject;
                        wordHolders[selectedWord].Find(Objects.PClone.ToString()).gameObject.SetActive(false);
                        for (int i = 0; i < wordHolders.Count; i++)
                        {
                            if (i != prevLoc)
                            {
                                wordHolders[i].Find(Objects.PClone.ToString()).GetComponent<RawImage>().color = new Color32(255, 255, 255, 255 / 2);
                            }
                        }
                        JournalStateMachine(journalState);
                    }
                    else
                    {

                        
                        if (selectedWord != prevLoc)
                        {
                            Texture tmp = wordHolders[prevLoc].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture;
                            wordHolders[prevLoc].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture = wordHolders[selectedWord].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture;
                            wordHolders[selectedWord].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture = tmp;
                            wordHolders[prevLoc].Find(Objects.PClone.ToString()).gameObject.SetActive(true);
                            wordHolders[selectedWord].Find(Objects.PClone.ToString()).gameObject.SetActive(true);
                            zoomPic.GetComponent<RawImage>().texture = wordHolders[selectedWord].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture;

                            if (wordHolders[selectedWord].Find(Objects.Temp.ToString()) != null)
                            {
                                Destroy(wordHolders[selectedWord].Find(Objects.Temp.ToString()).gameObject);
                            }

                        }
                        else
                        {
                            wordHolders[prevLoc].Find(Objects.PClone.ToString()).gameObject.SetActive(true);
                        }
                        pic = null;
                        for (int i = 0; i < wordHolders.Count; i++)
                        {

                            wordHolders[i].Find(Objects.PClone.ToString()).GetComponent<RawImage>().color = new Color32(255, 255, 255, 255);

                        }
                        CheckPictures();
                    }

                }
                else
                {
                    if (pic != null)
                    {
                        ApplyPicture(pic);
                        pic = null;
                        selectedPic = FindFirstEmpty(picHolders, JournalState.PicturePage);
                        if (selectedPic != -1)
                        {
                            JournalStateMachine(journalState = JournalState.PicturePage);
                        }
                        else if (journalState == JournalState.WordPage && CheckNumbers())
                        {
                            pic = null;
                            JournalStateMachine(journalState = JournalState.Placed);
                            CheckPictures();
                            print("Checked Picture");
                            sentences.GetChild(selectedWord).GetComponent<Animator>().Play("Anim");
                        }
                        else
                        {
                            JournalStateMachine(journalState = JournalState.WordPage);
                            sentences.GetChild(selectedWord).GetComponent<Animator>().Play("Anim");
                        }

                    }
                }

            }
        }



        if (gameState == GameState.Searching)
        {

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
    }

    private bool CheckNumbers()
    {
        foreach (Transform t in wordHolders)
        {
            if (t.Find(Objects.PClone.ToString()) == null)
            {
                return false;
            }
        }
        return true;
    }

    private void ScatterPictures()
    {
        GameObject obj = new GameObject();
        obj.name = "Picture";
        obj.AddComponent<SpriteRenderer>();
        obj.AddComponent<BoxCollider>().size = new Vector3(10, 10, 20);
        obj.AddComponent<SphereCollider>().radius = 10;
        obj.GetComponent<SphereCollider>().isTrigger = true;
        obj.GetComponent<SpriteRenderer>().material = SpriteMaterial;
        
        
        
        

        Rand rand = new Rand();
        Mesh worldMesh = world.GetComponent<MeshFilter>().mesh;
        List<int> vertLocs = new List<int>();
        Vector3[] n = worldMesh.normals;
        Vector3[] v = worldMesh.vertices;
        int tmpLoc = 0;
        int index = 0;
        int offset = worldMesh.vertices.Length / 9;
        rawPics = rawPics.OrderBy(item => rand.Next()).ToArray();

        for (int i = 0; i < rawPics.Length; i++)
        {
            do
            {
                tmpLoc = rand.Next(index, worldMesh.vertices.Length);
                
                if (!vertLocs.Contains(tmpLoc))
                {
                    vertLocs.Add(tmpLoc);
                    break;
                }
                index += offset;
            }
            while (!vertLocs.Contains(tmpLoc));
        }
    
        for (int i = 0; i < vertLocs.Count; i++)
        {
            
            Vector3 tmp = world.TransformPoint(v[vertLocs[i]]);
            Vector3 nTmp = world.TransformDirection(n[vertLocs[i]]);
            GameObject newObj = Instantiate(obj, tmp, Quaternion.identity);

            newObj.GetComponent<SpriteRenderer>().sprite = rawPics[i];
            newObj.transform.rotation = Quaternion.FromToRotation(newObj.transform.forward, nTmp);
            newObj.transform.localScale = new Vector3(PictureSize, PictureSize, PictureSize);
            Instantiate(picBoderFloor, newObj.transform);
            
        }
     
    }

    private void MainStateMachine(GameState state)
    {
        switch (state)
        {
            case GameState.Searching:
                journal.gameObject.SetActive(false);

                break;

            case GameState.Journal:
                charRB.Sleep();
                journal.gameObject.SetActive(true);

                if (PicPageChecker())
                {
                    JournalStateMachine(journalState = JournalState.PicturePage);
                    break;
                }
                else if (prevState != JournalState.Placed)
                {
                    JournalStateMachine(journalState = JournalState.WordPage);
                    break;
                }
                else
                {
                    JournalStateMachine(journalState = JournalState.Placed);
                    break;
                }
                
            
        }
    }


    private void JournalStateMachine(JournalState state)
    {
        RemoveBorders(picHolders);
        RemoveBorders(wordHolders);

        for (int i = 0; i < sentences.childCount; i++)
        {
            if (state == JournalState.Placed)
            {
                if (i != selectedWord)
                {
                    sentences.GetChild(i).GetComponent<Animator>().Play("Empty");
                
                }

            }
            else
            {
                sentences.GetChild(i).GetComponent<Animator>().Play("Empty");

            }
        }

        switch (state)
        {
            case JournalState.PicturePage:


                zoomPage.gameObject.SetActive(false);
                if (selectedPic == -1)
                {
                    for (int i = 0; i < picHolders.Count; i++)
                    {
                        if (picHolders[i].childCount > 0 && picHolders[i].gameObject.activeSelf)
                        {
                            selectedPic = i;
                            break;
                        }
                    }
                }

                if (selectedPic != -1)
                {
                    Instantiate(picBorder, picHolders[selectedPic]).name = Objects.BClone.ToString();

                }

                break;

            case JournalState.WordPage:

                foreach (Transform t in wordHolders)
                {
                    if (t.childCount > 0)
                    {
                        if (t.Find(Objects.PClone.ToString()) != null)
                        {

                            t.Find(Objects.PClone.ToString()).gameObject.SetActive(true);
                        }
                        if (pic != null)
                        {
                            for (int c = 0; c < t.childCount; c++)
                            {
                                if (t.GetChild(c).GetComponent<RawImage>().texture == pic.GetComponent<RawImage>().texture)
                                {
                                    Destroy(t.GetChild(c).gameObject);
                                }
                            }

                        }
                    }
                }
                sentences.GetChild(selectedWord).GetComponent<Animator>().Play("Anim");

                Instantiate(wordBorder, wordHolders[selectedWord]).name = Objects.BClone.ToString();
                zoomPage.gameObject.SetActive(false);
                if (pic != null)
                {
                    if (FindOtherChild(picHolders[selectedPic].gameObject) != null)
                    {
                        FindOtherChild(pic).SetActive(false);
                    }
                    Instantiate(pic, wordHolders[selectedWord]).name = Objects.PClone.ToString();

                    for (int i = 0; i < wordHolders[selectedWord].childCount; i++)
                    {
                        if (wordHolders[selectedWord].GetChild(i).name != Objects.BClone.ToString())
                        {
                            wordHolders[selectedWord].GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(60, 60);
                        }
                    }

                }
                break;

            case JournalState.Zoom:
                if (prevState == JournalState.PicturePage)
                {
                    zoomPage.gameObject.SetActive(true);
                    zoomPic.GetComponent<RawImage>().texture = pictures[picHolders[selectedPic]].GetComponent<RawImage>().texture;

                }
                else if (prevState == JournalState.Placed)
                {
                    Instantiate(wordBorder, wordHolders[selectedWord]).name = Objects.BClone.ToString();
                    zoomPage.gameObject.SetActive(true);
                    zoomPic.GetComponent<RawImage>().texture = wordHolders[selectedWord].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture;
                }
                
                
                break;

            case JournalState.Placed:
                zoomPage.gameObject.SetActive(false);

                sentences.GetChild(selectedWord).GetComponent<Animator>().Play("Anim");

                Instantiate(wordBorder, wordHolders[selectedWord]).name = Objects.BClone.ToString();
                if (pic != null)
                {
                    for (int i = 0; i < wordHolders.Count; i++)
                    {
                    
                        if (wordHolders[i].Find(Objects.PClone.ToString()).gameObject != pic)
                        {
                            
                            wordHolders[i].Find(Objects.PClone.ToString()).gameObject.SetActive(true);
                        }
                        if (wordHolders[i].Find(Objects.Temp.ToString()) != null)
                        {
                            Destroy(wordHolders[i].Find(Objects.Temp.ToString()).gameObject);
                        }
                    }

                    if (selectedWord != prevLoc)
                    {
                        wordHolders[selectedWord].Find(Objects.PClone.ToString()).gameObject.SetActive(false);
                    }
                        Instantiate(pic, wordHolders[selectedWord]).name = Objects.Temp.ToString();
                        wordHolders[selectedWord].Find(Objects.Temp.ToString()).gameObject.SetActive(true);
                    

                }

                break;
        }
    }

    private void MoveCharacter()
    {
        KeyCode lastKey = inputs.Peek();

        Animator charAnim = character.GetComponent<Animator>();

        switch (lastKey)
        {
            case KeyCode.W:
                prevLastKey = lastKey;

                break;

            case KeyCode.D:
                prevLastKey = lastKey;

                break;

            case KeyCode.S:
                prevLastKey = lastKey;

                break;

            case KeyCode.A:
                prevLastKey = lastKey;

                break;
        }


        Vector2 inputAxes = Vector2.zero;
        if (prevLastKey == KeyCode.D || prevLastKey == KeyCode.A)
        {
            velocity = character.right * Input.GetAxisRaw("Horizontal");
            inputAxes = new Vector2(Input.GetAxisRaw("Horizontal"), 0);
        }
        else if (prevLastKey == KeyCode.W || prevLastKey == KeyCode.S)
        {
            velocity = character.up * Input.GetAxisRaw("Vertical");
            inputAxes = new Vector2(0, Input.GetAxisRaw("Vertical"));
        }

        charAnim.SetFloat("MoveX", inputAxes.x);
        charAnim.SetFloat("MoveY", inputAxes.y);

        if (Input.GetAxisRaw("Horizontal") == 1 || Input.GetAxisRaw("Horizontal") == -1
                || Input.GetAxisRaw("Vertical") == 1 || Input.GetAxisRaw("Vertical") == -1)
        {

            charAnim.SetFloat("LastMoveX", Input.GetAxisRaw("Horizontal"));
            charAnim.SetFloat("LastMoveY", Input.GetAxisRaw("Vertical"));
        }
        if (character.GetComponent<MG5Character>().inCol && prevVel == velocity)
        {
            prevVel = velocity;
            charAnim.SetFloat("MoveX", 0);
            charAnim.SetFloat("MoveY", 0);
        }

       
        if (Input.GetAxisRaw("Vertical") != 0 || Input.GetAxisRaw("Horizontal") != 0)
        {
            charRB.WakeUp();
            //MemoryCamera.GetComponent<Rigidbody>().WakeUp();
            charRB.velocity = velocity * CharacteSpeed;
        }
        else
        {
            charRB.velocity = Vector2.zero;
            //MemoryCamera.GetComponent<Rigidbody>().velocity = Vector3.zero;
            //MemoryCamera.GetComponent<Rigidbody>().Sleep();
            charRB.Sleep();
            
        }
        
    }

    private void CollectPicture()
    {
        for (int i = 0; i < picHolders.Count; i++)
        {
            if (picHolders[i].GetChild(0).GetComponent<RawImage>().texture == null)
            {
                picHolders[i].GetChild(0).GetComponent<RawImage>().texture = character.GetComponent<MG5Character>().picTexture;
                picHolders[i].gameObject.SetActive(true);
                pictures[picHolders[i]] = picHolders[i].GetChild(0);
                character.GetComponent<MG5Character>().DestroyPicture();
                return;
            }
        }
    }


    private void MoveCursor(KeyCode key)
    {
        if (key == KeyCode.RightArrow)
        {
            if ((journalState == JournalState.Zoom && prevState != JournalState.Placed) || journalState == JournalState.PicturePage)
            {
                if (selectedPic == picHolders.Count - 1)
                {
                    selectedPic = 0;
                }
                else
                {
                    selectedPic++;

                }
                if (!picHolders[selectedPic].gameObject.activeSelf)
                {
                    MoveCursor(key);
                }
            }
            else if ((journalState == JournalState.Zoom && prevState == JournalState.Placed) || journalState == JournalState.WordPage || journalState == JournalState.Placed)
            {
                if (selectedWord == wordHolders.Count -1)
                {
                    selectedWord = 0;
                }
                else
                {
                    selectedWord++;
                }
            }
        }
        else if (key == KeyCode.LeftArrow)
        {
            if ((journalState == JournalState.Zoom && prevState != JournalState.Placed) || journalState == JournalState.PicturePage)
            {
                if (selectedPic == 0)
                {
                    selectedPic = picHolders.Count - 1;
                }
                else
                {
                    selectedPic--;
                }
                if (!picHolders[selectedPic].gameObject.activeSelf)
                {
                    MoveCursor(key);
                }
            }
            else if ((journalState == JournalState.Zoom && prevState == JournalState.Placed) || journalState == JournalState.WordPage || journalState == JournalState.Placed)
            {
                if (selectedWord == 0)
                {
                    selectedWord = wordHolders.Count - 1;
                }
                else
                {
                    selectedWord--;
                }
                
            }
        }

     
        JournalStateMachine(journalState);
        
    }
    private bool PicPageChecker()
    {
        foreach (KeyValuePair<Transform, Transform> t in pictures)
        {
            if (t.Value != null)
            {
                return true;
            }    
        }
        return false;
        
    }

    private int FindFirstEmpty(List<Transform> holder, JournalState state)
    {
        if (state == JournalState.WordPage)
        {
            for (int i = 0; i < holder.Count; i++)
            {
                if (holder[i].childCount == 0)
                {
                    return i;
                }
            }
        }
        else if (state == JournalState.PicturePage)
        {
            for (int i = 0; i < holder.Count; i++)
            {
                if (pictures[holder[i]] != null)
                {
                    return i;
                }
            }
        }
        return -1;
    } 

    private void RemoveBorders(List<Transform> holders)
    {
        foreach (Transform t in holders)
        {
            for (int c = 0; c < t.childCount; c++)
            {
                if (t.GetChild(c).name == Objects.BClone.ToString())
                {
                
                    Destroy(t.GetChild(c).gameObject);
                }

            }
        }
    } 

    private void ApplyPicture(GameObject picture)
    {
        if (FindOtherChild(picture) != null)
        {
            picHolders[selectedPic].Find(Objects.Picture.ToString()).GetComponent<RawImage>().texture = FindOtherChild(picture).GetComponent<RawImage>().texture;
        }
        else {
            foreach (Transform t in picHolders)
            {
                if (t.Find(Objects.Picture.ToString()).gameObject == picture)
                {
                    t.gameObject.SetActive(false); 
                
                    pictures[t] = null;
                }
            }
        }

    }

    private GameObject FindOtherChild(GameObject picture)
    {
        for (int c = 0; c < wordHolders[selectedWord].childCount; c++)
        {
            if (wordHolders[selectedWord].GetChild(c).GetComponent<RawImage>().texture != picture.GetComponent<RawImage>().texture &&
                wordHolders[selectedWord].GetChild(c).name != Objects.BClone.ToString())
            {
                return (wordHolders[selectedWord].GetChild(c).gameObject);
            }
        }
        return null;
    }

    private void RemoveChilds()
    {
        
        for (int i = 0; i < wordHolders[selectedWord].childCount; i++)
        {
            Destroy(wordHolders[selectedWord].GetChild(i).gameObject);
        }
    }

    private void CheckPictures()
    {

        string[] answers = new string[] { "MG5I1", "MG5I2", "MG5I3", "MG5I4", "MG5I5", "MG5I6", "MG5I7", "MG5I8", "MG5I9" };

        for (int i = 0; i < answers.Length; i++)
        {
            if (wordHolders[i].Find(Objects.PClone.ToString()).GetComponent<RawImage>().texture.name != answers[i])
            {
                return;
            }
        }
        journal.gameObject.SetActive(false);

        tentLogic.ChangeWinLoseState(TentLogic.WinLoseState.Win);

    }
}
