using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TentLayers : MonoBehaviour
{
 

    private LayerOrders layerOrders;
    private Dictionary<string, bool[]> triggers = new Dictionary<string, bool[]>();
    public LayerOrders tentOrder;
    public bool[] verticals = new bool[] { false, false, false };
    public bool[] horizontals = new bool[] { false, false, false };
    public List<GameObject> objects = new List<GameObject>();
    void Start()
    {
        foreach(LayerOrders.Order o in tentOrder.orders)
        {
            if (!triggers.ContainsKey(o.name))
            {
                triggers.Add(o.name, new bool[] { false, false });

            }
            
        }

        
    }

    void Update()
    {

        foreach(KeyValuePair<string, bool[]> kp in triggers)
        {
            string key = kp.Key;
            //print(key.Contains("First").ToString() + " " + key.Contains("Middle").ToString());

            if (key.LastIndexOf("First") != -1)
            {
                //print("First");
                kp.Value[0] = verticals[0];
            }
            else if (key.LastIndexOf("Second") != -1)
            {
                //print("Second");
                kp.Value[0] = verticals[1];
            }
            else if (key.LastIndexOf("Third") != -1)
            {
                //print("Third");
                kp.Value[0] = verticals[2];
            }
            
            if (key.LastIndexOf("Left") != -1)
            {
                //print("Left");
                kp.Value[1] = horizontals[0];
            }
            else if (key.LastIndexOf("Middle") != -1)
            {
                //print("Middle");
                kp.Value[1] = horizontals[1];
            }
            else if (key.LastIndexOf("Right") != -1)
            {
                //print("Right");
                kp.Value[1] = horizontals[2];
            }
        }

        foreach(KeyValuePair<string, bool[]> kp in triggers)
        {
            if (kp.Value[0] == true && kp.Value[1] == true)
            {
                for (int o = 0; o < tentOrder.orders.Length; o++)
                {
                    
                    var order = tentOrder.orders[o];
                    if (order.name == kp.Key)
                    {

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
                                        selObj.GetComponent<Renderer>().sortingOrder = obj.objLoc;
                                        selObj.GetComponent<Renderer>().sortingLayerID = SortingLayer.NameToID(layer.name);
                                    }

                                    for (int i = 0; i < obj.locs.Length; i++)
                                    {
                                        var inside = obj.locs[i];
                                        if (selObj.transform.Find(inside.name) != null)
                                        {
                                            if (inside.name == "Snow")
                                            {
                                                
                                                selObj.transform.Find(inside.name).GetComponent<ParticleSystemRenderer>().sortingOrder = inside.locs;
                                                selObj.transform.Find(inside.name).GetComponent<ParticleSystemRenderer>().sortingLayerID = SortingLayer.NameToID(layer.name);
                                            }
                                            else
                                            {
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
        
    }
}
