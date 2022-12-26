using ControlzEx.Standard;
using LiteDB;
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
using MahApps.Metro.Controls.Dialogs;

namespace ExpertSystem
{
    /// <summary>
    /// SubModel.xaml etkileşim mantığı
    /// </summary>
    public partial class SubModel : MetroWindow
    {
        private SoruModel soruModel = new SoruModel();
        private int SiradakiSoru = 2;
        public int BountId;
        public SoruModel TheValue
        {
            get { return soruModel; }
        }
        public SubModel()
        {
            InitializeComponent();
        }

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
           
            soruModel.Sorun = SorunTx.Text;
            soruModel.BoundedTo = BountId;

            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                if (col.FindOne(x => x.BoundedTo == BountId && x.Sorun == SorunTx.Text) != null)
                {

                }
               else{
                    col.Insert(soruModel);
                }
                db.Dispose();
            }
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            soruModel.Sorun = SorunTx.Text;
            soruModel.BoundedTo = BountId;
            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                if (col.FindOne(x => x.BoundedTo == BountId && x.Sorun == SorunTx.Text) != null)
                {

                }
                else
                {
                    col.Insert(soruModel);
                }
                var SonEleman = col.FindOne(x => x.BoundedTo == soruModel.BoundedTo && x.Sorun == soruModel.Sorun);
                var bagliEleman = col.Find(x => x.BoundedTo == SonEleman.Id);
                if (bagliEleman != null && bagliEleman.ToList().Count == ((int)CevapSayisi.Value))
                {
                    this.ShowMessageAsync("Hata", "Girilen Eleman Sayısı Kadar Veri Girişi Yapıldı!");
                    db.Dispose();
                    return;
                }
                SubModel subMod = new SubModel();
                subMod.BountId = SonEleman.Id;
                subMod.SorunTx.Text = ((TextBox)CevapSayiSp.Children[SiradakiSoru]).Text;
                SiradakiSoru++;
                db.Dispose();
                subMod.ShowDialog();
            }
            
        }
    }
}
