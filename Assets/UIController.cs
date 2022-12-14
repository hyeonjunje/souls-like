using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIController : MonoBehaviour
{
    public static UIController instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [Header("Item Loot UI")]
    public CanvasGroup itemLootUI;
    public Image itemImage;
    public Text itemName;

    public void ShowLootItemUI(ItemData item)
    {
        Sequence seq = DOTween.Sequence();

        seq = DOTween.Sequence().AppendCallback(() => itemLootUI.gameObject.SetActive(true))
        .Append(itemLootUI.DOFade(1, 2f).From(0))
        .Append(itemLootUI.DOFade(0, 2f))
        .AppendCallback(() => itemLootUI.gameObject.SetActive(false));
    }
}
