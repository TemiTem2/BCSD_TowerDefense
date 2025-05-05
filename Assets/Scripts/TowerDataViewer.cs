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
        //타워 정보 받기
        currentTower = towerWeapon.GetComponent<TowerWeapon>();
        //타워 정보 Panel On
        gameObject.SetActive(true);
        //타워 정보 갱신
        UpdateTowerData();
        //타워 공격범위 표시
        towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
    }    

    public void OffPanel()
    {
        //타워 정보 Panel Off
        gameObject.SetActive(false);
        //공격범위 끄기
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
