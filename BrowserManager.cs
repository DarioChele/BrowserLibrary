using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BrowserLibrary;
public class BrowserManager : IBrowserManager{

    public void OpenURLInMultipleInstances(string url, int VistasPedidas, int seconds){
        int vistas = 0; int vueltaActual = 0; int numVueltas = 1; bool res = true;
        try{
            if (VistasPedidas > 5){
                numVueltas = VistasPedidas / 5;
            }
            while ((vistas < VistasPedidas) && res){
                int vistasPend = VistasPedidas - (vueltaActual * 5);
                if (vistasPend < 5){
                    GetDefaultBrowser(url, vistasPend, seconds);
                    vistas = vistas + vistasPend;
                }else{
                    GetDefaultBrowser(url, 5, seconds);
                    vistas = vistas + 5;
                }
                vueltaActual++;
            }
        }catch (Exception ex){

        }
    }

    public void CloseInstancesAfterTime(int seconds, string browser){
        Thread.Sleep(seconds * 1000);
        if (string.IsNullOrEmpty(browser))
        {
            throw new Exception("No se pudo encontrar el navegador predeterminado.");
        }

        var processes = Process.GetProcessesByName(browser);
        foreach (var process in processes)
        {
            process.WaitForInputIdle();
            process.EnableRaisingEvents = true;
            process.Exited += (sender, e) => { Console.WriteLine($"La instancia {process.Id} ha sido cerrada."); };
        }
    }

    public string GetDefaultBrowser(string url, int CantVistas, int seconds){
        string browser = string.Empty;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)){
            // Windows
            GetWindowsDefaultBrowser(url, CantVistas, seconds);
        }else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)){
            // Linux
            GetLinuxDefaultBrowser(url, CantVistas, seconds);
        }else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)){
            // macOS
            GetMacDefaultBrowser(url, CantVistas, seconds);
        }

        return browser;
    }

    private string GetWindowsDefaultBrowser(string url, int CantVistas, int seconds){
        string browser = string.Empty;
        try{
            // Obtiene la clave de registro que contiene el navegador predeterminado
            using (var key = Registry.ClassesRoot.OpenSubKey(@"HTTP\shell\open\command", false))
            {
                if (key != null)
                {
                    // Obtiene el valor de la clave, que contiene el comando para abrir el navegador
                    string command = key.GetValue(null).ToString();

                    // Verifica si el comando contiene la ruta del navegador
                    if (command.ToLower().StartsWith("http") || command.ToLower().StartsWith("https"))
                    {
                        browser = command.Split(new char[] { '"', ' ' }, StringSplitOptions.RemoveEmptyEntries)[0];
                    }
                    else
                    {
                        browser = command.Split(new char[] { '"' }, StringSplitOptions.RemoveEmptyEntries)[1];
                    }
                }
                else
                {
                    Console.WriteLine("No se pudo encontrar la clave del registro del navegador predeterminado");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener el navegador predeterminado: {ex.Message}");
        }

        return browser;
    }
    private string GetLinuxDefaultBrowser(string url, int CantVistas, int seconds)
    {
        string browser = string.Empty;

        try{
            // Obtiene el navegador predeterminado desde el archivo /etc/alternatives/x-www-browser
            browser = System.IO.File.ReadAllText("/etc/alternatives/x-www-browser").Trim();
        }catch (Exception ex){
            Console.WriteLine($"Error al obtener el navegador predeterminado: {ex.Message}");
        }

        return browser;
    }
    private string GetMacDefaultBrowser(string url, int CantVistas, int seconds){
        string browser = string.Empty;
        try{
            // Iniciar el proceso del navegador Safari
            List<Process> processes = new List<Process>();
            for (int i = 0; i < CantVistas; i++){
                Process process = new Process();
                process.StartInfo.FileName = "/Applications/Safari.app/Contents/MacOS/Safari/";
                process.StartInfo.Arguments = url;
                //process.StartInfo.Arguments = "--args " + url;
                process.Start();
                processes.Add(process);
            }
            // Esperar
            Thread.Sleep(seconds*1000);
            foreach (Process navegador in processes){
                navegador.Kill();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error al obtener el navegador predeterminado: {ex.Message}");
        }

        return browser;
    }


}
/*
 file:///Users/darius/Library/Containers/com.apple.Safari/Data/https:/www.youtube.com/watch%3Fv=qZjWUkohSQg
 */