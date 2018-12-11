// dnlib: See LICENSE.txt for more info

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace dnlib.IO {
	static class DataReaderFactoryFactory {
		static readonly bool isUnix = GetOSVersion();

		static bool GetOSVersion() {
			// https://github.com/dotnet/platform-compat/blob/master/docs/DE0009.md
			bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
			bool IsMacOS() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
			bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);

			return !IsWindows() && (IsLinux() || IsMacOS());
		}

		public static DataReaderFactory Create(string fileName, bool mapAsImage) {
			var creator = CreateDataReaderFactory(fileName, mapAsImage);
			if (creator != null)
				return creator;

			return ByteArrayDataReaderFactory.Create(File.ReadAllBytes(fileName), fileName);
		}

		static DataReaderFactory CreateDataReaderFactory(string fileName, bool mapAsImage) {
			if (!isUnix)
				return MemoryMappedDataReaderFactory.CreateWindows(fileName, mapAsImage);
			else
				return MemoryMappedDataReaderFactory.CreateUnix(fileName, mapAsImage);
		}
	}
}
