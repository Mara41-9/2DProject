using Cysharp.Threading.Tasks;
using System;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class LoadingUI : UIBase
{
    [SerializeField] private RawImage RawImage_LoadingImg;   // 로딩 화면에 보여줄 RawImage
    [SerializeField] private Slider Slider_LoadingBar;       // 로딩 진행률을 보여줄 슬라이더
    [SerializeField] private Image Image_SliderColor;        // 로딩바의 색상을 바꿀 대상 이미지
    [SerializeField] private UnityEngine.Color[] ColorArray_LoadingBar;   // 로딩바 색상 배열

    private CancellationTokenSource _cancelToken;   // 비동기 로딩바 작업을 취소할 때 쓰려고 만든 변수
    float[] _pausePoints = { 0.2f, 0.2f, 0.2f };    // 로딩바가 잠깐 멈출 지점들
    int _pauseIndex = 0;                            // 현재 몇 번째 멈춤 지점을 처리 중인지 저장하는 변수

    private void OnEnable()
    {
        LoadAndSetLoadingImg();
    }

    // 로딩 이미지 선택 + 로딩바 시작을 담당하는 함수
    private void LoadAndSetLoadingImg()
    {
        // 0 ~ 2 정수 랜덤으로 뽑아
        int randomIdx = UnityEngine.Random.Range(0, 2);

        // 불러올 이미지 경로를 담을 문자열 변수
        string texturePath = string.Empty;

        switch (randomIdx)
        {
            case 0:
                texturePath = "Texture/Texture_Loading_1";
                break;
            case 1:
                texturePath = "Texture/Texture_Loading_2";
                break;
        }

        // 이미지를 비동기로 불러와서 RawImage_LoadingImg에 넣자
        GameUtil.LoadAndSetTexture(RawImage_LoadingImg, texturePath).Forget();
        // 2.7초 동안 로딩바를 채우는 비동기 함수를 실행
        StartLoadingResouce(1f).Forget();
    }

    // 로딩바를 일정 시간 동안 채우는 비동기 함수
    public async UniTaskVoid StartLoadingResouce(float duration)
    {
        // 비동기 작업 취소용 토큰
        _cancelToken = new CancellationTokenSource();

        // 지금까지 흐른 시간을 저장하는 변수
        float elapsed = 0f;
        // 로딩바를 처음에 비워두는 코드
        Slider_LoadingBar.value = 0f;

        // 1. 지정된 시간(duration) 동안 반복
        while (elapsed < duration)
        {
            // 이전 프레임에서 현재 프레임까지 흐른 시간을 더해
            elapsed += Time.deltaTime;

            // 2. 진행률 계산 (0.0 ~ 1.0)
            float progress = Mathf.Clamp01(elapsed / duration);
            Slider_LoadingBar.value = progress;

            // 가짜 연출용 ====
            if (_pauseIndex < _pausePoints.Length && progress >= _pausePoints[_pauseIndex])
            {
                float pausePointValue = _pausePoints[_pauseIndex];
                Slider_LoadingBar.value = pausePointValue;
                // 1초간 대기 (비동기)
                await UniTask.Delay(TimeSpan.FromSeconds(pausePointValue), cancellationToken: _cancelToken.Token);
                _pauseIndex++;
            }

            Slider_LoadingBar.value = progress;
            ChangeColorByLoadingBarValue(progress);

            // 3. 다음 프레임까지 대기 (매 프레임 갱신)
            await UniTask.Yield(PlayerLoopTiming.Update, _cancelToken.Token);
        }

        // 4. 완료 처리
        Slider_LoadingBar.value = 1.0f;
        UIManager.Instance.CloseLoadingUI();
    }

    private void ChangeColorByLoadingBarValue(float curValue)
    {
        if (curValue > 0.8)
        {
            Image_SliderColor.color = ColorArray_LoadingBar.Length >= 4 ? ColorArray_LoadingBar[3] : Color.white;
        }
        else if (curValue > 0.6)
        {
            Image_SliderColor.color = ColorArray_LoadingBar.Length >= 3 ? ColorArray_LoadingBar[2] : Color.white;
        }
        else if (curValue > 0.4)
        {
            Image_SliderColor.color = ColorArray_LoadingBar.Length >= 2 ? ColorArray_LoadingBar[1] : Color.white;
        }
        else
        {
            Image_SliderColor.color = ColorArray_LoadingBar.Length >= 1 ? ColorArray_LoadingBar[0] : Color.white;
        }
    }
}
