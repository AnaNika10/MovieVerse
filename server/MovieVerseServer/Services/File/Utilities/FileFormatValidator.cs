using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Models;
using Microsoft.Extensions.Logging;


namespace File.Utilities{
    
    public static class FileFormatValidator{

        private static string[] permittedImgExtensions = {".jpg", ".jpeg", ".gif", ".png"};
        private static string[] permittedVideoExtensions = {".mkv", ".mp4"};
        private static readonly Dictionary<string, Tuple<List<byte[]>, int>> _imgFileSignature = 
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
                                            };

        private static readonly Dictionary<string, Tuple<List<byte[]>, int>> _videoFileSignature = 
                                            new Dictionary<string, Tuple<List<byte[]>, int>>
                                            {
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
        public static bool VaildFileSignature(string ext, Stream file, bool video=false){
            
            file.Position = 0;
            
            using (var reader = new BinaryReader(file))
            {
                List<byte[]> signatures;
                int offset;

                if(video){
                    (signatures, offset) = _videoFileSignature[ext];    
                }
                else{
                    (signatures, offset) = _imgFileSignature[ext];
                }
                reader.ReadBytes(offset);
                var headerBytes = reader.ReadBytes(signatures.Max(m => m.Length));
                var temp = Convert.ToHexString(headerBytes.Take(4).ToArray<byte>());
                return signatures.Any(signature => 
                headerBytes.Take(signature.Length).SequenceEqual(signature));
                
            }

        }
        public static bool ValidFileExt(string ext, bool isVideo=false){

            if (string.IsNullOrEmpty(ext))
            {
                return false;
            }
            if (isVideo && !permittedVideoExtensions.Contains(ext)){
                return false;
            }
            if (!isVideo && !permittedImgExtensions.Contains(ext)){
                return false;
            }
            return true;
        }

    }
}