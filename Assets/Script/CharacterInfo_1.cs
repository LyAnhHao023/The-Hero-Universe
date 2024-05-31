using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;


public class CharacterInfo_1 : MonoBehaviour
{
    public InventorySlotsManager inventorySlotsManager;

    public MenuManager menuManager;

    public int currentHealth;

    public int currentSlowhealth = 0;

    public HealthBar healthBar;

    [SerializeField] GameObject slowHealth;
    public SlowHealthBar slowHealthBar;

    [SerializeField] GameObject shield;
    public ShieldBar shieldBar;
    public Slider shieldSlider;

    public ExpBar expBar;
    public Slider expSlider;
    public int level;
    public int maxExpValue;

    public float elapsedTime;

    public CountSys countSys;

    public GameOverCoin overCoin;

    public PlayerStatShow statShow;

    [SerializeField] LevelUpSelectBuff levelUpSelectBuff;

    List<UpgradeData> upgradeDatas;
    List<UpgradeData> weaponSlotsManager = new List<UpgradeData>();
    List<UpgradeData> itemSlotsManager = new List<UpgradeData>();

    [SerializeField] WeaponsManager weaponsManager;
    [SerializeField] PassiveItemsManager itemsManager;

    int currentExp;

    private int coins = 0;

    int maxHealth;

    public float healthPercent = 0;
    public float attackPercent = 0;
    public float speedPercent = 0;
    public float critPercent = 0;

    int baseHealth;
    int baseAttack;
    float baseCrit;
    int baseSpeed;

    GameObject character;
    public CharacterStats characterStats;

    public int numberMonsterKilled = 0;

    int slowHealthLevel = 1;
    bool slowHealthAcquired = true;
    float time;

    int shieldMaxValue;
    public int shieldCurrentValue;

    private void Awake()
    {
        character = GameObject.Find("FistCharDev");
        characterStats = character.GetComponent<CharacterStats>();

        baseAttack = characterStats.strenght;
        baseCrit = characterStats.crit;
        baseSpeed = characterStats.speed;
        baseHealth = characterStats.maxHealth;

        maxHealth = characterStats.maxHealth;
        currentHealth = maxHealth + Mathf.FloorToInt(characterStats.maxHealth * healthPercent);
        healthBar.SetMaxHealth(maxHealth);

        slowHealth.SetActive(false);
        shield.SetActive(false);
        shieldMaxValue = 0;
        shieldCurrentValue = 0;

        weaponSlotsManager.Add(characterStats.beginerWeapon);
        weaponsManager.AddWeapon(characterStats.beginerWeapon.weaponData);
        inventorySlotsManager.WeaponSlotUpdate(weaponSlotsManager);

        level = 1;
        maxExpValue = 10;
        currentExp = 0;
        expBar.SetMaxExp(level, maxExpValue);
        countSys.SetCoinCount(0);
        countSys.SetKillCount(0);
        overCoin.SetCoinGain(0);

        statShow.SetHealth(maxHealth);
        statShow.SetAttack(characterStats.strenght);
        statShow.SetSpeed(characterStats.speed);
        statShow.SetCrit(characterStats.crit);
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (slowHealthAcquired)
        {
            slowHealth.SetActive(true);

            if (currentHealth >= currentSlowhealth && currentHealth > 1)
            {
                currentSlowhealth = currentHealth;
                slowHealthBar.SetMaxHealth(maxHealth);
                slowHealthBar.SetHealth(currentSlowhealth);
            }

            if (currentSlowhealth > currentHealth)
            {
                time += Time.deltaTime;

                switch (slowHealthLevel)
                {
                    case 1:
                        {
                            if (time >= 0.5)
                            {
                                currentSlowhealth -= 1;
                                time = 0;
                            }
                        }
                        break;
                    case 2:
                        {
                            if (time >= 0.7)
                            {
                                currentSlowhealth -= 1;
                                time = 0;
                            }
                        }
                        break;
                    case 3:
                        {
                            if (time >= 1)
                            {
                                currentSlowhealth -= 1;
                                time = 0;
                            }
                        }
                        break;
                    case 4:
                        {
                            if (time >= 1.5)
                            {
                                currentSlowhealth -= 1;
                                time = 0;
                            }
                        }
                        break;
                }

                slowHealthBar.SetHealth(currentSlowhealth);
            }

        }
    }

    public void PlusMaxSheild(int shieldPlus)
    {
        if (shieldMaxValue == 0)
        {
            shield.SetActive(true);
            shieldMaxValue += shieldPlus;
            shieldCurrentValue = shieldMaxValue;

            shieldBar.SetMaxShield(shieldMaxValue);
            shieldBar.SetShield(shieldCurrentValue);
        }
        else
        {
            shieldMaxValue += shieldPlus;
            shieldCurrentValue += shieldPlus;

            shieldBar.SetMaxShield(shieldMaxValue);
            shieldBar.SetShield(shieldCurrentValue);
        }
    }

    public void ShieldRecovery(int shieldRecovery)
    {
        shieldCurrentValue = (shieldCurrentValue + shieldRecovery > shieldMaxValue) ? shieldMaxValue : shieldCurrentValue + shieldRecovery;
        shieldBar.SetShield(shieldCurrentValue);
    }

    public void KilledMonster()
    {
        ++numberMonsterKilled;
        countSys.SetKillCount(numberMonsterKilled);
    }

