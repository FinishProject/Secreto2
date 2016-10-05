using UnityEngine;
using System.Collections;

public class MonsterRespawnMgr : MonoBehaviour {

    GameObject[] monsters;

    public static MonsterRespawnMgr instance;
    void Awake () {
        instance = this;
    }

    public void Respawn(GameObject monster)
    {
        StartCoroutine(MonstaerRespawn(monster));
    }

    IEnumerator MonstaerRespawn(GameObject monster)
    {
        yield return new WaitForSeconds(2.0f);
        monster.gameObject.SetActive(true);
    }
}
