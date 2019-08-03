using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    [SerializeField]
    private GameObject KnifeGo;

    [SerializeField]
    private TargetController target;

    [SerializeField]
    private Transform KnifeSpawnpoint;
    [SerializeField]
    private List<Image> KnifeIcons;

    private List<Rigidbody2D> curKnifeHit =new List<Rigidbody2D>();

    [SerializeField]
    private int Knifecount;
    private int curKnifecount;

    private int hitsSucc;

    private Knife curKnife;
    private bool canPlay = true;
    
    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        Knifecount = Random.Range(5,11);

        for (int i = 0; i < KnifeIcons.Count; i++)
        {
            if (i >= Knifecount)
            {
                KnifeIcons[i].color = new Color(KnifeIcons[i].color.r, KnifeIcons[i].color.g, KnifeIcons[i].color.b,0);
            }
        }

        int StickingKnife = Random.Range(0,5);
        float maxAngle = 360 / (float)StickingKnife;
        float lastAngle = 0;
        for (int i = 0; i < StickingKnife; i++)
        {
            float angle = lastAngle + Random.Range(20,maxAngle)*Mathf.Deg2Rad;
            lastAngle = angle;
            Vector3 pos = target.transform.position + new Vector3(Mathf.Sin(angle),Mathf.Cos(angle),0)* 1.25f;
            GameObject Knife = Instantiate(KnifeGo ,pos,Quaternion.identity);
            Knife.transform.up = target.transform.position - Knife.transform.position;
            Knife.transform.parent = target.transform;
            Knife KnifeBehavior = Knife.GetComponent<Knife>();
            KnifeBehavior.myCollider.enabled = true;
            curKnifeHit.Add(KnifeBehavior.myRigid);
        }
        curKnifecount = Knifecount;
        SpawnKnife();
    }
    // Update is called once per frame
    void Update()
    {
        if(!canPlay)
        { return; }
        if (Input.GetMouseButtonDown(0) && curKnife != null && curKnifecount > -1)
        {
            shootKnife();
        }

    }
    void shootKnife()
    {
        curKnife.shoot();
        curKnifecount--;
        KnifeIcons[curKnifecount].color = new Color(0,0,0,0.5f);
        if (curKnifecount > 0)
        {
            //Spawn Knife
            SpawnKnife();
        }
        else
            curKnife = null;
    }
    void SpawnKnife()
    {
        GameObject Knife = Instantiate(KnifeGo, KnifeSpawnpoint.position, Quaternion.identity);
        curKnife = Knife.GetComponent<Knife>();
        curKnife.playAnimation();
    }

    public void SuccessfulHit(Rigidbody2D Knife)
    {
        target.GotHit();
        hitsSucc++;

        curKnifeHit.Add(Knife);
        GameManager.Instance.ADDScore();
        if (hitsSucc == Knifecount)
        {
            // pass the level
            win();
        }
    }
    public void WrongHit()
    {
        Lose();
    }
    void win()
    {
        for (int i = 0; i < curKnifeHit.Count; i++)
        {
           Rigidbody2D AllKnife= curKnifeHit[i];
            AllKnife.transform.parent = null;

            AllKnife.bodyType = RigidbodyType2D.Dynamic; // because ridgitbody 2D on Knife is Satic
            Vector3 ForceDirection = (AllKnife.transform.position - target.transform.position).normalized * 5;

            if (i == curKnifeHit.Count - 1) // the last knife that hit the target
            {
                ForceDirection.y = 10;
            }

            AllKnife.AddForceAtPosition(ForceDirection,target.transform.position ,ForceMode2D.Impulse);
            AllKnife.AddTorque(4,ForceMode2D.Impulse);
            Destroy(AllKnife,3);
        }
        target.DestroyTarget();
        GameManager.Instance.ReloadScene();
    }
    void Lose()
    {
        canPlay = false;
        GameManager.Instance.ReloadScene();
        GameManager.Instance.resetScore();
    }
}
