using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPoolable <T> where T : Component
{
    void Added (Pool <T> pool);
    void Gotten ();
    void Returned ();
}

public static class PoolContainer
{
    public static Transform g_root;
}

public class Pool <T> where T : Component
{
    int count => stack.Count;
    Stack<T> stack;
    HashSet<T> borrowd;

    Transform root;

    bool implementsInterface;
    T prefab;

    public Pool (T prefab, int count)
    {
        this.prefab = prefab;
        this.stack = new Stack<T>();
        this.borrowd = new HashSet<T>();
        this.implementsInterface = (prefab as IPoolable<T>) != null;

        if(PoolContainer.g_root == null)
        {
            PoolContainer.g_root = new GameObject("Pool").transform;
            PoolContainer.g_root.gameObject.SetActive(false);
        }

        root = new GameObject($"Pool_{prefab.name}").transform;
        root.SetParent(PoolContainer.g_root);
        Warm(count);
    }

    public void Warm (int amount)
    {
        if(count > amount)
            return;

        int c = Mathf.Max(1, amount - count);
        
        for(int i = 0; i < c; i++)
        {
            T item = GameObject.Instantiate(prefab);
            item.gameObject.SetActive(false);
            item.transform.SetParent(root);
            stack.Push(item);

            if(implementsInterface)
                (item as IPoolable<T>)?.Added(this);
        }
    }

    public T Get (Transform parent = null)
    {
        if(stack.Count == 0)
            Warm(1);

        T item = stack.Pop();
        if(item == null)
        {
            Debug.Log("awjidajiwdji");
            Debug.Log(stack.Count);
            Debug.Log(item);

            Debug.Assert(item);
        }

        item.gameObject.SetActive(true);
        item.transform.SetParent(parent);
        borrowd.Add(item);
        
        if(implementsInterface)
            (item as IPoolable<T>)?.Gotten();

        return item; 
    }

    public void Return (T item)
    {
        if(borrowd.Contains(item))
        {
            if(implementsInterface)
                (item as IPoolable<T>)?.Returned();

            borrowd.Remove(item);
            item.transform.SetParent(root);
            item.gameObject.SetActive(false);
            stack.Push(item);
        }
        else
        {
            
            Debug.LogWarning($"{item.name} ended up in the wrong pool or something. Destroying object.");
            GameObject.Destroy(item.gameObject);
        }
    }
}
