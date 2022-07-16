using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
using Cinemachine;
public class PlayerParent : MonoBehaviour
{
    [SerializeField] RectTransform levelIcon;
   [SerializeField] TextMeshProUGUI levelText, levelYearText;
    [SerializeField] Slider levelBar;
    [SerializeField] public List<GameObject> humans;
    public int currentYear;
    [SerializeField] TextMeshProUGUI yearText;
    [SerializeField] public GameObject yearCanvas;
    //public NavMeshAgent agent;
    public int level;
    PlayerControl playerControl;
    int currentPlayerCount;
    public int[] levelYears;

    [SerializeField] GameObject yearUpParticle;
    bool yearParticleActive = false;
    private void Start()
    {
        playerControl = GetComponent<PlayerControl>();
 
        level = 1;
        currentYear = 0;
        Globals.currentYear = 0;
        yearText.text = currentYear.ToString();
        //agent = GetComponent<NavMeshAgent>();
        //agent.enabled = false;

        levelText.text = (level).ToString();
        levelYearText.text = currentYear.ToString("N0") + "/" + levelYears[level].ToString();
        levelBar.value = 0f;
    }
    public void throughlyScale()
    {
    }
    IEnumerator scaleCalling()
    {
        int humanCount = humans.Count;
        for (int i = 0; i < humanCount - 1; i++)
        {
            StartCoroutine(throughlyScaling(humans[humanCount - 1 - i].transform));
            yield return new WaitForSeconds(0.05f);
        }
        yearParticleActive = false;
    }
    IEnumerator throughlyScaling(Transform hmn)
    {
        //////
        //if (yearParticleActive)
        //{
        //    GameObject yearUp = Instantiate(yearUpParticle, hmn.transform.position + new Vector3(0, 1, 0), Quaternion.Euler(-90, 0, 0));
        //    yearUp.transform.localScale = new Vector3(7, 7, 7);
        //}
        float counter = 0f;
        float firstSize = 1f;
        float sizeDelta;
        while (counter < Mathf.PI)
        {
            counter += 15 * Time.deltaTime;
            sizeDelta = 1f - Mathf.Abs(Mathf.Cos(counter));
            sizeDelta /= 3f;
            hmn.localScale = new Vector3(firstSize + sizeDelta, firstSize + sizeDelta, firstSize + sizeDelta);

            yield return null;
        }
        hmn.localScale = new Vector3(firstSize, firstSize, firstSize);

    }
    //public void UItargetSelect()
    // {
    //     if (humans.Count > 1)
    //     {
    //         direction.selectTarget(humans[humans.Count - 1].GetComponent<Employee>().jobId, transform);
    //     }
    //     else
    //     {
    //         direction.selectTarget(0, transform);
    //         direction.arrowScaleSet();
    //     }
    // }

    public void playerYearSet(int year)
    {
        currentYear += year;
        YearUpdate(year);

        //if (currentYear < 0)
        //{
        //    currentYear = 0;
        //    YearUpdate(-Globals.currentYear);
        //}
        //else
        //{
        //    YearUpdate(year);
        //}
        //yearText.text = currentYear.ToString();
        //Globals.currentYear = currentYear;
        //if (currentYear > 40)
        //{

        //}
        if (currentYear> levelYears[level] && levelYears[levelYears.Length - 1] > currentYear)
        {
            yearParticleActive = true;
            level++;
            evolutionSet();
            StartCoroutine(iconScaling(levelIcon));
        }
        if (level > 0)
        {
            if (currentYear < levelYears[level - 1])
            {
                yearParticleActive = false;

                level--;
                StartCoroutine(iconScaling(levelIcon));

                evolutionSet();
            }
        }
        if(level < 3)
        {
            Globals.maxEnemyCount = 150;
        }else if(level < 6)
        {
            Globals.maxEnemyCount = 200;
        }
        else
        {
            Globals.maxEnemyCount = 225;
        }
    }
    public void YearUpdate(int miktar)
    {
        levelText.text = (level).ToString();
        int yearOld = Globals.currentYear;
        Globals.currentYear = Globals.currentYear + miktar;
        LeanTween.value(yearOld, Globals.currentYear, 0.2f).setOnUpdate((float val) =>
        {
            yearText.text = ((int)val).ToString();
            levelYearText.text = ((int)val).ToString() + "/" + levelYears[level].ToString();
            //levelBar.fillAmount = (val) / levelYears[level];
            if (level > 0)
            {
                //levelYearText.text = (val - levelYears[level - 1]).ToString("N0") + "/" + levelYears[level].ToString();
                levelBar.value = (val - levelYears[level - 1]) / (levelYears[level] - levelYears[level - 1]);
            }
            else
            {
                //levelYearText.text = val.ToString("N0") + "/" + levelYears[level].ToString();
                levelBar.value = (val) / levelYears[level];
            }
        });//.setOnComplete(() =>{});
        //PlayerPrefs.SetInt("money", Globals.moneyAmount);

    }
    void evolutionSet()
    {
        currentPlayerCount = playerControl.players.Count;

        StartCoroutine(evolutionSett());
    }
    IEnumerator evolutionSett()
    {
        if (yearParticleActive)
        {
            GameObject yearUp = Instantiate(yearUpParticle, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(-90, 0, 0));
            yearUp.transform.localScale = new Vector3(7, 7, 7) * (1 + 0.1f * playerControl.players.Count);
        }
        for (int i = 0; i < playerControl.players.Count; i++)
        {
            GameObject player = Instantiate(humans[level], playerControl.players[i].transform.position, Quaternion.identity, this.transform);
            if (playerControl.players.Count > 1)
            {
                player.transform.localPosition = Vector3.zero;
            }
            player.GetComponent<PlayerEvolution>().maxHelath = GetComponent<PlayerControl>().currentHealth;
            Destroy(playerControl.players[i]);
            playerControl.players[i] = player;
            StartCoroutine(throughlyScaling(player.transform));
            targetSelectManager.Instance.Notify_ChangeObservers();

            yield return new WaitForSeconds(0.1f);

        }
    }


    IEnumerator iconScaling(RectTransform image)
    {
        float counter = 0;
        float scaleDelta = 0.3f;
        image.localScale = new Vector3(1, 1, 1);
        float scale = 0;
        while (counter < Mathf.PI)
        {
            counter += 5 * Time.deltaTime;
            scale = scaleDelta * Mathf.Abs(Mathf.Cos(counter));
            image.localScale = new Vector3(1 + scaleDelta - scale, 1 + scaleDelta - scale, 1 + scaleDelta - scale);
            yield return null;
        }
        image.localScale = new Vector3(1, 1, 1);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.name == "Right")
        {
            other.transform.parent.position += new Vector3(2000, 0, 0);
        }
        if (other.transform.name == "Left")
        {
            other.transform.parent.position += new Vector3(-2000, 0, 0);

        }
        if (other.transform.name == "Front")
        {
            other.transform.parent.position += new Vector3(0, 0, 2000);

        }
        if (other.transform.name == "Back")
        {
            other.transform.parent.position += new Vector3(0, 0, -2000);

        }
    }
}