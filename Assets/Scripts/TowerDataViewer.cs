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
    private TextMeshProUGUI textUpgradeCost;
    [SerializeField]
    private TextMeshProUGUI textSellCost;
    [SerializeField]
    private TowerAttackRange towerAttackRange;
    [SerializeField]
    private Button buttonUpgrade;
    [SerializeField]
    private SystemTextViewer systemTextViewer;

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
        if ( currentTower.WeaponType == WeaponType.Cannon || currentTower.WeaponType == WeaponType.Laser )
        {
            imageTower.rectTransform.sizeDelta = new Vector2(88, 59);
            textDamage.text = "Damage : " + currentTower.Damage + "+" + "<color=red>" + currentTower.AddedDamage.ToString("F1") + "</color>";
        }
        else
        {
            imageTower.rectTransform.sizeDelta = new Vector2(59, 59);

            if ( currentTower.WeaponType == WeaponType.Slow )
            {
                textDamage.text = "Slow : " + currentTower.Slow * 100 + "%";
            }
            else if ( currentTower.WeaponType == WeaponType.Buff )
            {
                textDamage.text = "Buff : " + currentTower.Buff * 100 + "%";
            }

        }

        imageTower.sprite = currentTower.TowerSprite;
        textRate.text = "Rate : " + currentTower.Rate;
        textRange.text = "Range : " + currentTower.Range;
        textLevel.text = "Level : " + currentTower.Level;
        textUpgradeCost.text = currentTower.UpgradeCost.ToString();
        textSellCost.text = currentTower.SellCost.ToString();

        //업그레이드 불가능 시 버튼 비활
        buttonUpgrade.interactable = currentTower.Level < currentTower.MaxLevel ? true : false;
    }

    public void OnClickEventTowerUpgrade()
    {
        //타워 레벨업시도
        bool isSuccess = currentTower.Upgrade();

        if ( isSuccess == true )
        {
            //성공했으므로 타워 정보 갱신
            UpdateTowerData();
            //공격범위 갱신
            towerAttackRange.OnAttackRange(currentTower.transform.position, currentTower.Range);
        }
        else
        {
            //비용 부족 출력
            systemTextViewer.PrintText(SystemType.Money);
        }
    }

    public void OnClickEventTowerSell()
    {
        //타워 판매
        currentTower.Sell();
        OffPanel();
    }
}
