using System.IO;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager Instance;

    private void Awake()
    {
        Instance = this;
    }


    // 세이브 파일 경로 반환 - 세이브 파일 위치를 반환하는 함수
    private string GetPath()
    {
        // 실제 저장 경로를 만들어줌
        return Path.Combine(Application.persistentDataPath, "SaveData.json");
    }


    // 플레이어 데이터를 저장하는 함수
    public void RequestSaveData(PlayerModel data)
    {
        // 플레이어 클래스를 JSON 문자열로 변환
        // prettyPrint = true : JSON을 보기 좋게 줄바꿈해서 저장해줌
        string json = JsonUtility.ToJson(data, true);

        // JSON 문자열을 실제 파일로 저장
        File.WriteAllText(GetPath(), json);
        Debug.Log($"저장 완료: {GetPath()}");
    }


    // 저장된 데이터를 읽어오는 함수
    public PlayerModel RequestLoadSaveData()
    {
        // 세이브 파일 경로 가져오기
        string path = GetPath();

        // 세이브 파일이 실제로 존재한다면
        if(File.Exists(path))
        {
            // 파일 내용을 읽어옴
            string json = File.ReadAllText(path);
            // JSON -> 객체 복구 : JSON 문자열을 다시 플레이어 데이터 클래스로 변환 
            PlayerModel data = JsonUtility.FromJson<PlayerModel>(json);
            Debug.Log("데이터를 불러왔습니다.");
            return data;
        }
        // 존재하지 않는다면
        else
        {
            Debug.LogWarning("세이브 파일이 없습니다. 새 데이터를 생성합니다.");
            // 새 기본 데이터를 만들어 반환
            return GetDefaultPlayerData();
        }
    }

    // 새 플레이어 데이터를 만드는 함수
    public PlayerModel GetDefaultPlayerData()
    {
        // 새 플레이어 데이터 객체 생성
        var newPlayerData = new PlayerModel();
        newPlayerData.PlayerName = "NoName";
        newPlayerData.PlayerTotalExp = 0;
        return newPlayerData;
    }
}
