using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MedusaController : MonoBehaviour
{
    [HideInInspector]
    public GameObject targPos;
    bool isAlive = true;
    public AudioClip[] roars, victoryVoiceLines;
    public AudioSource roarAud, voiceLineAud;
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<NavMeshAgent>().SetDestination(targPos.transform.position);
        roarAud.clip = roars[Random.Range(0, roars.Length)];
        roarAud.Play();
    }
    private void Update()
    {
        CharacterController chara = FindObjectOfType<CharacterController>();
        if (isTargetVisible(Camera.mainCamera, GetComponentInChildren<SkinnedMeshRenderer>().gameObject))
        {
            GameManager.stoneTimer -= Time.deltaTime;
        }
        /* if (GetComponentInChildren<SkinnedMeshRenderer>().isVisible && isAlive)
        {
            RaycastHit hit;
            if (Physics.Raycast(this.transform.position, (chara.transform.position - this.transform.position), out hit))
            {
                if (hit.collider.gameObject == chara.gameObject)
                    GameManager.stoneTimer -= Time.deltaTime;
            }
        }*/
        if (GameManager.stoneTimer <= 0 && !FindObjectOfType<GameManager>().BETA_Release)
        {
            voiceLineAud.clip = victoryVoiceLines[Random.Range(0, victoryVoiceLines.Length)];
            voiceLineAud.Play();
        }
        if (Vector3.Distance(this.transform.position, targPos.transform.position) < 2.5f)
        {
            Destroy(this.gameObject);
        }
    }
    bool isTargetVisible(Camera c, GameObject go)
    {
        var planes = GeometryUtility.CalculateFrustrumPlanes(c);
        var point = go.transform.position;
        foreach(var plane in planes)
        {
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        }
        return true;
    }
    public void KilledMe()
    {
        if (GameManager.canBeKilled)
        {
            GetComponent<Animator>().SetTrigger("Killed");
            isAlive = false;
            Destroy(this.GetComponent<NavMeshAgent>());
        }
    }
}