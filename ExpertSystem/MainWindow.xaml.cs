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
using MahApps.Metro.Controls.Dialogs;
using System.IO;

namespace ExpertSystem
{
    /// <summary>
    /// MainWindow.xaml etkileşim mantığı
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public int LastId = 1;
        private int SiradakiSoru = 2;
        private int degisecekId;
        private bool isEditable = false;
        private bool isWarned = false;
        public List<int> AltModeller = new List<int>();

        public MainWindow()
        {
            InitializeComponent();
            SoruModelGetir(1);
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

        private void Ekle_Button_Click(object sender, RoutedEventArgs e)
        {
            DbKontrolEt();
            if (!isEditable)
            {
                return;
            }
            if (!isWarned)
            {
                KullaniciUyar();
                return;
            }
            _soruModel.Sorun = SorunTx.Text;
            _soruModel.Id = 1;
            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                if (col.FindOne(x => x.Id == LastId && x.Sorun == SorunTx.Text) != null)
                {

                }
                else
                {
                    col.Insert(_soruModel);
                }
                db.Dispose();
            }
            isEditable = false;
            isWarned = false;
            SoruModelGetir(1);

        }

        private async void KullaniciUyar()
        {
            MetroDialogSettings metroAyar = new MetroDialogSettings();
            metroAyar.AffirmativeButtonText = "Evet";
            metroAyar.NegativeButtonText = "Hayır";
            var metroResult = await this.ShowMessageAsync("Bilgi", "Bu işlemden sonra alt model ekleyemezsiniz! Onaylıyor musunuz?", MessageDialogStyle.AffirmativeAndNegative, metroAyar);
            if (metroResult == MessageDialogResult.Affirmative)
            {
                isEditable = true;
                isWarned = true;
            }
            else
            {
                isEditable = false;
            }
        }

