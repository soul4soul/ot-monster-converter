using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterInterface.MonsterTypes
{
    public class BestiaryData
    {
        /// <summary>
        /// TODO: Evalute, turning this into an enum. The name and number of them may be restricted by the client
        /// </summary>
        public string Class { get; set; }
        public int CharmPoints { get; set; }
        public int FirstDetailStage { get; set; }
        public int SecondDetailStage { get; set; }
        public int FinalDetailStage { get; set; }
        public string Location { get; set; }
        /// <summary>
        /// Cip goes from 0 (harmless) to 5 (Challenging)
        /// </summary>
        public int DifficultlyStarCount { get; set; }
        /// <summary>
        /// Cip goes from 0 (common) to 3 (very rare)
        /// </summary>
        public int OccuranceDiamondCount { get; set; }
    }
}
