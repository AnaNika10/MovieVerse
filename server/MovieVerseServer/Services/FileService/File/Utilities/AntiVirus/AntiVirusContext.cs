using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using File.Controllers;
using ClamAV.Net.Client;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace File.Utilities.Antivirus
{
    public class AntiVirusContext : IAntiVirusContext
    {
        public IClamAvClient _client {get;}
        public AntiVirusContext()
        {
            _client = ClamAvClient.Create(new Uri("tcp://host.docker.internal:3310"));
            
        }
        public async Task<bool> ScanFileForViruses(IFormFile file, ILogger<FileUploadController> logger)
        {
            
            try{
                using(var ms = new MemoryStream()){
                    await file.OpenReadStream().CopyToAsync(ms);  
                    var scanResult = await _client.ScanDataAsync(ms);  
                    
                    logger.LogInformation("Infected? " + scanResult.Infected);
                    if(scanResult.Infected)  
                    {  
                        logger.LogInformation(scanResult.VirusName);
                        return false;  
                    }
                }
                return true;
            }
            catch(Exception ex){
                logger.LogInformation("Exception happened while scanning for viruses: {0} {1}", ex.Message, ex.InnerException);
                return false;
            } 
        }

        public async Task<bool> ScanFileForViruses(byte[] content, ILogger<FileUploadController> logger){

            try{
                using(var ms = new MemoryStream(content)){
                    
                    var scanResult = await _client.ScanDataAsync(ms);  
                    
                    logger.LogInformation("Infected? " + scanResult.Infected);
                    if(scanResult.Infected)  
                    {  
                        logger.LogInformation(scanResult.VirusName);
                        return false;  
                    }
                }
                return true;
            }
            catch(Exception ex){
                logger.LogInformation("Exception happened while scanning for viruses: {0}", ex.Message);
                return false;
            } 
        }
    }
}