using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Models;
using Microsoft.Extensions.Logging;


namespace File.Utilities{
    
    public static class FileFormatValidator{

        private static string[] permittedFileExtensions = {".jpg", ".jpeg", ".gif", ".png", ".mkv", ".mp4"};
        private static readonly Dictionary<string, Tuple<List<byte[]>, int>> _fileSignature = 
                                            new Dictionary<string, Tuple<List<byte[]>, int>>
                                            {
                                                { ".jpeg", new Tuple<List<byte[]>, int>(
                                                    new List<byte[]>
                                                    {
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                                                    },
                                                    0)
                                                },
                                                { ".jpg", new Tuple<List<byte[]>, int>(
                                                    new List<byte[]>
                                                    {
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },  	 
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                                                    },
                                                    0)
                                                },
                                                { ".gif", new Tuple<List<byte[]>, int>(
                                                    new List<byte[]>
                                                    {
                                                        new byte[] { 0x47, 0x49, 0x46, 0x38 }
                                                    },
                                                    0)
                                                },
                                                { ".png", new Tuple<List<byte[]>, int>(
                                                    new List<byte[]>
                                                    {
                                                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 
                                                                     0x0D, 0x0A, 0x1A, 0x0A }
                                                    },
                                                    0)
                                                },
                                                { ".mkv", new Tuple<List<byte[]>, int>(
                                                    new List<byte[]>
                                                    {
                                                        new byte[] { 0x1A, 0x45, 0xDF, 0xA3},
                                                    },
                                                    0)
                                                },
                                                { ".mp4", new Tuple<List<byte[]>, int>(
                                                    new List<byte[]>
                                                    {
                                                        new byte[] { 0x66, 0x74, 0x79, 0x70, 
                                                                     0x69, 0x73, 0x6F, 0x6D}
                                                    },
                                                    4)
                                                },
                                                
                                                
                                            };

        public static bool VaildFileSignature(string ext, Stream file){
            
            file.Position = 0;
            
            using (var reader = new BinaryReader(file))
            {
                var (signatures, offset) = _fileSignature[ext];
                reader.ReadBytes(offset);
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                var temp = Convert.ToHexString(headerBytes.Take(4).ToArray<byte>());
                return signatures.Any(signature => 
                headerBytes.Take(signature.Length).SequenceEqual(signature));
                
            }

        }
        public static bool ValidFileExt(string ext){

            if (string.IsNullOrEmpty(ext) || !permittedFileExtensions.Contains(ext))
            {
                return false;
            }
            return true;
        }

    }
}