    public void GainCoin(int coinGain)
    {
        coins += coinGain;
        countSys.SetCoinCount(coins);
    }

    public void GainExp(int exp)
    {
        currentExp += exp;

        if (currentExp >= maxExpValue)
        {
            LevelUp();
        }

        expBar.SetExp(currentExp);
    }

    public void LevelUp()
    {
        upgradeDatas = levelUpSelectBuff.GetUpgrades(4);
        menuManager.LevelUpScene(upgradeDatas);
        currentExp -= maxExpValue;
        level += 1;
        maxExpValue += Mathf.FloorToInt((float)(maxExpValue * 0.5));
        expBar.SetMaxExp(level, maxExpValue);
    }

    public void Upgrade(int id)
    {
        switch ((int)upgradeDatas[id].upgradeType)
        {
            case 0: //WeaponUpgrade
                {

                }
                break;
            case 1: //ItemUpgrade
                {

                }
                break;
            case 2: //WeaponUnlock
                {
                    weaponSlotsManager.Add(upgradeDatas[id]);
                    weaponsManager.AddWeapon(upgradeDatas[id].weaponData);
                    inventorySlotsManager.WeaponSlotUpdate(weaponSlotsManager);
                    upgradeDatas[id].acquired = true;
                }
                break;
            case 3: //ItemUnlock
                {
                    itemSlotsManager.Add(upgradeDatas[id]);
                    itemsManager.AddItem(upgradeDatas[id].itemsData);
                    inventorySlotsManager.ItemSlotUpdate(itemSlotsManager);
                    upgradeDatas[id].acquired = true;
                }
                break;
            case 4: //StatUpgrade
                {
                    if (upgradeDatas[id].buffName.Contains("ATK"))
                    {
                        attackPercent += 0.1f;
                        statUpdate();
                    }
                    if (upgradeDatas[id].buffName.Contains("CRT"))
                    {
                        critPercent += 2;
                        statUpdate();
                    }
                    if (upgradeDatas[id].buffName.Contains("HP"))
                    {
                        healthPercent += 0.1f;
                        statUpdate();
                    }
                    if (upgradeDatas[id].buffName.Contains("SPD"))
                    {
                        speedPercent += 0.05f;
                        statUpdate();
                    }
                }
                break;
            case 5: //GainCoin
                {
                    coins += 50;
                    countSys.SetCoinCount(coins);
                }
                break;
        }

        menuManager.LevelUpDone();
    }

    public void TakeDamage(int damage)
    {
        if (shieldCurrentValue<=0)
        {
            currentHealth -= damage;

            if (slowHealthAcquired)
            {
                if (currentHealth < 1)
                {
                    currentHealth = 1;
                }

                currentSlowhealth -= 1;

                slowHealthBar.SetHealth(currentSlowhealth);
            }

            if (currentHealth <= 0 || currentSlowhealth <= 0)
            {
                menuManager.GameOverScreen();
                coins = CoinGainPercent(coins, Mathf.FloorToInt(elapsedTime % 60));
                overCoin.SetCoinGain(coins);
                int coinLocal = PlayerPrefs.GetInt("Coins", 0);
                //Debug.Log(coinLocal +" local");
                PlayerPrefs.SetInt("Coins", coinLocal + coins);
                PlayerPrefs.Save();
                //Debug.Log(PlayerPrefs.GetInt("Coins", 0));
            }

            healthBar.SetHealth(currentHealth);
        }
        else
        {
            if(shieldCurrentValue>damage)
            {
                shieldCurrentValue -= damage;
                shieldBar.SetShield(shieldCurrentValue);
            }
            else
            {
                int dmg = damage - shieldCurrentValue;
                shieldCurrentValue=0;
                shieldBar.SetShield(shieldCurrentValue);
                TakeDamage(dmg);
            }
        }
    }

    public void HealthByPercent(int health)
    {
        currentHealth += maxHealth * health / 100;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    public void HealthByNumber(int health)
    {
        currentHealth += health;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    public int CoinGainPercent(int coins, int timer)
    {
        if (timer <= 600)
            return ((int)((float)(coins * 0.25)));
        else if (timer <= 900)
            return ((int)((float)(coins * 0.5)));
        else if (timer <= 1200)
            return ((int)((float)(coins * 0.75)));

        return coins;
    }

    public void statUpdate()
    {
        characterStats.strenght = baseAttack + Mathf.FloorToInt((float)baseAttack * attackPercent);
        characterStats.crit = baseCrit + critPercent;
        characterStats.maxHealth = baseHealth + Mathf.FloorToInt((float)baseHealth * healthPercent);
        characterStats.speed = baseSpeed + Mathf.FloorToInt((float)baseSpeed * speedPercent);
        statShow.SetAttack(characterStats.strenght);
        statShow.SetCrit(characterStats.crit);
        maxHealth = characterStats.maxHealth;
        statShow.SetHealth(characterStats.maxHealth);
        statShow.SetSpeed(characterStats.speed);

        if (slowHealthAcquired)
        {
            slowHealthBar.SetMaxHealth(characterStats.maxHealth);
            slowHealthBar.SetHealth(currentSlowhealth);
        }
        else
        {
            healthBar.SetMaxHealth(characterStats.maxHealth);
            healthBar.SetHealth(currentHealth);
        }
    }
}

