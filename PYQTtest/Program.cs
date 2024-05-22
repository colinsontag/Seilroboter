using System;
using Python.Runtime;


class Program
{
    static void Main(string[] args)
    {



        Environment.SetEnvironmentVariable("PYTHONNET_PYDLL", @"G:\Python\python312.dll");
        // Initialize the Python runtime
        PythonEngine.Initialize();
        
        try
        {
            using (Py.GIL())
            {
                // Import the Python script and call the main function
                dynamic PyQt5 = Py.Import("PyQt5");
                dynamic sys = Py.Import("sys");


                sys.path.append("W:\\Seilroboter\\SeilroboterUI");
                // Import the Python script and call the main function
                dynamic ui = Py.Import("SeilroboterUI");
                ui.main();
                
            }
        }
        catch (PythonException ex)
        {           
            Console.WriteLine("Python error: " + ex.Message);
            Console.WriteLine("Stack trace: " + ex.StackTrace);
        }
        finally
        {
            // Shutdown the Python runtime
            
        }
    }
}
