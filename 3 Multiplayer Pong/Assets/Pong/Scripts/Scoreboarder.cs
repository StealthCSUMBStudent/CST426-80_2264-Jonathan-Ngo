using TMPro;
using UnityEngine;

public class Scoreboarder : MonoBehaviour
{
    public TestServerBall scoreman;
    public TextMeshProUGUI leftText;
    public TextMeshProUGUI rightText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "LNet")
        {
            SetCountTextR();
            Debug.Log(scoreman.rightScoreNum);

        }
        if (collision.gameObject.name == "RNet")
        {
            SetCountTextL();
            Debug.Log(scoreman.leftScoreNum);
        }
    }
    // Update is called once per frame
    void SetCountTextL()
    {
        leftText.text = scoreman.leftScoreNum.ToString();
    }
    void SetCountTextR()
    {
        rightText.text = scoreman.rightScoreNum.ToString();
    }
}
