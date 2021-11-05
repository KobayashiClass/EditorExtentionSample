using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HogeBehaviour : MonoBehaviour
{
    Rigidbody m_rb;
    void Start(){
        m_rb = GetComponent<Rigidbody>();
    }

    public void KnockBack(float force){
        var back = -transform.forward;
        back.y = 1;
        m_rb.AddForce(force * back.normalized, ForceMode.Impulse);
    }
}
