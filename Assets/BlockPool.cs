using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class BlockPool : MonoBehaviour
{
    public static BlockPool Instance;
    public GameObject blockPrefab;
    private Queue<GameObject> pool = new Queue<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public GameObject GetBlock()
    {
        if (pool.Count > 0)
        {
            GameObject block = pool.Dequeue();
            block.SetActive(true);

            Block blockComponent = block.GetComponent<Block>();
            if (blockComponent != null)
            {
                blockComponent.spriteRenderer.color = Color.white; // Reset highlight
                blockComponent.blockColor = null; // Clear old color
                blockComponent.spriteRenderer.sprite = null; // Clear old sprite
            }

            return block;
        }
        return Instantiate(blockPrefab);
    }



    public void ReturnBlock(GameObject block)
    {
        block.SetActive(false);
        pool.Enqueue(block);
    }
}
