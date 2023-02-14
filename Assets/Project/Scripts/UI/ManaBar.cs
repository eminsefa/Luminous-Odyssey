using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ManaBar : Singleton<ManaBar>
{
    private bool    m_ManaDecreasing;
    private float   m_ManaAmount = 1;
    private Tweener m_TwBarDecrease;

    [SerializeField] private Image m_ManaBarFill;

#region Unity Methods

    private void Update()
    {
        if(!m_ManaDecreasing) fillMana();
        updateBar();
    }

#endregion

    public bool IsManaEnough()
    {
        var cost = GameConfig.Instance.Movement.DashManaCost;
        if (m_ManaAmount < cost) return false;
        if (m_ManaDecreasing) m_TwBarDecrease.Kill();

        m_ManaDecreasing = true;
        var newMana = m_ManaAmount - cost;
        m_TwBarDecrease = DOTween.To(() => m_ManaAmount, x => m_ManaAmount = x, newMana, GameConfig.Instance.Movement.DashManaDecreaseSpeed)
                                 .SetSpeedBased()
                                 .SetEase(GameConfig.Instance.Movement.DashManaDecreaseEase)
                                 .OnKill(() =>
                                         {
                                             m_ManaAmount     = newMana;
                                             m_ManaDecreasing = false;
                                         });
        return true;
    }

    private void fillMana()
    {
        if(!m_ManaDecreasing) m_ManaAmount += GameConfig.Instance.Movement.ManaFillAmount * Time.deltaTime;
    }

    private void updateBar()
    {
        m_ManaAmount             = Mathf.Clamp01(m_ManaAmount);
        m_ManaBarFill.fillAmount = Mathf.Lerp(0, 1, m_ManaAmount);
    }
}