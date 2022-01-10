using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using UnityEngine.AI;
using VHS;
using System.Diagnostics;

public class GameManager : MonoBehaviour
{
    public Text speedrunTimer;
    public System.TimeSpan completedTime;
    float timeElapsed;
    public bool BETA_Release;
    public GameObject playerPrefab, medusaPrefab, statuePrefab;
    public Transform createnewPlayer, animationSpawn;
    int escapedConvicts = 0;
    public static float stoneTimer = 4;
    public Image stoneTimerImg, centerIndicator;
    bool isMedusaActive = true;
    public static bool canBeKilled = false;
    public static bool isHitting;
    [Range(0, 5)]
    public int MedusaStomachCount;
    public Slider MedusaIndicator;
    public List<Transform> ObjPossibleSpawns = new List<Transform>();
    public GameObject keyJar, knifeJar, animationationer;

    public Sprite basic, pickup;
    public AudioClip[] MedusaGreetings;
    public AudioClip[] MedusaHungerGreeting;

    public GameObject taskHolder, taskObj;
    public GameObject[] centralUI;

    public System.TimeSpan timeElapses { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        MedusaIndicator.gameObject.SetActive(false);
        if (!BETA_Release)
            StartCoroutine(PlayOpeningAnimation());
        MedusaStomachCount = 2;
        int knifeLocation = Random.Range(0, ObjPossibleSpawns.Count);
        GameObject key = Instantiate(knifeJar, ObjPossibleSpawns[knifeLocation].position, Quaternion.identity);
        key.name = "knife";
        ObjPossibleSpawns.Remove(ObjPossibleSpawns[knifeLocation]);
        if (BETA_Release)
        {
            SpawnPlayer();
            StartCoroutine(timeMedusa());
            GameObject newTask01 = Instantiate(taskObj, taskHolder.transform);
            newTask01.GetComponentInChildren<Text>().text = "Avoid Medusa";
            GameObject newTask02 = Instantiate(taskObj, taskHolder.transform);
            newTask02.GetComponentInChildren<Text>().text = "Locate Key";
            GameObject newTask03 = Instantiate(taskObj, taskHolder.transform);
            newTask03.GetComponentInChildren<Text>().text = "Escape";
        }
    }
    IEnumerator PlayOpeningAnimation()
    {
        foreach (var item in centralUI)
        {
            item.SetActive(false);
        }
        GameObject animationManager = Instantiate(animationationer, animationSpawn.transform);
        Transform[] destroys = taskHolder.GetComponentsInChildren<Transform>();
        foreach (var item in destroys)
        {
            Destroy(item.gameObject);
        }
        yield return new WaitForSeconds(1.25f);
        MedusaIndicator.gameObject.SetActive(true);
        yield return new WaitForSeconds(7.66f);
        if (MedusaStomachCount > 0)
        {
            animationManager.GetComponentInChildren<AudioSource>().clip = MedusaGreetings[Random.Range(0, MedusaGreetings.Length)];
            animationManager.GetComponentInChildren<AudioSource>().Play();
            GameObject newTask01 = Instantiate(taskObj, taskHolder.transform);
            newTask01.GetComponentInChildren<Text>().text = "Avoid Medusa";
            yield return new WaitForSeconds(.25f);
            GameObject newTask02 = Instantiate(taskObj, taskHolder.transform);
            newTask02.GetComponentInChildren<Text>().text = "Locate Key";
            yield return new WaitForSeconds(.25f);
            GameObject newTask03 = Instantiate(taskObj, taskHolder.transform);
            newTask03.GetComponentInChildren<Text>().text = "Escape";
            yield return new WaitForSeconds(.25f);
        }
        else
        {
            animationManager.GetComponentInChildren<AudioSource>().clip = MedusaHungerGreeting[Random.Range(0, MedusaHungerGreeting.Length)];
            animationManager.GetComponentInChildren<AudioSource>().Play();
            GameObject newTask01 = Instantiate(taskObj, taskHolder.transform);
            newTask01.GetComponentInChildren<Text>().text = "Survive Medusa";
            yield return new WaitForSeconds(.25f);
            GameObject newTask02 = Instantiate(taskObj, taskHolder.transform);
            newTask02.GetComponentInChildren<Text>().text = "Locate Knife";
            yield return new WaitForSeconds(.25f);
            GameObject newTask03 = Instantiate(taskObj, taskHolder.transform);
            newTask03.GetComponentInChildren<Text>().text = "Kill Medusa";
            yield return new WaitForSeconds(.25f);
            GameObject newTask04 = Instantiate(taskObj, taskHolder.transform);
            newTask04.GetComponentInChildren<Text>().text = "Find Key";
            yield return new WaitForSeconds(.25f);
            GameObject newTask05 = Instantiate(taskObj, taskHolder.transform);
            newTask04.GetComponentInChildren<Text>().text = "Escape";
        }
        Destroy(animationManager);
        foreach (var item in centralUI)
        {
            item.SetActive(true);
        }
        MedusaIndicator.gameObject.SetActive(false);
        yield return new WaitForSeconds(.5f);
        SpawnPlayer();
        StartCoroutine(timeMedusa());
    }
    public void EscapedPlayer()
    {
        if (!isMedusaActive)
            UnityEngine.SceneManagement.SceneManager.LoadScene("VictoryScene");
        escapedConvicts++;
        Destroy(FindObjectOfType<CharacterController>().gameObject);
        StopAllCoroutines();
        if (!BETA_Release)
            StartCoroutine(PlayOpeningAnimation());
        else
            SpawnPlayer();
        MedusaStomachCount--;
        if (MedusaStomachCount <= 0)
        {
            canBeKilled = true;
            MedusaStomachCount = 0;
        }
    }
    void SpawnPlayer()
    {
        if (MedusaStomachCount < 5)
            Instantiate(playerPrefab, createnewPlayer.position, Quaternion.identity);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("FailedScene");
        CommandSpawns();
    }
    private void Update()
    {
        timeElapsed = Time.timeSinceLevelLoad;
        speedrunTimer.text = string.Format("{0:mm} : {0:ss} ", System.TimeSpan.FromSeconds(timeElapsed));
        stoneTimerImg.fillAmount = (float)stoneTimer / 4f;
        if (stoneTimer <= 0)
            killPlayer();
        MedusaIndicator.value = Mathf.Lerp(MedusaIndicator.value, MedusaStomachCount, 1f);
        if (isHitting)
            centerIndicator.sprite = pickup;
        else
            centerIndicator.sprite = basic;
    }
    void CommandSpawns()
    {
        List<Transform> spawnHolders = new List<Transform>();
        spawnHolders.AddRange(ObjPossibleSpawns);
        int keyLocation = Random.Range(0, spawnHolders.Count);
        GameObject knife = Instantiate(keyJar, spawnHolders[keyLocation].position, Quaternion.identity);
        knife.name = "key";
    }
    void PingMedusa()
    {
        MedusaController medusa = FindObjectOfType<MedusaController>();
        if (medusa == null)
        {
            CharacterController player = FindObjectOfType<CharacterController>();
            GameObject[] tunnels = GameObject.FindGameObjectsWithTag("TunnelPoint");
            Transform spawnpoint = null;
            float minRange = 10000;
            foreach (var item in tunnels)
            {
                if (Vector3.Distance(item.transform.position, player.transform.position) < minRange)
                {
                    minRange = Vector3.Distance(item.transform.position, player.transform.position);
                    spawnpoint = item.transform;
                }
            }
            GameObject naga = Instantiate(medusaPrefab, spawnpoint.transform.position, Quaternion.identity);
            if (naga != null)
                naga.GetComponent<MedusaController>().targPos = tunnels[Random.Range(0, tunnels.Length)];
            else
            {
                naga = FindObjectOfType<MedusaController>().gameObject;
                naga.GetComponent<MedusaController>().targPos = tunnels[Random.Range(0, tunnels.Length)];
            }
            if (naga.GetComponent<MedusaController>().targPos == spawnpoint)
                naga.GetComponent<MedusaController>().targPos = tunnels[Random.Range(0, tunnels.Length)];
        }
        StartCoroutine(timeMedusa());
    }
    IEnumerator timeMedusa()
    {
        yield return new WaitForSeconds(Random.Range(0, 20 - escapedConvicts) + Random.Range(1, 20 - escapedConvicts));
        PingMedusa();
    }
    public void killPlayer()
    {
        if (!BETA_Release)
            StartCoroutine(playerDeath());
        else
        {
            stoneTimer = 4;
            MedusaStomachCount++;
            MedusaController medusa = FindObjectOfType<MedusaController>();
            CharacterController player = FindObjectOfType<CharacterController>();
            Destroy(medusa.gameObject);
            Instantiate(statuePrefab, player.transform.position, player.transform.rotation);
            Destroy(player.gameObject);
            SpawnPlayer();
            StartCoroutine(timeMedusa());
        }
    }
    IEnumerator playerDeath()
    {
        stoneTimer = 4;
        MedusaStomachCount++;
        MedusaController medusa = FindObjectOfType<MedusaController>();
        CharacterController player = FindObjectOfType<CharacterController>();
        player.GetComponent<FirstPersonController>().enabled = false;
        medusa.targPos = player.gameObject;
        yield return new WaitForSeconds(1f);
        medusa.GetComponent<NavMeshAgent>().enabled = false;
        medusa.voiceLineAud.clip = medusa.victoryVoiceLines[Random.Range(0, medusa.victoryVoiceLines.Length)];
        medusa.voiceLineAud.Play();
        yield return new WaitForSeconds(4f);
        Destroy(medusa.gameObject);
        Instantiate(statuePrefab, player.transform.position, player.transform.rotation);
        Destroy(player.gameObject);
        StopAllCoroutines();
        StartCoroutine(PlayOpeningAnimation());
    }
    public void DefeatedMedusa()
    {
        isMedusaActive = false;
        PlayerPrefs.SetFloat("recentTime", timeElapsed);
        completedTime = System.TimeSpan.FromSeconds(Time.timeSinceLevelLoad);
    }
}