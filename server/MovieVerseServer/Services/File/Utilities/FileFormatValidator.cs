using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Models;


namespace File.Utilities{
    
    public static class FileFormatValidator{

        private static string[] permittedFileExtensions = {".jpg", ".jpeg", ".gif", ".png"};
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = 
                                            new Dictionary<string, List<byte[]>>
                                            {
                                                { ".jpeg", new List<byte[]>
                                                    {
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                                                    }
                                                },
                                                { ".jpg", new List<byte[]>
                                                    {
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },  	 
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                                                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 },
                                                    }
                                                },
                                                { ".gif", new List<byte[]>
                                                    {
                                                        new byte[] { 0x47, 0x49, 0x46, 0x38 }
                                                    }
                                                },
                                                { ".png", new List<byte[]>
                                                    {
                                                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 
                                                                     0x0D, 0x0A, 0x1A, 0x0A }
                                                    }
                                                },
                                                { ".mkv", new List<byte[]>
                                                    {
                                                        new byte[] { 0x1A, 0x45, 0xDF, 0xA3, 
                                                                     0x93, 0x42, 0x82, 0x88 }
                                                    }
                                                },
                                                
                                            };

        public static bool VaildFileSignature(string ext, FileModel file){
            using (var reader = new BinaryReader(file.FormFile.OpenReadStream()))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                return signatures.Any(signature => 
                        headerBytes.Take(signature.Length).SequenceEqual(signature));
            }

        }
        public static bool VaildFileSignatureVideo(string ext, Stream file){
            using (var reader = new BinaryReader(file))
            {
                var signatures = _fileSignature[ext];
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
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
        public static bool IsValidFileExtensionAndSignature(string ext, Stream stream, string[] permittedExtensions)
        {
            if(permittedExtensions.Contains<string>(ext) && VaildFileSignatureVideo(ext, stream))
            {
                return true;
            }
            return false;
        }

    }
}