// ------------------------------------------------------------------------------
// -- Copyright ERTMS Solutions
// -- Licensed under the EUPL V.1.1
// -- http://joinup.ec.europa.eu/software/page/eupl/licence-eupl
// --
// -- This file is part of ERTMSFormalSpec software and documentation
// --
// --  ERTMSFormalSpec is free software: you can redistribute it and/or modify
// --  it under the terms of the EUPL General Public License, v.1.1
// --
// -- ERTMSFormalSpec is distributed in the hope that it will be useful,
// -- but WITHOUT ANY WARRANTY; without even the implied warranty of
// -- MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// --
// ------------------------------------------------------------------------------

using System;
using System.Globalization;
using System.IO;
using System.ServiceModel;
using System.Threading;
using System.Windows.Forms;
using DataDictionary;
using GUI.IPCInterface;
using GUI.LongOperations;
using GUIUtils;
using GUIUtils.LongOperations;
using log4net.Config;
using Utils;
using Util = DataDictionary.Util;

namespace GUI
{
    public static class ErtmsFormalSpecGui
    {
        /// <summary>
        ///     The EFS IPC service
        /// </summary>
        private static ServiceHost _host;

        /// <summary>
        ///     Hosts the EFS IPC service
        /// </summary>
        /// <returns>The hosting service</returns>
        private static void HostEfsService()
        {
            _host = new ServiceHost(EFSService.Instance);
            try
            {
                _host.Open();
            }
            catch (CommunicationException exception)
            {
                Console.WriteLine(@"An exception occurred: {0}", exception.Message);
                _host.Abort();
            }
        }

        /// <summary>
        ///     Closes the EFS IPC host service
        /// </summary>
        private static void CloseEfsService()
        {
            if (_host != null)
            {
                // Close the ServiceHostBase to shutdown the service.
                _host.Close();
            }
        }

        [STAThread]
        private static void Main(string[] args)
        {
            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                XmlConfigurator.Configure(new FileInfo("logconfig.xml"));

                Options.Options.SetSettings();
                EfsSystem.Instance.DictionaryChangesOnFileSystem += HandleInstanceDictionaryChangesOnFileSystem;

                MainWindow window = new MainWindow();
                Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

                {
                    // TRICKY SECTION
                    // This thread is mandatory otherwise WCF does not create a new thread to handle the service requests. 
                    // Since the call to Cycle is blocking, creating such threads is mandatory
                    Thread thread = ThreadUtil.CreateThread("EFS Service", HostEfsService);
                    thread.Start();
                }

                // Opens the Dictionary files and check them
                bool shouldPlace = true;
                foreach (string fileName in args)
                {
                    const bool allowErrors = false;
                    OpenFileOperation openFileOperation = new OpenFileOperation(fileName, EfsSystem.Instance, allowErrors, true);
                    openFileOperation.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Opening file " + fileName, false);
                    if (openFileOperation.Dictionary != null)
                    {
                        window.SetupWindows(openFileOperation.Dictionary, shouldPlace);
                        shouldPlace = false;
                    }
                    else
                    {                        
                        Console.Out.WriteLine("Cannot open dictionary file " + fileName);
                    }
                }

                CheckModelOperation checkModel = new CheckModelOperation();
                checkModel.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Checking model");

                Application.Run(window);
                CloseEfsService();
            }
            finally
            {
                Util.UnlockAllFiles();
            }

            EfsSystem.Instance.Stop();
            SynchronizerList.Stop();
        }

        /// <summary>
        ///     Handles the change of a dictionary on the file system
        /// </summary>
        /// <param name="dictionary"></param>
        private static void HandleInstanceDictionaryChangesOnFileSystem(Dictionary dictionary)
        {
            OpenFileOperation openFile = new OpenFileOperation(dictionary.FilePath, EfsSystem.Instance, false, true);
            openFile.ExecuteUsingProgressDialog(GuiUtils.MdiWindow, "Refreshing dictionary " +
                                                Path.GetFileNameWithoutExtension(dictionary.FilePath), false);

            RefreshModel.Execute();
        }
    }
}