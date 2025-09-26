using PropertyHook;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSHook : PHook
    {
        private readonly PHPointer GameManagerImp;
        private readonly PHPointer PlayerDataPtr;
        private readonly PHPointer ItemBag;
        private readonly PHPointer InventoryList;
        private readonly PHPointer ItemGiveFunc;
        private readonly PHPointer ItemRemoveFunc;
        private readonly PHPointer SetWarpTargetFunc;
        private readonly PHPointer WarpFunc;
        private readonly PHPointer EventManager;
        private readonly PHPointer WarpManager;

        public DS2SotFSHook(int refreshInterval, int minLifetime) :
            base(refreshInterval, minLifetime, p => p.MainWindowTitle == "DARK SOULS II")
        {
            GameManagerImp = RegisterRelativeAOB(DS2SotFSOffsets.GameManagerImpAOB, 3, 7);
            PlayerDataPtr = CreateChildPointer(GameManagerImp, DS2SotFSOffsets.GameManagerImpOffset1, (int)DS2SotFSOffsets.GameDataMan.PlayerName);
            ItemBag = CreateChildPointer(PlayerDataPtr, 0x10, 0x10);
            InventoryList = CreateChildPointer(PlayerDataPtr, 0x10, 0xD0);
            ItemGiveFunc = RegisterAbsoluteAOB(DS2SotFSOffsets.ItemGiveAOB);
            ItemRemoveFunc = RegisterAbsoluteAOB(DS2SotFSOffsets.ItemRemoveAOB);
            SetWarpTargetFunc = RegisterAbsoluteAOB(DS2SotFSOffsets.SetWarpTargetFuncAOB);
            WarpFunc = RegisterAbsoluteAOB(DS2SotFSOffsets.WarpFuncAOB);
            EventManager = CreateChildPointer(GameManagerImp, (int)DS2SotFSOffsets.EventManagerOffset);
            WarpManager = CreateChildPointer(EventManager, (int)DS2SotFSOffsets.WarpManagerOffset);
        }

        public List<DS2SotFSInventoryItem> GetCurrentInventory()
        {
            var currentInventory = new List<DS2SotFSInventoryItem>();

            for (int i = 1; i < 0xEFF; i++)
            {
                if (InventoryList.ReadInt32(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.Id) == 0x00)
                {
                    continue;
                }
                currentInventory.Add(
                    new DS2SotFSInventoryItem(
                        InventoryList.ReadIntPtr(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.PreviousItemPointer),
                        InventoryList.ReadIntPtr(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.NextItemPointer),
                        InventoryList.ReadInt32(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.Id),
                        InventoryList.ReadInt32(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.ListIndexCagtegoryAndLocation),
                        InventoryList.ReadInt32(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.QuantityOrDurability),
                        InventoryList.ReadInt32(i * 0x28 + (int)DS2SotFSOffsets.InventoryItem.UpgradeLevelAndIdk),
                        i - 1
                    )
                );
            }
            return currentInventory;
        }

        public void GiveItem(int item, short amount, byte upgrade, byte infusion)
        {
            var itemStruct = Allocate(0x8A);
            Kernel32.WriteBytes(Handle, itemStruct + 0x4, BitConverter.GetBytes(item));
            Kernel32.WriteBytes(Handle, itemStruct + 0x8, BitConverter.GetBytes(float.MaxValue));
            Kernel32.WriteBytes(Handle, itemStruct + 0xC, BitConverter.GetBytes(amount));
            Kernel32.WriteByte(Handle, itemStruct + 0xE, upgrade);
            Kernel32.WriteByte(Handle, itemStruct + 0xF, infusion);

            var asm = (byte[])DS2SotFSAssembly.GetItem.Clone();

            var bytes = BitConverter.GetBytes(0x1);
            Array.Copy(bytes, 0, asm, 0x6, 4);
            bytes = BitConverter.GetBytes(itemStruct.ToInt64());
            Array.Copy(bytes, 0, asm, 0xC, 8);
            bytes = BitConverter.GetBytes(ItemBag.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x19, 8);
            bytes = BitConverter.GetBytes(ItemGiveFunc.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x26, 8);

            Execute(asm);
            Free(itemStruct);
        }

        public void RemoveItem(int index)
        {
            var asm = (byte[])DS2SotFSAssembly.RemoveItem.Clone();

            var bytes = BitConverter.GetBytes(InventoryList.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x6, 8);
            bytes = BitConverter.GetBytes(InventoryList.ReadInt32((index+1) * 0x28 + 0x1C));
            Array.Copy(bytes, 0, asm, 0xF, 4);
            bytes = BitConverter.GetBytes(ItemRemoveFunc.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x1B, 8);

            Execute(asm);
        }

        public void Warp(ushort id)
        {
            var value = Allocate(sizeof(short));
            Kernel32.WriteBytes(Handle, value, BitConverter.GetBytes(id));

            var asm = (byte[])DS2SotFSAssembly.BonfireWarp.Clone();
            var bytes = BitConverter.GetBytes(value.ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x9, bytes.Length);
            bytes = BitConverter.GetBytes(SetWarpTargetFunc.Resolve().ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x21, bytes.Length);
            bytes = BitConverter.GetBytes(WarpManager.Resolve().ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x2E, bytes.Length);
            bytes = BitConverter.GetBytes(WarpFunc.Resolve().ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x3B, bytes.Length);

            Execute(asm);
            Free(value);
        }
    }
}