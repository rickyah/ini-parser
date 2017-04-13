using System;
using System.IO;
using System.Text;
using IniParser.Exceptions;
using IniParser.Model;
using Windows.Storage;
using System.Threading.Tasks;

namespace IniParser
{
    /// <summary>
    ///     Represents an INI data parser for files.
    /// </summary>
    public class FileIniDataParser : StreamIniDataParser
    {
        #region Deprecated methods

        [Obsolete("Please use ReadFile method instead of this one as is more semantically accurate")]
        public async Task<IniData> LoadFile(StorageFile file)
        {
            return await ReadFile(file);
        }

		[Obsolete("Please use ReadFile method instead of this one as is more semantically accurate")]
		public async Task<IniData> LoadFile(StorageFile file, Encoding fileEncoding) {
			return await ReadFile(file, fileEncoding);
		}
		#endregion

		/// <summary>
		///     Implements reading ini data from a file.
		/// </summary>
		/// <remarks>
		///     Autodetect encoding.
		/// </remarks>
		/// <param name="file">
		///     Path to the file
		/// </param>
		public async Task<IniData> ReadFile(StorageFile file) 
		{
			try 
			{
				var stream = await file.OpenStreamForReadAsync();
				using (StreamReader sr = new StreamReader(stream, true)) 
				{
					return ReadData(sr);
				}
			} 
			catch (IOException ex) 
			{
				throw new ParsingException(String.Format("Could not parse file {0}", file), ex);
			}
		}

		/// <summary>
		///     Implements reading ini data from a file.
		/// </summary>
		/// <param name="file">
		///     Path to the file
		/// </param>
		/// <param name="fileEncoding">
		///     File's encoding.
		/// </param>
		public async Task<IniData> ReadFile(StorageFile file, Encoding fileEncoding) {
			try {
				var stream = await file.OpenStreamForReadAsync();
				using (StreamReader sr = new StreamReader(stream, fileEncoding)) {
					return ReadData(sr);
				}
			} catch (IOException ex) {
				throw new ParsingException(String.Format("Could not parse file {0}", file), ex);
			}

		}


		/// <summary>
		///     Saves INI data to a file.
		/// </summary>
		/// <remarks>
		///     Creats an ASCII encoded file by default.
		/// </remarks>
		/// <param name="file">
		///     Path to the file.
		/// </param>
		/// <param name="parsedData">
		///     IniData to be saved as an INI file.
		/// </param>
		[Obsolete("Please use WriteFile method instead of this one as is more semantically accurate")]
        public void SaveFile(StorageFile file, IniData parsedData)
        {
            WriteFile(file, parsedData, Encoding.ASCII);
        }

		/// <summary>
		///     Writes INI data to a text file.
		/// </summary>
		/// <param name="file">
		///     Path to the file.
		/// </param>
		/// <param name="parsedData">
		///     IniData to be saved as an INI file.
		/// </param>
		/// <param name="fileEncoding">
		///     Specifies the encoding used to create the file.
		/// </param>
		public async Task WriteFile(StorageFile file, IniData parsedData, Encoding fileEncoding = null) 
		{
			// The default value can't be assigned as a default parameter value because it is not
			// a constant expression.
			if (fileEncoding == null)
				fileEncoding = Encoding.Unicode;

			if (parsedData == null)
				throw new ArgumentNullException("parsedData");

			var stream = await file.OpenStreamForWriteAsync();

			using (StreamWriter sr = new StreamWriter(stream, fileEncoding)) 
			{
				WriteData(sr, parsedData);
			}
		}
	}
}
