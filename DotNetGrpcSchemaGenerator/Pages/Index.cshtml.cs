using System.Linq;
using DotNetGrpcSchemaGenerator.Core;
using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace DotNetGrpcSchemaGenerator.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {
            Electron.IpcMain.On("assemblyPathBtn-selectDirectory", async (args) => {
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                var options = new OpenDialogOptions
                {
                    Properties = new OpenDialogProperty[] {
                        OpenDialogProperty.openFile
                    }
                };

                string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
                Electron.IpcMain.Send(mainWindow, "assemblyPath-reply", files);
            });

            Electron.IpcMain.On("destinationPathBtn-selectDirectory", async (args) => {
                var mainWindow = Electron.WindowManager.BrowserWindows.First();
                var options = new OpenDialogOptions
                {
                    Properties = new OpenDialogProperty[] {
                        OpenDialogProperty.openDirectory
                    }
                };

                string[] files = await Electron.Dialog.ShowOpenDialogAsync(mainWindow, options);
                Electron.IpcMain.Send(mainWindow, "destinationPathBtn-reply", files);
            });

            Electron.IpcMain.On("generate-schema", async (args) => {
                dynamic arguments = args;
                var assemblyFilePath = arguments.paths[0].Value;
                var destinationPath = arguments.paths[1].Value;

                var protoFileGenerator = new ProtoFileGenerator();
                protoFileGenerator.SaveProtoFile(assemblyFilePath, destinationPath);

                var options = new MessageBoxOptions("Successfull generated!")
                {
                    Type = MessageBoxType.info,
                    Title = "Information"
                };
                await Electron.Dialog.ShowMessageBoxAsync(options);
            });
        }
    }
}
