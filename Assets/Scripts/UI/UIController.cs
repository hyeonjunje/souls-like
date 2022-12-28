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

    [Header("Interactive UI")]
    public GameObject interactiveUI;
    public Text interactiveText;

    [Header("Info UI")]
    public CanvasGroup infoUI;
    public Text infoText;

    [Header("Setting UI")]
    public GameObject settingUI;
    public GameObject keyExplanation;
    private bool isSettingUI = false;
    private bool isKeyExplanation = false;

    Sequence seq;

    public void ShowLootItemUI(ItemData item)
    {
        seq.Kill();

        seq = DOTween.Sequence().AppendCallback(() =>
        {
            itemLootUI.alpha = 0;

            itemLootUI.gameObject.SetActive(true);
            itemImage.sprite = item.itemSprite;
            itemName.text = item.itemName;
        })
        .Append(itemLootUI.DOFade(1, 2f))
        .Append(itemLootUI.DOFade(0, 2f))
        .AppendCallback(() => itemLootUI.gameObject.SetActive(false));
    }


    public void ShowInteractiveEnterText(string contents)
    {
        interactiveUI.SetActive(true);
        interactiveText.text = "E : " + contents;
    }


    public void HideInteractiveExitText()
    {
        interactiveUI.SetActive(false);
    }


    public void ShowInfoUI(string text)
    {
        seq.Kill();

        seq = DOTween.Sequence().AppendCallback(() =>
        {
            itemLootUI.alpha = 0;

            infoUI.gameObject.SetActive(true);
            infoText.text = text;
        })
        .Append(itemLootUI.DOFade(1, 2f))
        .Append(itemLootUI.DOFade(0, 2f))
        .AppendCallback(() => infoUI.gameObject.SetActive(false));
    }



    public void ActiveSettingUI()
    {
        if(isSettingUI)
        {
            Debug.Log("설정창 끄기");

            isSettingUI = false;
            settingUI.SetActive(false);

            Time.timeScale = 1;
            Cursor.visible = false;
        }
        else
        {
            Debug.Log("설정창 켜기");

            isSettingUI = true;
            settingUI.SetActive(true);

            Time.timeScale = 0;
            Cursor.visible = true;
        }
    }


    public void SettingButtonMainMenu()
    {
        ActiveSettingUI();

        GameManager.instance.ReturnMainMenu();
    }


    public void SettingButtonContinue()
    {
        ActiveSettingUI();
    }


    public void SettingKeyExplanation()
    {
        if (isKeyExplanation)
        {
            isKeyExplanation = false;
            keyExplanation.SetActive(false);
        }
        else
        {
            isKeyExplanation = true;
            keyExplanation.SetActive(true);
        }
    }
}
