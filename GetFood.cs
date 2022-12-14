using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFood : MonoBehaviour
{
    //setup
    public GameObject Player;
    public float speed;
    private bool anything;
    public GameObject movetarget;
    public Camera cam;
    public RaycastHit rayhit;
    //food
    public float CurrentHunger;
    public float Hunger;
    public GameObject Food;
    public GameObject[] foodstuff;
    public bool ispickedup;
    public Vector2 WorldUnitsInCamera;
    public Vector2 WorldToPixelAmount;
    public GameObject rayhot;

    public GameObject Cameram;
    //health
    public float MaxHealth;
    public float CurrentHealth;
    public GameObject Heal;
    public GameObject[] GimmedatHealth;
    public RaycastHit2D rayHit;

    public Vector3 thing;

    public LayerMask onlyfood;


    void Start()
    {
        onlyfood = 3;

        anything = true;

        ispickedup = false;

        Player = GameObject.Find("Player");
        //health
        CurrentHealth = MaxHealth;

        //Food
        CurrentHunger = Hunger;
        StartCoroutine(throwitback());
        movetarget.transform.position = Player.transform.position;
    }
    void Awake()
    {
        //Finding Pixel To World Unit Conversion Based On Orthographic Size Of Camera
        WorldUnitsInCamera.y = Camera.main.GetComponent<Camera>().orthographicSize * 2;
        WorldUnitsInCamera.x = WorldUnitsInCamera.y * Screen.width / Screen.height;

        WorldToPixelAmount.x = Screen.width / WorldUnitsInCamera.x;
        WorldToPixelAmount.y = Screen.height / WorldUnitsInCamera.y;
    }
    void Update()
    {

        



        thing.x = ConvertToWorldUnits(Input.mousePosition).x;
        thing.y = ConvertToWorldUnits(Input.mousePosition).y;
        
        float posx = cam.ScreenToViewportPoint(Input.mousePosition).x;

        Debug.Log(posx);
        
        Debug.Log(Input.mousePosition);
        
        if (Input.GetMouseButtonDown(0))
        {


             rayHit = Physics2D.GetRayIntersection(Camera.main.ScreenPointToRay(Input.mousePosition));
            Debug.Log(rayHit.transform.name);

            if (rayHit.collider.tag is "food" && ispickedup is false || rayHit.collider.tag is "HpUp" && ispickedup is false)
            {
                ispickedup = true;
            }


        }

        if (ispickedup is true)
        {
            rayhot = rayHit.collider.gameObject;

            thing.z = rayhot.transform.position.z;
            rayhot.transform.position = Vector2.Lerp(rayhot.transform.position, thing, 0.1f);
                
                
        }


        Physics.Raycast(Player.transform.position, Player.transform.position - movetarget.transform.position, out RaycastHit hit);


        //health
        Health();

        //Food
        Getfood();

        //hunger
        CurrentHunger = CurrentHunger - 1 * Time.deltaTime;

        if (CurrentHealth >= 5 && CurrentHunger >= 10)
        {
            Player.transform.position = Vector2.MoveTowards(Player.transform.position, movetarget.transform.position, speed * Time.deltaTime);
        }
    }
    private IEnumerator throwitback()
    {
        while (anything is true)
        {
            Move();

            yield return new WaitForSecondsRealtime(3f);
            Move();
            yield return new WaitForSecondsRealtime(3f);
        }

    }
    void Move()
    {

        if (CurrentHealth >= 5 && CurrentHunger >= 10)
        {
            int Randomnum;
            Vector3 offset;

            
            Randomnum = Random.Range(1, 4);
            Debug.Log(Randomnum);

            switch (Randomnum)
            {
                case 1:
                    offset = Player.transform.position;
                    Debug.Log(offset);
                    offset.x += 3;
                    offset.y = 0;
                    movetarget.transform.position = offset;
                    Debug.Log("left");
                    break;

                default:

                    break;

                case 3:
                    offset = Player.transform.position;
                    offset.x -= 3;
                    offset.y = 0;

                    movetarget.transform.position = offset;

                    Debug.Log("Right");
                    Debug.Log(offset);

                    break;
            }

        }

    }

    void Getfood()
    {   //hunger

        foodstuff = GameObject.FindGameObjectsWithTag("food");
        Food = Getit(foodstuff, "food");
        if (CurrentHunger <= 0)
        {
            CurrentHealth = CurrentHealth - .25F * Time.deltaTime;
        }

        if (CurrentHunger <= 10 && Food != null)
        {

            Player.transform.position = Vector2.MoveTowards(Player.transform.position, Food.transform.position, speed * Time.deltaTime);
        }
        if (CurrentHunger >= Hunger + 1)
        {
            CurrentHunger = Hunger;
        }
        if (CurrentHunger <= -1)
        {
            CurrentHunger = 0;
        }

    }

    void Health()
    {
        GimmedatHealth = GameObject.FindGameObjectsWithTag("HpUp");
        Heal = Getit(GimmedatHealth, "HpUp");

        if (CurrentHealth <= 5 && Heal != null)
        {
            Debug.Log("heal");
            if (CurrentHealth >= 11 || Food == null)
            {

                Player.transform.position = Vector2.MoveTowards(Player.transform.position, Heal.transform.position, speed * Time.deltaTime);
                Debug.Log("ineedheals");
            }

        }
        if (CurrentHealth <= 0)
        {
            Destroy(Player);
        }
        if (CurrentHealth >= MaxHealth + 1)
        {
            CurrentHealth = MaxHealth;
        }






    }
    void OnCollisionEnter2D(Collision2D theCollision)
    {
        //food
        if (theCollision.gameObject.tag == "food" && CurrentHunger <= 11)
        {
            CurrentHunger = Hunger;
            Destroy(theCollision.gameObject);
        }

        //Health
        if (theCollision.gameObject.tag == "HpUp" && CurrentHealth <= 5)
        {
            CurrentHealth = MaxHealth;
            Destroy(theCollision.gameObject);

            CurrentHunger = CurrentHunger + 5;
        }
        if (theCollision.gameObject.tag == "wall")
        {
            movetarget.transform.position = Player.transform.position;
        }











    }
    //Taking Your Camera Location And Is Off Setting For Position And For Amount Of World Units In Camera
    public Vector2 ConvertToWorldUnits(Vector2 TouchLocation)
    {
        Vector2 returnVec2;

        returnVec2.x = ((TouchLocation.x / WorldToPixelAmount.x) - (WorldUnitsInCamera.x / 2)) +
        Camera.main.transform.position.x;
        returnVec2.y = ((TouchLocation.y / WorldToPixelAmount.y) - (WorldUnitsInCamera.y / 2)) +
        Camera.main.transform.position.y;

        return returnVec2;
    }

    public GameObject Getit(GameObject[] allthat, string Tag)
    {
        allthat = GameObject.FindGameObjectsWithTag(Tag);
        float thatbtooclose = Mathf.Infinity;
        GameObject home = null;
        foreach (GameObject go in allthat)
        {
            float CurrentDis;
            CurrentDis = Vector3.Distance(transform.position, go.transform.position);
            if (CurrentDis < thatbtooclose)
            {
                thatbtooclose = CurrentDis;
                home = go;
            }
        }
        return home;
    }
















    






















}
