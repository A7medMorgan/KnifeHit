using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    [SerializeField]
    private SpriteRenderer Flasher;

    [SerializeField]
    private GameObject destroyedTarget;

    [SerializeField]
    private List<Rigidbody2D> mylistOFpieces;

    private float RoundRotationSpeed;
    private float RoundStartTime;
    private float RoundDuration;

    float level;

    private Vector3 intialpos;
    // Start is called before the first frame update
    void Start()
    {
        intialpos = transform.position;
        NewRound();
    }

    // Update is called once per frame
    void Update()
    {
        float t = (Time.time - RoundStartTime) / RoundDuration;
        t = 1 - t;
        float curRotationSpeed = RoundRotationSpeed * t;
        transform.Rotate(new Vector3(0, 0, curRotationSpeed) * Time.deltaTime);
        if (t < 0.05f)
        {
            NewRound();
        }
    }

    void NewRound()
    {
        //float starttime = Time.time;
        //float duration = 0.025f;
        //level = (Time.time - starttime) / duration;


        RoundStartTime = Time.time;
        float roundPower = Random.Range(0,1f);
        RoundRotationSpeed = -150 - 150*roundPower;
        RoundDuration = 5 + 5 * roundPower;

    }
    public void GotHit()
    {
        // Stop the previous coroutine
        StopCoroutine("PushingTarget");
        StopCoroutine("Flashing");
        //Start the new coroutine
        StartCoroutine("PushingTarget");
        StartCoroutine("Flashing");
    }
    IEnumerator PushingTarget()
    {
        float starttime = Time.time;
        float duration = 0.025f;

        Vector3 uppos = intialpos + new Vector3(0,0.1f,0);
        while (Time.time<starttime+duration)
        {
            float t = (Time.time - starttime) / duration;
            transform.position = Vector3.Lerp(intialpos,uppos,t);
            yield return null;
        }
        starttime = Time.time;
        duration = 0.2f;
        while (Time.time<starttime+duration)
        {
            float t = (Time.time - starttime) / duration;
            t = 1 - Mathf.Abs(Mathf.Pow(t-1,2));
            transform.position = Vector3.Lerp(uppos,intialpos, t);
            yield return null;
        }
        transform.position = intialpos;
    }
    IEnumerator Flashing()
    {
        float starttime = Time.time;
        float duration = 0.025f;

        while (Time.time < starttime + duration)
        {
            float t = (Time.time - starttime) / duration;
            Flasher.color = new Color(Flasher.color.r, Flasher.color.g, Flasher.color.b,Mathf.Lerp(0,0.2f,t));
            yield return null;
        }
        starttime = Time.time;
        duration = 0.2f;
        while (Time.time < starttime + duration)
        {
            float t = (Time.time - starttime) / duration;
            t = 1 - Mathf.Abs(Mathf.Pow(t - 1, 2));
            Flasher.color = new Color(Flasher.color.r, Flasher.color.g, Flasher.color.b, Mathf.Lerp(0.2f,0,t));
            yield return null;
        }
        transform.position = intialpos;
        Flasher.color = new Color(Flasher.color.r, Flasher.color.g, Flasher.color.b, 0);
    }

    public void DestroyTarget()
    {
        destroyedTarget.transform.parent = null;
        destroyedTarget.SetActive(true);
        for (int i = 0; i < mylistOFpieces.Count; i++)
        {
            Vector3 ForceDirection = (mylistOFpieces[i].transform.position -transform.position).normalized *5;
            ForceDirection.y = ForceDirection.y < 0 ? ForceDirection.y * -1 : ForceDirection.y;
            mylistOFpieces[i].AddForceAtPosition(ForceDirection,transform.position,ForceMode2D.Impulse);
            mylistOFpieces[i].AddTorque(3,ForceMode2D.Impulse);
            Destroy(mylistOFpieces[i].gameObject,3);
        }
        Destroy(gameObject);
    }
}
