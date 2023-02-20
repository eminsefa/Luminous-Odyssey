using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ManaManager : Singleton<ManaManager>
{
    private bool  m_OnDashManaDecrease;
    private float m_IdleSpeedTimer;
    private bool  m_Wait;
    private int   m_ManaStackCount = 5;

    private ManaVariables m_ManaVars => GameConfig.Instance.Mana;

    [FoldoutGroup("Refs")] [SerializeField]
    private Image m_ManaBarFill;

    [SerializeField] private Image[] m_ManaStacks;

#region Unity Methods

    private void Update()
    {
        if (!m_Wait) updateManaAmount();
    }

#endregion

    private void updateManaAmount()
    {
        var onManaFillSpeed = PlayerMovement.Instance.IsOnManaFillSpeed;
        if (!onManaFillSpeed) m_IdleSpeedTimer += Time.deltaTime;
        else m_IdleSpeedTimer                  =  0;

        var onManaDecreaseIdle = m_IdleSpeedTimer > m_ManaVars.ManaDrainMinIdleDuration;

        var delta = onManaFillSpeed ? m_ManaVars.ManaFillAmount : onManaDecreaseIdle ? -m_ManaVars.ManaDrainAmount : 0;
        m_ManaBarFill.fillAmount += delta * Time.deltaTime;

        if (m_ManaBarFill.fillAmount >= 0.99f)
        {
            m_Wait = true;
            clearBar();
        }
        else
        {
            m_Wait = false;
        }
    }

    private void clearBar()
    {
        var amount = m_ManaBarFill.fillAmount;

        m_ManaBarFill.transform.DOPunchScale(Vector3.one * 0.25f, m_ManaVars.ManaStackedClearDelay + m_ManaVars.ManaStackedClearSpeed);
        DOTween.To(() => amount, x => amount = x, 0, m_ManaVars.ManaStackedClearSpeed)
               .SetDelay(m_ManaVars.ManaStackedClearDelay)
               .OnStart(() =>
                        {
                            m_ManaStackCount++;
                            updateStacks();
                        })
               .OnUpdate(() => m_ManaBarFill.fillAmount = amount)
               .OnComplete(() => m_Wait                 = false);
    }

    public bool IsManaEnough()
    {
        var cost    = GameConfig.Instance.Movement.DashManaCost;
        var newMana = m_ManaStackCount - cost;
        if (newMana < 0) return false;

        m_ManaStackCount = newMana;
        updateStacks();
        return true;
    }

    private void updateStacks()
    {
        var color = m_ManaStacks[0].color;
        for (int i = 0; i < m_ManaStacks.Length; i++)
        {
            color.a               = i < m_ManaStackCount ? 1 : 0;
            m_ManaStacks[i].color = color;
        }
    }
}