using UnityEngine;
using System.Collections;

public class ObjectMgr : MonoBehaviour {

    private ItemPool bullet_FourWay_Pool = null;
    private ItemPool bullet_Parabola_Pool = null;

    public GameObject bullet_FourWay_Prefab;
    public int fourWayNum;
    public GameObject bullet_Parabola_Prefab;
    public int parabolaNum;

    public static ObjectMgr instance;

    void Awake () {
        instance = this;
        ObjectPoolInit();
    }

    public void ObjectPoolInit()
    {
        bullet_FourWay_Pool = gameObject.AddComponent<ItemPool>();
        bullet_FourWay_Pool.CreateItemPool(bullet_FourWay_Prefab, fourWayNum);

        bullet_Parabola_Pool = gameObject.AddComponent<ItemPool>();
        bullet_Parabola_Pool.CreateItemPool(bullet_Parabola_Prefab, parabolaNum);
    }

    public ItemPool GetFourWayBullet()
    {
        return bullet_FourWay_Pool;
    }

    public ItemPool GetParabolaBullet()
    {
        return bullet_Parabola_Pool;
    }
}
