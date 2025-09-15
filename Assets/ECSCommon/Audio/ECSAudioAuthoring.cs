using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class ECSAudioAuthoring : MonoBehaviour
{
    public AudioClip clip;
    public bool playOnStart = false;
    [Range(0f, 1f)] public float volume = 1.0f;
    [Range(0.5f, 2f)] public float pitch = 1.0f;
    public bool loop = false;
    public float spatialBlend = 1.0f; // 1=3D, 0=2D
    public float minDistance = 1.0f;
    public float maxDistance = 50.0f;

    class mBaker : Baker<ECSAudioAuthoring>
    {
        public override void Bake(ECSAudioAuthoring authoring)
        {
            if (authoring.clip == null)
            {
                Debug.LogWarning($"AudioClip is null on {authoring.name}", authoring);
                return;
            }

            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new AudioSourceData
            {
                Clip = authoring.clip,
                Volume = authoring.volume,
                Pitch = authoring.pitch,
                PlayOnStart = authoring.playOnStart,
                IsPlaying = false,
                Loop = authoring.loop
            });

            // 获取或添加 AudioSource 组件
            var audioSource = authoring.GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = authoring.gameObject.AddComponent<AudioSource>();
                audioSource.playOnAwake = false;
                audioSource.clip = authoring.clip;
            }

            // 配置 3D 音效
            audioSource.spatialBlend = authoring.spatialBlend;
            audioSource.minDistance = authoring.minDistance;
            audioSource.maxDistance = authoring.maxDistance;
            audioSource.loop = authoring.loop;

            // 添加 LinkedEntityComponent 用于运行时访问
            AddComponent(entity, new LinkedEntityComponent
            {
                GameObject = authoring.gameObject
            });
        }
    }

}