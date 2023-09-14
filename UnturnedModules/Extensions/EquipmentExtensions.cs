using SDG.Unturned;

namespace Hydriuk.UnturnedModules.Extensions
{
    public static class EquipmentExtensions
    {
        public static ItemJar? GetItemJar(this PlayerEquipment equipment)
        {
            if (!equipment.isEquipped)
                return null;

            return equipment.player.inventory.GetItemJar(equipment.equippedPage, equipment.equipped_x, equipment.equipped_y);
        }

        public static Item GetItem(this PlayerEquipment equipment)
        {
            return equipment.player.inventory.GetItem(equipment.equippedPage, equipment.equipped_x, equipment.equipped_y);
        }
    }
}