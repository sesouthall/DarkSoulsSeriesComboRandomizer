using PropertyHook;

namespace DarkSoulsSeriesComboRandomizer.DarkSouls2SotFS
{
    public class DS2SotFSHook : PHook
    {
        private readonly PHPointer GameManagerImp;
        private readonly PHPointer GameDataManager;
        private readonly PHPointer PlayerData;
        private readonly PHPointer ItemBag;
        private readonly PHPointer InventoryList;
        private readonly PHPointer ItemGive_Call;
        private readonly PHPointer ItemRemove_Call;
        private readonly PHPointer SetWarpTarget_Call;
        private readonly PHPointer Warp_Call;
        private readonly PHPointer EventManager;
        private readonly PHPointer WarpManager;
        private readonly PHPointer EventFlagManager;

        public bool CharacterLoaded { get; private set; } = false;

        public DS2SotFSHook(int refreshInterval, int minLifetime) :
            base(refreshInterval, minLifetime, p => p.Id != Environment.ProcessId && p.MainWindowTitle == "DARK SOULS II")
        {
            GameManagerImp = RegisterRelativeAOB(DS2SotFSOffsets.GameManagerImpAOB, 3, 7, DS2SotFSOffsets.GameManagerImpOffset1);
            GameDataManager = CreateChildPointer(GameManagerImp, (int)DS2SotFSOffsets.GameManagerImp.GameDataManager);
            PlayerData = CreateChildPointer(GameDataManager, (int)DS2SotFSOffsets.GameDataMan.PlayerData);
            ItemBag = CreateChildPointer(GameDataManager, 0x10, 0x10);
            InventoryList = CreateChildPointer(GameDataManager, 0x10, 0xD0);
            ItemGive_Call = RegisterAbsoluteAOB(DS2SotFSOffsets.ItemGiveAOB);
            ItemRemove_Call = RegisterAbsoluteAOB(DS2SotFSOffsets.ItemRemoveAOB);
            SetWarpTarget_Call = RegisterAbsoluteAOB(DS2SotFSOffsets.SetWarpTargetFuncAOB);
            Warp_Call = RegisterAbsoluteAOB(DS2SotFSOffsets.WarpFuncAOB);
            EventManager = CreateChildPointer(GameManagerImp, (int)DS2SotFSOffsets.GameManagerImp.EventManager);
            WarpManager = CreateChildPointer(EventManager, (int)DS2SotFSOffsets.EventManager.WarpManager);
            EventFlagManager = CreateChildPointer(EventManager, (int)DS2SotFSOffsets.EventManager.EventFlagManager);
            base.OnHooked += FinishSetup;
        }

        public void FinishSetup(object? sender, PHEventArgs pHEventArgs)
        {
            while (PlayerData.ReadString((int)DS2SotFSOffsets.PlayerData.NameString1, System.Text.Encoding.UTF8, 0x10, trim: true) == "")
            {
                Thread.Sleep(1000);
            }
            CharacterLoaded = true;
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
            bytes = BitConverter.GetBytes(ItemGive_Call.Resolve().ToInt64());
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
            bytes = BitConverter.GetBytes(ItemRemove_Call.Resolve().ToInt64());
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
            bytes = BitConverter.GetBytes(SetWarpTarget_Call.Resolve().ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x21, bytes.Length);
            bytes = BitConverter.GetBytes(WarpManager.Resolve().ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x2E, bytes.Length);
            bytes = BitConverter.GetBytes(Warp_Call.Resolve().ToInt64());
            Array.Copy(bytes, 0x0, asm, 0x3B, bytes.Length);

            Execute(asm);
            Free(value);
        }

        public bool ReadEventFlag(int flag)
        {
            var resultMemory = Allocate(sizeof(long));
            var asm = (byte[])DS2SotFSAssembly.ReadEventFlag.Clone();

            var bytes = BitConverter.GetBytes(EventFlagManager.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x6, 8);
            bytes = BitConverter.GetBytes(flag);
            Array.Copy(bytes, 0, asm, 0xF, 4);
            bytes = BitConverter.GetBytes((ulong)this.Process.MainModule.BaseAddress + DS2SotFSOffsets.ReadEventFlagMethodOffset);
            Array.Copy(bytes, 0, asm, 0x15, 8);
            bytes = BitConverter.GetBytes((ulong)resultMemory);
            Array.Copy(bytes, 0, asm, 0x21, 8);

            Execute(asm);
            var result = Kernel32.ReadInt64(Handle, resultMemory) > 0;
            Free(resultMemory);

            return result;
        }

        public void WriteEventFlag(int flag, bool active)
        {
            var asm = (byte[])DS2SotFSAssembly.WriteEventFlag.Clone();

            var bytes = BitConverter.GetBytes(EventFlagManager.Resolve().ToInt64());
            Array.Copy(bytes, 0, asm, 0x6, 8);
            bytes = BitConverter.GetBytes(flag);
            Array.Copy(bytes, 0, asm, 0xF, 4);
            bytes = BitConverter.GetBytes(active);
            Array.Copy(bytes, 0, asm, 0x15, 1);
            bytes = BitConverter.GetBytes((ulong)this.Process.MainModule.BaseAddress + DS2SotFSOffsets.WriteEventFlagMethodOffset);
            Array.Copy(bytes, 0, asm, 0x1B, 8);

            Execute(asm);
        }
    }
}