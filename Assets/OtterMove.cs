using UnityEngine;

public class OtterMove : MonoBehaviour
{
    public float moveSpeed = 2f;       // 이동 속도
    public float rotationSpeed = 2f;   // 회전 속도
    private Vector3 targetPosition;
    private bool isMoving = false;

    private Animator animator;

    void Start()
    {
        // 시작 시 현재 위치를 목표 위치로 설정
        targetPosition = transform.position;

        // Animator 가져오기 (자식에 있으면 GetComponentInChildren, 같은 오브젝트면 GetComponent)
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        // 1. 마우스/터치 클릭 시 목표 위치 갱신
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Input.mousePosition;
            // 탑다운 시점에서 카메라 높이를 z로 사용
            clickPos.z = Camera.main.transform.position.y;

            // 스크린 좌표 → 월드 좌표
            targetPosition = Camera.main.ScreenToWorldPoint(clickPos);
            targetPosition.y = 0f; // 바닥(0)에 고정

            // 이동 시작
            isMoving = true;

            // 걷기 애니메이션 켜기
            if (animator != null)
                animator.SetBool("isWalking", true);
        }

        // 2. 실제 이동 로직
        if (isMoving)
        {
            // (a) 위치 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // (b) 이동 방향 구해서 회전
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRot = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRot,
                    rotationSpeed * Time.deltaTime
                );
            }

            // (c) 목표 지점 도착 체크
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;

                // 걷기 중단 → Idle로 전환
                if (animator != null)
                    animator.SetBool("isWalking", false);
            }
        }
    }
}
