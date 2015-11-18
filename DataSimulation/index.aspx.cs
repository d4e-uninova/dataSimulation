using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DataSimulation
{
    public partial class index : System.Web.UI.Page
    {
        private DataExchange.DataSimulationClient client = new DataExchange.DataSimulationClient();

        protected void Page_Load(object sender, EventArgs e)
        {
            newFile.Visible = ModelFile.HasFile;
        }

        protected void Upload_Click(object sender, EventArgs e)
        {
            try
            {
                if (ModelFile.HasFile)
                {
                    byte[] modelFile;
                    byte[] weatherFile = null;
                    string guid = Guid.NewGuid().ToString();
                    string path = Server.MapPath("~") + "\\files\\";
                    path = path.Replace("\\\\", "\\");
                    DataExchange.ServiceDataFormat modelFormat = 0;
                    DataExchange.ServiceSimulationType simulationType = 0;
                    string outFile = "";
                    switch (ModelFormatSelector.SelectedValue)
                    {
                        case "IFC":
                            modelFormat = DataExchange.ServiceDataFormat.IFC;
                            break;
                        case "RVT":
                            modelFormat = DataExchange.ServiceDataFormat.RVT;
                            break;
                        case "gbXML":
                            modelFormat = DataExchange.ServiceDataFormat.gbXML;
                            break;
                        case "IDF":
                            modelFormat = DataExchange.ServiceDataFormat.IDF;
                            break;
                    }
                    switch (SimulationTypeSelector.SelectedValue)
                    {
                        case "Energetic":
                            simulationType = DataExchange.ServiceSimulationType.Energetic;
                            outFile = path + Path.ChangeExtension(ModelFile.FileName, "zip");
                            break;
                    }
                    modelFile = ModelFile.FileBytes;
                    if (WeatherFile.HasFile)
                    {
                        weatherFile = WeatherFile.FileBytes;
                    }

                    outFile = getFileName(outFile);
                    Global.logger.log(simulationType + " simulation of \"" + ModelFile.FileName + "\" requested by: " + Request.UserHostAddress);
                    simulateAsync(modelFile, weatherFile, modelFormat, simulationType, outFile);
                }
            }
            catch(Exception ex)
            {
                Global.logger.log(ex.Message, Logger.LogType.ERROR);
            }
        }

        private void simulateAsync(byte[] modelFile,byte[] weatherFile,DataExchange.ServiceDataFormat modelFormat,DataExchange.ServiceSimulationType simulationType,string outFile)
        {
            try
            {
                IAsyncResult asyncResult = client.Beginsimulate(new DataExchange.ServiceSimulationInfo
                {
                    Data = modelFile,
                    WeatherData = weatherFile,
                    ModelFormat = modelFormat,
                    Type = simulationType
                }, callback, client);
                Global.fileNames.Add(asyncResult.AsyncWaitHandle.SafeWaitHandle, outFile);    
                Global.addresses.Add(asyncResult.AsyncWaitHandle.SafeWaitHandle, Request.UserHostAddress);

            }
            catch(Exception ex)
            {
                Global.logger.log(ex.Message, Logger.LogType.ERROR);
            }
        }

        private void callback(IAsyncResult asyncResult)
        {
            try
            {
                string path = Global.fileNames[asyncResult.AsyncWaitHandle.SafeWaitHandle];
                string address = Global.addresses[asyncResult.AsyncWaitHandle.SafeWaitHandle];
                DataExchange.ServiceSimulationResult result = client.Endsimulate(asyncResult);
                if (result.Succeeded)
                {
                    File.WriteAllBytes(path, result.Data);
                    Global.logger.log("Writing file: \""+path+"\"");
                    Global.logger.log("Simulation requested by: " + address + " succeeded");
                }
                else
                {
                    Global.logger.log("Simulation requested by: " + address + " failed", Logger.LogType.ERROR);
                }
                Global.fileNames.Remove(asyncResult.AsyncWaitHandle.SafeWaitHandle);
                Global.addresses.Remove(asyncResult.AsyncWaitHandle.SafeWaitHandle);
            }
            catch(Exception ex)
            {
                Global.logger.log(ex.Message, Logger.LogType.ERROR);
            }
        }

        private string getFileName(string outFile)
        {
            string tmpPath = "";
            int maxFileCount = 1000;
            if (File.Exists(outFile) || Global.fileNames.ContainsValue(outFile))
            {
                for (var i = 1; i < maxFileCount; i++)
                {
                    tmpPath = Path.GetDirectoryName(outFile) + "\\" + Path.GetFileNameWithoutExtension(outFile) + "_" + i + Path.GetExtension(outFile);
                    if (!File.Exists(tmpPath) && !Global.fileNames.ContainsValue(tmpPath))
                    {
                        return tmpPath;
                    }
                }
                File.Delete(tmpPath);
                return tmpPath;
            }
            return outFile;
        }
    }
}