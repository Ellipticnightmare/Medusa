using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGameplayContr : MonoBehaviour
{
    public float speed = 10;
    public bool canHold = true;
    public GameObject curHeldObj;
    public Transform guide;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.E))
        {
            if (!canHold)
                throwItem();
            else
            {
                RaycastHit hit;
                if (Physics.SphereCast(Camera.main.transform.position, .5f, Camera.main.transform.forward, out hit))
                {
                    if (hit.collider.CompareTag("Interactable"))
                        grabItem(hit.collider.gameObject);
                }
            }
        }
        if (!canHold && curHeldObj)
            curHeldObj.transform.position = guide.position;
        RaycastHit hit2;
        if (Physics.SphereCast(Camera.main.transform.position, .5f, Camera.main.transform.forward, out hit2))
        {
            if (hit2.collider.CompareTag("Interactable"))
                GameManager.isHitting = true;
            else
                GameManager.isHitting = false;
        }
        else
            GameManager.isHitting = false;
    }
    void grabItem(GameObject grabItem)
    {
        grabItem.transform.SetParent(guide);
        canHold = false;
        grabItem.transform.position = guide.position;
        grabItem.transform.localRotation = transform.rotation;
        grabItem.GetComponent<Rigidbody>().useGravity = false;
        grabItem.GetComponent<Rigidbody>().isKinematic = false;
        curHeldObj = grabItem;
    }
    void throwItem()
    {
        if (curHeldObj.name == "knife")
        {
            RaycastHit hit;
            if (Physics.SphereCast(Camera.main.ViewportPointToRay(new Vector3(0, 0, 0)), .5f, out hit, 5f))
            {
                if (hit.collider.gameObject.GetComponent<MedusaController>() != null)
                    hit.collider.gameObject.GetComponent<MedusaController>().KilledMe();
            }
        }
        else if(curHeldObj.name == "key")
        {
            RaycastHit hit;
            if (Physics.SphereCast(Camera.main.ViewportPointToRay(new Vector3(0, 0, 0)), .5f, out hit, 5f))
            {
                if (hit.collider.gameObject.CompareTag("Door"))
                    FindObjectOfType<GameManager>().EscapedPlayer();
            }
        }
        curHeldObj.GetComponent<Rigidbody>().useGravity = true;
        if (curHeldObj.GetComponent<BreakableWindow>() != null)
            curHeldObj.GetComponent<BreakableWindow>().canShatter = true;
        curHeldObj = null;
        guide.GetChild(0).gameObject.GetComponent<Rigidbody>().velocity = transform.forward * speed;
        guide.GetChild(0).parent = null;
        canHold = true;
    }
}