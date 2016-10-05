using UnityEngine;
using System.Collections;

public class ItemPool : MonoBehaviour {

    private GameObject[] items = null;

    public void CreateItemPool(GameObject prefab, int number)
    {
        items = new GameObject[number];
        for (int i = 0; i < number; i++)
        {
            items[i] = Instantiate(prefab);
            items[i].SetActive(false);
        }
    }

    public GameObject UseItem()
    {
        if (items == null)
            return null;

        for (int i = 0; i < items.Length; i++)
        {
            if (!items[i].activeSelf)
            {
                items[i].SetActive(true);
                return items[i].gameObject;
            }
        }
        return null;
    }

    public void RemoveItem(GameObject gameObject)
    {
        if (items == null || gameObject == null)
            return;
        int count = items.Length;

        for (int i = 0; i < count; i++)
        {
            GameObject temp = items[i];
            if (temp.gameObject == gameObject)
            {
                temp.SetActive(false);
                break;
            }
        }
    }


}
