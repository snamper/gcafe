using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using gcafeApp.gcafeSvc;

namespace gcafeApp.ViewModel
{
    public class Method
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class MethodInGroup : List<Method>
    {
        public MethodInGroup(string catalog)
        {
            Name = catalog;
        }

        public string Name { get; set; }

        public bool HasItems { get { return Count > 0; } }
    }

    public class MethodCata : List<MethodInGroup>
    {
        private const string GlobeGroupKey = "\uD83C\uDF10";

        public MethodCata()
        {
            Dictionary<string, MethodInGroup> groups = new Dictionary<string, MethodInGroup>();

            MethodInGroup mig = new MethodInGroup("成数");
            MethodInGroup mig1 = new MethodInGroup("汁类");
            MethodInGroup mig2 = new MethodInGroup("做法");
            //MethodInGroup mig2 = new MethodInGroup(GlobeGroupKey);

            this.Add(mig);
            groups["成数"] = mig;
            this.Add(mig1);
            groups["汁类"] = mig1;
            this.Add(mig2);
            groups["做法"] = mig2;

            groups["成数"].Add(new Method() { Name = "一成" });
            groups["成数"].Add(new Method() { Name = "二成" });
            groups["成数"].Add(new Method() { Name = "三成" });
            groups["成数"].Add(new Method() { Name = "四成" });
            groups["成数"].Add(new Method() { Name = "五成" });
            groups["汁类"].Add(new Method() { Name = "一汁" });
            groups["汁类"].Add(new Method() { Name = "二汁" });
            groups["做法"].Add(new Method() { Name = "做法一" });
            groups["做法"].Add(new Method() { Name = "做法二" });
        }

        public MethodCata(ObservableCollection<MethodCatalog> catalogs)
        {
            foreach (var mc in catalogs)
            {
                MethodInGroup methodInGroup = new MethodInGroup(mc.Name);
                foreach (var method in mc.Methods)
                {
                    methodInGroup.Add(new Method() { Id = method.ID, Name = method.Name });
                }
                this.Add(methodInGroup);
            }
        }
    }

    public class VMSelectMethod : VMBase
    {
        private readonly IgcafeSvcClient _svc;

        public VMSelectMethod(IgcafeSvcClient svc)
        {
            if (IsInDesignMode)
            {
                //List<method_catalog> methodCatalogs = new List<method_catalog>();

                //List<method> methods = new List<method>();
                //methods.Add(new method() { id = 1, name = "一成", });
                //methods.Add(new method() { id = 2, name = "二成", });
                //methods.Add(new method() { id = 3, name = "三成", });
                //methods.Add(new method() { id = 4, name = "四成", });
                //methods.Add(new method() { id = 5, name = "五成", });
                //methodCatalogs.Add(new method_catalog() { id = 1, name = "成数", method = new ObservableCollection<method>(methods) });

                //methods = new List<method>();
                //methods.Add(new method() { id = 6, name = "叉烧汁", });
                //methods.Add(new method() { id = 7, name = "蜜汁", });
                //methods.Add(new method() { id = 8, name = "黑椒汁", });
                //methodCatalogs.Add(new method_catalog() { id = 2, name = "汁类", method = new ObservableCollection<method>(methods) });

                //MethodCatalogs = new ObservableCollection<method_catalog>(methodCatalogs);

                MethodCatalog = new MethodCata();
            }
            else
            {
                _svc = svc;
                _svc.GetMethodCatalogsCompleted += _svc_GetMethodCatalogsCompleted;

                _svc.GetMethodCatalogsAsync();
            }

        }

        protected override void Dispose(bool dispose)
        {
            base.Dispose(dispose);
            _svc.GetMethodCatalogsCompleted -= _svc_GetMethodCatalogsCompleted;
        }

        void _svc_GetMethodCatalogsCompleted(object sender, GetMethodCatalogsCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MethodCatalog = new MethodCata(e.Result);
            }
        }

        public MethodCata MethodCatalog
        {
            get { return _methodCata; }
            private set
            {
                _methodCata = value;
                RaisePropertyChanged();
            }
        }
        private MethodCata _methodCata;

        public ObservableCollection<method_catalog> MethodCatalogs
        {
            get { return _methodCatalogs; }
            private set
            {
                _methodCatalogs = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<method_catalog> _methodCatalogs;

        public ObservableCollection<method> Methods
        {
            get { return _methods; }
            private set
            {
                _methods = value;
                RaisePropertyChanged();
            }
        }
        private ObservableCollection<method> _methods;
    }
}
