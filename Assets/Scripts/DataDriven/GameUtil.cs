using Cysharp.Threading.Tasks;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

// 객체 생성 없이 어디서든 바로 사용 가능한 함수
public static class GameUtil
{
    // 마지막으로 할당된 ID를 전역적으로 기록 (스레드 안전)
    private static long _lastId = 0;

    public static void LoadFullData()
    {
        // 게임 로딩할 때 불러올 데이터는 여기서! 
        GameDataManager.Instance.LoadSkillData(("Skill"));
        GameDataManager.Instance.LoadCharacterData(("Character"));
        GameDataManager.Instance.LoadWeaponData(("Weapon"));
        GameDataManager.Instance.LoadCostumeData(("Costume"));
        GameDataManager.Instance.LoadItemData(("Item"));
        GameDataManager.Instance.LoadDialogueData();
        GameDataManager.Instance.LoadAll();
    }


    public static int CalcCharacterFinalDamage(int curCharacterLevel, int levelPerDamage, bool isCritical)
    {
        int damagePerLevel = (curCharacterLevel + levelPerDamage);
        int finalDamage = isCritical ? (damagePerLevel * 2) : damagePerLevel;
        return finalDamage;
    }

    public static Sprite LoadSpriteCanBeNull(string spriteName)
    {
        // 1. Resources/ 경로에서 이름으로 스프라이트 로드
        // 예: spriteName이 "Sword"라면 Assets/Resources/2D/Sword.png를 찾음
        // 이 2D같은 경로는 나중에 Sprite, Texture 등등 다양하게 바꿔도 무관합니다!
        Sprite loadedSprite = Resources.Load<Sprite>($"{spriteName}");

        if (loadedSprite != null)
        {
            Debug.LogWarning("에셋을 잘 찾았다!");
            return loadedSprite;
        }

        Debug.LogError($"에셋을 찾을 수 없습니다: {spriteName}");
        return null;
    }

    // 비동기로 스프라이트 로드 - 이미지에 자동 적용, Sprite 반환
    public static async UniTask<Sprite> LoadAndSetSpriteImage(Image targetImage, string spritePath)
    {
        // Addressables 기반 비동기 로드 / 로딩 끝날 때까지 await
        Sprite sprite = await ResourceManager.Instance.LoadSprite(spritePath);
        if (sprite != null)
        {
            targetImage.sprite = sprite;
        }

        return sprite;
    }

    // 비동기로 텍스처 로드 -> RawImage에 적용, 반환 없음
    public static async UniTaskVoid LoadAndSetTexture(RawImage targetRawImage, string texturePath)
    {
        // 비동기로 로드하기 전까지는 해당 오브젝트를 잠깐 비활성화
        targetRawImage.gameObject.SetActive(false);
        // Addressables 기반 비동기 로드 / 로딩 끝날 때까지 await
        Texture texture = await ResourceManager.Instance.LoadAsset<Texture>(texturePath);
        if (texture != null)
        {
            targetRawImage.texture = texture;
        }
        // 로드 끝났으니까 다시 보여줌
        targetRawImage.gameObject.SetActive(true);

    }

    // 비동기로 오디오 파일 로드 -> 재생, 반환 없음
    public static async UniTaskVoid LoadAndPlayAudioClip(AudioSource audioSource, string audioPath, bool isLoop = false)
    {
        // Addressables 기반 비동기 로드 / 로딩 끝날 때까지 await
        AudioClip clip = await ResourceManager.Instance.LoadAsset<AudioClip>(audioPath);

        if(clip == null)
        {
            Debug.LogError($"{audioPath}를 찾을 수 없습니다! 어드레서블 설정이 되어 있는지 확인해주세요.");
        }

        // 반복 재생(BGM)인지 확인
        if(isLoop == true)
        {
            audioSource.clip = clip;     // AudioSource에 재생할 클립 등록
            audioSource.loop = true;     // 오디오를 무한 반복 재생하도록 설정
            audioSource.Play();          // 등록된 clip 재생 시작
        }
        // 반복 재생이 아니라면 - 효과음
        else
        {
            // 한 번만 재생 (기존 재생 중인 효과음 위에 겹쳐서 재생 가능)
            audioSource.PlayOneShot(clip);
        }
    }

    public static List<string> GetDialogueIdList(string dialogueGroupId)
    {
        var list = new List<string>();

        // "dialogue_group_mainstream_1_1"
        var data = GameDataManager.Instance.GetDialogueGroupData(dialogueGroupId);
        if (data != null)
        {
            var idArr = data.DialogueIdList;
            foreach (var id in idArr)
            {
                list.Add(id);
            }
        }

        return list;
    }

    // 그냥 유니크 키가 발급되어야 할 때 사용하려고 만든 것 (의미가 있는 건 아니니 그냥 사용만 하자)
    public static long GenerateUniqueId()
    {
        long newId = DateTime.UtcNow.Ticks;

        // 원자적 연산으로 안전하게 ID 갱신
        while (true)
        {
            long lastId = Volatile.Read(ref _lastId);

            // 만약 현재 시간이 이전 ID보다 작거나 같다면 (루프가 너무 빠른 경우 포함)
            // 이전 ID + 1로 강제 설정하여 중복 방지
            long idToAssign = (newId <= lastId) ? lastId + 1 : newId;

            // _lastId가 내가 읽은 시점과 같다면 idToAssign으로 교체 (성공 시 루프 탈출)
            if (Interlocked.CompareExchange(ref _lastId, idToAssign, lastId) == lastId)
            {
                return idToAssign;
            }
            // 그 사이 다른 스레드가 값을 바꿨다면 다시 시도
        }
    }

}