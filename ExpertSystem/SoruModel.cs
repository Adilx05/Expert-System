using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertSystem
{
    public class SoruModel
    {
        public int KacSecenek;
        public string Sorun;
        public List<string> Secenekler = new List<string>();
        public List<SoruModel> AltSoruModeller = new List<SoruModel>();

        public SoruModel(int kacSecenek, string sorun, List<string> secenekler, List<SoruModel> altSoruModeller)
        {
            KacSecenek = kacSecenek;
            Sorun = sorun ?? throw new ArgumentNullException(nameof(sorun));
            Secenekler = secenekler ?? throw new ArgumentNullException(nameof(secenekler));
            AltSoruModeller = altSoruModeller ?? throw new ArgumentNullException(nameof(altSoruModeller));
        }

        public SoruModel()
        {
        }
    }
}
