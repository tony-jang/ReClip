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
        private Mutex _mutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            bool isNew = true;
            Mutex mutex = new Mutex(true, "ReClip", out isNew);

            if (isNew == false)
            {
                Environment.Exit(0);
                // 중복실행시 처리
            }
            else
            {
                // 실행
                mutex.ReleaseMutex();

                BitmapCache.Open();

                base.OnStartup(e);
            }

            
        }
    }
}