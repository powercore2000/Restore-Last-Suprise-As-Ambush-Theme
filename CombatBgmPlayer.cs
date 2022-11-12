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

//Find address for LastSUpriseMod
//Find address for takeOver mod
//Rewrite the two addresses so they are swapped.

//memory.SafeWrite(address, (short)[Wave ID from Amicitia], marshal: false )
namespace RestoreLastSupriseMod
{
    public class CombatBgmPlayer
    {
        public CombatBgmPlayer(IReloadedHooks hooks, ILogger logger, IModLoader modLoader)
        {
            
            Memory memory = Memory.Instance;
            logger.Write("Attempting to give info : Memory down");
            using Process thisProcess = Process.GetCurrentProcess();
            logger.Write("Attempting to give info : Process running");
            long baseAddress = thisProcess.MainModule!.BaseAddress.ToInt64();
            logger.Write("Attempting to give info : Base 64 convert");
            modLoader.GetController<IStartupScanner>().TryGetTarget(out var startupScanner);

            logger.Write("Attempting to give info : Gotten startupScanner");

            if (startupScanner != null)
            {
                logger.TextColor = logger.ColorGreen;
                logger.Write("Mod Initalized! Replacing themes...");
                SetAmbushTheme(memory, logger, baseAddress,  startupScanner);
                SetBattleTheme(memory, logger, baseAddress, startupScanner);
            }
            else {

                logger.TextColor = logger.ColorRed;
                logger.Write("Set up scanner came back null!");
            }

            logger.Write("Finish Test");
        }

        private static void SetAmbushTheme(Memory memory, ILogger logger, long baseAddress, IStartupScanner startupScanner)
        {
            logger.Write("Setting ambush");
            startupScanner.AddMainModuleScan("BA 8B 03 00 00 83 F8 01", delegate (PatternScanResult result)
            {
                long num = result.Offset + baseAddress;
                logger.Write($"long num is {num} and the result was {result.Found}");
                if (result.Found)
                {  
                    memory.SafeWrite(num + 1, (short)300, marshal: false);
                    logger.TextColor = logger.ColorGreen;
                    logger.Write($"Mod Initalized! Replacing ambush themes with {300}...");
                }
                else
                {
                    logger.TextColor = logger.ColorRed;
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
                    logger.TextColor = logger.ColorGreen;
                    logger.Write($"Mod Initalized! Replacing battle themes with {907}...");
                }
                else
                {
                    logger.TextColor = logger.ColorRed;
                    logger.Write($"Bad address recivied! Cant replace battle themes with {907} !");
                }
            });
        }
    }
    //B9 85 03 00 00 66 89 56 ??
}

