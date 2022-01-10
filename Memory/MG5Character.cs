using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MG5Character : MonoBehaviour
{
    public bool inCol = false;
    public Vector3 prevPos;
    private Vector3 camPrevPos;
    public Collision col = null;
    public Texture picTexture = null;
    private GameObject foundPic = null;
    public void DestroyPicture()
    {
        picTexture = null;
        Destroy(foundPic);
    }
    private void OnTriggerEnter(Collider other)
    {
        foundPic = other.gameObject;
        picTexture = other.GetComponent<SpriteRenderer>().sprite.texture;
    }
    private void OnTriggerExit(Collider other)
    {
        foundPic = null;
        picTexture = null;
    }
    private void OnCollisionEnter(Collision collision)
    {
            inCol = true;
        col = collision;

    }

    private void OnCollisionExit(Collision collision)
    {
        inCol = false;
        col = null;
    }
}
