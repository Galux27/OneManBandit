using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopShelf : MonoBehaviour {
    public bool displayIlligal = false;
    public List<Transform> positionsForItems;
    Shop nearestShop;
	// Use this for initialization
	void Start () {
        float d = 999999.0f;
        

		foreach(Shop s in Shop.shopsInWorld)
        {
            float dist = Vector2.Distance(s.transform.position, this.transform.position);
            RoomScript r = LevelController.me.getRoomPosIsIn(this.transform.position);
            if (r == null)
            {
                if (dist < d && dist < 15)
                {
                    d = dist;
                    nearestShop = s;

                }
            }
            else
            {
                if (dist < d && r == LevelController.me.getRoomPosIsIn(s.transform.position))
                {
                    d = dist;
                    nearestShop = s;
                }
            }
        }
        if (nearestShop != null && nearestShop.shopAvailable == true && nearestShop.robbed == false)
        {
            int r = Random.Range(0, 100);
            if (r < 100)
            {
                List<GameObject> itemsWeCouldCreate = new List<GameObject>();
                foreach(string st in nearestShop.getItemsISell())
                {
                    GameObject g = ItemDatabase.me.getItem(st);
                    if (g == null)
                    {

                    }
                    else
                    {
                        Item i = g.GetComponent<Item>();
                        if(displayIlligal==true)
                        {
                            itemsWeCouldCreate.Add(g);
                        }
                        else
                        {
                            if(i.illigal==false)
                            {
                                itemsWeCouldCreate.Add(g);
                            }
                        }
                    }
                }

                if (itemsWeCouldCreate.Count > 0)
                {
                    int r2 = Random.Range(0, itemsWeCouldCreate.Count);
                    float value = itemsWeCouldCreate[r2].GetComponent<Item>().price;

                    if(value>500)
                    {
                        GameObject instance = (GameObject)Instantiate(itemsWeCouldCreate[r2], positionsForItems[0].position, this.transform.rotation);
                        instance.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
                        Destroy(instance.GetComponent<Item>());
                    }else if(value>100)
                    {
                        GameObject instance = (GameObject)Instantiate(itemsWeCouldCreate[r2], positionsForItems[0].position, this.transform.rotation);
                        instance.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
                        instance = (GameObject)Instantiate(itemsWeCouldCreate[r2], positionsForItems[1].position, this.transform.rotation);
                        instance.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
                        Destroy(instance.GetComponent<Item>());

                    }
                    else
                    {
                        foreach(Transform t in positionsForItems)
                        {
                            GameObject instance = (GameObject)Instantiate(itemsWeCouldCreate[r2], t.position, this.transform.rotation);
                            instance.GetComponent<SpriteRenderer>().sortingOrder = this.GetComponent<SpriteRenderer>().sortingOrder + 1;
                            Destroy(instance.GetComponent<Item>());

                        }
                    }
                }
            }
        }

	}
	
	
}
