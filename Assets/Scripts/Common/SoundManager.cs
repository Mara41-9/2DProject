using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioSource BGMSourcePlayer; // 배경음용
    [SerializeField] private AudioSource AudioSourcePlayer; // 효과음용

    public static SoundManager Instance { get; set; }

    private void Awake()
    {
        Instance = this;
    }

    // 사운드 ID를 받아 실제 경로 문자열을 반환하는 함수
    public string GetSoundPath(string soundDataId)
    {
        // 전달받은 사운드 ID를 path 변수에 저장
        string path = soundDataId;

        // 최종 경로 문자열로 반환
        // 여기서 데이터 매니저를 통해 사운드 Id로
        // 실제 사운드 데이터 경로를 받아오면 좋음
        return path;
    }

    // 효과음 재생 (겹쳐서 재생 가능)
    public void PlaySFX(string soundDataId)
    {

        GameUtil.LoadAndPlayAudioClip(AudioSourcePlayer, soundDataId).Forget();
    }

    // 배경음을 재생하는 함수
    public void PlayBGM(string soundDataId)
    {
        BGMSourcePlayer.Stop();

        // 오디오를 비동기로 로드 -> 재생
        // Forget(): 비동기 작업(UniTask)을 기다리지 않고 실행만 함!
        // 지금은 단순 재생만 하면 되니까 결과를 기다리지 않고 .Forget() 사용
        GameUtil.LoadAndPlayAudioClip(BGMSourcePlayer, soundDataId, true).Forget();
    }
}
