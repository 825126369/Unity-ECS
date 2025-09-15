using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public struct AudioReference : IComponentData
{
    public Entity Entity; // 指向音频实体
}

public struct AudioPool : IComponentData
{
    public NativeList<Entity> Available;
    public Entity Prefab;
}

public struct AudioSourceData : IComponentData
{
    public UnityObjectRef<AudioClip> Clip;
    public float Volume;
    public float Pitch;
    public bool PlayOnStart;
    public bool IsPlaying; // 避免重复播放
    public bool Loop;
}

public struct PlayAudioRequest : IComponentData
{

}

public struct LinkedEntityComponent : IComponentData
{
    public UnityObjectRef<GameObject> GameObject;
}

public static class AudioExtensions
{
    public static void PlayAudio(this EntityManager em, Entity audioEntity)
    {
        if (em.HasComponent<AudioSourceData>(audioEntity))
        {
            em.AddComponent<PlayAudioRequest>(audioEntity);
        }
    }
}

[UpdateInGroup(typeof(PresentationSystemGroup))] // 必须在主线程
public partial class ECSAudioSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (audioData, lec, entity) in SystemAPI.Query<RefRW<AudioSourceData>, RefRO<LinkedEntityComponent>>()
                     .WithAll<AudioSourceData>()
                     .WithEntityAccess())
        {
            if (audioData.ValueRO.PlayOnStart && !audioData.ValueRO.IsPlaying)
            {
                PlayAudio(audioData, lec, entity);
            }
        }

        // 处理动态播放请求
        foreach (var (request, audioData, lec, entity) in SystemAPI.Query<RefRO<PlayAudioRequest>, RefRW<AudioSourceData>, RefRO<LinkedEntityComponent>>()
                     .WithEntityAccess())
        {
            PlayAudio(audioData, lec, entity);
            EntityManager.RemoveComponent<PlayAudioRequest>(entity); // 清理请求
        }
    }

    private void PlayAudio(RefRW<AudioSourceData> audioData, RefRO<LinkedEntityComponent> lec, Entity entity)
    {
        if (lec.ValueRO.GameObject == null)
        {
            Debug.LogWarning("Linked GameObject is null, cannot play audio.", lec.ValueRO.GameObject);
            return;
        }

        AudioSource audioSource = lec.ValueRO.GameObject.Value.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            Debug.LogError("AudioSource component missing on GameObject.", lec.ValueRO.GameObject);
            return;
        }

        if (audioData.ValueRO.Clip == null)
        {
            Debug.LogWarning("AudioClip is null, cannot play.", lec.ValueRO.GameObject);
            return;
        }

        // 配置 AudioSource
        audioSource.clip = audioData.ValueRO.Clip;
        audioSource.volume = audioData.ValueRO.Volume;
        audioSource.pitch = audioData.ValueRO.Pitch;
        audioSource.loop = audioData.ValueRO.Loop;

        // 播放
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
            audioData.ValueRW.IsPlaying = true; // 标记为正在播放
        }
    }
}

public struct AttackComponent : IComponentData
{

}

// 例如：角色攻击时播放音效
public partial class AttackSystem : SystemBase
{
    protected override void OnUpdate()
    {
        foreach (var (attack, audioEntity, entity) in SystemAPI.Query<RefRO<AttackComponent>, RefRO<AudioReference>>().WithEntityAccess())
        {
            if (EntityManager.Exists(audioEntity.ValueRO.Entity))
            {
                // 触发播放
                EntityManager.AddComponent<PlayAudioRequest>(audioEntity.ValueRO.Entity);
            }
        }
    }
}