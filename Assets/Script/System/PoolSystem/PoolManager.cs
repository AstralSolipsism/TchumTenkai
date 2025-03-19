using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    //玩家子弹对象池
    [SerializeField] Pool[] playerProjectilePools;
    //敌人对象池
    [SerializeField] Pool[] enemyPools;
    //经验球对象池
    [SerializeField] Pool[] expBalls;
    //敌人捕捉器池
    [SerializeField] Pool[] EnemyCatchers;
    //爆炸效果池
    [SerializeField] Pool[] VFXPool;

    //用来根据不同的类选择不对的对象池
    static Dictionary<GameObject, Pool> dictionary;

    private void Awake()
    {
        dictionary = new Dictionary<GameObject, Pool>();
        Initialize(playerProjectilePools);
        Initialize(enemyPools);
        Initialize(expBalls);
        Initialize(EnemyCatchers);
        Initialize(VFXPool);
    }
#if UNITY_EDITOR
    //调整对象池size使用

    private void OnDestroy()
    {
        CheckPoolSize(playerProjectilePools);
        CheckPoolSize(enemyPools);
        CheckPoolSize(expBalls);
        CheckPoolSize(EnemyCatchers);
        CheckPoolSize(VFXPool);
    }
#endif
    void CheckPoolSize(Pool[] pools)
    {
        foreach (var pool in pools)
        {
            if (pool.RuntimeSize > pool.Size)
            {
                Debug.LogWarning(string.Format("Pool:{0} has a runtime size{1} than its initial size{2}!",
                    pool.Prefab.name,
                    pool.RuntimeSize,
                    pool.Size)
                    );
            }
        }
    }
    void Initialize(Pool[] pools)
    {
        foreach (var pool in pools)
        {

#if UNITY_EDITOR
            //防止重复添加相同预制体
            if (dictionary.ContainsKey(pool.Prefab))
            {
                Debug.LogError("Same prefab in mutiple pools! Prefab:" + pool.Prefab.name);
                continue;
            }
#endif
            dictionary.Add(pool.Prefab, pool);

            Transform poolParent = new GameObject("Pool:" + pool.Prefab.name).transform;

            poolParent.parent = transform;
            pool.Initialize(poolParent);
        }
    }


    /// <summary>
    /// <para>根据输入的<paramref name="prefab"/>参数，返回对象池中预备好的对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <returns>
    /// <para>对象池中预备好的游戏对象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager couldn't find prefab:" + prefab.name);
        }
#endif
        return dictionary[prefab].PreparedObject();
    }

    /// <summary>
    /// <para>根据输入的<paramref name="prefab"/>参数，返回对象池中预备好的对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <param name="position">
    /// <para>释放位置</para>
    /// </param>
    /// <returns>
    /// <para>对象池中预备好的游戏对象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector2 position)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager couldn't find prefab:" + prefab.name);
        }
#endif
        return dictionary[prefab].PreparedObject(position);
    }
    /// <summary>
    /// <para>根据输入的<paramref name="prefab"/>参数，返回对象池中预备好的对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <param name="position">
    /// <para>释放位置</para>
    /// </param>
    /// <param name="rotation">
    /// <para>旋转角度</para>
    /// </param>
    /// <returns>
    /// <para>对象池中预备好的游戏对象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector2 position, Quaternion rotation)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager couldn't find prefab:" + prefab.name);
        }
#endif
        return dictionary[prefab].PreparedObject(position, rotation);
    }
    /// <summary>
    /// <para>根据输入的<paramref name="prefab"/>参数，返回对象池中预备好的对象。</para>
    /// </summary>
    /// <param name="prefab">
    /// <para>指定的游戏对象预制体</para>
    /// </param>
    /// <param name="position">
    /// <para>释放位置</para>
    /// </param>
    /// <param name="rotation">
    /// <para>旋转角度</para>
    /// </param>
    /// <param name="localScale">
    /// <para>缩放大小</para>
    /// </param>
    /// <returns>
    /// <para>对象池中预备好的游戏对象</para>
    /// </returns>
    public static GameObject Release(GameObject prefab, Vector2 position, Quaternion rotation, Vector2 localScale)
    {
#if UNITY_EDITOR
        if (!dictionary.ContainsKey(prefab))
        {
            Debug.LogError("Pool Manager couldn't find prefab:" + prefab.name);
        }
#endif
        return dictionary[prefab].PreparedObject(position, rotation, localScale);
    }
}
