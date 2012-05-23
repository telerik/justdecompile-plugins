// Copyright 2012 Telerik AD
// 
// This program is free software: you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// 
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JustDecompile.Core;
using Mono.Cecil;
using Reflexil.Plugins;
using Reflexil.Plugins.CecilStudio;

namespace Reflexil.JustDecompile
{
	public class JustDecompileCecilPlugin : CecilStudioPlugin
	{
		public JustDecompileCecilPlugin(IPackage package) : base(package)
		{
		}

		#region AssemblyDefinition
		public override bool IsAssemblyDefinitionHandled(object item)
		{
			CheckAssemblyNode(item);

			return (item as ITreeViewItem).TreeNodeType == TreeNodeType.AssemblyDefinition;
		}

		public override AssemblyDefinition GetAssemblyDefinition(object item)
		{
			CheckAssemblyNode(item);

			var assemblyDefinitionTreeViewItem = item as IAssemblyDefinitionTreeViewItem;

			return LoadAssembly(assemblyDefinitionTreeViewItem.AssemblyDefinition.MainModule.FilePath, false);
		}

		#endregion

		#region ModuleDefinition
		public override bool IsModuleDefinitionHandled(object item)
		{
			this.CheckAssemblyNode(item);

			return ((ITreeViewItem)item).TreeNodeType == TreeNodeType.AssemblyModuleDefinition;
		}

		public override string GetModuleLocation(object item)
		{
			CheckAssemblyNode(item);

			return ((IAssemblyModuleDefinitionTreeViewItem)item).ModuleDefinition.FilePath;
		}

		#endregion

		#region TypeDefinition
		public override bool IsTypeDefinitionHandled(object item)
		{
			CheckAssemblyNode(item);

			return ((ITreeViewItem)item).TreeNodeType == TreeNodeType.AssemblyTypeDefinition;
		}

		public override TypeDefinition GetTypeDefinition(object item)
		{
			var treeViewItem = item as ITypeDefinitionTreeViewItem;

			return GetTypeDefinition(treeViewItem.TypeDefinition);
		}

		private TypeDefinition GetTypeDefinition(ITypeDefinition item)
		{
			var typeDef = item as ITypeDefinition;

			TypeDefinition typeDefinition = null;

			if (typeDef.IsNested)
			{
				var parents = new List<ITypeDefinition> { };

				while (typeDef.IsNested)
				{
					parents.Add(typeDef);

					typeDef = typeDef.DeclaringType;
				}
				typeDefinition = this.FindTypeDefinition(typeDef.Module.FilePath, typeDef.FullName);

				parents.Reverse();

				while (parents.Count > 0)
				{
					for (int i = 0; i < typeDefinition.NestedTypes.Count; i++)
					{
						TypeDefinition childTypeDef = typeDefinition.NestedTypes[i];

						if (childTypeDef.FullName == parents[0].FullName)
						{
							typeDefinition = childTypeDef;

							parents.RemoveAt(0);

							break;
						}
					}
				}
			}
			else
			{
				typeDefinition = this.FindTypeDefinition(typeDef.Module.FilePath, typeDef.FullName);
			}
			return typeDefinition;
		}

		#endregion

		#region MethodDefinition
		public override bool IsMethodDefinitionHandled(object item)
		{
			CheckAssemblyNode(item);

			return this.GetAssemblyNode<ITreeViewItem>(item).TreeNodeType == TreeNodeType.AssemblyMethodDefinition;
		}

		public override MethodDefinition GetMethodDefinition(object item)
		{
			var treeViewItem = item as IMethodDefinitionTreeViewItem;

			IMethodDefinition method = treeViewItem.MethodDefinition;

			TypeDefinition typeDef = GetTypeDefinition(method.DeclaringType);

			MethodDefinition methodDefinition = this.FindMethodDefinition(method.FullName, typeDef);

			return methodDefinition;
		}

		#endregion

		#region PropertyDefinition
		public override bool IsPropertyDefinitionHandled(object item)
		{
			CheckAssemblyNode(item);

			return this.GetAssemblyNode<ITreeViewItem>(item).TreeNodeType == TreeNodeType.AssemblyPropertyDefinition;
		}

