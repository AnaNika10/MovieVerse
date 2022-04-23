using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mono.Unix;
using System.Security.Principal;
using System.Security.AccessControl;
using System.Runtime.InteropServices;


namespace File.Utilities{
    public static class FileAccessHandler{

        private static void RemoveExecutePermissionsWindows(string dirPath)
        {
            DirectoryInfo dInfo = new DirectoryInfo(dirPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                dSecurity.RemoveAccessRule(new FileSystemAccessRule("Everyone", 
                                                                    FileSystemRights.ExecuteFile, 
                                                                    AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);   
        }
        private static void RemoveExecutePermissionsUnix(string dirPath)
        {
        
            var unixFileInfo = new Mono.Unix.UnixFileInfo(dirPath);
            unixFileInfo.FileAccessPermissions =
                FileAccessPermissions.UserRead | FileAccessPermissions.UserWrite | FileAccessPermissions.UserExecute
                | FileAccessPermissions.GroupRead | FileAccessPermissions.GroupWrite
                | FileAccessPermissions.OtherRead | FileAccessPermissions.OtherWrite;
        }
        public static void RemoveExecutePermissions(string dirPath)
        {
            
            if(System.Runtime.InteropServices.RuntimeInformation
                                               .IsOSPlatform(OSPlatform.Windows))
            {
                RemoveExecutePermissionsWindows(dirPath);
            }
            else
            {
                RemoveExecutePermissionsUnix(dirPath);
            }        
            
        }
    }
}