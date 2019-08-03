using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knife : MonoBehaviour
{
    public float Speed;
    [HideInInspector]
    public bool Shot;

    public SpriteRenderer myrenderer;
    [SerializeField]
    private AudioClip shootSound;
    [SerializeField]
    private AudioClip hitSound;
    [SerializeField]
    private AudioClip crashSound; //Knife in Knife

    public Collider2D myCollider;
    public Rigidbody2D myRigid;

    private Vector3 lastpos;
    private Vector3 intialpos;
    private AudioSource MyAudioSource;
    // Start is called before the first frame update
    void Start()
    {
        intialpos = transform.position;
        myRigid = GetComponent<Rigidbody2D>();
        MyAudioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        lastpos = transform.position;

        if (Shot)
        {
            transform.position += Vector3.up * Speed * Time.deltaTime;

            RaycastHit2D Hit = Physics2D.Linecast(lastpos, transform.position);

            if (Hit.collider != null)
            {
                Shot = false;
                if (Hit.transform.tag == "KnifeTag")
                {
                    // lose
                    LevelManager.instance.WrongHit();
                    myRigid.bodyType = RigidbodyType2D.Dynamic;
                    myRigid.AddTorque(7,ForceMode2D.Impulse);
                    MyAudioSource.clip = crashSound;
                    MyAudioSource.Play();
                }
                else
                {
                    transform.position = new Vector3(transform.position.x, Hit.point.y, transform.position.z);
                    transform.parent = Hit.transform;
                    myCollider.enabled = true;
                    LevelManager.instance.SuccessfulHit(myRigid);// GetComponent<Rigidbody2D>()
                    MyAudioSource.clip = hitSound;
                    MyAudioSource.Play();
                }
            }
        }

    }
    public void playAnimation()
    {
        StartCoroutine("ShowKnife");
    }
    public void shoot()
    {
        Shot = true;
        MyAudioSource.clip = shootSound;
        MyAudioSource.Play();
    }
    IEnumerator ShowKnife()
    {
        yield return new WaitForEndOfFrame();
        float Starttime = Time.time;
        float duration = 0.3f;
        Vector3 downpos = intialpos -new Vector3(0,0.5f,0);
        while (Time.time < Starttime + duration)
        {
            float t = (Time.time - Starttime) / duration;
            myrenderer.color = new Color(myrenderer.color.a,myrenderer.color.g,myrenderer.color.b,Mathf.Lerp(0,1,t));
            transform.position = Vector3.Lerp(downpos,intialpos,t);
            yield return null;

        }
        myrenderer.color = new Color(myrenderer.color.a, myrenderer.color.g, myrenderer.color.b, 1);
        transform.position = intialpos;
    }
}
