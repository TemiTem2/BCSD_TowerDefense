using UnityEngine;
using System.Collections;

public class EnemyHP : MonoBehaviour
{
    [SerializeField]
    private float maxHP; //�ִ� ü��
    private float currentHP; //���� ü��
    private bool isDie = false;
    private Enemy enemy;
    private SpriteRenderer spriteRenderer;

    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP; //���� ü���� �ִ� ü�°� ���� ����
        enemy = GetComponent<Enemy>();
        spriteRenderer = GetComponent<SpriteRenderer>();    
    }

    public void TakeDamage(float damage)
    {
        //���� ü���� damage ��ŭ �����ؼ� ���� ��Ȳ�϶� ���� ������ ���ÿ� ������
        //enemy.OnDie() �Լ��� ������ ��� �� �� �ִ�.
        if ( isDie == true) return;

        //���� ä���� damage��ŭ �����
        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        //ü�� 0 ���� = ���
        if (currentHP <= 0) 
        {
            isDie = true;
            //���
            enemy.OnDie();
        }
    }

    private IEnumerator HitAlphaAnimation()
    {
        // ���� ���� ������ color ������ ���ڤӤ�
        Color color = spriteRenderer.color;

        //���� ������ 40%�� ����
        color.a = 0.4f;
        spriteRenderer.color = color;

        //0.05�� ���
        yield return new WaitForSeconds(0.05f);

        color.a = 1f;
        spriteRenderer.color = color;
    }
}
