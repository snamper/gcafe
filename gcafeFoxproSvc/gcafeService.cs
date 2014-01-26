using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.ServiceModel;

namespace gcafeFoxproSvc
{
    public partial class gcafeService : ServiceBase
    {
        ServiceHost _serviceHost = null;

        public gcafeService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _serviceHost = new ServiceHost(typeof(gcafeSvc));
            Global.PrintTasmMgrInit();
            _serviceHost.Open();
        }

        protected override void OnStop()
        {
            _serviceHost.Close();
            Global.PrintTaskMgr.Dispose();
        }
    }
}
