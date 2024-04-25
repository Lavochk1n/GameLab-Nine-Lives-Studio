using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class ScoreDisplay : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI textBox;

    private string m_text;


    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.Instance.GetTimeLeft() <= 0f) 
        {
            m_text = "Game Over. Score: " + GameManager.Instance.GetScore().ToString(); 
        }
        else
        {
            m_text = "Score : " + GameManager.Instance.GetScore().ToString() + " Overige Tijd: " + GameManager.Instance.GetTimeLeft().ToString();

        }
        textBox.text = m_text; 

    }

}
