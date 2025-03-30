using UnityEngine;

public class OtterMove : MonoBehaviour
{
    public float moveSpeed = 5f;       // 이동 속도
    public float rotationSpeed = 10f;  // 회전 속도 (커질수록 더 빠르게 방향 전환)

    private Vector3 targetPosition;
    private bool isMoving = false;

    void Start()
    {
        // 시작 시 현재 위치를 목표 위치로 설정
        targetPosition = transform.position;
    }

    void Update()
    {
        // 1) 마우스 클릭하면 목표 위치 갱신
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 clickPos = Input.mousePosition;
            // 카메라가 위에서 내려보는 탑다운 시점이라면,
            // 카메라 높이(Camera.main.transform.position.y)를 z 대신 사용
            clickPos.z = Camera.main.transform.position.y;

            targetPosition = Camera.main.ScreenToWorldPoint(clickPos);
            // 탑다운에서는 y를 바닥(0)으로 고정해 주면 캐릭터가 뜨지 않음
            targetPosition.y = 0f;

            isMoving = true;
        }

        // 2) 이동 및 회전 처리
        if (isMoving)
        {
            // (a) 이동
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPosition,
                moveSpeed * Time.deltaTime
            );

            // (b) 방향 회전
            // 이동할 방향 벡터 = 목표위치 - 현재위치
            Vector3 direction = targetPosition - transform.position;
            direction.y = 0f; // 수평 회전만 하기 위해 y축은 0으로 만든다

            // 혹시 direction이 (0,0,0)이 아닐 때만 회전
            if (direction.sqrMagnitude > 0.001f)
            {
                // 목표 방향으로의 회전(쿼터니언)
                Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
                // 부드럽게 회전 → Slerp 사용
                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            // (c) 목표 지점에 거의 도달하면 이동 중단
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isMoving = false;
            }
        }
    }
}
