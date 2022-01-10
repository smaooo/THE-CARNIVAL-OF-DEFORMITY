using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TentLogic : MonoBehaviour
{

    public enum MGS { MG1, MG2, MG3, MG4, MG5, None }
    private enum SortLayers { TentFirstLayer, TentSecondLayer, TentThirdLayer }

    public enum WinLoseState { Win, Lose, TBD}

    private enum Objects { CAGEF, MG1Camera, MG2AnimCamera, MG2Camera, MG3AnimCamera, MG3Camera, BUTTON, SnowMask, VisualSnowScene, MG4AnimCamera, MG4Camera, ShapeScene, MG5AnimCamera, MG5Camera, MemoryScene }

    private OurCharacter character;
    public Dictionary<MGS, bool> GameStates = new Dictionary<MGS, bool>() { { MGS.MG1, true }, { MGS.MG2, true }, { MGS.MG3, true }, { MGS.MG4, false}, { MGS.MG5, false} };
    private Dictionary<MGS,WinLoseState> winState = new Dictionary<MGS, WinLoseState>() { { MGS.MG1, WinLoseState.TBD}, { MGS.MG2, WinLoseState.TBD }, { MGS.MG3, WinLoseState.TBD }, { MGS.MG4, WinLoseState.TBD }, { MGS.MG5, WinLoseState.TBD } };
    public MGS mgState;
    public Dictionary<MGS, Transform> MiniGames = new Dictionary<MGS, Transform>();
    private Transform Tent;
    public bool playing = false;
    private DancingLettersV2 dancingLetters;
    private Trail trail;
    private VisualSnow visualSnow;
    private Shape shape;
    private Memory memory;
    public Vector3[] MG1CamPos;
    public Vector3[] MG2CamPos;
    public Vector3[] MG3CamPos;
    public Vector3[] MG4CamPos;
    public Vector3[] MG5CamPos;
    
    private TentDialogueSystem tentDialogue;
    public bool inDialogue = false;
    public bool typing = false;
    public GameObject CamMain;
    public Transform lanterns;
    public Transform[] firstLayerLanterns;
    public Transform[] SecondLayerLanterns;
    public Transform[] thirdLayerLanterns;
    public Transform firstLeftLantern;
    public Transform firstMidLantern;
    public Transform firstRightLantern;
    public Transform secondLeftLantern;
    public Transform secondRightLantern;
    public GameObject memoryScene;
    public GameObject keyBack;
    public GameObject lantentBase;
    public Sprite[] originals;
    public bool canMove = false;
    public Texture[] MG5Pics;
    public GameObject MG5PicHolder;
    public GameObject MG5PicPage;
    public GameObject endAnimCam;
    public Transform endCam;
    public BoxCollider[] triggers;
    private bool ending = false;
    private bool shouldLoad = false;
    public WLState WinLose;
    private MainSceneAudio sceneAudio;
    public Vector3[] sourcesPos;

    public GameObject shapeScene;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        WinLose.MG1 = false;
        WinLose.MG2 = false;
        WinLose.MG3 = false;
        WinLose.MG4 = false;
        WinLose.MG5 = false;

        mgState = MGS.None;
        character = FindObjectOfType<OurCharacter>();
        Tent = transform;
        dancingLetters = FindObjectOfType<DancingLettersV2>();
        trail = FindObjectOfType<Trail>();
        visualSnow = FindObjectOfType<VisualSnow>();
        //shape = FindObjectOfType<Shape>();
        //memory = FindObjectOfType<Memory>();
        tentDialogue = FindObjectOfType<TentDialogueSystem>();
        sceneAudio = FindObjectOfType<MainSceneAudio>();
        foreach (MGS m in Enum.GetValues(typeof(MGS)))
        {
            if (m != MGS.None)
            {
                MiniGames.Add(m, Tent.Find(m.ToString()));

            }
        }


        SetSourcesPos();
        StartCoroutine(PlayBeginingAnims());
    }

    void Update()
    {
        if (!playing && !inDialogue)
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                ManageMiniGames();
            }

        }

        if (inDialogue && !typing)
        {
            
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(tentDialogue.RunConversation(mgState));
            }
        }

        if (ending)
        {
            RotateObjects();
        }
    }

    private void SetSourcesPos()
    {
        sceneAudio.mg1Sound.GetComponent<Transform>().position = sourcesPos[0];
        sceneAudio.mg2Sound.GetComponent<Transform>().position = sourcesPos[1];
        sceneAudio.mg3Sound.GetComponent<Transform>().position = sourcesPos[2];
        sceneAudio.mg4Sound.GetComponent<Transform>().position = sourcesPos[3];
        sceneAudio.mg5Sound.GetComponent<Transform>().position = sourcesPos[4];
    }
    private void ManageMiniGames()
    {
        switch (mgState)
        {
            case MGS.MG1:
                if (GameStates[mgState])
                {
                    character.GetComponent<Rigidbody>().Sleep();
                    playing = true;
                    StartCoroutine(ChangeAnimatorState());

                    ChangeCamera(MiniGames[MGS.MG1].Find(Objects.MG1Camera.ToString()));
                    //StartCoroutine(CageFall(MiniGames[MGS.MG1].Find(Objects.CAGEF.ToString()).GetComponent<Animator>(),
                    //Camera.main.transform, MG1CamPos[1], MiniGames[MGS.MG1].Find(Objects.MG1Camera.ToString()), MG1CamPos[2]));
                    
                }
                break;

            case MGS.MG2:
                if (GameStates[mgState])
                {
                    character.GetComponent<Rigidbody>().Sleep();
                    playing = true;
                    
                    ChangeCamera(MiniGames[MGS.MG2].Find(Objects.MG2AnimCamera.ToString()));
                    StartCoroutine(CageFall(MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).GetComponent<Animator>(),
                    Camera.main.transform, MG2CamPos[1], MiniGames[MGS.MG2].Find(Objects.MG2AnimCamera.ToString()), MG2CamPos[2]));
                    
                }
                break;

            case MGS.MG3:
                if (GameStates[mgState])
                {
                    character.GetComponent<Rigidbody>().Sleep();
                    playing = true;
                    StartCoroutine(MG3Waiter());
                }
                break;

            case MGS.MG4:
                if (GameStates[mgState])
                {
                    character.GetComponent<Rigidbody>().Sleep();
                    playing = true;
                    ChangeCamera(MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()));

                    StartCoroutine(CageFall(MiniGames[MGS.MG4].Find(Objects.CAGEF.ToString()).GetComponent<Animator>(),
                    Camera.main.transform, MG4CamPos[1], MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()), MG4CamPos[2]));
                }
                else
                {
                    StartCoroutine(tentDialogue.RunConversation(mgState));
                    inDialogue = true;
                }
                break;

            case MGS.MG5:
                if (GameStates[mgState])
                {
                    character.GetComponent<Rigidbody>().Sleep();
                    character.enabled = false;
                    DisabpleAllTriggers();
                    playing = true;
                    ChangeCamera(MiniGames[MGS.MG5].Find(Objects.MG5AnimCamera.ToString()));
                    StartCoroutine(CageFall(MiniGames[MGS.MG5].Find(Objects.CAGEF.ToString()).GetComponent<Animator>(),
                    Camera.main.transform, MG5CamPos[1], MiniGames[MGS.MG5].Find(Objects.MG5AnimCamera.ToString()), MG5CamPos[2]));
                }
                else
                {
                    StartCoroutine(tentDialogue.RunConversation(mgState));
                    inDialogue = true;
                }
                break;
        }
    }

    private void ChangeCamera(Transform cam) 
    {
        cam.position = Camera.main.transform.position;
        cam.rotation = Camera.main.transform.rotation;

        Camera.main.gameObject.SetActive(false);
        cam.tag = "MainCamera";
        cam.gameObject.SetActive(true);

    }
   
    public void RotateObjects()
    {
        if (ending)
        {

            foreach (KeyValuePair<MGS, Transform> kp in MiniGames)
            {
                if (kp.Key != MGS.None)
                {
                    Quaternion rot = Quaternion.LookRotation(endAnimCam.GetComponent<Transform>().position - kp.Value.position);

                    kp.Value.rotation = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));

                }
            }

            for (int i = 0; i < lanterns.childCount; i++)
            {
            
                Quaternion rot = Quaternion.LookRotation(endAnimCam.GetComponent<Transform>().position - lanterns.GetChild(i).position);

                lanterns.GetChild(i).rotation = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));
            }
        }

        else
        {
            foreach (KeyValuePair<MGS, Transform> kp in MiniGames)
            {
                if (kp.Key != MGS.None)
                {
                    Quaternion rot = Quaternion.LookRotation(character.GetComponent<Transform>().position - kp.Value.position);

                    kp.Value.rotation = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));

                }
            }

            for (int i = 0; i < lanterns.childCount; i++)
            {

                Quaternion rot = Quaternion.LookRotation(character.GetComponent<Transform>().position - lanterns.GetChild(i).position);

                lanterns.GetChild(i).rotation = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));
            }
        }
    }

    public void DisableColliders(Collider other)
    {

       
        if (other.name != "First" && other.name != "Second" && other.name != "Third" && other.name != "Left" && other.name != "Right" && other.name != "Middle")
        {
            
            other.GetComponent<BoxCollider>().enabled = false;

        }
    }

    private void DisabpleAllTriggers()
    {
        foreach (BoxCollider b in triggers)
        {
            b.enabled = false;
        }          
    }
    public void EnableColliders(Collider other)
    {
        if (other.name != "First" && other.name != "Second" && other.name != "Third" && other.name != "Left" && other.name != "Right")
        {
            if (other.TryGetComponent(typeof(BoxCollider), out Component comp))
            {
                other.GetComponent<BoxCollider>().enabled = true;

            }

        }


    }


    private IEnumerator CageFall(Animator cage, Transform cam, Vector3 target, Transform curCam, Vector3 nextTarget)
    {

        yield return null;
        cage.SetTrigger("Fall");
        yield return new WaitForSeconds(cage.GetCurrentAnimatorStateInfo(0).length - 0.5f);

        yield return StartCoroutine(CamZoom(cam, target, curCam));
        cage.SetTrigger("GoBack");
        yield return new WaitForSeconds(cage.GetCurrentAnimatorStateInfo(0).length);
        if (mgState == MGS.MG1)
        {
            DisableAnimator();

        }

        StartCoroutine(tentDialogue.RunConversation(mgState));
        inDialogue = true;
        yield return new WaitWhile(() => inDialogue);


        cage.SetTrigger("Fall");

        switch (mgState)
        {
            case MGS.MG2:
                yield return new WaitForSeconds(cage.GetCurrentAnimatorStateInfo(0).length);
                StartCoroutine(ShowKeyHelp());
                break;
            case MGS.MG3:
                yield return new WaitForSeconds(cage.GetCurrentAnimatorStateInfo(0).length);
                StartCoroutine(ShowKeyHelp());
                break;

            case MGS.MG4:
                yield return new WaitForSeconds(cage.GetCurrentAnimatorStateInfo(0).length);
                StartCoroutine(ShowKeyHelp());
                break;
        }

        float speed = 0;
        switch(mgState)
        {
            case MGS.MG1:
                speed = 6;
                break;

            case MGS.MG2:
                speed = 6;
                break;

            case MGS.MG3:
                speed = 15;
                break;

            case MGS.MG4:
                speed = 6;
                break;

            case MGS.MG5:
                speed = 6;
                break;
        }
        yield return StartCoroutine(CamFinalZoom(cam, nextTarget, curCam, speed));





    }

    private IEnumerator ShowKeyHelp()
    {

        keyBack.SetActive(true);
        yield return null;

        switch (mgState)
        {
            case MGS.MG1:
                keyBack.transform.Find("MG1C").gameObject.SetActive(true);
                yield return new WaitForSeconds(4);
                keyBack.transform.Find("MG1C").gameObject.SetActive(false);
                break;

            case MGS.MG2:
                keyBack.transform.Find("MG2C").gameObject.SetActive(true);
                yield return new WaitForSeconds(4);
                keyBack.transform.Find("MG2C").gameObject.SetActive(false);
                break;

            case MGS.MG3:
                keyBack.transform.Find("MG3C").gameObject.SetActive(true);
                yield return new WaitForSeconds(4);
                keyBack.transform.Find("MG3C").gameObject.SetActive(false);
                break;

            case MGS.MG4:
                keyBack.transform.Find("MG4C").gameObject.SetActive(true);
                yield return new WaitForSeconds(4);
                keyBack.transform.Find("MG4C").gameObject.SetActive(false);
                break;

            case MGS.MG5:
                keyBack.transform.Find("MG5C").gameObject.SetActive(true);
                yield return new WaitForSeconds(4);
                keyBack.transform.Find("MG5C").gameObject.SetActive(false);
                break;
        }
        keyBack.SetActive(false);

    }
    private IEnumerator CamZoom(Transform cam, Vector3 target, Transform curCam)
    {
        //Vector3 originalPos = cam.localPosition;
        
        float timer = 0;
        while (cam.localPosition !=target)
        {

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(cam.localPosition, target, Mathf.SmoothStep(0,1,Mathf.Log(timer)));
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(0, 180, 0), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
            
            if (cam.localPosition == target)
            {
                break;
            }
        }
        sceneAudio.SwitchStates(mgState);
        yield return StartCoroutine(NameDrop(curCam));

        switch (mgState)
        {
            case MGS.MG1:
                yield return StartCoroutine(CamZoomOut(cam, MG1CamPos[0], false, false, Vector3.zero));
                break;
            case MGS.MG2:
                yield return StartCoroutine(CamZoomOut(cam, MG2CamPos[0], false, false, Vector3.zero));
                break;
            case MGS.MG3:
                yield return StartCoroutine(CamZoomOut(cam, MG3CamPos[0], false, false, new Vector3(330.210022f, 180f, 0f)));
                break;
            case MGS.MG4:
                yield return StartCoroutine(CamZoomOut(cam, MG4CamPos[0], false, false, Vector3.zero));
                break;
            case MGS.MG5:
                yield return StartCoroutine(CamZoomOut(cam, MG5CamPos[0], false, false, Vector3.zero));
                break;

        }       

    }

    private IEnumerator NameDrop(Transform cam)
    {
        Animator nameAnim = cam.Find("Canvas").Find("Name").GetComponent<Animator>();
        nameAnim.SetTrigger("Drop");
        yield return new WaitForSeconds(11/60f);
        nameAnim.SetTrigger("PlayAnim");
        //nameAnim.ResetTrigger("Drop");

        yield return new WaitForSeconds(1.5f);
        nameAnim.SetTrigger("Back");
    }

    private IEnumerator CamZoomOut(Transform cam, Vector3 target, bool final, bool rotate, Vector3 angle)
    {
        float timer = 0;

        while (cam.localPosition != target)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            

            if (final)
            {
                cam.position = Vector3.Lerp(cam.position, target, Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
                if (rotate)
                {
                    cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(angle.x, angle.y, angle.z), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));

                }
                if (cam.position == target)
                {
                    break;
                }
                if (Mathf.Abs(cam.position.z - target.z) < 1.5f)
                {

                    switch (mgState)
                    {
                        case MGS.MG1:
                            MiniGames[MGS.MG1].Find("Canvas").GetComponent<Animator>().ResetTrigger("In");
                            MiniGames[MGS.MG1].Find("Canvas").GetComponent<Animator>().SetTrigger("Out");
                            MiniGames[MGS.MG1].Find("Ch").gameObject.SetActive(true);

                            for (int i = 0; i < MiniGames[mgState].Find("Canvas").childCount; i++)
                            {
                                MiniGames[mgState].Find("Canvas").GetChild(i).gameObject.SetActive(false);
                            }
                            break;

                        case MGS.MG2:
                            MiniGames[MGS.MG2].Find(Objects.MG2Camera.ToString()).Find("Canvas").gameObject.SetActive(false);
                            shapeScene.SetActive(false);
                            MiniGames[MGS.MG2].Find("Ch").gameObject.SetActive(true);
                            MiniGames[MGS.MG2].Find("Ch").GetComponent<SpriteRenderer>().sprite = originals[1];


                            break;

                        case MGS.MG3:
                            MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()).Find("Canvas").Find("Canvas").gameObject.SetActive(false);
                            MiniGames[MGS.MG3].Find("Ch").gameObject.SetActive(true);
                            MiniGames[MGS.MG3].Find("Ch").GetComponent<SpriteRenderer>().sprite = originals[2];
                            //MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).Find("Snow").SetParent(MiniGames[MGS.MG3]);
                            //MiniGames[MGS.MG3].Find("Snow").gameObject.SetActive(true);
                            //MiniGames[MGS.MG3].Find("Snow").GetComponent<ParticleSystem>().Play();
                            MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()).GetComponent<Camera>().fieldOfView = 66;

                            //for (int i = MiniGames[mgState].Find(Objects.MG3Camera.ToString()).childCount - 1; i > 0; i--)
                            //{
                            //    MiniGames[mgState].Find(Objects.MG3Camera.ToString()).GetChild(i).gameObject.SetActive(false);
                            //}
                            break;

                        case MGS.MG4:
                            MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()).GetComponent<Camera>().fieldOfView = 66;
                            MiniGames[MGS.MG4].Find("Ch").GetComponent<SpriteRenderer>().sprite = originals[3];

                            break;

                        case MGS.MG5:
                            MiniGames[mgState].Find("Canvas").GetComponent<Animator>().ResetTrigger("In");
                            MiniGames[mgState].Find("Canvas").GetComponent<Animator>().SetTrigger("Out");
                            MiniGames[mgState].Find("Ch").gameObject.SetActive(true);
                            MiniGames[MGS.MG5].Find("Ch").GetComponent<SpriteRenderer>().sprite = originals[4];

                            memoryScene.SetActive(false);
                            for (int i = 0; i < MiniGames[mgState].Find("Canvas").childCount; i++)
                            {
                                MiniGames[mgState].Find("Canvas").GetChild(i).gameObject.SetActive(false);
                            }

                            break;

                    }
                }
               
            }

            else
            {
                cam.localPosition = Vector3.Lerp(cam.localPosition, target, Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
                if (rotate)
                {
                    cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(angle.x, angle.y, angle.z), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));

                }
                if (cam.localPosition == target)
                {
                    break;
                }
            }
        }
        MiniGames[mgState].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().ResetTrigger("Fall");
        MiniGames[mgState].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("GoBack");
        

    }

    private IEnumerator CamFinalZoom(Transform cam, Vector3 target, Transform curCam, float speed)
    {
        Vector3 originalPos = cam.localPosition;
        float timer = 0;
        yield return new WaitForSeconds(1f);
        while (cam.localPosition != target)
        {

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            cam.localPosition = Vector3.Lerp(cam.localPosition, target, Mathf.SmoothStep(0, 1, Mathf.Log(timer)) * 6);
            cam.localRotation = Quaternion.Lerp(cam.localRotation, Quaternion.Euler(0, 180, 0), Mathf.SmoothStep(0, 1, Mathf.Log10(timer))*speed);
            if (Mathf.Abs(cam.localPosition.z - target.z) < 1.5f)
            {

                switch (mgState)
                {
                    case MGS.MG1:
                        break;
                    case MGS.MG2:

                        break;

                    case MGS.MG3:

                        break;

                    case MGS.MG4:

                        break;

                    case MGS.MG5:
                        
                        //MiniGames[MGS.MG5].GetComponent<BoxCollider>().enabled = false;
                        break;
                }

            }
            if (Mathf.Abs(cam.localPosition.z - target.z) < 0.1f)
            {
                switch (mgState)
                {
                    case MGS.MG1:
                        MiniGames[MGS.MG1].Find("Ch").gameObject.SetActive(false);
                        MiniGames[MGS.MG1].Find("Ch").GetComponent<Animator>().enabled = false;

                        for (int i = 0; i < MiniGames[mgState].Find("Canvas").childCount; i++)
                        {
                            MiniGames[mgState].Find("Canvas").GetChild(i).gameObject.SetActive(true);
                        }

                        break;

                    case MGS.MG2:
                        MiniGames[MGS.MG2].Find("Ch").GetComponent<Animator>().enabled = false;

                        MiniGames[MGS.MG2].Find("Ch").gameObject.SetActive(false);

                        break;

                    case MGS.MG3:
                        MiniGames[MGS.MG3].Find("Ch").GetComponent<Animator>().enabled = false;
                        MiniGames[MGS.MG3].Find("Ch").gameObject.SetActive(false);
                        //MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).Find("Snow").gameObject.SetActive(false);

                        MiniGames[MGS.MG3].Find(Objects.MG3Camera.ToString()).Find(Objects.VisualSnowScene.ToString()).gameObject.SetActive(true);
                        MiniGames[MGS.MG3].GetComponent<BoxCollider>().enabled = false;
                        ChangeCamera(MiniGames[MGS.MG3].Find(Objects.MG3Camera.ToString()));
                        break;

                    case MGS.MG4:
                        //MiniGames[MGS.MG4].Find("Ch").gameObject.SetActive(false);
                        MiniGames[MGS.MG4].Find("HANDS").gameObject.SetActive(false);
                        MiniGames[MGS.MG4].Find(Objects.ShapeScene.ToString()).gameObject.SetActive(true);
                        MiniGames[MGS.MG4].GetComponent<BoxCollider>().enabled = false;
                        ChangeCamera(MiniGames[MGS.MG4].Find(Objects.MG4Camera.ToString()));


                        break;

                    case MGS.MG5:
                        for (int i = 0; i < MiniGames[mgState].Find("Canvas").childCount-1; i++)
                        {
                            MiniGames[mgState].Find("Canvas").GetChild(i).gameObject.SetActive(true);
                        }
                        memoryScene.SetActive(true);
                        memoryScene.GetComponent<Memory>().enabled = true;
                        ChangeCamera(MiniGames[MGS.MG5].Find(Objects.MG5Camera.ToString()));
                        MiniGames[MGS.MG5].Find("Ch").GetComponent<Animator>().enabled = false;
                        MiniGames[MGS.MG5].Find("Ch").gameObject.SetActive(false);

                        break;
                }


            }
            if (cam.localPosition == target)
            {
                break;
            }
        }

        switch (mgState)
        {
            case MGS.MG1:
                yield return StartCoroutine(ShowKeyHelp());
                MiniGames[mgState].Find("Canvas").GetComponent<Animator>().SetTrigger("In");

                dancingLetters.enabled = true;
                break;

            case MGS.MG2:
                MiniGames[MGS.MG2].GetComponent<BoxCollider>().enabled = false;
                ChangeCamera(MiniGames[MGS.MG2].Find(Objects.MG2Camera.ToString()));
                shapeScene.SetActive(true);
                trail.enabled = true;
                break;

            case MGS.MG3:
                //visualSnow.enabled = true;

                
                break;

            case MGS.MG4:
                //shape.enabled = true;
                break;

            case MGS.MG5:
                //memory.enabled = true;
                yield return StartCoroutine(ShowKeyHelp());
                MiniGames[mgState].Find("Canvas").GetComponent<Animator>().SetTrigger("In");

                break;
        }

    }

    public void ChangeSprite(Sprite sprite)
    {
        MiniGames[mgState].Find("Ch").GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
        MiniGames[mgState].Find("Ch").GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public void BlackSprite()
    {
        MiniGames[mgState].Find("Ch").GetComponent<SpriteRenderer>().color = new Color32(52, 52, 52, 255);
    }

    public void ResetSprite()
    {
        MiniGames[mgState].Find("Ch").GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);

    }
    private void DisableAnimator()
    {
        MiniGames[mgState].Find("Ch").GetComponent<Animator>().enabled = false;
    }

    private void ActivaAnimator()
    {
        MiniGames[mgState].Find("Ch").GetComponent<Animator>().enabled = true;
        MiniGames[mgState].Find("Ch").GetChild(0).GetComponent<Animator>().enabled = true;
    }

    private IEnumerator ChangeAnimatorState()
    {
        yield return null;
        MiniGames[mgState].Find("Ch").GetComponent<Animator>().SetTrigger("GoToIdle");

        StartCoroutine(CageFall(MiniGames[MGS.MG1].Find(Objects.CAGEF.ToString()).GetComponent<Animator>(),
                    Camera.main.transform, MG1CamPos[1], MiniGames[MGS.MG1].Find(Objects.MG1Camera.ToString()), MG1CamPos[2]));

    }

    private IEnumerator MG3Waiter()
    {
        yield return StartCoroutine(PlayAnimation(MiniGames[MGS.MG3].Find(Objects.BUTTON.ToString()).GetComponent<Animator>(), "Press", 0.2f));
        //MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).Find(Objects.SnowMask.ToString()).GetComponent<Animator>().SetTrigger("Snow");
        MiniGames[MGS.MG3].Find("Snow").GetComponent<ParticleSystem>().Play();
        //yield return StartCoroutine(PlayAnimation(MiniGames[MGS.MG3].Find(Objects.BUTTON.ToString()).GetComponent<Animator>(), "Back", 0.2f));


        ChangeCamera(MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()));
        //yield return new WaitForSeconds(MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).Find(Objects.SnowMask.ToString()).GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length - 1f);

        yield return new WaitForSeconds(3);
        MiniGames[MGS.MG3].Find("Snow").SetParent(MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()));
        StartCoroutine(CageFall(MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).GetComponent<Animator>(),
        Camera.main.transform, MG3CamPos[1], MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()), MG3CamPos[2]));

    }
    private IEnumerator PlayAnimation(Animator anim, string trigger, float waitTime)
    {
        anim.SetTrigger(trigger);
        yield return new WaitForSeconds(waitTime);


    }

    public void ChangeWinLoseState(WinLoseState state)
    {
        winState[mgState] = state;
        switch (mgState)
        {
            case MGS.MG1:
                WinLose.MG1 = state == WinLoseState.Win ? true : false;
                break;
            case MGS.MG2:
                WinLose.MG2 = state == WinLoseState.Win ? true : false;
                break;
            case MGS.MG3:
                WinLose.MG3 = state == WinLoseState.Win ? true : false;
                break;

            case MGS.MG4:
                WinLose.MG4 = state == WinLoseState.Win ? true : false;
                break;
            case MGS.MG5:
                WinLose.MG5 = state == WinLoseState.Win ? true : false;
                break;
        }
        StartCoroutine(WrapGameUp());

    }

    private IEnumerator WrapGameUp()
    {
        yield return null;
        switch (mgState)
        {
            case MGS.MG1:
                dancingLetters.enabled = false;
                
                yield return StartCoroutine(CamZoomOut(Camera.main.transform, CamMain.transform.position, true, true, CamMain.transform.eulerAngles));
                MiniGames[MGS.MG1].Find("Ch").GetComponent<Animator>().enabled = true;

                MiniGames[MGS.MG1].Find("Ch").GetComponent<Animator>().SetTrigger("SRead");
                MiniGames[MGS.MG1].Find(Objects.MG1Camera.ToString()).tag = "Untagged";
                MiniGames[MGS.MG1].Find(Objects.MG1Camera.ToString()).gameObject.SetActive(false);
                CamMain.tag = "MainCamera";
                
                CamMain.SetActive(true);
                playing = false;
                GameStates[MGS.MG1] = false;
                ChangeSMGState(MGS.MG4);
                MiniGames[MGS.MG1].GetComponent<CapsuleCollider>().enabled = false;
                mgState = MGS.None;
                break;

            case MGS.MG2:
                trail.enabled = false;
                yield return StartCoroutine(CamZoomOut(Camera.main.transform, CamMain.transform.position, true, true, CamMain.transform.eulerAngles));
                MiniGames[MGS.MG2].Find(Objects.MG2Camera.ToString()).tag = "Untagged";
                MiniGames[MGS.MG2].Find(Objects.MG2Camera.ToString()).gameObject.SetActive(false);
                CamMain.tag = "MainCamera";
                CamMain.SetActive(true);
                playing = false;
                GameStates[MGS.MG2] = false;
                ChangeSMGState(MGS.MG4);
                MiniGames[MGS.MG2].GetComponent<CapsuleCollider>().enabled = false;
                mgState = MGS.None;
                break;

            case MGS.MG3:
                //visualSnow = FindObjectOfType<VisualSnow>();
                //visualSnow.enabled = false;
                ChangeCamera(MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()));
                MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()).GetComponent<Camera>().fieldOfView = 94;
                MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()).Find("Canvas").Find("Canvas").gameObject.SetActive(true);
                MiniGames[MGS.MG3].Find(Objects.MG3Camera.ToString()).gameObject.SetActive(false);
                //yield return StartCoroutine(CamZoomOut(Camera.main.transform, MG3CamPos[3], true, true, new Vector3(-22.4f, 180,0)));
                yield return StartCoroutine(CamZoomOut(Camera.main.transform, CamMain.transform.position, true, true, CamMain.transform.eulerAngles));

                MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()).tag = "Untagged";
                MiniGames[MGS.MG3].Find(Objects.MG3AnimCamera.ToString()).gameObject.SetActive(false);
                CamMain.tag = "MainCamera";
                CamMain.transform.eulerAngles = new Vector3(322.299988f, 0, 0);
                CamMain.SetActive(true);
                playing = false;
                GameStates[MGS.MG3] = false;
                ChangeSMGState(MGS.MG4);
                MiniGames[MGS.MG3].GetComponent<CapsuleCollider>().enabled = false;
                mgState = MGS.None;
                break;

            case MGS.MG4:
                //shape.enabled = false;
                MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()).GetComponent<Camera>().fieldOfView = 60;
                ChangeCamera(MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()));
                MiniGames[MGS.MG4].Find(Objects.ShapeScene.ToString()).Find("Canvas").gameObject.SetActive(false);
                MiniGames[MGS.MG4].Find(Objects.ShapeScene.ToString()).gameObject.SetActive(false);
                MiniGames[MGS.MG4].Find("Ch").Find("Ending").gameObject.SetActive(true);

                //yield return StartCoroutine(CamZoomOut(Camera.main.transform, MG4CamPos[3], true, true, new Vector3(-22.4f, 180, 0)));
                yield return StartCoroutine(CamZoomOut(Camera.main.transform, CamMain.transform.position, true, true, CamMain.transform.eulerAngles));

                MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()).tag = "Untagged";
                MiniGames[MGS.MG4].Find(Objects.MG4AnimCamera.ToString()).gameObject.SetActive(false);
                CamMain.tag = "MainCamera";
                CamMain.transform.eulerAngles = new Vector3(322.299988f, 0, 0);
                CamMain.SetActive(true);
                playing = false;
                GameStates[MGS.MG4] = false;
                GameStates[MGS.MG5] = true;
                MiniGames[MGS.MG3].GetComponent<CapsuleCollider>().enabled = false;
                mgState = MGS.None;

                break;

            case MGS.MG5:

                memoryScene.GetComponent<Memory>().enabled = false;
                yield return StartCoroutine(ShowMG5Pics());
                
                
                character.ending = true;
                ending = true;

                float timer = 0;
                Transform cam = Camera.main.transform;
                Vector3 target = endCam.position;
                Vector3 angle = endCam.eulerAngles;
                while (cam.localPosition != target)
                {
                    yield return new WaitForEndOfFrame();
                    timer += Time.deltaTime;

                    cam.position = Vector3.Lerp(cam.position, target, Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
                    cam.rotation = Quaternion.Lerp(cam.rotation, Quaternion.Euler(angle.x, angle.y, angle.z), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));

                    if (cam.position == target)
                    {
                        break;
                    }
                    if (Mathf.Abs(cam.position.z - target.z) < 1.5f)
                    {
                        MiniGames[MGS.MG5].Find("Canvas").GetComponent<Animator>().ResetTrigger("In");
                        MiniGames[MGS.MG5].Find("Canvas").GetComponent<Animator>().SetTrigger("Out");
                        MiniGames[MGS.MG5].Find("Ch").gameObject.SetActive(true);
                        MiniGames[MGS.MG5].Find("Ch").GetComponent<SpriteRenderer>().sprite = originals[4];

                        memoryScene.SetActive(false);
                        for (int i = 0; i < MiniGames[MGS.MG5].Find("Canvas").childCount; i++)
                        {
                            MiniGames[MGS.MG5].Find("Canvas").GetChild(i).gameObject.SetActive(false);
                        }


                    }
                }

                MiniGames[MGS.MG5].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().ResetTrigger("Fall");
                MiniGames[MGS.MG5].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("GoBack");


                MiniGames[MGS.MG5].Find(Objects.MG5Camera.ToString()).tag = "Untagged";
                MiniGames[MGS.MG5].Find(Objects.MG5Camera.ToString()).gameObject.SetActive(false);
                //CamMain.tag = "MainCamera";
                //CamMain.SetActive(true);
                endAnimCam.tag = "MainCamera";
                endAnimCam.SetActive(true);
                playing = false;
                GameStates[MGS.MG5] = false;
                endAnimCam.GetComponent<Animator>().SetTrigger("Play");
                StartCoroutine(LoadMainScene());
                yield return new WaitForSeconds(848 / 60f);
                yield return StartCoroutine(sceneAudio.FadeIn());
                shouldLoad = true;
                //MiniGames[MGS.MG5].GetComponent<CapsuleCollider>().enabled = false;
                //mgState = MGS.None;
                //ChangeCamera(MiniGames[MGS.MG5].Find(Objects.MG5AnimCamera.ToString()));

                break;
        }
        sceneAudio.SwitchBack();
    }
    
    private void ChangeSMGState(MGS state)
    {
        switch (state)
        {
            case MGS.MG4:
                if (GameStates[MGS.MG1] == false && GameStates[MGS.MG2] == false && GameStates[MGS.MG3] == false)
                {
                    GameStates[MGS.MG4] = true;
                }
                break;

            case MGS.MG5:
                if (GameStates[MGS.MG4] == false)
                {
                    GameStates[MGS.MG5] = true;
                }
                break;
        }
    }

    private IEnumerator PlayBeginingAnims()
    {
        yield return new WaitForSecondsRealtime(66/60f);
        lantentBase.GetComponent<Animator>().SetTrigger("Flicker");
        MiniGames[MGS.MG1].Find("Ch").GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG1].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG2].Find("Ch").GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG3].Find("Ch").GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG4].Find("Ch").GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG4].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG5].Find("Ch").GetComponent<Animator>().SetTrigger("Up");
        MiniGames[MGS.MG5].Find(Objects.CAGEF.ToString()).GetComponent<Animator>().SetTrigger("Up");
        yield return new WaitForSeconds(MiniGames[MGS.MG1].Find("Ch").GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        MiniGames[MGS.MG1].Find("Ch").GetComponent<Animator>().SetTrigger("Read");
        MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).Find("MSREEN").Find("StaticBackground").SetParent(MiniGames[MGS.MG2]);
        MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).Find("MSREEN").SetParent(MiniGames[MGS.MG2]);
        MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).Find("BUTTON1").SetParent(MiniGames[MGS.MG2]);
        MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).Find("BUTTON2").SetParent(MiniGames[MGS.MG2]);
        MiniGames[MGS.MG2].Find(Objects.CAGEF.ToString()).Find("Static").SetParent(MiniGames[MGS.MG2]);
        MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).Find("BUTTON").SetParent(MiniGames[MGS.MG3]);
        MiniGames[MGS.MG3].Find(Objects.CAGEF.ToString()).Find("Snow").SetParent(MiniGames[MGS.MG3]);
        MiniGames[MGS.MG4].Find(Objects.CAGEF.ToString()).Find("HANDS").SetParent(MiniGames[MGS.MG4]);


        canMove = true;



    }

    private IEnumerator ShowMG5Pics()
    {
        
        MG5PicPage.SetActive(true);
        foreach (Texture t in MG5Pics)
        {
            MG5PicHolder.GetComponent<RawImage>().texture = t;
            yield return new WaitForSeconds(1);
        }
        MG5PicPage.SetActive(false);
        yield return new WaitForSeconds(0.2f);
    }

    private IEnumerator LoadMainScene()
    {
        AsyncOperation loading = SceneManager.LoadSceneAsync("EndingScene");
        loading.allowSceneActivation = false;
        yield return new WaitUntil(() => shouldLoad);
        loading.allowSceneActivation = true;
    }
}
