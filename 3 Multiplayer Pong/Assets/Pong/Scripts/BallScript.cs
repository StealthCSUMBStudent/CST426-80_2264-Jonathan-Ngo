using Unity.Netcode;
using UnityEngine;
//[RequireComponent(typeof(AudioSource))]

/*
 * This script was a carry on from CST326 and will be modified to the best extent i can*
 */
public class BallScript : NetworkBehaviour
{
    public Rigidbody ball;
    public float ballSpeed = 3f;
    //public float forceStrength = 10f;
    public float speed = 1f;
    public float speedIncrease = 0.5f;
    public float changeTime = 0;
    public int randomNum;
    [SerializeField] public Paddle paddler;
    //public Paddle paddler = collision.gameObject.GetComponent<Paddle>();
    //ublic AudioClip boing;
    //AudioSource audioSource;
    //public AudioClip speedUp;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public bool IsServerOwned => IsSpawned && OwnerClientId == NetworkManager.ServerClientId;
    void Awake()
    {
        //audioSource = GetComponent<AudioSource>();
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer) { 
            return;
        }
        Vector3 force = new Vector3(0f, 0f, 0f);
        randomNum = Random.Range(0, 9);
        if (randomNum >= 5)
        {
            Debug.Log($" Ball Turn to Right" + randomNum);
            force = new Vector3(Random.Range(2f, 4f), 0f, Random.Range(2f, 4f));
        }
        if (randomNum <= 4)
        {
            Debug.Log($" Ball Turn to Left" + randomNum);
            force = new Vector3(Random.Range(-4f, -2f), 0f, Random.Range(-4f, -2f));
        }
        ball.linearVelocity = force * ballSpeed;
    }
    /*
    void Start()
    {
        Vector3 force = new Vector3(0f, 0f, 0f);
        randomNum = Random.Range(0, 9);
        if (randomNum >= 5)
        {
            Debug.Log($" Ball Turn to Right" + randomNum);
            force = new Vector3(Random.Range(2f, 4f), 0f, Random.Range(2f, 4f));
        }
        if (randomNum <= 4)
        {
            Debug.Log($" Ball Turn to Left" + randomNum);
            force = new Vector3(Random.Range(-4f, -2f), 0f, Random.Range(-4f, -2f));
        }
        ball.linearVelocity = force * ballSpeed;
    }
    */
    public float resetSpeed()
    {
        speed = 3f;
        changeTime = 0f;

        return speed;
    }
    // Update is called once per frame
    void Update()
    {
        //Vector3 force = new Vector3(Random.Range(-3f,3f), 0f, Random.Range(-3f,3f));
        //Rigidbody lBody = GetComponent<Rigidbody>();
        //ball.linearVelocity = force;
        //Transform myTransform = GetComponent<Transform>();
        /*
         * Vector3 force = new Vector3(0f, 0f, forceStrength);
            //Rigidbody lBody = GetComponent<Rigidbody>();
            lBody.linearVelocity = force;
         */
    }
    void OnCollisionEnter(Collision collision)
    {
       Paddle paddler = collision.gameObject.GetComponent<Paddle>();
        if (!IsServer)
            return;
        if (paddler.side == PaddleSide.Left && Time.time > changeTime + 1)
        {
            speed += speedIncrease;
            Vector3 force = new Vector3(Random.Range(1f, 2f), 0f, Random.Range(0.5f, 1f));
            //audioSource.clip = boing;
            //audioSource.Play();
            ball.linearVelocity = (ballSpeed * force) * speed;
            changeTime = Time.time;
            Debug.Log($"$current speed is: " + speed);
        }

        if (paddler.side == PaddleSide.Right && Time.time > changeTime + 1)
        {
            speed += speedIncrease;
            Vector3 force = new Vector3(Random.Range(-2f, -1f), 0f, Random.Range(-1f, -0.5f));
            //audioSource.clip = boing;
            //audioSource.Play();
            ball.linearVelocity = (ballSpeed * force) * speed;
            changeTime = Time.time;
            Debug.Log($"current speed is: " + speed);
        }
    }
}