		public override PropertyDefinition GetPropertyDefinition(object item)
		{
			var treeViewItem = item as IPropertyDefinitionTreeViewItem;

			var property = treeViewItem.PropertyDefinition;

			TypeDefinition typeDef = GetTypeDefinition(property.DeclaringType);

			PropertyDefinition propertyDefinition = this.FindPropertyDefinition(property.FullName, typeDef);

			return propertyDefinition;
		}

		#endregion

		#region FieldDefinition
		public override bool IsFieldDefinitionHandled(object item)
		{
			CheckAssemblyNode(item);

			return this.GetAssemblyNode<ITreeViewItem>(item).TreeNodeType == TreeNodeType.AssemblyFieldDefinition;
		}

		public override FieldDefinition GetFieldDefinition(object item)
		{
			var treeViewItem = item as IFieldDefinitionTreeViewItem;

			var field = treeViewItem.FieldDefinition;

			TypeDefinition typeDef = GetTypeDefinition(field.DeclaringType);

			FieldDefinition fieldDefinition = this.FindFieldDefinition(field.FullName, typeDef);

			return fieldDefinition;
		}

		#endregion

		#region EventDefinition
		public override bool IsEventDefinitionHandled(object item)
		{
			CheckAssemblyNode(item);

			return this.GetAssemblyNode<ITreeViewItem>(item).TreeNodeType == TreeNodeType.AssemblyEventDefinition;
		}

		public override EventDefinition GetEventDefinition(object item)
		{
			var treeViewItem = item as IEventDefinitionTreeViewItem;

			var eventDef = treeViewItem.EventDefinition;

			TypeDefinition typeDef = GetTypeDefinition(eventDef.DeclaringType);

			EventDefinition propertyDefinition = this.FindEventDefinition(eventDef.FullName, typeDef);

			return propertyDefinition;
		}

		#endregion

		#region AssemblyNameReference
		public override bool IsAssemblyNameReferenceHandled(object item)
		{
			CheckAssemblyNode(item);

			return this.GetAssemblyNode<ITreeViewItem>(item).TreeNodeType == TreeNodeType.AssemblyReference;
		}

		public override AssemblyNameReference GetAssemblyNameReference(object item)
		{
			var assemblyReference = (item as IAssemblyReferenceTreeViewItem).AssemblyNameReference;

			var assemblyNameRef = new AssemblyNameReference(assemblyReference.Name, assemblyReference.Version);
			assemblyNameRef.Culture = assemblyReference.Culture;
			assemblyNameRef.Hash = assemblyReference.Hash;
			assemblyNameRef.HashAlgorithm = (Mono.Cecil.AssemblyHashAlgorithm)assemblyReference.HashAlgorithm;
			assemblyNameRef.HasPublicKey = assemblyReference.HasPublicKey;
			assemblyNameRef.IsRetargetable = assemblyReference.IsRetargetable;
			assemblyNameRef.IsSideBySideCompatible = assemblyReference.IsSideBySideCompatible;
			assemblyNameRef.PublicKey = assemblyReference.PublicKey;
			assemblyNameRef.PublicKeyToken = assemblyReference.PublicKeyToken;
			assemblyNameRef.Attributes = (Mono.Cecil.AssemblyAttributes)assemblyReference.Attributes;

			return assemblyNameRef;
			////var assemblyReference = item as IAssemblyReferenceTreeViewItem;
			////AssemblyNameReference assemblyNameRef = LoadAssembly(assemblyReference.AssemblyPath, false)
			////                                                .MainModule
			////                                                .AssemblyReferences
			////                                                .FirstOrDefault(i => i.FullName == ((IAssemblyNameReference)assemblyReference).FullName);
			////return assemblyNameRef;
		}

		#endregion

		#region EmbeddedResource
		public override bool IsEmbeddedResourceHandled(object item)
		{
			CheckAssemblyNode(item);

			return this.GetAssemblyNode<ITreeViewItem>(item).TreeNodeType == TreeNodeType.AssemblyResource;
		}

		public override EmbeddedResource GetEmbeddedResource(object item)
		{
			var resourceTreeViewItem = item as IResourceTreeViewItem;

			EmbeddedResource resource = FindEmbeddedResource(resourceTreeViewItem.Resource, resourceTreeViewItem.AssemblyFile);

			return resource;
		}

		#endregion

