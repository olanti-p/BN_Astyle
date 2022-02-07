using System.ComponentModel;

namespace BN_Astyle
{
    internal partial class OptionsProvider
    {
        public class GeneralOptions : BaseOptionPage<General> { }
    }

    public class General : BaseOptionModel<General>
    {
        [Category("C++")]
        [DisplayName("Formatting options")]
        [Description("Formatting options for C++")]
        [DefaultValue("")]
        public string CppOptions { get; set; } = "";

        [Category("C++")]
        [DisplayName("Format on save")]
        [Description("Whether to format C++ code on save")]
        [DefaultValue(false)]
        public bool CppFormatOnSave { get; set; } = false;
    }
}
