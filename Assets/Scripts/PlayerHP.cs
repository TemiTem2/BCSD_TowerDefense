using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerHP : MonoBehaviour
{
    [SerializeField]
    private Image imageScreen;//전체화면의 빨간색 이미지
    [SerializeField]
    private float maxHP = 20;//최대 체력
    private float currentHP; //현재체력
    
    public float MaxHP => maxHP;
    public float CurrentHP => currentHP;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float damage)
    {
        currentHP -= damage;

        StopCoroutine("HitAlphaAnimation");
        StartCoroutine("HitAlphaAnimation");

        if (currentHP <= 0) 
        { 
        
        }
        
    }

    private IEnumerator HitAlphaAnimation()
    {
        Color color = imageScreen.color;
        color.a = 0.4f;
        imageScreen.color = color;

       while (color.a > -0.0f)
        {
            color.a -= Time.deltaTime;
            imageScreen.color = color;

            yield return null;

        }
        
    }
}
