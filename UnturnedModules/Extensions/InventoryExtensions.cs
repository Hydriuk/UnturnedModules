using SDG.Unturned;

namespace Hydriuk.UnturnedModules.Extensions
{
    public static class InventoryExtensions
    {
        public static ItemJar GetItemJar(this PlayerInventory inventory, byte page, byte x, byte y)
        {
            byte index = inventory.getIndex(page, x, y);

            return inventory.getItem(page, index);
        }

        public static Item? GetItem(this PlayerInventory inventory, byte page, byte x, byte y)
        {
            return inventory.GetItemJar(page, x, y)?.item;
        }

        public static void DeleteItem(this PlayerInventory inventory, byte page, byte x, byte y)
        {
            byte index = inventory.getIndex(page, x, y);

            inventory.removeItem(page, index);
        }

        public static void DeleteItem(this PlayerInventory inventory, byte page, ItemJar itemJar)
        {
            inventory.DeleteItem(page, itemJar.x, itemJar.y);
        }
    }
}