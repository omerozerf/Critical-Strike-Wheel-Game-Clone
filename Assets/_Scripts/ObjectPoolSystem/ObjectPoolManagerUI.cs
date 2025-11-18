using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace ObjectPoolSystem
{
    [DisallowMultipleComponent]
    public class ObjectPoolManagerUI : MonoBehaviour
    {
        [Serializable]
        private class PoolConfig
        {
            [SerializeField] private ObjectPoolType _objectPoolType;
            [SerializeField] private GameObject _prefab;
            [SerializeField] private int _initialSize = 5;

            public ObjectPoolType ObjectPoolType => _objectPoolType;
            public GameObject Prefab => _prefab;
            public int InitialSize => Mathf.Max(_initialSize, 0);
        }

        [SerializeField] private List<PoolConfig> _pools = new ();

        private readonly Dictionary<ObjectPoolType, Queue<GameObject>> m_PoolMap = new ();

        private readonly Dictionary<ObjectPoolType, GameObject> m_PrefabMap = new ();

        private void Awake()
        {
            InitializePools();
        }

        private void InitializePools()
        {
            m_PoolMap.Clear();
            m_PrefabMap.Clear();

            foreach (var config in _pools)
            {
                if (config.ObjectPoolType == ObjectPoolType.None)
                {
                    Debug.LogWarning("[ObjectPooler] Boş key tanımlanmış, atlanıyor.", this);
                    continue;
                }

                if (config.Prefab == null)
                {
                    Debug.LogWarning($"[ObjectPooler] '{config.ObjectPoolType}' için prefab atanmamış, atlanıyor.", this);
                    continue;
                }

                if (m_PoolMap.ContainsKey(config.ObjectPoolType))
                {
                    Debug.LogWarning($"[ObjectPooler] Aynı key iki kere tanımlanmış: {config.ObjectPoolType}", this);
                    continue;
                }

                var queue = new Queue<GameObject>();
                m_PoolMap.Add(config.ObjectPoolType, queue);
                m_PrefabMap.Add(config.ObjectPoolType, config.Prefab);

                for (int i = 0; i < config.InitialSize; i++)
                {
                    var instance = CreateNewInstance(config.ObjectPoolType);
                    if (instance != null)
                    {
                        queue.Enqueue(instance);
                    }
                }
            }
        }

        private GameObject CreateNewInstance(ObjectPoolType key)
        {
            if (!m_PrefabMap.TryGetValue(key, out var prefab) || prefab == null)
            {
                Debug.LogError($"[ObjectPooler] '{key}' için prefab bulunamadı.", this);
                return null;
            }

            var instance = Instantiate(prefab, transform);
            instance.SetActive(false);
            return instance;
        }

        public GameObject Spawn(ObjectPoolType key, Vector3 position, Quaternion rotation)
        {
            var instance = GetFromPool(key);
            if (instance == null)
                return null;

            var tr = instance.transform;
            tr.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            return instance;
        }

        private GameObject GetFromPool(ObjectPoolType key)
        {
            if (!m_PoolMap.TryGetValue(key, out var queue))
            {
                Debug.LogError($"[ObjectPooler] '{key}' için havuz bulunamadı.", this);
                return null;
            }

            if (queue.Count == 0)
            {
                // Havuz boşsa genişlet
                var newInstance = CreateNewInstance(key);
                return newInstance;
            }

            return queue.Dequeue();
        }

        public void Despawn(ObjectPoolType key, GameObject instance)
        {
            if (instance == null)
                return;

            if (!m_PoolMap.TryGetValue(key, out var queue))
            {
                Debug.LogError($"[ObjectPooler] '{key}' için havuz yok, obje Destroy ediliyor.", this);
                Destroy(instance);
                return;
            }

            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            queue.Enqueue(instance);
        }

        public async UniTask DespawnAfter(ObjectPoolType key, GameObject instance, float delay)
        {
            if (instance == null) return;
            await UniTask.Delay(TimeSpan.FromSeconds(delay));
            Despawn(key, instance);
        }
    }
}