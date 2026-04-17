using SoulsFormats;

namespace DarkSoulsItemMigrator
{
    public static class ESDCommandUtil
    {
        public static bool IsAddWarpToMenuCommand(ESD.CommandCall command)
        {
            return command.CommandID == 19 &&
                (command.CommandBank == 1 || command.CommandBank == 5) &&
                (command.Arguments[1].ArrayEquals<byte>([130, 86, 226, 228, 0, 161]) || command.Arguments[2].ArrayEquals<byte>([130, 86, 226, 228, 0, 161]));
        }

        public static bool IsCrossGameWarpCommand(ESD.CommandCall command)
        {
            return (command.CommandID == 128 || command.CommandID == 129 || command.CommandID == 130) &&
                (command.CommandBank == 1 || command.CommandBank == 5) &&
                command.Arguments.Count == 0;
        }

        public static bool IsAddTalkListDataCommand(ESD.CommandCall command)
        {
            return command.CommandID == 19 &&
                (command.CommandBank == 1 || command.CommandBank == 5) &&
                (command.Arguments.Count == 3 || command.Arguments.Count == 4);
        }

        public static int GetMenuIndexIgnoreLeave(ESD.CommandCall command)
        {
            if (!IsAddTalkListDataCommand(command)) throw new ArgumentException($"Command {command.CommandID} is not adding an option to a menu, there is no index to extract");

            var result = 0;
            if (command.CommandBank == 5)
            {
                result = command.Arguments[0].Length == 6 ? command.Arguments[0][1] : command.Arguments[1][1]; // AddTalkListData has the index as argument 0, AddTalkListDataIf hsa it as argument 1
                result = result > 99 ? 0 : result;
            }
            else
            {
                result = command.Arguments[0].Length == 2 ? command.Arguments[0][0] : command.Arguments[1][0];
                result = result > 99 + 64 ? 64 : result;
            }
            return result;
        }
    }
}
