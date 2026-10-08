using UnityEngine;

public class FlyTime : MonoBehaviour
{
    public SplineFollow rocketCheck;
    public float flySpeed = 5f;
    Vector3 originalPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (rocketCheck.rockChecker == true)
        {
            transform.position += Vector3.up * flySpeed * Time.deltaTime;
        }

        if (rocketCheck.rockChecker == false)
        {
            transform.position = originalPosition;
        }
    }

}
