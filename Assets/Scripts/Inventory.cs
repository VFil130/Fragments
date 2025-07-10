using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class Inventory : MonoBehaviour
{
    public List<ObjectClass> inventory = new List<ObjectClass>();
    public int inventorySize = 18;
    public float interactionRadius = 1f;
    public GameObject inventoryPanel;
    public Image[] inventorySlots;
    public TMP_Text itemNameText;
    public TMP_Text itemDescriptionText;
    public GameObject inventoryCanvas;

    public Image craftSlot1;
    public Image craftSlot2;
    public Button craftButton;
    private int currentCraftSlot = 0;
    private int craftItem1;
    private int craftItem2;

    public List<List<int>> craftingRecipes = new List<List<int>>();
    public string pathToCraftableObjects = "CraftableObjects";

    void Start()
    {
        if (inventoryPanel != null)
        {
            inventorySlots = new Image[inventorySize];
            for (int i = 0; i < inventorySize; i++)
            {
                Transform slotTransform = inventoryPanel.transform.GetChild(i);
                if (slotTransform != null)
                {
                    Image itemImage = slotTransform.Find("ItemImage").GetComponent<Image>();
                    if (itemImage != null)
                    {
                        inventorySlots[i] = itemImage;
                    }
                    else
                    {
                        Debug.LogError("Slot " + i + " does not contain a child Image with name 'ItemImage'!");
                    }
                }
                else
                {
                    Debug.LogError("Slot " + i + " not found!");
                }
            }
        }
        else
        {
            Debug.LogError("Inventory panel not assigned!");
        }
        craftButton.onClick.AddListener(CraftItems);
        UpdateInventoryUI();
        craftingRecipes.Add(new List<int> { 1, 2, 4 });

        // Проверка существования папки
        string folderPath = pathToCraftableObjects;
        if (string.IsNullOrEmpty(folderPath))
        {
            Debug.LogError("Путь к папке с префабами не указан!");
        }
        else
        {
            GameObject[] itemPrefabs = Resources.LoadAll<GameObject>(folderPath);
            if (itemPrefabs == null || itemPrefabs.Length == 0)
            {
                Debug.LogError($"Не удалось найти префабы в папке: {folderPath}. Убедитесь, что путь указан верно и в папке есть префабы.");
            }
            else
            {
                Debug.Log($"Найдено {itemPrefabs.Length} префабов в папке: {folderPath}");
                foreach (GameObject itemPrefab in itemPrefabs)
                {
                    ObjectClass obj = itemPrefab.GetComponent<ObjectClass>();
                    if (obj != null)
                    {
                        Debug.Log($"Префаб: {itemPrefab.name}, ID: {obj.id}, Name: {obj.itemName}, Description: {obj.itemDescription}");
                    }
                    else
                    {
                        Debug.LogWarning($"Префаб: {itemPrefab.name} не имеет компонента ObjectClass.");
                    }
                }
            }
        }
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            inventoryCanvas.SetActive(!inventoryCanvas.activeSelf);
        }
        if (inventoryCanvas.activeSelf)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            for (int i = 0; i < inventorySize; i++)
            {
                if (inventorySlots[i] != null)
                {
                    RectTransform slotRect = inventorySlots[i].GetComponent<RectTransform>();
                    if (RectTransformUtility.RectangleContainsScreenPoint(slotRect, Input.mousePosition))
                    {
                        if (i < inventory.Count)
                        {
                            itemNameText.text = inventory[i].itemName;
                            itemDescriptionText.text = inventory[i].itemDescription;
                        }
                        else
                        {
                            itemNameText.text = "";
                            itemDescriptionText.text = "";
                        }
                        return;
                    }
                }
            }
            itemNameText.text = "";
            itemDescriptionText.text = "";
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, interactionRadius);
            foreach (Collider2D collider in colliders)
            {
                ObjectClass obj = collider.GetComponent<ObjectClass>();
                if (obj != null)
                {
                    if (AddItem(obj))
                    {
                        Destroy(collider.gameObject);
                        UpdateInventoryUI();
                        break;
                    }
                    else
                    {
                        Debug.Log("Inventory is full!");
                        break;
                    }
                }
            }
        }
    }
    public void AddToCraftSlot(Image itemImage, int itemIndex)
    {
        if (currentCraftSlot >= 2)
        {
            Debug.Log("Крафт слоты заполнены");
            return;
        }
        if (itemIndex >= 0 && itemIndex < inventory.Count)
        {
            if (currentCraftSlot == 0)
            {
                craftSlot1.sprite = itemImage.sprite;
                craftSlot1.enabled = true;
                craftItem1 = inventory[itemIndex].id;
                currentCraftSlot++;
            }
            else if (currentCraftSlot == 1)
            {
                craftSlot2.sprite = itemImage.sprite;
                craftSlot2.enabled = true;
                craftItem2 = inventory[itemIndex].id;
                currentCraftSlot++;
            }
        }
    }
    public void CraftItems()
    {
        Debug.Log("Crafting...");
        if (craftItem1 != 0 && craftItem2 != 0)
        {
            foreach (List<int> recipe in craftingRecipes)
            {
                if (recipe.Count >= 3 && recipe[0] == craftItem1 && recipe[1] == craftItem2)
                {
                    Debug.Log("Крафт успешен");
                    craftSlot1.sprite = null;
                    craftSlot1.enabled = false;
                    craftItem1 = 0;
                    craftSlot2.sprite = null;
                    craftSlot2.enabled = false;
                    craftItem2 = 0;
                    currentCraftSlot = 0;
                    CreateCraftedItem(recipe[recipe.Count - 1]);
                    RemoveCraftingItems(new List<int> { recipe[0], recipe[1] });
                    return;
                }
            }
            Debug.Log("Недостаточно предметов для крафта");
        }
        else
        {
            Debug.Log("Недостаточно предметов для крафта");
        }
    }
    bool AddItem(ObjectClass item)
    {
        if (inventory.Count < inventorySize)
        {
            inventory.Add(item);
            Debug.Log("Added " + item.itemName + " to inventory.");
            return true;
        }
        else
        {
            return false;
        }
    }
    public void UpdateInventoryUI()
    {
        for (int i = 0; i < inventorySize; i++)
        {
            if (i < inventory.Count)
            {
                if (inventorySlots[i] != null && inventory[i].itemIcon != null)
                {
                    inventorySlots[i].sprite = inventory[i].itemIcon;
                    inventorySlots[i].gameObject.SetActive(true);
                    ItemClickHandler itemClickHandler = inventorySlots[i].GetComponent<ItemClickHandler>();
                    if (itemClickHandler == null)
                    {
                        itemClickHandler = inventorySlots[i].gameObject.AddComponent<ItemClickHandler>();
                        itemClickHandler.Initialize(i, this);
                    }
                    else
                    {
                        itemClickHandler.Initialize(i, this);
                    }
                }
                else if (inventorySlots[i] != null)
                {
                    inventorySlots[i].gameObject.SetActive(false);
                }
            }
            else
            {
                if (inventorySlots[i] != null)
                {
                    inventorySlots[i].gameObject.SetActive(false);
                }
            }
        }
    }
    private void CreateCraftedItem(int itemId)
    {
        string folderPath = $"{pathToCraftableObjects}";
        GameObject[] itemPrefabs = Resources.LoadAll<GameObject>(folderPath);
        foreach (GameObject itemPrefab in itemPrefabs)
        {
            ObjectClass obj = itemPrefab.GetComponent<ObjectClass>();
            if (obj != null && obj.id == itemId)
            {
                GameObject newItem = Instantiate(itemPrefab);
                ObjectClass newObj = newItem.GetComponent<ObjectClass>();
                if (newObj != null)
                {
                    AddItem(newObj);
                    Debug.Log($"Предмет с ID {itemId} был успешно добавлен!");
                    return;
                }
                else
                {
                    Debug.LogError($"Не удалось получить ObjectClass у созданного предмета с ID {itemId}");
                    return;
                }
            }
        }
        Debug.LogError($"Не удалось найти префаб с ID {itemId} в папке: {folderPath}");
    }
    private void RemoveCraftingItems(List<int> itemIds)
    {
        for (int i = inventory.Count - 1; i >= 0; i--)
        {
            if (itemIds.Contains(inventory[i].id))
            {
                inventory.RemoveAt(i);
            }
        }
        UpdateInventoryUI();
    }
}