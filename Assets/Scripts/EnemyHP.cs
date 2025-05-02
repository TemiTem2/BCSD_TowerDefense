using UnityEngine;
using System.Collections;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP; //최대 체력
    private float currentHP; //현재 체력
    private bool isDie = false;
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP; //현재 체력을 최대 체력과 같게 설정
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void TakeDamage(float damage)
    {
        //적의 체력이 damage 만큼 감소해서 죽을 상황일때 여러 공격을 동시에 맞으면
        //enemy.OnDie() 함수가 여러변 출력 될 수 있다.
        if ( isDie == true) return;

        //현재 채력을 damage만큼 ㄱ담소
        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        //체력 0 이하 = 사망
        if (currentHP <= 0) 
        {
            isDie = true;
            //사망
            enemy.OnDie();
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // 현재 적의 색상을 color 변수에 저자ㅣㅇ
        Color color = spriteRenderer.color;

        //적의 투명도를 40%로 설정
        color.a = 0.4f;
        spriteRenderer.color = color;

        //0.05초 대기
        yield return new WaitForSeconds(0.05f);

        color.a = 1f;
        spriteRenderer.color = color;
    }
}
