using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.IO;

namespace DataSimulation
{
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
    public class RestApi
    {


        [WebGet(UriTemplate = "")]
        public string[] GetFiles()
        {
            string filesDir = Global.serverRoot + "\\files";
            filesDir = filesDir.Replace("\\\\", "\\");
            if (!Directory.Exists(filesDir))
            {
                Directory.CreateDirectory(filesDir);
            }
            IEnumerable<string> filesList = Directory.EnumerateFiles(filesDir);
            string[] fileNames = new string[filesList.Count()];
            for (var i = 0; i < filesList.Count(); i++)
            {
                fileNames[i] = Path.GetFileName(filesList.ElementAt(i));
            }

            return fileNames;
        }

    }
}