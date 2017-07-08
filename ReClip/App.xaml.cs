using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

using ReClip.Database;
using System.Threading;

namespace ReClip
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        Mutex mutex;
        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "Re:Clip";

            try
            {
                mutex = new Mutex(false, mutexName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                Environment.Exit(0);
            }

            if (mutex.WaitOne(0, false))
            {
                base.OnStartup(e);
            }
            else
            {
                MessageBox.Show("Re:Clip은 이미 동작중입니다.");
                Environment.Exit(0);
            }
        }
    }
}