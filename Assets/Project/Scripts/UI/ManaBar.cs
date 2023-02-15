using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : Singleton<ManaBar>
{
    private bool    m_OnDashManaDecrease;
    private float   m_ManaAmount = 1;
    private float   m_IdleSpeedTimer;
    private Tweener m_TwBarDecrease;

    private MovementVariables m_Movement => GameConfig.Instance.Movement;

    [FoldoutGroup("Refs")] [SerializeField] private Image m_ManaBarFill;

#region Unity Methods

    private void Update()
    {
        if (!m_OnDashManaDecrease) updateManaAmount();
        updateBar();
    }

#endregion

    public bool IsManaEnough()
    {
        var cost = GameConfig.Instance.Movement.DashManaCost;
        if (m_ManaAmount < cost) return false;
        if (m_OnDashManaDecrease) m_TwBarDecrease.Kill();

        m_OnDashManaDecrease = true;
        var newMana = m_ManaAmount - cost;
        m_TwBarDecrease = DOTween.To(() => m_ManaAmount, x => m_ManaAmount = x, newMana, m_Movement.DashManaDrainSpeed)
                                 .SetSpeedBased()
                                 .SetEase(m_Movement.DashManaDrainEase)
                                 .OnKill(() =>
                                         {
                                             m_ManaAmount         = newMana;
                                             m_OnDashManaDecrease = false;
                                         });
        return true;
    }

    private void updateManaAmount()
    {
        var onManaFillSpeed = PlayerMovement.Instance.IsOnManaFillSpeed;
        if (!onManaFillSpeed) m_IdleSpeedTimer += Time.deltaTime;
        else m_IdleSpeedTimer                  =  0;

        var onManaDecreaseIdle = m_IdleSpeedTimer > m_Movement.ManaDrainMinIdleDuration;

        var delta = onManaFillSpeed ? m_Movement.ManaFillAmount : onManaDecreaseIdle ? -m_Movement.ManaDrainAmount : 0;
        m_ManaAmount += delta * Time.deltaTime;
    }

    private void updateBar()
    {
        m_ManaAmount             = Mathf.Clamp01(m_ManaAmount);
        m_ManaBarFill.fillAmount = Mathf.Lerp(0, 1, m_ManaAmount);
    }
}