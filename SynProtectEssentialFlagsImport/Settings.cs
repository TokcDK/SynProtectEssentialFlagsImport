using Mutagen.Bethesda.Synthesis.Settings;

namespace SynProtectEssentialFlagsImport
{
    public class Settings
    {
        [SynthesisSettingName("Check Essential Flag")]
        [SynthesisTooltip("Enable if need to check Essential flag for npc edits in all mods except excluded")]
        [SynthesisDescription("When enabled Essential flag will be checked for all edits of npc in enabled mods and imported if any found")]
        public bool CheckEssentialFlag { get; set; } = true;
        [SynthesisSettingName("Check Protected Flag")]
        [SynthesisTooltip("Enable if need to check Protected flag for npc edits in all mods except excluded")]
        [SynthesisDescription("When enabled Protected flag will be checked for all edits of npc in enabled mods and imported if any found")]
        public bool CheckProtectedFlag { get; set; } = true;
        [SynthesisSettingName("Excluded mods list")]
        [SynthesisTooltip("Add mods which you want to be excluded from checking")]
        [SynthesisDescription("Any mod names here will be excluded form flag checking")]
        public List<string> ExcludedModsList { get; set; } = new List<string>()
        {
            // default excluded vanila masters
            "Skyrim.esm",
            "Update.esm",
            "Dawnguard.esm",
            "HearthFires.esm",
            "Dragonborn.esm",
        };
    }
}