using PropertyHook;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls3
{
    public class DS3Hook : PHook
    {
        private readonly PHPointer GameDataManBasePtr;
        private readonly PHPointer GameMan;
        private readonly PHPointer SprjLuaEventMan;
        private readonly PHPointer SprjEventFlagMan;
        private readonly PHPointer PlayerDataPtr;
        private readonly PHPointer InventoryPtr;
        private readonly PHPointer ItemGet_Call;
        private readonly PHPointer MapItemManAddr;
        private readonly PHPointer ReadEventFlag_Call;
        private readonly PHPointer WriteEventFlag_Call;

        public DS3Hook(int refreshInterval, int minLifetime) :
            base(refreshInterval, minLifetime, p => p.MainWindowTitle == "DARK SOULS III")
        {
            GameDataManBasePtr = RegisterRelativeAOB(DS3Offsets.GameDataManAOB, 3, 7);
            GameMan = RegisterRelativeAOB(DS3Offsets.GameManAOB, 3, 7, 0);
            SprjLuaEventMan = RegisterRelativeAOB(DS3Offsets.SprjLuaEventManAOB, 3, 8, 0);
            SprjEventFlagMan = RegisterRelativeAOB(DS3Offsets.SprjEventFlagManAOB, 3, 7, 0);
            PlayerDataPtr = CreateChildPointer(GameDataManBasePtr, DS3Offsets.GameDataManOffset1, (int)DS3Offsets.GameDataMan.PlayerGameData);
            InventoryPtr = CreateChildPointer(PlayerDataPtr, (int)DS3Offsets.PlayerGameData.InventoryPointer);
            ItemGet_Call = RegisterAbsoluteAOB(DS3Offsets.ItemGetAOB);
            MapItemManAddr = RegisterRelativeAOB(DS3Offsets.MapItemManAOB, 3, 7);
            ReadEventFlag_Call = RegisterAbsoluteAOB(DS3Offsets.ReadEventFlagAOB);
            WriteEventFlag_Call = RegisterAbsoluteAOB(DS3Offsets.WriteEventFlagAOB);
        }

        public List<DS3InventoryItem> GetCurrentInventory()
        {
            var inventorySize = PlayerDataPtr.ReadInt32((int)DS3Offsets.PlayerGameData.InventoryCount);
            var inventoryArraySize = PlayerDataPtr.ReadInt32((int)DS3Offsets.PlayerGameData.InventoryArraySize);
            var currentInventory = new List<DS3InventoryItem>(inventorySize);

            for (int i = 0; i < inventorySize; i++)
            {
                if (currentInventory.Count == inventorySize)
                {
                    break;
                }
                if (InventoryPtr.ReadByte(i * 16 + (int)DS3Offsets.InventoryItem.Id) == 0xFF)
                {
                    continue;
                }
                currentInventory.Add(
                    new DS3InventoryItem(
                        InventoryPtr.ReadInt32(i * 16 + (int)DS3Offsets.InventoryItem.Property1),
                        InventoryPtr.ReadUInt32(i * 16 + (int)DS3Offsets.InventoryItem.Id),
                        InventoryPtr.ReadUInt32(i * 16 + (int)DS3Offsets.InventoryItem.Quantity),
                        InventoryPtr.ReadInt32(i * 16 + (int)DS3Offsets.InventoryItem.Property4),
                        i
                    )
                );
            }
            return currentInventory;
        }

        public void GiveItem(int id, int quantity)
        {
            byte[] asm = (byte[])DS3Assembly.GetItem.Clone();

            IntPtr itemToGive = Allocate(0x80);
            Kernel32.WriteInt32(Handle, itemToGive, 0);
            Kernel32.WriteInt32(Handle, itemToGive+0x04, 0);
            Kernel32.WriteInt32(Handle, itemToGive+0x08, 0);
            Kernel32.WriteInt32(Handle, itemToGive+0x0C, 0);
            Kernel32.WriteInt32(Handle, itemToGive+0x10, 1);
            Kernel32.WriteInt32(Handle, itemToGive+0x14, id);
            Kernel32.WriteInt32(Handle, itemToGive+0x18, quantity);
            Kernel32.WriteInt32(Handle, itemToGive+0x1C, -1);

            byte[] bytes = BitConverter.GetBytes((ulong)MapItemManAddr.ReadIntPtr(0));
            Array.Copy(bytes, 0, asm, 0x6, 8); 
            bytes = BitConverter.GetBytes((ulong)itemToGive+0x10);
            Array.Copy(bytes, 0, asm, 0x10, 8);
            bytes = BitConverter.GetBytes((ulong)itemToGive);
            Array.Copy(bytes, 0, asm, 0x1A, 8);
            bytes = BitConverter.GetBytes((ulong)ItemGet_Call.Resolve()-0x2F);
            Array.Copy(bytes, 0, asm, 0x24, 8);

            Execute(asm);
            Free(itemToGive);
        }

        public void RemoveItem(int index)
        {
            var inventoryArraySize = PlayerDataPtr.ReadInt32((int)DS3Offsets.PlayerGameData.InventoryArraySize);
            if (index >= inventoryArraySize)
            {
                throw new IndexOutOfRangeException($"Cannot remove item {index + 1} from inventory. There are only {inventoryArraySize} items.");
            }
            var inventoryCount = PlayerDataPtr.ReadInt32((int)DS3Offsets.PlayerGameData.InventoryCount);
            PlayerDataPtr.WriteInt32((int)DS3Offsets.PlayerGameData.InventoryCount, inventoryCount - 1);
            InventoryPtr.WriteBytes(16 * index + (int)DS3Offsets.InventoryItem.Property1, new byte[] { 0x00, 0x00, 0x00, 0x00 });
            InventoryPtr.WriteBytes(16 * index + (int)DS3Offsets.InventoryItem.Id, new byte[] { 0xFF, 0xFF, 0xFF, 0xFF });
            InventoryPtr.WriteBytes(16 * index + (int)DS3Offsets.InventoryItem.Quantity, new byte[] { 0x00, 0x00, 0x00, 0x00 });
        }

        public void Warp(int bonfireId)
        {
            byte[] asm = (byte[])DS3Assembly.BonfireWarp.Clone();

            byte[] bytes = BitConverter.GetBytes((ulong)SprjLuaEventMan.Resolve());
            Array.Copy(bytes, 0, asm, 0x6, 8);
            bytes = BitConverter.GetBytes(bonfireId - 0x3E8);
            Array.Copy(bytes, 0, asm, 0x10, 4);
            bytes = BitConverter.GetBytes((ulong)GameMan.Resolve() + 0xACC);
            Array.Copy(bytes, 0, asm, 0x19, 8);
            bytes = BitConverter.GetBytes((ulong)this.Process.MainModule.BaseAddress + 0x475F00);
            Array.Copy(bytes, 0, asm, 0x29, 8);

            Execute(asm);
        }

        public bool ReadEventFlag(int flag)
        {
            var resultMemory = Allocate(8);
            byte[] asm = (byte[])DS3Assembly.ReadFlag.Clone();

            byte[] bytes = BitConverter.GetBytes((ulong)SprjEventFlagMan.Resolve());
            Array.Copy(bytes, 0, asm, 0x6, 8);
            bytes = BitConverter.GetBytes(flag);
            Array.Copy(bytes, 0, asm, 0xF, 4);
            bytes = BitConverter.GetBytes((ulong)ReadEventFlag_Call.Resolve());
            Array.Copy(bytes, 0, asm, 0x15, 8);
            bytes = BitConverter.GetBytes((ulong)resultMemory);
            Array.Copy(bytes, 0, asm, 0x21, 8);

            Execute(asm);
            var result = Kernel32.ReadInt32(Handle, resultMemory) > 0;
            Free(resultMemory);

            return result;
        }

        public void WriteEventFlag(int flag, bool active)
        {
            byte[] asm = (byte[])DS3Assembly.WriteFlag.Clone();

            byte[] bytes = BitConverter.GetBytes((ulong)SprjEventFlagMan.Resolve());
            Array.Copy(bytes, 0, asm, 0x6, 8);
            bytes = BitConverter.GetBytes(flag);
            Array.Copy(bytes, 0, asm, 0xF, 4);
            bytes = BitConverter.GetBytes(active);
            Array.Copy(bytes, 0, asm, 0x15, 1);
            bytes = BitConverter.GetBytes((ulong)WriteEventFlag_Call.Resolve());
            Array.Copy(bytes, 0, asm, 0x1B, 8);

            Execute(asm);
        }
    }
}