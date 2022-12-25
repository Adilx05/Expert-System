using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem
{
    public class SoruModel
    {
        public int Id { get; set; }
      //  public int KacSecenek { get; set; }
        public string Sorun { get; set; }
        public int BoundedTo { get; set; }

       // public List<string> Secenekler { get; set; } = new List<string>();
       // public List<SoruModel> AltSoruModeller { get; set; } = new List<SoruModel>();

        /*public SoruModel(int kacSecenek, string sorun, List<string> secenekler, List<SoruModel> altSoruModeller)
        {
            KacSecenek = kacSecenek;
            Sorun = sorun ?? throw new ArgumentNullException(nameof(sorun));
            Secenekler = secenekler ?? throw new ArgumentNullException(nameof(secenekler));
            AltSoruModeller = altSoruModeller ?? throw new ArgumentNullException(nameof(altSoruModeller));
        }

        public SoruModel()
        {
        }*/
    }
}
