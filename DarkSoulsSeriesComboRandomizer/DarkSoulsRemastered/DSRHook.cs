using PropertyHook;

namespace DarkSoulsSeriesComboRandomizer.DarkSoulsRemastered
{
    public class DSRHook : PHook
    {
        private readonly PHPointer GameDataManBasePtr;
        private readonly PHPointer PlayerDataPtr;
        private readonly PHPointer InventoryPtr;
        private readonly PHPointer ItemGetAddr;
        private readonly PHPointer ChrClassWarp;
        private readonly PHPointer BonfireWarpAddr;
        
        public DSRHook(int refreshInterval, int minLifetime) :
            base(refreshInterval, minLifetime, p => p.MainWindowTitle == "DARK SOULS™: REMASTERED")
        {
            GameDataManBasePtr = RegisterRelativeAOB(DSROffsets.GameDataManAOB, 3, 7);
            PlayerDataPtr = CreateChildPointer(GameDataManBasePtr, DSROffsets.GameDataManOffset1, (int)DSROffsets.GameDataMan.PlayerGameData);
            InventoryPtr = CreateChildPointer(PlayerDataPtr, (int)DSROffsets.PlayerGameData.InventoryPointer);
            ItemGetAddr = RegisterAbsoluteAOB(DSROffsets.ItemGetAOB);
            ChrClassWarp = RegisterRelativeAOB(DSROffsets.ChrClassWarpAOB, 3, 7, DSROffsets.ChrClassWarpOffset1);
            BonfireWarpAddr = RegisterAbsoluteAOB(DSROffsets.BonfireWarpAOB);
        }

        public List<DSRInventoryItem> GetCurrentInventory()
        {
            var inventorySize = PlayerDataPtr.ReadInt32((int)DSROffsets.PlayerGameData.InventorySize);
            var currentInventory = new List<DSRInventoryItem>(inventorySize);

            for (int i = 0; i < inventorySize; i++)
            {
                if(InventoryPtr.ReadByte(i * 28) == 0xFF)
                {
                    continue;
                }
                currentInventory.Add(
                    new DSRInventoryItem(
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Category),
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Id),
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Quantity),
                        InventoryPtr.ReadInt32(i * 28 + (int)DSROffsets.InventoryItem.Property4),
                        InventoryPtr.ReadInt32(i * 28 + (int)DSROffsets.InventoryItem.Property5),
                        InventoryPtr.ReadUInt32(i * 28 + (int)DSROffsets.InventoryItem.Durability),
                        InventoryPtr.ReadInt32(i * 28 + (int)DSROffsets.InventoryItem.Property7),
                        i
                    )
                );
            }
            return currentInventory;
        }

        public void GiveItem(int category, int id, int quantity)
        {
            byte[] asm = (byte[])DSRAssembly.GetItem.Clone();

            byte[] bytes = BitConverter.GetBytes(category);
            Array.Copy(bytes, 0, asm, 0x1, 4);
            bytes = BitConverter.GetBytes(quantity);
            Array.Copy(bytes, 0, asm, 0x7, 4);
            bytes = BitConverter.GetBytes(id);
            Array.Copy(bytes, 0, asm, 0xD, 4);
            bytes = BitConverter.GetBytes((ulong)GameDataManBasePtr.Resolve());
            Array.Copy(bytes, 0, asm, 0x19, 8);
            bytes = BitConverter.GetBytes((ulong)ItemGetAddr.Resolve());
            Array.Copy(bytes, 0, asm, 0x46, 8);

            Execute(asm);
        }

        public void RemoveItem(int index)
        {
            var inventorySize = PlayerDataPtr.ReadInt32((int)DSROffsets.PlayerGameData.InventorySize);
            if (index >= inventorySize)
            {
                throw new IndexOutOfRangeException($"Cannot remove item {index+1} from inventory. There are only {inventorySize} items.");
            }
            var inventoryCount = PlayerDataPtr.ReadInt32((int)DSROffsets.PlayerGameData.InventoryCount);
            PlayerDataPtr.WriteInt32((int)DSROffsets.PlayerGameData.InventoryCount, inventoryCount - 1);
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Category, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Id, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Quantity, new byte[] { 0x00, 0x00, 0x00, 0x00 });
            InventoryPtr.WriteBytes(28 * index + (int)DSROffsets.InventoryItem.Property5, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }

        public void Warp(int id)
        {
            ChrClassWarp.WriteInt32((int)DSROffsets.ChrClassWarp.LastBonfire, id);

            byte[] asm = (byte[])DSRAssembly.BonfireWarp.Clone();
            byte[] bytes = BitConverter.GetBytes(GameDataManBasePtr.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x2, 8);
            bytes = BitConverter.GetBytes(BonfireWarpAddr.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x18, 8);
            Execute(asm);
        }
    }
}