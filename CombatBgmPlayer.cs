using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Reloaded.Hooks.Definitions;
using Reloaded.Memory.Sigscan.Definitions.Structs;
using Reloaded.Memory.SigScan.ReloadedII.Interfaces;
using Reloaded.Memory.Sources;
using Reloaded.Mod.Interfaces;
using Reloaded.Mod.Interfaces.Internal;

namespace RestoreLastSupriseMod
{
    public class CombatBgmPlayer
    {
        public CombatBgmPlayer(IReloadedHooks hooks, ILogger logger, IModLoader modLoader)
        {
            
            Memory memory = Memory.Instance;
            using Process thisProcess = Process.GetCurrentProcess();
            long baseAddress = thisProcess.MainModule!.BaseAddress.ToInt64();
            modLoader.GetController<IStartupScanner>().TryGetTarget(out var startupScanner);

            if (startupScanner != null)
            {
                SetAmbushTheme(memory, logger, baseAddress,  startupScanner);
                SetBattleTheme(memory, logger, baseAddress, startupScanner);
            }
            else {

                logger.TextColor = logger.ColorRed;
                logger.Write("Set up scanner came back null!");
            }

        }

        private static void SetAmbushTheme(Memory memory, ILogger logger, long baseAddress, IStartupScanner startupScanner)
        {

            startupScanner.AddMainModuleScan("BA 8B 03 00 00 83 F8 01", delegate (PatternScanResult result)
            {
                long num = result.Offset + baseAddress;
                logger.Write($"long num is {num} and the result was {result.Found}");
                if (result.Found)
                {  
                    memory.SafeWrite(num + 1, (short)300, marshal: false);
                }
                else
                {
                    logger.Write($"Bad address recivied! Cant replace ambush themes with {300} !");
                }
            });
        }

        private static void SetBattleTheme(Memory memory, ILogger logger, long baseAddress, IStartupScanner startupScanner)
        {
            startupScanner.AddMainModuleScan("BA 2C 01 00 00 49 8B CF", delegate (PatternScanResult result)
            {
                long num = result.Offset + baseAddress;
                if (result.Found)
                {
                    memory.SafeWrite(num + 1, (short)907, marshal: false);
                }
                else
                {
                    logger.Write($"Bad address recivied! Cant replace battle themes with {907} !");
                }
            });
        }
    }
}

