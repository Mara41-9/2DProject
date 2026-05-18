using UnityEngine;

public enum EntityAnimState
{
    None = 0,   // 그냥 None으로 적어도 됨
    Idle,
    Walk,
    Atk,
    JumpStart,
    JumpLoop,
    JumpEnd,
}

public class EntityAnimController : MonoBehaviour
{
    [SerializeField] private Animator Animator_Entity;

    // "현재" 어떤 애니메이션 상태인지 저장하기 위한 변수
    // 현재에 존재할 수 있는 상태는 1개!
    // 두 상태가 동시에 발생하길 원한다면, 따로 enum에 추가해라
    private EntityAnimState _currentAnimState;


    // 애니메이션 상태를 변경하는 함수
    // EntityAnimState newState : 어떤 상태로 바꿀 건지 받아온다
    public void SetState(EntityAnimState newState)
    {
        // 현재 상태와 변경하려는 상태가 둘 다 Idle이면 중복 처리하지 않고 함수 종료
        if (newState == EntityAnimState.Idle && _currentAnimState == EntityAnimState.Idle)
        {
            return;
        }

        // 비교를 했는데, 같은 값이 아니고, 이제 동작을 바꿔도 된다면 이렇게 대입
        _currentAnimState = newState;

        switch(_currentAnimState)
        {
            case EntityAnimState.Idle:
                ResetAllParameters();
                break;
            case EntityAnimState.Walk:
                Animator_Entity.SetBool("IsWalk", true);
                break;
            case EntityAnimState.Atk:
                Animator_Entity.SetBool("IsAtk", true);
                break;
            case EntityAnimState.JumpStart:
                Animator_Entity.SetBool("IsJump", true);
                break;
            case EntityAnimState.JumpEnd:
                Animator_Entity.SetBool("IsJump", false);
                break;
            //case EntityAnimState.TargetDetected:   // 시야 범위 안에 플레이어가 들어왔을때
            //    Animator_Entity.SetBool("IsTargetDetected", true);
            //    break;
            //case EntityAnimState.TargetInteracted: // 플레이어와 상호작용 일어날때
            //    Animator_Entity.SetBool("IsTargetInteracted", true);
            //    break;
            //case EntityAnimState.InteractSuccess:  // 성공인 경우 (와~! 그걸 구매하시다니 대단하시군)
            //    Animator_Entity.SetBool("IsInteractSuccess", true);
            //    break;
            //case EntityAnimState.InteractFailed:   // 실패, 부정일 경우 (할인은 안되네!)
            //    Animator_Entity.SetBool("IsInteractFailed", true);
            //    break;
            default:
                ResetAllParameters();
                break;
        }
    }

    // Animator의 애니메이션 상태값들을 전부 초기화하는 함수
    private void ResetAllParameters()
    {
        Animator_Entity.SetBool("IsWalk", false);
        Animator_Entity.SetBool("IsAtk", false);
        // Animator_Entity.SetBool("IsJump", false); -> 
    }

}
