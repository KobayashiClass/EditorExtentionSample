using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FugaBehaviour : MonoBehaviour
{
    Text m_text;

    private void Start()
    {
        m_text = GetComponent<Text>();
    }

    public void SetText(string str)
    {
        m_text.text = str;
    }

    public void SetTextAnimation(string str)
    {
        StartCoroutine(StepAnimation(str));
    }

    private IEnumerator StepAnimation(string str)
    {
        var current = "";
        m_text.text = current;
        foreach(var c in str)
        {
            current = current + c;
            m_text.text = current;
            yield return new WaitForSeconds(0.1f);
        }

        m_text.text = str;
    }
}
