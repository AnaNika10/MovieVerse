using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using File.Controllers;
using File.Utilities;
using File.Utilities.Antivirus;
namespace File.Utilities
{
    public static class FileHelpers
    {
         public static async Task<byte[]> ProcessStreamedFile(
            MultipartSection section, ContentDispositionHeaderValue contentDisposition, 
            ModelStateDictionary modelState, long sizeLimit)
        {
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    await section.Body.CopyToAsync(memoryStream);
                    var ext = Path.GetExtension(contentDisposition.FileName.Value).ToLowerInvariant();
                    
                    if (memoryStream.Length == 0)
                    {
                        modelState.AddModelError("File", "The file is empty.");
                    }
                    else if (memoryStream.Length > sizeLimit)
                    {
                        var megabyteSizeLimit = sizeLimit / 1048576;
                        modelState.AddModelError("File",
                        $"The file exceeds {megabyteSizeLimit:N1} MB.");
                    }
                    else if (!FileFormatValidator.ValidFileExt(ext, true)){
                        modelState.AddModelError("File",
                            "The file type isn't permitted");
                        
                    }
                    else if(!FileFormatValidator.VaildFileSignature(ext, memoryStream, true))
                    {

                        modelState.AddModelError("File",
                            "The file's " +
                            "signature doesn't match the file's extension. Ext " + $"{ext}");
                    }
                    else
                    {
                        return memoryStream.ToArray();
                    }
                }
            }
            catch(Exception ex)
            {
                modelState.AddModelError("File",
                    "The upload failed. Error:" + $"{ex.Message}");
            }

            return Array.Empty<byte>();
        }

        public static (int, string) ValidateFileProperties(IFormFile file, long sizeLimit,
                                                                        ILogger<FileUploadController> logger)
        {
            if(file.Length == 0 || file.Length > sizeLimit){
                return (415, "Invalid file size of: " + file.FileName);
            }
            (string _, string originalFileExt) = FileFormatValidator.GetOriginalFileNameAndExtension(file.FileName); 
            if(!FileFormatValidator.ValidFileExt(originalFileExt))
            {
                return (415, "Unsupported file format of: " + file.FileName);
            }
            if(!FileFormatValidator.VaildFileSignature(originalFileExt, file.OpenReadStream())){
                logger.LogInformation("File ext signature failed");
                return (415, "File extension signature check failed: " + file.FileName);
            }

            return (200, "Ok.");
        }
    }
}