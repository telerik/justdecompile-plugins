using System;
using System.Linq;
using Reflexil.Plugins.CecilStudio;
using Reflexil.Utils;
using Mono.Cecil;
using JustDecompile.Core;

namespace Reflexil.JustDecompile
{
    internal class JustDecompileCecilStudioPackage : CecilStudioPackage
    {
        public JustDecompileCecilStudioPackage() { }

        public void Rename()
        {
            this.RenameItem(this, EventArgs.Empty);
        }

        public void Delete()
        {
            this.DeleteMember(this, EventArgs.Empty);
        }

        public ITreeViewItem SelectedTreeViewItem { get; set; }

        public override object ActiveItem
        {
            get
            {
                return this.SelectedTreeViewItem;
            }
        }

        protected override void ActiveItemChanged(object sender, EventArgs e) { }

        internal void ReloadAssembly()
        {
            this.ReloadAssembly(this, EventArgs.Empty);
        }

        internal AssemblyDefinition GetCurrentAssemblyDefinition()
        {
            return base.GetCurrentAssemblyDefinition();
        }

        internal void Inject(EInjectType injectType)
        {
            base.Inject(injectType);
        }

        internal string GetProductVersion()
        {
            return this.REFLEXIL_BUTTON_TEXT;
        }

        internal string GetProductTitle()
        {
            return REFLEXIL_WINDOW_TEXT;
        }
    }
}
