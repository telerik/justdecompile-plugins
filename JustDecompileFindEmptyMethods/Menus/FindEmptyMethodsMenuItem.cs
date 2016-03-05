namespace FindEmptyMethods.Menus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Input;
    using JustDecompile.API.CompositeEvents;
    using JustDecompile.API.Core;
    using Microsoft.Practices.Prism.Commands;
    using Finder = System.Func<System.Collections.Generic.IEnumerable<System.String>>;

    internal class FindEmptyMethodsMenuItem : IMenuItem
    {
        private readonly Action<string> showResults;

        private ITreeViewItem item;
        private readonly Dictionary<Type, Finder> finders = new Dictionary<Type, Finder>();

        public FindEmptyMethodsMenuItem(SelectedTreeViewItemChangedEvent @event, Action<string> showResults)
        {
            this.Command = new DelegateCommand(this.Do);

            this.finders.Add(typeof(ITypeDefinitionTreeViewItem), this.FindInType);
            this.finders.Add(typeof(IAssemblyModuleDefinitionTreeViewItem), this.FindInModule);
            this.finders.Add(typeof(IAssemblyDefinitionTreeViewItem), this.FindInAssembly);

            @event.Subscribe(item => { this.item = item; });

            this.showResults = showResults;
        }

        public ICommand Command
        {
            get; set;
        }

        public object Header { get; } = "Find visible empty methods";

        public object Icon
        {
            get; set;
        }

        public IList<IMenuItem> MenuItems { get; } = new List<IMenuItem>();

        private void Do()
        {
            if (this.item == null)
                return;

            // Namespace requires special handling, because won't work correctly until expanded.
            var namespaceItem = this.item as INamespaceTreeViewItem;
            if (namespaceItem != null)
            {
                namespaceItem.Expand(_ =>
                {
                    var res = from t in namespaceItem.Types
                              where !(t.IsEnum || t.IsFunctionPointer || t.IsGenericInstance
                               || t.IsGenericParameter || t.IsImport || t.IsInterface
                               || t.IsNestedAssembly || t.IsNestedPrivate || t.IsNotPublic)
                              from m in FindInType(t)
                              select m;
                    this.ReportFindings(res);
                });
                return;
            }

            var key = this.finders.Keys.FirstOrDefault(t => t.IsAssignableFrom(item.GetType()));

            if (key != null)
            {
                var names = this.finders[key]();
                ReportFindings(names);
            }
            else
            {
                this.showResults("Cannot search on this item kind.");
            }
        }

        private void ReportFindings(IEnumerable<string> names)
        {
            var result = String.Join(Environment.NewLine, names);
            this.showResults(string.IsNullOrWhiteSpace(result)
                ? "No empty methods found."
                : "Empty methods:" + Environment.NewLine + result);
        }

        private IEnumerable<string> FindInType()
        {
            return FindInType(((ITypeDefinitionTreeViewItem)item).TypeDefinition);
        }

        private IEnumerable<string> FindInNamespace()
        {
            var namespaceItem = (INamespaceTreeViewItem)item;
            return from t in ((INamespaceTreeViewItem)item).Types
                   where !(t.IsEnum || t.IsFunctionPointer || t.IsGenericInstance
                    || t.IsGenericParameter || t.IsImport || t.IsInterface
                    || t.IsNestedAssembly || t.IsNestedPrivate || t.IsNotPublic)
                   from m in FindInType(t)
                   select m;
        }

        private IEnumerable<string> FindInModule()
        {
            return from t in ((IAssemblyModuleDefinitionTreeViewItem)item).ModuleDefinition.Types
                   where !(t.IsEnum || t.IsFunctionPointer || t.IsGenericInstance
                    || t.IsGenericParameter || t.IsImport || t.IsInterface
                    || t.IsNestedAssembly || t.IsNestedPrivate || t.IsNotPublic)
                   from m in FindInType(t)
                   select m;
        }

        private IEnumerable<string> FindInAssembly()
        {
            return from module in ((IAssemblyDefinitionTreeViewItem)item).AssemblyDefinition.Modules
                   from t in module.Types
                   where !(t.IsEnum || t.IsFunctionPointer || t.IsGenericInstance
                    || t.IsGenericParameter || t.IsImport || t.IsInterface
                    || t.IsNestedAssembly || t.IsNestedPrivate || t.IsNotPublic)
                   from m in FindInType(t)
                   select m;
        }

        private static IEnumerable<string> FindInType(ITypeDefinition type)
        {
            foreach (var method in type.Methods)
            {
                if (method.IsAbstract || method.IsAddOn || method.IsAssembly
                    || method.IsCompilerControlled || method.IsConstructor
                    || method.IsFire || method.IsInternalCall || method.IsOther
                    || method.IsRemoveOn || method.IsRuntime || method.IsRuntimeSpecialName
                    || method.IsSpecialName || method.IsVirtual
                    || !method.HasBody)
                    continue;

                if (method.Body.CodeSize <= 1)
                    yield return method.FullName;
            }
        }
    }
}
