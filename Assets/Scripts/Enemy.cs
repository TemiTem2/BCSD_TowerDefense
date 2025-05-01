using UnityEngine;
using System.Collections;
public class Enemy : MonoBehaviour
{
    private int wayPointCount; //이동경로 개수
    private Transform[] waypoints;//이동경로 정보
    private int currentIndex = 0;//현재 목표지점 인덱스
    private Movement2D movement2D;//오브젝트 이동 제어
    private EnemySpawner enemySpawner;

    public void Setup(EnemySpawner enemySpawner,Transform[] waypoints)
    {
        movement2D = GetComponent<Movement2D>();
        this.enemySpawner = enemySpawner;

        //적 이동 경로 WayPoints 정보 설정
        wayPointCount = waypoints.Length;
        this.waypoints = new Transform[wayPointCount];
        this.waypoints = waypoints;

        //적의 위치를 첫번째 wayPoint 위치로 설정
        transform.position = waypoints[currentIndex].position;

        //적이동/목표지점 설정 코루틴 함수 시작

        StartCoroutine("OnMove");
    }

    private IEnumerator OnMove()
    {
        //다음 이동 방향 설정
        NextMoveTo();

        while (true)
        {
            //회전
            transform.Rotate(Vector3.forward * 10);

            //적의 현재위치와 목표위치의 거리가 0.02 * movement2D.movespeed보다 작을때 if조건문 ㅣㄹ행
            if (Vector3.Distance(transform.position, waypoints[currentIndex].position) < 0.02f * movement2D.MoveSpeed)
            {
                NextMoveTo();
            }

            yield return null;
        }
    }


    private void NextMoveTo()
    {
        //아직 이동할 way포인트가 남아있다면
        if (currentIndex < wayPointCount - 1)
        {
            transform.position = waypoints[currentIndex].position;

            currentIndex++;
            Vector3 direction = (waypoints[currentIndex].position-transform.position).normalized;
            movement2D.MoveTo(direction);
        }

        else
        {
            OnDie();
        }
    }

    public void OnDie()
    {

        enemySpawner.DestroyEnemy(this);
    }
}
