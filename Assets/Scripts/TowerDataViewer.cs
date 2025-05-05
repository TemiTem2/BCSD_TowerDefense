using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TowerDataViewer : MonoBehaviour
{
    [SerializeField]
    private Image imageTower;
    [SerializeField]
    private TextMeshProUGUI textDamage;
    [SerializeField]
    private TextMeshProUGUI textRate;
    [SerializeField]
    private TextMeshProUGUI textRange;
    [SerializeField]
    private TextMeshProUGUI textLevel;
    [SerializeField]
    private TowerAttackRange towerAttackRange;

    private TowerWeapon currentTower;

    private void Awake()
    {
        OffPanel();
    }

    private void Update()
    {
        if ( Input.GetKeyDown(KeyCode.Escape) )
        {
            OffPanel();
        }
    }

    public void Onpanel(Transform towerWeapon)
    {
        //Ÿ�� ���� �ޱ�
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        //Ÿ�� ���� Panel On
        gameObject.SetActive(true);
        //Ÿ�� ���� ����
        UpdateTowerData();
        //Ÿ�� ���ݹ��� ǥ��
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }    

    public void OffPanel()
    {
        //Ÿ�� ���� Panel Off
        gameObject.SetActive(false);
        //���ݹ��� ����
        towerAttackRange.OffAttackRange();
    }

    private void UpdateTowerData()
    {
        textDamage.text = "Damage : " + currentTower.Damage;
        textRate.text = "Rate : " + currentTower.Rate;
        textRange.text = "Range : " + currentTower.Range;
        textLevel.text = "Level : " + currentTower.Level;
    }
}
