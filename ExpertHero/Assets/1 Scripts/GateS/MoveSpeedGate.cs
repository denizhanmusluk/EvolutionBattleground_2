using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MoveSpeedGate : MonoBehaviour
{
    [SerializeField] int affectTime;
    [SerializeField] int speedUp;
    GateBreak gateBreak;
    private void Start()
    {
        gateBreak = GetComponent<GateBreak>();
        gateBreak.particleActice = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerEvolution>() != null)
        {
            other.GetComponent<PlayerEvolution>().speedGate(affectTime, speedUp);
            GetComponent<Collider>().enabled = false;
            gateBreak.glassHit(other.gameObject);
            GateSpawner.Instance.gateAll.Remove(gameObject);
        }
    }
}
