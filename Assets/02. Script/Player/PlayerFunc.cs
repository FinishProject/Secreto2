using UnityEngine;
using System.Collections;

public class PlayerFunc : MonoBehaviour {

    public static PlayerFunc instance;
    PlayerCtrl playerCtrl;

    void Awake()
    {
        playerCtrl = GetComponent<PlayerCtrl>();
        instance = this;
    }

	public void SetImpactObject()
    {
        Collider[] hitColl = Physics.OverlapSphere(this.transform.position, 5f);
        for(int i = 0; i< hitColl.Length; i++) {
            if(hitColl[i].CompareTag("OBJECT")) {
                hitColl[i].SendMessage("GetImpact");
            }
        }
    }

    public void SetPowerDamage()
    {
        Collider[] hitColl = Physics.OverlapSphere(this.transform.position, 10f);
        for(int i = 0; i < hitColl.Length; i++)
        {
            hitColl[i].SendMessage("GetDamage");
        }
    }
}
