using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndingScene : MonoBehaviour
{
    public WLState WinLose;
    private bool[] states = new bool[5];
    private int count = 0;
    public Transform ZoomOut;
    public Transform ZoomIn;
    public GameObject animCam;
    private SceneDialogueSystem dialogueSystem;
    public GameObject credits;
    private bool zoomIn = false;
    private MainSceneAudio sceneAudio;
    public AudioClip creditsClip;
    public SpriteRenderer Guard;
    public Sprite guardDef;

    void Start()
    {
        states[0] = WinLose.MG1;
        states[1] = WinLose.MG2;
        states[2] = WinLose.MG3;
        states[3] = WinLose.MG4;
        states[4] = WinLose.MG5;
        dialogueSystem = FindObjectOfType<SceneDialogueSystem>();
        CountStates();
        sceneAudio = FindObjectOfType<MainSceneAudio>();
        StartCoroutine(ManageEnding());

        
    }

    private IEnumerator ManageEnding()
    {
        yield return new WaitForSeconds(1);

        yield return StartCoroutine(Zoom(ZoomOut.position, Vector3.zero));
        
        yield return StartCoroutine(dialogueSystem.RunConversation(SceneDialogueSystem.Convo.GuardExit));

        yield return new WaitWhile(() => dialogueSystem.running);
        switch (count)
        {
            case 0:
                yield return StartCoroutine(dialogueSystem.RunConversation(SceneDialogueSystem.Convo.Ending1));

                break;

            case 1:
                yield return StartCoroutine(dialogueSystem.RunConversation(SceneDialogueSystem.Convo.Ending2));

                break;

            case 2:
                yield return StartCoroutine(dialogueSystem.RunConversation(SceneDialogueSystem.Convo.Ending3));

                break;

            case 3:
                yield return StartCoroutine(dialogueSystem.RunConversation(SceneDialogueSystem.Convo.Ending4));

                break;


        }
        yield return new WaitWhile(() => dialogueSystem.running);
        zoomIn = true;
        Guard.sprite = guardDef;
        yield return StartCoroutine(Zoom(ZoomIn.position, Vector3.zero));
        credits.SetActive(true);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(Credits());
    }

    private IEnumerator Credits()
    {
        yield return null;

        for (int i = 0; i < credits.transform.childCount; i++)
        {
            credits.transform.GetChild(i).gameObject.SetActive(true);
            yield return new WaitForSeconds(4f);
            credits.transform.GetChild(i).gameObject.SetActive(false);
        }

        float timer = 0f;
        while (credits.GetComponent<Image>().color != new Color(0f,0f,0f,1f))
        {
            
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            credits.GetComponent<Image>().color = Color.Lerp(credits.GetComponent<Image>().color, new Color(0f, 0f, 0f, 1f), timer/5);
            if (credits.GetComponent<Image>().color == new Color(0f, 0f, 0f, 1f))
            {
                break;
            }
        }

        timer = 0;
        while (sceneAudio.GetComponent<AudioSource>().volume != 0f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            sceneAudio.GetComponent<AudioSource>().volume = Mathf.Lerp(sceneAudio.GetComponent<AudioSource>().volume, 0f, timer / 100f);
            if (sceneAudio.GetComponent<AudioSource>().volume < 0.1f)
            {
                sceneAudio.GetComponent<AudioSource>().Stop();
                break;
            }
        }
        sceneAudio.GetComponent<AudioSource>().volume = 0.5f;
        SceneManager.LoadScene("Menu");

    } 

    
    private IEnumerator Zoom(Vector3 target, Vector3 rotation)
    {


        float timer = 0;
        while (animCam.transform.position != target)
        {

            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            animCam.transform.position = Vector3.Lerp(animCam.transform.position, target, Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
            animCam.transform.rotation = Quaternion.Lerp(animCam.transform.rotation, Quaternion.Euler(rotation.x, rotation.y, rotation.z), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));
           
            if (animCam.transform.position == target)
            {
                break;
            }
            if (zoomIn)
            {
                sceneAudio.GetComponent<AudioSource>().volume = Mathf.Lerp(sceneAudio.GetComponent<AudioSource>().volume, 0f, timer/ 100f);
            }
        }
        if (zoomIn)
        {
            sceneAudio.GetComponent<AudioSource>().clip = creditsClip;
            sceneAudio.GetComponent<AudioSource>().volume = 1f;
            sceneAudio.GetComponent<AudioSource>().Play();
            zoomIn = false;
            

        }
    }

    private void CountStates()
    {
    
        foreach (bool b in states)
        {
            if (!b)
            {
                count++;
            }
        }
    }
}
