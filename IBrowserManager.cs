using System;
namespace BrowserLibrary;

public interface IBrowserManager{
    void OpenURLInMultipleInstances(string url, int VistasPedidas, int seconds);
    void CloseInstancesAfterTime(int seconds, string browser);
    string GetDefaultBrowser(string url, int VistasPedidas, int seconds);
}


