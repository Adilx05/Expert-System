using ControlzEx.Standard;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ExpertSystem
{
    /// <summary>
    /// SubModel.xaml etkileşim mantığı
    /// </summary>
    public partial class SubModel : MetroWindow
    {
        private SoruModel soruModel = new SoruModel();
        public SoruModel TheValue
        {
            get { return soruModel ; }
        }
        public SubModel()
        {
            InitializeComponent();
        }

        private void CevapSayisi_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            for (int i = 0; i < CevapSayisi.Value; i++)
            {
                CevapSayiSp.Children.RemoveRange(2, CevapSayiSp.Children.Count - 2);
                TextBox tb = new TextBox();
                tb.Name = "Cevap" + i;
                CevapSayiSp.Children.Add(tb);
                Console.WriteLine(tb.Name);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            soruModel.KacSecenek = ((int)CevapSayisi.Value);
            soruModel.Sorun = SorunTx.Text;
            List<string> liste = new List<string>();

            foreach (var item in CevapSayiSp.Children)
            {
                if (item is TextBox)
                {
                    if (((TextBox)item).Name.Contains("Cevap"))
                    {
                        liste.Add(((TextBox)item).Text);
                    }
                }
            }
            soruModel.Secenekler = liste;
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubModel subMod = new SubModel();
            subMod.ShowDialog();
            soruModel.AltSoruModeller.Add(subMod.TheValue);
        }
    }
}
