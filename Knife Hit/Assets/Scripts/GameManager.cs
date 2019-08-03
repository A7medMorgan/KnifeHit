using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance ;
    public int Score;
    [SerializeField]
    private Text Scoretxt;
    [SerializeField]
   private Image Fader;


    // Start is called before the first frame update
    void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ADDScore()
    {
        Score++;
        Scoretxt.text = Score.ToString();
    }
    public void resetScore()
    {
        Score = 0;
        Scoretxt.text = Score.ToString();
    }
    public void ReloadScene()
    {
        Fader.color = new Color(Fader.color.r, Fader.color.g, Fader.color.b, 0); // incase loss at the time of fading the new scene
        StopCoroutine("FaderReload");
        StartCoroutine("FaderReload");
    }
    IEnumerator FaderReload()
    {
        yield return new WaitForSeconds(2);
        Fader.gameObject.SetActive(true);

        float startTime=Time.time;
        float Duration = 0.2f;
        while (Time.time < startTime + Duration)
        {
            float t = (Time.time - startTime) / Duration;
            Fader.color = new Color(Fader.color.r, Fader.color.g, Fader.color.b ,Mathf.Lerp(0,1,t));
            yield return null;
        }
        Fader.color = new Color(Fader.color.r, Fader.color.g, Fader.color.b, 1);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

         startTime = Time.time;
        while (Time.time < startTime + Duration)
        {
            float t = (Time.time - startTime) / Duration;
            Fader.color = new Color(Fader.color.r, Fader.color.g, Fader.color.b, Mathf.Lerp(1, 0, t));
            yield return null;
        }
        Fader.color = new Color(Fader.color.r, Fader.color.g, Fader.color.b, 0);
    }
}
