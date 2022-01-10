using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneAudio : MonoBehaviour
{

    public AudioSource source;
    public AudioClip mainTheme;
    public AudioClip loop;
    public AudioPosition pos;
    private bool mainPlaying = false;
    private bool fading = false;
    
    public MG1Sound mg1Sound;
    public MG2Sound mg2Sound;
    public MG3Sound mg3Sound;
    public MG4Sound mg4Sound;
    public MG5Sound mg5Sound;
    private static MainSceneAudio instance = null;
    public static MainSceneAudio Instance
    {
        get { return instance; }
    }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);
    }
    void Start()
    {
        
        mg1Sound = FindObjectOfType<MG1Sound>();
        mg2Sound = FindObjectOfType<MG2Sound>();
        mg3Sound = FindObjectOfType<MG3Sound>();
        mg4Sound = FindObjectOfType<MG4Sound>();
        mg5Sound = FindObjectOfType<MG5Sound>();
    }


    void Update()
    {
        if (mainPlaying)
        {
            if (source.time == mainTheme.length)
            {
                mainPlaying = false;
                source.clip = loop;
                source.loop = true;
                source.Play();
            }
        }   
    }

    public void PlayMain()
    {
        source.clip = mainTheme;
        source.volume = 1f;
        source.Play();
        mainPlaying = true;
        source.loop = false;
    }

    private IEnumerator FadeOut()
    {
        float timer = 0;
        fading = true;

        mg1Sound = FindObjectOfType<MG1Sound>();
        mg2Sound = FindObjectOfType<MG2Sound>();
        mg3Sound = FindObjectOfType<MG3Sound>();
        mg4Sound = FindObjectOfType<MG4Sound>();
        mg5Sound = FindObjectOfType<MG5Sound>();
        StartCoroutine(mg1Sound.FadeIn());
        StartCoroutine(mg2Sound.FadeIn());
        StartCoroutine(mg3Sound.FadeIn());
        StartCoroutine(mg4Sound.FadeIn());
        StartCoroutine(mg5Sound.FadeIn());
        while (source.spatialBlend < 0.839f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            source.spatialBlend = Mathf.Lerp(source.spatialBlend, 0.839f, timer);
            if (source.spatialBlend >= 0.839f)
            {
                break;
            }
        }
        fading = false;
    }
    public IEnumerator WritePos()
    {
        StartCoroutine(FadeOut());
        yield return new WaitWhile(() => fading);
        pos.pos = source.time;
    }

    public IEnumerator FadeIn()
    {
        float timer = 0;
        StartCoroutine(mg1Sound.FadeOut());
        StartCoroutine(mg2Sound.FadeOut());
        StartCoroutine(mg3Sound.FadeOut());
        StartCoroutine(mg4Sound.FadeOut());
        StartCoroutine(mg5Sound.FadeOut());
        while (source.spatialBlend > 0f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            source.spatialBlend = Mathf.Lerp(source.spatialBlend, 0f, timer);

            if (source.spatialBlend == 0)
            {
                break;
            }
        }
    }
    private IEnumerator Switch2D(AudioSource soruce)
    {
        float timer = 0;

        while (soruce.spatialBlend > 0.8f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            soruce.spatialBlend = Mathf.Lerp(soruce.spatialBlend, 0.8f, timer/10f);

            if (soruce.spatialBlend == 0.8f)
            {
                break;
            }
        }
    }
    private IEnumerator Switch3D(AudioSource soruce)
    {
        float timer = 0;

        while (soruce.spatialBlend < 1f)
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;
            soruce.spatialBlend = Mathf.Lerp(soruce.spatialBlend, 1f, timer / 10f);

            if (soruce.spatialBlend == 1)
            {
                break;
            }
        }
    }

    public void SwitchStates(TentLogic.MGS mgState)
    {
        AudioSource mg1Source = mg1Sound.GetComponent<AudioSource>();
        AudioSource mg2Source = mg2Sound.GetComponent<AudioSource>();
        AudioSource mg3Source = mg3Sound.GetComponent<AudioSource>();
        AudioSource mg4Source = mg4Sound.GetComponent<AudioSource>();
        AudioSource mg5Source = mg5Sound.GetComponent<AudioSource>();

        switch (mgState)
        {
            case TentLogic.MGS.MG1:
                StartCoroutine(Switch2D(mg1Source));
                mg2Source.minDistance = 0;
                mg3Source.minDistance = 0;
                mg4Source.minDistance = 0;
                mg5Source.minDistance = 0;
                
                break;
            case TentLogic.MGS.MG2:
                StartCoroutine(Switch2D(mg2Source));
                mg1Source.minDistance = 0;
                mg3Source.minDistance = 0;
                mg4Source.minDistance = 0;
                mg5Source.minDistance = 0;
                break;

            case TentLogic.MGS.MG3:
                StartCoroutine(Switch2D(mg3Source));
                mg2Source.minDistance = 0;
                mg1Source.minDistance = 0;
                mg4Source.minDistance = 0;
                mg5Source.minDistance = 0;
                break;

            case TentLogic.MGS.MG4:
                StartCoroutine(Switch2D(mg4Source));
                mg2Source.minDistance = 0;
                mg3Source.minDistance = 0;
                mg1Source.minDistance = 0;
                mg5Source.minDistance = 0;
                break;

            case TentLogic.MGS.MG5:
                StartCoroutine(Switch2D(mg5Source));
                mg2Source.minDistance = 0;
                mg3Source.minDistance = 0;
                mg4Source.minDistance = 0;
                mg1Source.minDistance = 0;
                break;
        }
    }

    public void SwitchBack()
    {
        AudioSource mg1Source = mg1Sound.GetComponent<AudioSource>();
        AudioSource mg2Source = mg2Sound.GetComponent<AudioSource>();
        AudioSource mg3Source = mg3Sound.GetComponent<AudioSource>();
        AudioSource mg4Source = mg4Sound.GetComponent<AudioSource>();
        AudioSource mg5Source = mg5Sound.GetComponent<AudioSource>();
        StartCoroutine(Switch3D(mg1Source));
        mg1Source.minDistance = 0.7f;
        StartCoroutine(Switch3D(mg2Source));
        mg2Source.minDistance = 0.7f;
        StartCoroutine(Switch3D(mg3Source));
        mg3Source.minDistance = 0.7f;
        StartCoroutine(Switch3D(mg4Source));
        mg4Source.minDistance = 0.7f;
        StartCoroutine(Switch3D(mg5Source));
        mg5Source.minDistance = 0.7f;
    }
}
