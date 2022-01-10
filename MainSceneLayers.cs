using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MainSceneLayers : MonoBehaviour
{
    public bool GuardPlaying = false;
    public GameObject[] rotationObjects;
    public GameObject[] objects;
    private MainSceneCharacter character;
    public GameObject Trees;
    public GameObject Lanterns;
    public LayerOrders sceneOrder;
    public bool[] verticals = new bool[] { false, false, false, false, false };
    public bool[] horizontals = new bool[] { false, false, false, false, false };
    private Dictionary<string, bool[]> triggers = new Dictionary<string, bool[]>();
    public bool TeacherConvo = false;

    void Start()
    {
        character = FindObjectOfType<MainSceneCharacter>();

        //StartCoroutine(DisableAnimators());
        //call this after cutscene finishes
        foreach (LayerOrders.Order o in sceneOrder.orders)
        {
            triggers.Add(o.name, new bool[] { false, false });
        }
    }

    void Update()
    {
        
        RotateObjects();

        foreach (KeyValuePair<string, bool[]> kp in triggers)
        {
            string key = kp.Key;
            //print(key.Contains("First").ToString() + " " + key.Contains("Middle").ToString());

            if (key.LastIndexOf("Front1") != -1)
            {
                //print("First");
                kp.Value[0] = verticals[0];
            }
            else if (key.LastIndexOf("Front2") != -1)
            {
                //print("Second");
                kp.Value[0] = verticals[1];
            }
            else if (key.LastIndexOf("MiddleV") != -1)
            {
                //print("Third");
                kp.Value[0] = verticals[2];
            }
            else if (key.LastIndexOf("Back1") != -1)
            {
                //print("Third");
                kp.Value[0] = verticals[4];
            }
            else if (key.LastIndexOf("Back2") != -1)
            {
                //print("Third");
                kp.Value[0] = verticals[3];
            }

            if (key.LastIndexOf("Left1") != -1)
            {
                //print("Left");
                kp.Value[1] = horizontals[1];
            }
            else if (key.LastIndexOf("Left2") != -1)
            {
                //print("Middle");
                kp.Value[1] = horizontals[0];
            }
            else if (key.LastIndexOf("MiddleH") != -1)
            {
                //print("Right");
                kp.Value[1] = horizontals[2];
            }
            else if (key.LastIndexOf("Right1") != -1)
            {
                //print("Right");
                kp.Value[1] = horizontals[4];
            }
            else if (key.LastIndexOf("Right2") != -1)
            {
                //print("Right");
                kp.Value[1] = horizontals[3];
            }
        }

        foreach (KeyValuePair<string, bool[]> kp in triggers)
        {
            if (kp.Value[0] == true && kp.Value[1] == true)
            {
                for (int o = 0; o < sceneOrder.orders.Length; o++)
                {

                    var order = sceneOrder.orders[o];
                    if (order.name == kp.Key)
                    {

                        print(kp.Key);
                        for (int l = 0; l < order.layer.Length; l++)
                        {
                            var layer = order.layer[l];

                            for (int ob = 0; ob < layer.objLocs.Length; ob++)
                            {
                                var obj = layer.objLocs[ob];
                                if (objects.Where(tmp => tmp.name == obj.name).SingleOrDefault() != null)
                                {
                                    GameObject selObj = objects.Where(tmp => tmp.name == obj.name).SingleOrDefault();
                                    
                                    if (selObj.TryGetComponent(typeof(Renderer), out Component comp))
                                    {
                                        if (selObj.name == "GUARD" && GuardPlaying)
                                        {
                                            selObj.GetComponent<Renderer>().sortingOrder = 99;
                                            selObj.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID("FirstLayer");
                                        }
                                        else 
                                        {
                                            selObj.GetComponent<Renderer>().sortingOrder = obj.objLoc;
                                            selObj.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID(layer.name);
                                        
                                        }
                                    }

                                    for (int i = 0; i < obj.locs.Length; i++)
                                    {
                                        var inside = obj.locs[i];
                                        if (selObj.transform.Find(inside.name) != null)
                                        {
                                            
                                               
                                            //print(obj.name);
                                            //print(inside.name);
                                            //print(layer.name);
                                           
                                                selObj.transform.Find(inside.name).GetComponent<Renderer>().sortingOrder = inside.locs;
                                                selObj.transform.Find(inside.name).GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID(layer.name);



                                        }
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

    }

    private void RotateObjects()
    {
        foreach (GameObject obj in rotationObjects) 
        {

            if (obj.name == "Teacher" && !TeacherConvo)
            {
                print("DETECT");
                Quaternion rot = Quaternion.LookRotation(obj.transform.position - character.GetComponent<Transform>().position);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));

            }    
            else if (obj.name == "Teacher" && TeacherConvo)
            {
                
                if (obj.transform.localRotation != Quaternion.Euler(0,0,0))
                {
                    StartCoroutine(RotateTeacher(obj));
                }

                
            }

            else if (obj.name == "ChildBS" && TeacherConvo)
            {
                obj.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
            else
            {
                Quaternion rot = Quaternion.LookRotation(obj.transform.position - character.GetComponent<Transform>().position);
                obj.transform.rotation = Quaternion.Euler(new Vector3(0, rot.eulerAngles.y, 0));
            }
        }
    }

    public IEnumerator DisableAnimators()
    {

        //yield return new WaitForSeconds(Trees.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        yield return new WaitForSeconds(Lanterns.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).length);
        Trees.GetComponent<Animator>().enabled = false;
        Lanterns.GetComponent<Animator>().enabled = false;

        foreach (GameObject obj in rotationObjects)
        {
            if (obj.TryGetComponent(typeof(Animator), out Component comp))
            {
                obj.GetComponent<Animator>().enabled = false;
            }
        }
    }

    private IEnumerator RotateTeacher(GameObject obj)
    {
        float timer = 0;
        while (obj.transform.localRotation != Quaternion.Euler(0,0,0))
        {
            yield return new WaitForEndOfFrame();
            timer += Time.deltaTime;

            obj.transform.localRotation = Quaternion.Lerp(obj.transform.localRotation, Quaternion.Euler(0, 0, 0), Mathf.SmoothStep(0, 1, Mathf.Log(timer)));

            if (obj.transform.localRotation == Quaternion.Euler(0, 0, 0))
            {
                break;
            }
        }
    }
}
