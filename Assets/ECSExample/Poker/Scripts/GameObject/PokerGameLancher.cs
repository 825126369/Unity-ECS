using Unity.Entities;
using UnityEngine;

public class PokerGameLancher : SingleTonMonoBehaviour<PokerGameLancher>
{
    protected override void Awake()
    {
        base.Awake();
        UnityMainThreadDispatcher.Instance.Init();
        UnityMainThreadDispatcher.Instance.AddListener(PokerECSEvent.PokerAniFinish, OnPokerAniFinish);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        UnityMainThreadDispatcher.Instance.RemoveListener(PokerECSEvent.PokerAniFinish, OnPokerAniFinish);
    }

    public void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            EntityManager mEntityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            mEntityManager.CreateEntity(typeof(StartDoAniEvent));
        }
    }

    public void OnPokerAniFinish(object data)
    {
        Debug.Log("¶¯»­Íê³É");
    }
}