        private async void SoruModelGetir(int Alindi)
        {
            BtSp.Children.Clear();

            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                var getir = col.FindOne(x => x.Id == Alindi);
                if (getir == null)
                {
                    await this.ShowMessageAsync("Hata", "Sistemde kayıtlı bir veritabanı bulunmuyor!");
                    return;
                }
                _soruModel = getir;
                SorunLabel.Content = _soruModel.Sorun;
                LastId = _soruModel.Id;
                var elemanlar = col.FindAll();
                int i = 0;
                foreach (var item in elemanlar)
                {
                    if (item.BoundedTo == _soruModel.Id)
                    {
                        AltModeller.Add(item.Id);
                        Button tb = new Button();
                        tb.Name = "Bt" + i;
                        tb.Content = item.Sorun;
                        tb.Click += Tb_Click;
                        BtSp.Children.Add(tb);
                        i++;
                    }
                }
                if (i == 0)
                {
                    MetroDialogSettings metroAyar = new MetroDialogSettings();
                    metroAyar.AffirmativeButtonText = "Evet";
                    metroAyar.NegativeButtonText = "Hayır";
                    var metroResult = await this.ShowMessageAsync("Bilgi", "Yardım Sayfasının Sonuna Ulaştınız Siteye Yönlendirilmek ister misiniz?", MessageDialogStyle.AffirmativeAndNegative, metroAyar);
                    if (metroResult == MessageDialogResult.Affirmative)
                    {
                        System.Diagnostics.Process.Start("http://google.com");
                    }
                }
                db.Dispose();
            }
            MevcurSorunLab.Content = _soruModel.Sorun;
            if (_soruModel.BoundedTo == 0)
            {
                GeriBt.IsEnabled = false;
            }
            else
            {
                GeriBt.IsEnabled = true;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            DbKontrolEt();
            if (!isEditable)
            {
                return;
            }

            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                var bagliEleman = col.Find(x => x.BoundedTo == _soruModel.Id);
                if (bagliEleman != null && bagliEleman.ToList().Count == ((int)CevapSayisi.Value))
                {
                    this.ShowMessageAsync("Hata", "Girilen Eleman Sayısı Kadar Veri Girişi Yapıldı!");
                    db.Dispose();
                    return;
                }
                db.Dispose();
            }
            _soruModel.Sorun = SorunTx.Text;
            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                if (col.FindOne(x => x.Id == LastId && x.Sorun == SorunTx.Text) != null)
                {

                }
                else
                {
                    col.Insert(_soruModel);
                }
                db.Dispose();
            }
            SubModel subModel = new SubModel();
            subModel.BountId = LastId;
            subModel.SorunTx.Text = ((TextBox)CevapSayiSp.Children[SiradakiSoru]).Text;
            SiradakiSoru++;
            subModel.ShowDialog();

        }

        private async void DbKontrolEt()
        {
            if (File.Exists("dbtest.db") && !isEditable)
            {
                MetroDialogSettings metroAyar = new MetroDialogSettings();
                metroAyar.AffirmativeButtonText = "Evet";
                metroAyar.NegativeButtonText = "Hayır";
                var metroResult = await this.ShowMessageAsync("Bilgi", "Mevcut bir veritabanı bulunuyor yeni giriş yapmnak için silinecektir. Onaylıyor musunuz?", MessageDialogStyle.AffirmativeAndNegative, metroAyar);
                if (metroResult == MessageDialogResult.Affirmative)
                {
                    File.Delete("dbtest.db");
                    isEditable = true;
                }
                else
                {
                    isEditable = false;
                    return;
                }
            }
            isEditable = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                _soruModel = col.FindById(1);
                db.Dispose();
                SoruModelGetir(1);
            }
        }

        private void Geri_Button_Click(object sender, RoutedEventArgs e)
        {
            SoruModelGetir(_soruModel.BoundedTo);

        }

        private void AramaTx_TextChanged(object sender, TextChangedEventArgs e)
        {
            AramaSp.Children.Clear();
            if (AramaTx.Text != "")
            {
                using (var db = new LiteDatabase("dbtest.db"))
                {
                    var col = db.GetCollection<SoruModel>("sorumodelleri");
                    var elemanlar = col.Find(x => x.Sorun.Contains(AramaTx.Text));
                    int i = 0;
                    foreach (var item in elemanlar)
                    {
                        Button tb = new Button();
                        tb.Name = "ID" + item.Id;
                        tb.Content = item.Sorun;
                        tb.Click += AramaGenBtClick;
                        AramaSp.Children.Add(tb);
                        i++;
                    }
                    db.Dispose();
                }
            }
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
                        using (var db = new LiteDatabase("dbtest.db"))
                        {
                            var col = db.GetCollection<SoruModel>("sorumodelleri");
                            var altElemanlar = col.Find(x => x.BoundedTo == _soruModel.Id);
                            List<SoruModel> liste = new List<SoruModel>();
                            liste = altElemanlar.ToList();
                            db.Dispose();
                            SoruModelGetir(liste[i].Id);
                        }

                        break;
                    }
                }
            }
            BtSp = stackPanel;
        }
        private void AramaGenBtClick(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            int gonder = int.Parse(bt.Name.Remove(0, 2));
            SoruModelGetir(gonder);
        }

        private void DegisKaydetBt_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                var eleman = col.FindOne(x => x.Id == degisecekId);
                eleman.Sorun = DegisecekTx.Text;
                col.Update(eleman);
                db.Dispose();
            }
        }

        private void AranacakTextTb_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (AranacakTextTb.Text != "")
            {
                using (var db = new LiteDatabase("dbtest.db"))
                {
                    var col = db.GetCollection<SoruModel>("sorumodelleri");
                    var eleman = col.FindOne(x => x.Sorun.Contains(AranacakTextTb.Text));
                    if (eleman != null)
                    {
                        DegisecekLb.Content = "Id = " + eleman.Id + Environment.NewLine + "İçerik : " + eleman.Sorun;
                        degisecekId = eleman.Id;
                        DegisKaydetBt.IsEnabled = true;
                    }
                    else
                    {
                        DegisecekLb.Content = "Gösterilecek Bir Sorun Yok!";
                        DegisKaydetBt.IsEnabled = false;
                    }
                    db.Dispose();
                }
            }
        }

        private void AranacakIdTb_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            if (AranacakIdTb.Value != 0 && AranacakIdTb.Value != null)
            {
                using (var db = new LiteDatabase("dbtest.db"))
                {
                    var col = db.GetCollection<SoruModel>("sorumodelleri");
                    var eleman = col.FindOne(x => x.Id == ((int)AranacakIdTb.Value));
                    if (eleman != null)
                    {
                        DegisecekLb.Content = "Id = " + eleman.Id + Environment.NewLine + "İçerik : " + eleman.Sorun;
                        degisecekId = eleman.Id;
                        DegisKaydetBt.IsEnabled = true;
                    }
                    else
                    {
                        DegisecekLb.Content = "Gösterilecek Bir Sorun Yok!";
                        DegisKaydetBt.IsEnabled = false;
                    }
                    db.Dispose();
                }
            }
        }

        private void SingleEkleBt_Click(object sender, RoutedEventArgs e)
        {
            if ((int)SingleEkleVal.Value <= 0 && !SingleEkleVal.Value.HasValue)
            {
                this.ShowMessageAsync("Hata!", "1 veya daha büyük bir ID'ye bağlamalısınız!");
                return;
            }
            SoruModel sm = new SoruModel();
            sm.BoundedTo = (int)SingleEkleVal.Value;
            sm.Sorun = SingleEkleTb.Text;
            using (var db = new LiteDatabase("dbtest.db"))
            {
                var col = db.GetCollection<SoruModel>("sorumodelleri");
                col.Insert(sm);
                db.Dispose();
            }

        }

        private async void AdminPanBt_Click(object sender, RoutedEventArgs e)
        {
            LoginDialogSettings lds = new LoginDialogSettings();
            lds.UsernameWatermark = "Kullanıcı Adı...";
            lds.PasswordWatermark = "Parola...";
            lds.AffirmativeButtonText = "Giriş Yap";
            lds.NegativeButtonText = "İptal";
            lds.NegativeButtonVisibility = Visibility.Visible;
            var dialogRes = await this.ShowLoginAsync("Giriş", "Lütfen Id Ve Parolanızı Girin!",lds);
            if (dialogRes == null)
            {
                return;
            }
            if ( dialogRes.Username == "admin" && dialogRes.Password == "admin")
            {
                SorunEkleTab.Visibility = Visibility.Visible;
                DegistirTab.Visibility = Visibility.Visible;
                SingleEkleTab.Visibility = Visibility.Visible;
            }
            else
            {
                await this.ShowMessageAsync("Hata", "Kullanıcı adı veya parola hatalı!");
            }

        }
    }
}