		public override ICollection GetAssemblies(bool wrap)
		{
			if (wrap)
			{
				var result = new Collection<CecilStudioAssemblyWrapper>();
				foreach (AssemblyDefinition adef in m_assemblies)
				{
					result.Add(new CecilStudioAssemblyWrapper(adef));
				}
				return result;
			}
			else
			{
				return m_assemblies;
			}
		}

		public override AssemblyDefinition LoadAssembly(string location, bool loadsymbols)
		{
			AssemblyDefinition assemblyDefinition = base.LoadAssembly(location, loadsymbols);
			try
			{
				if (assemblyDefinition == null)
				{
					assemblyDefinition = AssemblyDefinition.ReadAssembly(location);

					var newCollection = m_assemblies.OfType<AssemblyDefinition>().Union(new AssemblyDefinition[] { assemblyDefinition });

					ReloadAssemblies(new List<AssemblyDefinition>(newCollection));
				}
				return assemblyDefinition;
			}
			catch
			{
				return null;
			}
		}

		internal void LoadAssemblies(IEnumerable<IAssemblyDefinition> assemblies)
		{
			ReloadAssemblies(assemblies.Select(i => LoadAssembly(i.MainModule.FilePath, false))
									   .ToList());
		}

		private void CheckAssemblyNode(object item)
		{
			if (item is ITreeViewItem == false)
			{
				throw new ArgumentNullException("item must be ITreeViewItem");
			}
		}

		private T GetAssemblyNode<T>(object item) where T : ITreeViewItem
		{
			return (T)item;
		}

		private TypeDefinition FindTypeDefinition(string filePath, string fullName)
		{
			IAssemblyContext assemblyContext = this.GetAssemblyContext(filePath);

			TypeDefinition typeDef = assemblyContext.AssemblyDefinition
													.MainModule
													.Types
													.FirstOrDefault(t => t.FullName == fullName);

			return typeDef;
		}

		protected MethodDefinition FindMethodDefinition(string fullName, TypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
			{
				throw new ArgumentNullException("typeDefinition is null");
			}
			return typeDefinition.Methods.FirstOrDefault(m => m.FullName == fullName);
		}

		private FieldDefinition FindFieldDefinition(string fullName, TypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
			{
				throw new ArgumentNullException("typeDefinition is null");
			}
			return typeDefinition.Fields.FirstOrDefault(m => m.FullName == fullName);
		}

		private PropertyDefinition FindPropertyDefinition(string fullName, TypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
			{
				throw new ArgumentNullException("typeDefinition is null");
			}
			return typeDefinition.Properties.FirstOrDefault(m => m.FullName == fullName);
		}

		private EventDefinition FindEventDefinition(string fullName, TypeDefinition typeDefinition)
		{
			if (typeDefinition == null)
			{
				throw new ArgumentNullException("typeDefinition is null");
			}
			return typeDefinition.Events.FirstOrDefault(m => m.FullName == fullName);
		}

		private EmbeddedResource FindEmbeddedResource(IResource value, string filePath)
		{
			IAssemblyContext assemblyContext = this.GetAssemblyContext(filePath);

			return assemblyContext.AssemblyDefinition
								  .MainModule
								  .Resources
								  .Where(r => r.ResourceType == Mono.Cecil.ResourceType.Embedded)
								  .Select(r => HandleEmbeddeResource(r as EmbeddedResource, value))
								  .FirstOrDefault(r => r.Name == value.Name);
		}

		private EmbeddedResource HandleEmbeddeResource(EmbeddedResource r, IResource value)
		{
			if (!value.Name.EndsWith(".g.resources", StringComparison.InvariantCultureIgnoreCase) &&
				r.Name.EndsWith(".g.resources", StringComparison.InvariantCultureIgnoreCase))
			{
				using (var resourceReader = new System.Resources.ResourceReader(r.GetResourceStream()))
				{
					DictionaryEntry de = resourceReader.OfType<DictionaryEntry>().FirstOrDefault(d => d.Key.ToString() == value.Name);

					if (de.Value is System.IO.Stream == false)
					{
						return r;
					}
					var stream = de.Value as System.IO.Stream;
					return new EmbeddedResource(value.Name, Mono.Cecil.ManifestResourceAttributes.Public, new System.IO.BinaryReader(stream).ReadBytes((int)stream.Length));
				}
			}
			return r;
		}
	}
}