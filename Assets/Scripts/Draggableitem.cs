using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemClickHandler : MonoBehaviour, IPointerClickHandler
{
    public int itemIndex;
    public Inventory inventory;

    public void Initialize(int index, Inventory inv)
    {
        itemIndex = index;
        inventory = inv;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked on item with index: " + itemIndex);

        if (itemIndex >= 0)
        {
            inventory.itemNameText.text = inventory.inventory[itemIndex].itemName;
            inventory.itemDescriptionText.text = inventory.inventory[itemIndex].itemDescription;
            inventory.AddToCraftSlot(this.GetComponent<Image>(), itemIndex);
        }
    }
}