using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ulesanne_keel
{
    public class Sona
    {
        public string SonaTekst { get; set; } // Слово
        public string Tolge { get; set; }     // Перевод
        public string Selgitus { get; set; }  // Объяснение
        public string Kategooria { get; set; } // Категория 

        public Sona(string sonaTekst, string tolge, string selgitus, string kategooria)
        {
            SonaTekst = sonaTekst;
            Tolge = tolge;
            Selgitus = selgitus;
            Kategooria = kategooria;
        }
    }
}
