//using Unity.Entities;
//using UnityEngine;

//public static class ECS_Authoring_Helper
//{
//    public static void AddCData(IBaker baker, GameObject go)
//    {
//        //var mTempEntity = baker.GetEntity(go, TransformUsageFlags.Dynamic);
//        //var mGameObjectCData = new GameObjectCData();
//        //mGameObjectCData.Init(go);
//        //baker.AddComponent(mTempEntity, mGameObjectCData);

//        Debug.Log("AddCData: " + go.name);
//        for (int i = 0; i < baker.GetChildCount(); i++)
//        {
//            GameObject v = baker.GetChild(go, i);

//            var mTempEntity = baker.GetEntity(v, TransformUsageFlags.Dynamic);
//            var mGameObjectCData = new GameObjectCData();
//            mGameObjectCData.Init(v);
//            baker.AddComponent(mTempEntity, mGameObjectCData);

//            AddCData(baker, v);
//        }
//    }

//    public static void AddCData<T1, T2>(IBaker baker, GameObject go)
//        where T1 : UnityEngine.Component
//        where T2 : unmanaged, KKIComponentData, IComponentData
//    {
//        var mTempEntity = baker.GetEntity(go, TransformUsageFlags.Dynamic);
//        if (baker.GetComponent<T1>(go))
//        {
//            var m = new T2();
//            m.Init(go);
//            baker.AddComponent(mTempEntity, m);
//        }
        
//        for (int i = 0; i < baker.GetChildCount(); i++)
//        {
//            GameObject v = baker.GetChild(go, i);
//            AddCData<T1,T2>(baker, v);
//        }
//    }
//}
