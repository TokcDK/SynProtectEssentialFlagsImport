using Mutagen.Bethesda;
using Mutagen.Bethesda.Synthesis;
using Mutagen.Bethesda.Skyrim;

namespace SynProtectEssentialFlagsImport
{
    public class Program
    {
        public static Lazy<Settings> PatchSettings = null!;
        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<ISkyrimMod, ISkyrimModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out PatchSettings)
                .SetTypicalOpen(GameRelease.SkyrimSE, "SynProtectEssentialFlagsImport.esp")
                .Run(args);
        }

        static readonly NpcConfiguration.Flag _flagEssential = NpcConfiguration.Flag.Essential;
        static readonly NpcConfiguration.Flag _flagProtected = NpcConfiguration.Flag.Protected;
        static readonly NpcConfiguration.Flag[] _protectedEssentialFlags = new NpcConfiguration.Flag[2] { _flagEssential, _flagProtected };

        public static void RunPatch(IPatcherState<ISkyrimMod, ISkyrimModGetter> state)
        {
            var excludedForFlagCheck = PatchSettings.Value.ExcludedModsList;
            bool haveExcludedForFlagCheck = excludedForFlagCheck.Count > 0;

            bool checkEssentialFlag = PatchSettings.Value.CheckEssentialFlag;
            bool checkProtectedFlag = PatchSettings.Value.CheckProtectedFlag;
            foreach (var npcGetter in state.LoadOrder.PriorityOrder.Npc().WinningOverrides())
            {
                var npcGetterConfigurationFlags = npcGetter.Configuration.Flags;
                if (npcGetterConfigurationFlags.HasFlag(_flagEssential)) continue; // skip if last edit have essential flag
                if (!checkEssentialFlag && npcGetterConfigurationFlags.HasFlag(_flagProtected)) continue; // skip the record if not need to check essential and protected flag already added

                // check both flags
                foreach (var flag in _protectedEssentialFlags)
                {
                    // skip if unchecked in Settings
                    if (!checkEssentialFlag && flag == _flagEssential) continue;
                    if (!checkProtectedFlag && flag == _flagProtected) continue;

                    // skip if last edit of the npc contains the flag
                    if (npcGetter.Configuration.Flags.HasFlag(flag)) continue; 

                    // check all npc edits for the flag contain
                    foreach (var modContext in state.LinkCache.ResolveAllContexts<INpc, INpcGetter>(npcGetter.FormKey)) // check all edits of the npc
                    {
                        if (haveExcludedForFlagCheck && excludedForFlagCheck.Contains(modContext.ModKey.FileName)) continue;
                        if (!modContext.Record.Configuration.Flags.HasFlag(flag)) continue; // skip if npc record in the mod have no this flag

                        Console.WriteLine($"Found '{flag}' flag for npc '{npcGetter.FormKey.ID}|{npcGetter.EditorID}' from mod '{modContext.ModKey.FileName}'");
                        state.PatchMod.Npcs.GetOrAddAsOverride(npcGetter).Configuration.Flags |= flag; // add the flag

                        break; // import 1st Essential flag will skip check of 2nd Protected flag
                    }
                }
            }
        }
    }
}
