#region File Info
// File       : VisualPrintDialog.cs
// Description: 
// Package    : VisualPrint
//
// Authors    : Fred Song
//
#endregion
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Printing;
using System.IO.Packaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;
using System.Windows.Xps.Serialization;

namespace VisualPrint
{
    public class VisualPrintDialog
    {
        //private PrintWindow m_Window;
        private Visual m_Visual;
        private PrintDialog m_PrintDialog;

        public VisualPrintDialog(Visual visual)
        {
            //m_Window = new PrintWindow(visual);
            m_Visual = visual;
            m_PrintDialog = new PrintDialog();
            m_PrintDialog.UserPageRangeEnabled = false;
        }

        #region 'Public Properties'

        public Visual Visual
        {
            get { return m_Visual; }
            set { m_Visual = value; }
        }

        #endregion

        #region 'Public Methods'
        public void ShowDialog(string printerName)
        {
            try
            {
                //bool? result = m_PrintDialog.ShowDialog();
                bool result = true;
                var printers = new LocalPrintServer().GetPrintQueues();
                var selectedPrinter = printers.FirstOrDefault(p => p.Name == printerName);

                if (selectedPrinter == null)
                    return;

                m_PrintDialog.PrintQueue = selectedPrinter;
                //m_PrintDialog.PageRange = new PageRange(1, 0);

                FlowDocument flowDocument = null;
                if (result == true)
                {
                    System.Windows.Input.Mouse.OverrideCursor = System.Windows.Input.Cursors.Wait;
                    try
                    {
                        flowDocument = Helper.CreateFlowDocument(m_Visual, new Size(m_PrintDialog.PrintableAreaWidth, m_PrintDialog.PrintableAreaHeight));
                    }
                    finally
                    {
                        System.Windows.Input.Mouse.OverrideCursor = null;
                    }
                    if (flowDocument != null)
                    {
                        //PrintPreview(flowDocument);
                        Print(flowDocument);
                    }
                }
            }
            catch (Exception ex)
            {
                string s = ex.Message;
                MessageBox.Show(s);
            }
        }
        #endregion

        #region 'Private Methods/Events'
        
        private void OnPrint(object sender, RoutedEventArgs e)
        {
            FlowDocument document = (e.Source as PreviewWindow).Document;
            if (document != null)
                Print(document);
        }

        private void Print(FlowDocument document)
        {
            DocumentPaginator paginator = ((IDocumentPaginatorSource)document).DocumentPaginator;
            m_PrintDialog.PrintDocument(paginator, "Printing");
        }

        private void PrintPreview(FlowDocument document)
        {
            PreviewWindow win = new PreviewWindow(document);
            win.Print += new RoutedEventHandler(OnPrint);
            win.ShowDialog();
        }
 
        #endregion
    }
}
