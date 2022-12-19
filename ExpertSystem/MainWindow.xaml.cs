using ControlzEx.Standard;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using LiteDB;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ExpertSystem
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            
        }
        SoruModel _soruModel = new SoruModel();

        private void CevapSayisi_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            CevapSayiSp.Children.RemoveRange(2, CevapSayiSp.Children.Count - 2);
            for (int i = 0; i < CevapSayisi.Value; i++)
            {
                TextBox tb = new TextBox();
                tb.Name = "Cevap" + i;
                CevapSayiSp.Children.Add(tb);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _soruModel.KacSecenek = ((int)CevapSayisi.Value);
            _soruModel.Sorun = SorunTx.Text;
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
            _soruModel.Secenekler = liste;
            SorunLabel.Content = _soruModel.Sorun;
            for (int i = 0; i < _soruModel.Secenekler.Count; i++)
            {
                Button tb = new Button();
                tb.Name = "Bt" + i;
                tb.Content = _soruModel.Secenekler[i];
                tb.Click += Tb_Click;
                BtSp.Children.Add(tb);
            }
            using (var db = new LiteDatabase("dbtest.db"))
            {
                // Get a collection (or create, if doesn't exist)
                var col = db.GetCollection<SoruModel>("sorumodelleri");

                // Create your new customer instance
                var soruModel = new SoruModel
                {
                    KacSecenek = _soruModel.KacSecenek,
                    Sorun = _soruModel.Sorun,
                    AltSoruModeller = _soruModel.AltSoruModeller,
                    Secenekler = _soruModel.Secenekler
                };
                
                // Insert new customer document (Id will be auto-incremented)
                col.Insert(soruModel);
                /*// Update a document inside a collection
                customer.Name = "Jane Doe";

                col.Update(customer);

                // Index document using document Name property
                col.EnsureIndex(x => x.Name);

                // Use LINQ to query documents (filter, sort, transform)
                var results = col.Query()
                    .Where(x => x.Name.StartsWith("J"))
                    .OrderBy(x => x.Name)
                    .Select(x => new { x.Name, NameUpper = x.Name.ToUpper() })
                    .Limit(10)
                    .ToList();

                // Let's create an index in phone numbers (using expression). It's a multikey index
                col.EnsureIndex(x => x.Phones);

                // and now we can query phones
                var r = col.FindOne(x => x.Phones.Contains("8888-5555"));*/
            }
        }

        private void SoruModelGetir(int Alindi)
        {
            BtSp.Children.Clear();
            SoruModel sm = _soruModel.AltSoruModeller[Alindi];
            SorunLabel.Content = sm.Sorun;
            for (int i = 0; i < sm.Secenekler.Count; i++)
            {
                Button tb = new Button();
                tb.Name = "Bt" + i;
                tb.Content = sm.Secenekler[i];
                tb.Click += Tb_Click;
                BtSp.Children.Add(tb);
            }
            _soruModel = sm;
        }

        private void Tb_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            StackPanel stackPanel = new StackPanel();
            stackPanel = BtSp;
            for (int i = 0; i < stackPanel.Children.Count; i++)
            {
                if (stackPanel.Children[i] is Button)
                {
                    if (((Button)stackPanel.Children[i]).Name == bt.Name)
                    {
                        SoruModelGetir(i);
                        break;
                    }
                }
            }
            /*foreach (var item in stackPanel.Children)
            {
                if (((Button)item).Name == bt.Name)
                {
                    SoruModelGetir(i);
                }
                else
                {
                    i++;
                }
            }*/
            BtSp = stackPanel;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SubModel subModel = new SubModel();
            subModel.ShowDialog();
            if (subModel.DialogResult.Value == true)
            {
                _soruModel.AltSoruModeller.Add(subModel.TheValue);
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }
    }
}
