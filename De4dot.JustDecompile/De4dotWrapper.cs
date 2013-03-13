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
using System.Collections.Generic;
using System.Linq;
using de4dot.code;
using de4dot.code.AssemblyClient;
using de4dot.code.deobfuscators;
using System.Windows;
using dnlib.DotNet;

namespace De4dot.JustDecompile
{
	public class De4dotWrapper
	{
        private static IList<IDeobfuscatorInfo> CreateDeobfuscatorInfos()
        {
            return new List<IDeobfuscatorInfo> {
				new de4dot.code.deobfuscators.Unknown.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Agile_NET.DeobfuscatorInfo(), // aka CliSecure
				new de4dot.code.deobfuscators.Babel_NET.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.CodeFort.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.CodeVeil.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.CodeWall.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.CryptoObfuscator.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.DeepSea.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Dotfuscator.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.dotNET_Reactor.v3.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.dotNET_Reactor.v4.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Eazfuscator_NET.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Goliath_NET.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.ILProtector.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.MaxtoCode.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.MPRESS.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Rummage.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Skater_NET.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.SmartAssembly.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Spices_Net.DeobfuscatorInfo(),
				new de4dot.code.deobfuscators.Xenocode.DeobfuscatorInfo(),	
			};
        }

        public bool IsUnknownDeobfuscator(IObfuscatedFile file)
        {
            return (file == null || file.Deobfuscator == null || file.Deobfuscator.Name == "Unknown Obfuscator");
        }

		public IObfuscatedFile SearchDeobfuscator(string filename)
		{
			ModuleContext context = new ModuleContext();
			ObfuscatedFile.Options fileOptions = new ObfuscatedFile.Options { Filename = filename };
			ObfuscatedFile ofile = CreateObfuscationFile(fileOptions, context);
			return ofile;
		}
  
		public ObfuscatedFile CreateObfuscationFile(ObfuscatedFile.Options fileOptions, ModuleContext moduleContext)
		{

			ObfuscatedFile ofile = new ObfuscatedFile(fileOptions, moduleContext, new NewAppDomainAssemblyClientFactory());
			ofile.DeobfuscatorContext = new DeobfuscatorContext();

			try
			{
				ofile.load(CreateDeobfuscatorInfos().Select(di => di.createDeobfuscator()).ToList());
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
				return null;
			}
			return ofile;
		}
	}
}
