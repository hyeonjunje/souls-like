using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "Item Data", menuName = "Scriptable Object/Item Data")]
public class ItemData : ScriptableObject
{
    [SerializeField] private int _itemId;
    public int itemId { get { return _itemId; } }

    [SerializeField] private string _itemName;
    public string itemName { get { return _itemName; } }

    [SerializeField] private Sprite _itemSprite;
    public Sprite itemSprite { get { return _itemSprite; } }

    [SerializeField] private GameObject _itemPrefab;
    public GameObject itemPrefab { get { return _itemPrefab; } }

    [SerializeField] private Define.EItemType _itemType;
    public Define.EItemType itemType { get { return _itemType; } }
}
