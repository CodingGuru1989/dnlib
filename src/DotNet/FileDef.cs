// dnlib: See LICENSE.txt for more info

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using dnlib.DotNet.MD;
using dnlib.DotNet.Pdb;

namespace dnlib.DotNet {
	/// <summary>
	/// A high-level representation of a row in the File table
	/// </summary>
	public abstract class FileDef : IHasCustomAttribute, IImplementation, IHasCustomDebugInformation, IManagedEntryPoint {
		/// <summary>
		/// The row id in its table
		/// </summary>
		private uint rid;

		/// <inheritdoc/>
		public MDToken MDToken => new MDToken(Table.File, rid);

		/// <inheritdoc/>
		public uint Rid {
			get => rid;
			set => rid = value;
		}

		/// <inheritdoc/>
		public int HasCustomAttributeTag => 16;

		/// <inheritdoc/>
		public int ImplementationTag => 0;

		/// <summary>
		/// From column File.Flags
		/// </summary>
		public FileAttributes Attributes {
			get => (FileAttributes)attributes;
			set => attributes = (int)value;
		}
		/// <summary>Attributes</summary>
		private int attributes;

		/// <summary>
		/// From column File.Name
		/// </summary>
		public UTF8String Name {
			get => name;
			set => name = value;
		}
		/// <summary>Name</summary>
		private UTF8String name;

		/// <summary>
		/// From column File.HashValue
		/// </summary>
		public byte[] HashValue {
			get => hashValue;
			set => hashValue = value;
		}
		/// <summary/>
		private byte[] hashValue;

		/// <summary>
		/// Gets all custom attributes
		/// </summary>
		public CustomAttributeCollection CustomAttributes {
			get {
				if (customAttributes == null)
					InitializeCustomAttributes();
				return customAttributes;
			}
		}
		/// <summary/>
		internal CustomAttributeCollection customAttributes;
		/// <summary>Initializes <see cref="customAttributes"/></summary>
		protected virtual void InitializeCustomAttributes() =>
			Interlocked.CompareExchange(ref customAttributes, new CustomAttributeCollection(), null);

		/// <inheritdoc/>
		public bool HasCustomAttributes => CustomAttributes.Count > 0;

		/// <inheritdoc/>
		public int HasCustomDebugInformationTag => 16;

		/// <inheritdoc/>
		public bool HasCustomDebugInfos => CustomDebugInfos.Count > 0;

		/// <summary>
		/// Gets all custom debug infos
		/// </summary>
		public IList<PdbCustomDebugInfo> CustomDebugInfos {
			get {
				if (customDebugInfos == null)
					InitializeCustomDebugInfos();
				return customDebugInfos;
			}
		}
		/// <summary/>
		internal IList<PdbCustomDebugInfo> customDebugInfos;
		/// <summary>Initializes <see cref="customDebugInfos"/></summary>
		protected virtual void InitializeCustomDebugInfos() =>
			Interlocked.CompareExchange(ref customDebugInfos, new List<PdbCustomDebugInfo>(), null);

		/// <summary>
		/// Set or clear flags in <see cref="attributes"/>
		/// </summary>
		/// <param name="set"><c>true</c> if flags should be set, <c>false</c> if flags should
		/// be cleared</param>
		/// <param name="flags">Flags to set or clear</param>
		void ModifyAttributes(bool set, FileAttributes flags) {
			if (set)
				attributes |= (int)flags;
			else
				attributes &= ~(int)flags;
		}

		/// <summary>
		/// Gets/sets the <see cref="FileAttributes.ContainsMetadata"/> bit
		/// </summary>
		public bool ContainsMetadata {
			get => ((FileAttributes)attributes & FileAttributes.ContainsNoMetadata) == 0;
			set => ModifyAttributes(!value, FileAttributes.ContainsNoMetadata);
		}

		/// <summary>
		/// Gets/sets the <see cref="FileAttributes.ContainsNoMetadata"/> bit
		/// </summary>
		public bool ContainsNoMetadata {
			get => ((FileAttributes)attributes & FileAttributes.ContainsNoMetadata) != 0;
			set => ModifyAttributes(value, FileAttributes.ContainsNoMetadata);
		}

		/// <inheritdoc/>
		public string FullName => UTF8String.ToSystemStringOrEmpty(name);

		/// <inheritdoc/>
		public override string ToString() => FullName;
	}

	/// <summary>
	/// A File row created by the user and not present in the original .NET file
	/// </summary>
	public class FileDefUser : FileDef {
		/// <summary>
		/// Default constructor
		/// </summary>
		public FileDefUser() {
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="name">Name of file</param>
		/// <param name="flags">Flags</param>
		/// <param name="hashValue">File hash</param>
		public FileDefUser(UTF8String name, FileAttributes flags, byte[] hashValue) {
			this.Name = name;
			Attributes = (FileAttributes)flags;
			this.HashValue = hashValue;
		}
	}

	/// <summary>
	/// Created from a row in the File table
	/// </summary>
	sealed class FileDefMD : FileDef, IMDTokenProviderMD {
		/// <summary>The module where this instance is located</summary>
		readonly ModuleDefMD readerModule;

		readonly uint origRid;

		/// <inheritdoc/>
		public uint OrigRid => origRid;

		/// <inheritdoc/>
		protected override void InitializeCustomAttributes() {
			var list = readerModule.Metadata.GetCustomAttributeRidList(Table.File, origRid);
			var tmp = new CustomAttributeCollection(list.Count, list, (list2, index) => readerModule.ReadCustomAttribute(list[index]));
			Interlocked.CompareExchange(ref customAttributes, tmp, null);
		}

		/// <inheritdoc/>
		protected override void InitializeCustomDebugInfos() {
			var list = new List<PdbCustomDebugInfo>();
			readerModule.InitializeCustomDebugInfos(new MDToken(MDToken.Table, origRid), new GenericParamContext(), list);
			Interlocked.CompareExchange(ref customDebugInfos, list, null);
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="readerModule">The module which contains this <c>File</c> row</param>
		/// <param name="rid">Row ID</param>
		/// <exception cref="ArgumentNullException">If <paramref name="readerModule"/> is <c>null</c></exception>
		/// <exception cref="ArgumentException">If <paramref name="rid"/> is invalid</exception>
		public FileDefMD(ModuleDefMD readerModule, uint rid) {
#if DEBUG
			if (readerModule == null)
				throw new ArgumentNullException("readerModule");
			if (readerModule.TablesStream.FileTable.IsInvalidRID(rid))
				throw new BadImageFormatException($"File rid {rid} does not exist");
#endif
			origRid = rid;
			this.Rid = rid;
			this.readerModule = readerModule;
			bool b = readerModule.TablesStream.TryReadFileRow(origRid, out var row);
			Debug.Assert(b);
			Attributes = (FileAttributes)row.Flags;
			Name = readerModule.StringsStream.ReadNoNull(row.Name);
			HashValue = readerModule.BlobStream.Read(row.HashValue);
		}
	}
}
