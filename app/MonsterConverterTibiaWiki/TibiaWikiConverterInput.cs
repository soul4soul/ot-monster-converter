using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MonsterConverterTibiaWiki
{
    public partial class TibiaWikiConverter
    {
        private static readonly IDictionary<int, LookData> hardcodedLooks = new Dictionary<int, LookData>()
        {
            {2, new LookData() { LookType = LookType.Outfit, LookId=2, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {3, new LookData() { LookType = LookType.Outfit, LookId=3, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {4, new LookData() { LookType = LookType.Outfit, LookId=4, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {5, new LookData() { LookType = LookType.Outfit, LookId=5, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {6, new LookData() { LookType = LookType.Outfit, LookId=6, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {7, new LookData() { LookType = LookType.Outfit, LookId=7, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {8, new LookData() { LookType = LookType.Outfit, LookId=8, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {9, new LookData() { LookType = LookType.Outfit, LookId=9, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {10, new LookData() { LookType = LookType.Outfit, LookId=130, Head=0, Body=52, Legs=128, Feet=95, Addons=1 } },
            {11, new LookData() { LookType = LookType.Outfit, LookId=129, Head=95, Body=116, Legs=120, Feet=115, Addons=0 } },
            {12, new LookData() { LookType = LookType.Outfit, LookId=139, Head=113, Body=38, Legs=76, Feet=96, Addons=0 } },
            {13, new LookData() { LookType = LookType.Outfit, LookId=13, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {14, new LookData() { LookType = LookType.Outfit, LookId=14, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {15, new LookData() { LookType = LookType.Outfit, LookId=15, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {16, new LookData() { LookType = LookType.Outfit, LookId=16, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {17, new LookData() { LookType = LookType.Outfit, LookId=17, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {18, new LookData() { LookType = LookType.Outfit, LookId=18, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {19, new LookData() { LookType = LookType.Outfit, LookId=19, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {20, new LookData() { LookType = LookType.Outfit, LookId=19, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {21, new LookData() { LookType = LookType.Outfit, LookId=21, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {22, new LookData() { LookType = LookType.Outfit, LookId=22, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {23, new LookData() { LookType = LookType.Outfit, LookId=23, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {24, new LookData() { LookType = LookType.Outfit, LookId=24, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {25, new LookData() { LookType = LookType.Outfit, LookId=25, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {26, new LookData() { LookType = LookType.Outfit, LookId=26, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {27, new LookData() { LookType = LookType.Outfit, LookId=27, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {28, new LookData() { LookType = LookType.Outfit, LookId=28, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {29, new LookData() { LookType = LookType.Outfit, LookId=29, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {30, new LookData() { LookType = LookType.Outfit, LookId=30, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {31, new LookData() { LookType = LookType.Outfit, LookId=31, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {32, new LookData() { LookType = LookType.Outfit, LookId=32, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {33, new LookData() { LookType = LookType.Outfit, LookId=33, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {34, new LookData() { LookType = LookType.Outfit, LookId=34, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {35, new LookData() { LookType = LookType.Outfit, LookId=35, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {36, new LookData() { LookType = LookType.Outfit, LookId=36, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {37, new LookData() { LookType = LookType.Outfit, LookId=37, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {38, new LookData() { LookType = LookType.Outfit, LookId=38, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {39, new LookData() { LookType = LookType.Outfit, LookId=39, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {40, new LookData() { LookType = LookType.Outfit, LookId=40, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {41, new LookData() { LookType = LookType.Outfit, LookId=41, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {42, new LookData() { LookType = LookType.Outfit, LookId=42, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {43, new LookData() { LookType = LookType.Outfit, LookId=43, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {44, new LookData() { LookType = LookType.Outfit, LookId=44, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {45, new LookData() { LookType = LookType.Outfit, LookId=45, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {47, new LookData() { LookType = LookType.Outfit, LookId=131, Head=38, Body=38, Legs=38, Feet=38, Addons=0 } },
            {48, new LookData() { LookType = LookType.Outfit, LookId=48, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {49, new LookData() { LookType = LookType.Outfit, LookId=49, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {50, new LookData() { LookType = LookType.Outfit, LookId=50, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {51, new LookData() { LookType = LookType.Outfit, LookId=51, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {52, new LookData() { LookType = LookType.Outfit, LookId=52, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {53, new LookData() { LookType = LookType.Outfit, LookId=53, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {54, new LookData() { LookType = LookType.Outfit, LookId=54, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {55, new LookData() { LookType = LookType.Outfit, LookId=55, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {56, new LookData() { LookType = LookType.Outfit, LookId=56, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {57, new LookData() { LookType = LookType.Outfit, LookId=57, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {58, new LookData() { LookType = LookType.Outfit, LookId=58, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {59, new LookData() { LookType = LookType.Outfit, LookId=59, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {60, new LookData() { LookType = LookType.Outfit, LookId=60, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {61, new LookData() { LookType = LookType.Outfit, LookId=61, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {62, new LookData() { LookType = LookType.Outfit, LookId=62, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {63, new LookData() { LookType = LookType.Outfit, LookId=63, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {64, new LookData() { LookType = LookType.Outfit, LookId=64, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {65, new LookData() { LookType = LookType.Outfit, LookId=65, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {66, new LookData() { LookType = LookType.Outfit, LookId=66, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {67, new LookData() { LookType = LookType.Outfit, LookId=67, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {68, new LookData() { LookType = LookType.Outfit, LookId=68, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {69, new LookData() { LookType = LookType.Outfit, LookId=69, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {70, new LookData() { LookType = LookType.Outfit, LookId=70, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {71, new LookData() { LookType = LookType.Outfit, LookId=71, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {72, new LookData() { LookType = LookType.Outfit, LookId=128, Head=97, Body=116, Legs=95, Feet=95, Addons=0 } },
            {73, new LookData() { LookType = LookType.Outfit, LookId=73, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {74, new LookData() { LookType = LookType.Outfit, LookId=74, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {76, new LookData() { LookType = LookType.Outfit, LookId=76, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {77, new LookData() { LookType = LookType.Outfit, LookId=137, Head=113, Body=120, Legs=95, Feet=115, Addons=0 } },
            {78, new LookData() { LookType = LookType.Outfit, LookId=78, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {79, new LookData() { LookType = LookType.Outfit, LookId=79, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {80, new LookData() { LookType = LookType.Outfit, LookId=80, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {81, new LookData() { LookType = LookType.Outfit, LookId=81, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {82, new LookData() { LookType = LookType.Outfit, LookId=82, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {83, new LookData() { LookType = LookType.Outfit, LookId=83, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {94, new LookData() { LookType = LookType.Outfit, LookId=94, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {95, new LookData() { LookType = LookType.Outfit, LookId=95, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {99, new LookData() { LookType = LookType.Outfit, LookId=99, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {100, new LookData() { LookType = LookType.Outfit, LookId=100, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {101, new LookData() { LookType = LookType.Outfit, LookId=101, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {103, new LookData() { LookType = LookType.Outfit, LookId=103, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {104, new LookData() { LookType = LookType.Outfit, LookId=104, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {105, new LookData() { LookType = LookType.Outfit, LookId=105, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {106, new LookData() { LookType = LookType.Outfit, LookId=106, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {108, new LookData() { LookType = LookType.Outfit, LookId=108, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {109, new LookData() { LookType = LookType.Outfit, LookId=109, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {110, new LookData() { LookType = LookType.Outfit, LookId=110, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {111, new LookData() { LookType = LookType.Outfit, LookId=111, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {112, new LookData() { LookType = LookType.Outfit, LookId=112, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {113, new LookData() { LookType = LookType.Outfit, LookId=113, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {114, new LookData() { LookType = LookType.Outfit, LookId=114, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {115, new LookData() { LookType = LookType.Outfit, LookId=115, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {116, new LookData() { LookType = LookType.Outfit, LookId=116, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {117, new LookData() { LookType = LookType.Outfit, LookId=117, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {118, new LookData() { LookType = LookType.Outfit, LookId=118, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {119, new LookData() { LookType = LookType.Outfit, LookId=119, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {120, new LookData() { LookType = LookType.Outfit, LookId=120, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {121, new LookData() { LookType = LookType.Outfit, LookId=121, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {122, new LookData() { LookType = LookType.Outfit, LookId=122, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {123, new LookData() { LookType = LookType.Outfit, LookId=123, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {124, new LookData() { LookType = LookType.Outfit, LookId=124, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {125, new LookData() { LookType = LookType.Outfit, LookId=125, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {211, new LookData() { LookType = LookType.Outfit, LookId=211, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {212, new LookData() { LookType = LookType.Outfit, LookId=212, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {213, new LookData() { LookType = LookType.Outfit, LookId=213, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {214, new LookData() { LookType = LookType.Outfit, LookId=214, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {215, new LookData() { LookType = LookType.Outfit, LookId=215, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {216, new LookData() { LookType = LookType.Outfit, LookId=216, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {217, new LookData() { LookType = LookType.Outfit, LookId=217, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {218, new LookData() { LookType = LookType.Outfit, LookId=218, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {219, new LookData() { LookType = LookType.Outfit, LookId=219, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {220, new LookData() { LookType = LookType.Outfit, LookId=220, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {221, new LookData() { LookType = LookType.Outfit, LookId=221, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {222, new LookData() { LookType = LookType.Outfit, LookId=134, Head=95, Body=0, Legs=113, Feet=115, Addons=0 } },
            {223, new LookData() { LookType = LookType.Outfit, LookId=129, Head=58, Body=40, Legs=24, Feet=95, Addons=0 } },
            {224, new LookData() { LookType = LookType.Outfit, LookId=152, Head=95, Body=95, Legs=95, Feet=95, Addons=3 } },
            {225, new LookData() { LookType = LookType.Outfit, LookId=225, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {227, new LookData() { LookType = LookType.Outfit, LookId=227, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {228, new LookData() { LookType = LookType.Outfit, LookId=228, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {236, new LookData() { LookType = LookType.Outfit, LookId=286, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {237, new LookData() { LookType = LookType.Outfit, LookId=20, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {238, new LookData() { LookType = LookType.Outfit, LookId=20, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {239, new LookData() { LookType = LookType.Outfit, LookId=46, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {240, new LookData() { LookType = LookType.Outfit, LookId=46, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {241, new LookData() { LookType = LookType.Outfit, LookId=72, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {242, new LookData() { LookType = LookType.Outfit, LookId=72, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {243, new LookData() { LookType = LookType.Outfit, LookId=47, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {244, new LookData() { LookType = LookType.Outfit, LookId=47, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {245, new LookData() { LookType = LookType.Outfit, LookId=77, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {246, new LookData() { LookType = LookType.Outfit, LookId=77, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {247, new LookData() { LookType = LookType.Outfit, LookId=93, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {248, new LookData() { LookType = LookType.Outfit, LookId=96, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {249, new LookData() { LookType = LookType.Outfit, LookId=97, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {250, new LookData() { LookType = LookType.Outfit, LookId=98, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {251, new LookData() { LookType = LookType.Outfit, LookId=192, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {252, new LookData() { LookType = LookType.Outfit, LookId=193, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {253, new LookData() { LookType = LookType.Outfit, LookId=194, Head=95, Body=100, Legs=100, Feet=19, Addons=0 } },
            {254, new LookData() { LookType = LookType.Outfit, LookId=194, Head=95, Body=94, Legs=94, Feet=19, Addons=0 } },
            {255, new LookData() { LookType = LookType.Outfit, LookId=133, Head=114, Body=114, Legs=76, Feet=114, Addons=0 } },
            {256, new LookData() { LookType = LookType.Outfit, LookId=195, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {257, new LookData() { LookType = LookType.Outfit, LookId=196, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {258, new LookData() { LookType = LookType.Outfit, LookId=197, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {259, new LookData() { LookType = LookType.Outfit, LookId=198, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {260, new LookData() { LookType = LookType.Outfit, LookId=199, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {261, new LookData() { LookType = LookType.Outfit, LookId=200, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {262, new LookData() { LookType = LookType.Outfit, LookId=222, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {264, new LookData() { LookType = LookType.Outfit, LookId=223, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {265, new LookData() { LookType = LookType.Outfit, LookId=19, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {267, new LookData() { LookType = LookType.Outfit, LookId=224, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {268, new LookData() { LookType = LookType.Outfit, LookId=226, Head=87, Body=85, Legs=85, Feet=87, Addons=0 } },
            {269, new LookData() { LookType = LookType.Outfit, LookId=226, Head=114, Body=79, Legs=78, Feet=114, Addons=0 } },
            {270, new LookData() { LookType = LookType.Outfit, LookId=226, Head=94, Body=78, Legs=77, Feet=112, Addons=0 } },
            {271, new LookData() { LookType = LookType.Outfit, LookId=226, Head=71, Body=14, Legs=71, Feet=70, Addons=0 } },
            {277, new LookData() { LookType = LookType.Outfit, LookId=282, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {279, new LookData() { LookType = LookType.Outfit, LookId=11, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {281, new LookData() { LookType = LookType.Outfit, LookId=230, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {282, new LookData() { LookType = LookType.Outfit, LookId=231, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {283, new LookData() { LookType = LookType.Outfit, LookId=232, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {284, new LookData() { LookType = LookType.Outfit, LookId=233, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {285, new LookData() { LookType = LookType.Outfit, LookId=234, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {286, new LookData() { LookType = LookType.Outfit, LookId=235, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {287, new LookData() { LookType = LookType.Outfit, LookId=236, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {288, new LookData() { LookType = LookType.Outfit, LookId=237, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {289, new LookData() { LookType = LookType.Outfit, LookId=238, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {290, new LookData() { LookType = LookType.Outfit, LookId=239, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {291, new LookData() { LookType = LookType.Outfit, LookId=149, Head=94, Body=77, Legs=78, Feet=79, Addons=1 } },
            {292, new LookData() { LookType = LookType.Outfit, LookId=241, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {294, new LookData() { LookType = LookType.Outfit, LookId=240, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {295, new LookData() { LookType = LookType.Outfit, LookId=243, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {296, new LookData() { LookType = LookType.Outfit, LookId=244, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {298, new LookData() { LookType = LookType.Outfit, LookId=246, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {299, new LookData() { LookType = LookType.Outfit, LookId=245, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {310, new LookData() { LookType = LookType.Outfit, LookId=146, Head=97, Body=39, Legs=40, Feet=3, Addons=3 } },
            {313, new LookData() { LookType = LookType.Outfit, LookId=242, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {314, new LookData() { LookType = LookType.Outfit, LookId=247, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {317, new LookData() { LookType = LookType.Outfit, LookId=248, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {318, new LookData() { LookType = LookType.Outfit, LookId=250, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {319, new LookData() { LookType = LookType.Outfit, LookId=249, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {321, new LookData() { LookType = LookType.Outfit, LookId=256, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {322, new LookData() { LookType = LookType.Outfit, LookId=254, Head=0, Body=77, Legs=96, Feet=114, Addons=0 } },
            {323, new LookData() { LookType = LookType.Outfit, LookId=255, Head=114, Body=113, Legs=132, Feet=94, Addons=0 } },
            {324, new LookData() { LookType = LookType.Outfit, LookId=257, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {325, new LookData() { LookType = LookType.Outfit, LookId=258, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {326, new LookData() { LookType = LookType.Outfit, LookId=261, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {327, new LookData() { LookType = LookType.Outfit, LookId=262, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {328, new LookData() { LookType = LookType.Outfit, LookId=259, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {329, new LookData() { LookType = LookType.Outfit, LookId=260, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {330, new LookData() { LookType = LookType.Outfit, LookId=263, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {331, new LookData() { LookType = LookType.Outfit, LookId=149, Head=0, Body=9, Legs=86, Feet=86, Addons=0 } },
            {332, new LookData() { LookType = LookType.Outfit, LookId=264, Head=78, Body=97, Legs=95, Feet=121, Addons=0 } },
            {333, new LookData() { LookType = LookType.Outfit, LookId=253, Head=115, Body=86, Legs=119, Feet=113, Addons=0 } },
            {334, new LookData() { LookType = LookType.Outfit, LookId=265, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {335, new LookData() { LookType = LookType.Outfit, LookId=250, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {371, new LookData() { LookType = LookType.Outfit, LookId=133, Head=58, Body=95, Legs=51, Feet=131, Addons=2 } },
            {372, new LookData() { LookType = LookType.Outfit, LookId=133, Head=78, Body=57, Legs=95, Feet=115, Addons=1 } },
            {376, new LookData() { LookType = LookType.Outfit, LookId=129, Head=60, Body=118, Legs=118, Feet=116, Addons=1 } },
            {377, new LookData() { LookType = LookType.Outfit, LookId=61, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {379, new LookData() { LookType = LookType.Outfit, LookId=160, Head=115, Body=77, Legs=93, Feet=114, Addons=0 } },
            {383, new LookData() { LookType = LookType.Outfit, LookId=137, Head=99, Body=41, Legs=5, Feet=102, Addons=3 } },
            {384, new LookData() { LookType = LookType.Outfit, LookId=274, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {385, new LookData() { LookType = LookType.Outfit, LookId=271, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {386, new LookData() { LookType = LookType.Outfit, LookId=272, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {387, new LookData() { LookType = LookType.Outfit, LookId=276, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {388, new LookData() { LookType = LookType.Outfit, LookId=273, Head=0, Body=0, Legs=114, Feet=0, Addons=2 } },
            {389, new LookData() { LookType = LookType.Outfit, LookId=277, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {391, new LookData() { LookType = LookType.Outfit, LookId=280, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {392, new LookData() { LookType = LookType.Outfit, LookId=281, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {393, new LookData() { LookType = LookType.Outfit, LookId=61, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {402, new LookData() { LookType = LookType.Outfit, LookId=283, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {437, new LookData() { LookType = LookType.Outfit, LookId=200, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {438, new LookData() { LookType = LookType.Outfit, LookId=275, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {439, new LookData() { LookType = LookType.Outfit, LookId=317, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {446, new LookData() { LookType = LookType.Outfit, LookId=298, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {455, new LookData() { LookType = LookType.Outfit, LookId=285, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {456, new LookData() { LookType = LookType.Outfit, LookId=290, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {457, new LookData() { LookType = LookType.Outfit, LookId=293, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {458, new LookData() { LookType = LookType.Outfit, LookId=301, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {460, new LookData() { LookType = LookType.Outfit, LookId=299, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {461, new LookData() { LookType = LookType.Outfit, LookId=291, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {462, new LookData() { LookType = LookType.Outfit, LookId=294, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {463, new LookData() { LookType = LookType.Outfit, LookId=296, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {464, new LookData() { LookType = LookType.Outfit, LookId=297, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {465, new LookData() { LookType = LookType.Outfit, LookId=300, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {483, new LookData() { LookType = LookType.Outfit, LookId=312, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {502, new LookData() { LookType = LookType.Outfit, LookId=305, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {503, new LookData() { LookType = LookType.Outfit, LookId=304, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {508, new LookData() { LookType = LookType.Outfit, LookId=306, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {509, new LookData() { LookType = LookType.Outfit, LookId=307, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {510, new LookData() { LookType = LookType.Outfit, LookId=308, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {511, new LookData() { LookType = LookType.Outfit, LookId=310, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {512, new LookData() { LookType = LookType.Outfit, LookId=311, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {513, new LookData() { LookType = LookType.Outfit, LookId=314, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {514, new LookData() { LookType = LookType.Outfit, LookId=315, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {515, new LookData() { LookType = LookType.Outfit, LookId=316, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {516, new LookData() { LookType = LookType.Outfit, LookId=318, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {518, new LookData() { LookType = LookType.Outfit, LookId=321, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {519, new LookData() { LookType = LookType.Outfit, LookId=322, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {520, new LookData() { LookType = LookType.Outfit, LookId=320, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {521, new LookData() { LookType = LookType.Outfit, LookId=323, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {523, new LookData() { LookType = LookType.Outfit, LookId=313, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {524, new LookData() { LookType = LookType.Outfit, LookId=304, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {525, new LookData() { LookType = LookType.Outfit, LookId=153, Head=40, Body=19, Legs=21, Feet=97, Addons=3 } },
            {526, new LookData() { LookType = LookType.Outfit, LookId=151, Head=114, Body=19, Legs=23, Feet=40, Addons=0 } },
            {527, new LookData() { LookType = LookType.Outfit, LookId=131, Head=78, Body=3, Legs=79, Feet=114, Addons=0 } },
            {528, new LookData() { LookType = LookType.Outfit, LookId=133, Head=39, Body=0, Legs=19, Feet=20, Addons=1 } },
            {529, new LookData() { LookType = LookType.Outfit, LookId=130, Head=78, Body=76, Legs=94, Feet=39, Addons=2 } },
            {533, new LookData() { LookType = LookType.Outfit, LookId=326, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {540, new LookData() { LookType = LookType.Outfit, LookId=281, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {541, new LookData() { LookType = LookType.Outfit, LookId=53, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {555, new LookData() { LookType = LookType.Outfit, LookId=14, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {556, new LookData() { LookType = LookType.Outfit, LookId=13, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {557, new LookData() { LookType = LookType.Outfit, LookId=32, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {558, new LookData() { LookType = LookType.Outfit, LookId=60, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {559, new LookData() { LookType = LookType.Outfit, LookId=31, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {560, new LookData() { LookType = LookType.Outfit, LookId=74, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {561, new LookData() { LookType = LookType.Outfit, LookId=111, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {562, new LookData() { LookType = LookType.Outfit, LookId=217, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {563, new LookData() { LookType = LookType.Outfit, LookId=224, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {570, new LookData() { LookType = LookType.Outfit, LookId=330, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {578, new LookData() { LookType = LookType.Outfit, LookId=9, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {579, new LookData() { LookType = LookType.Outfit, LookId=152, Head=95, Body=95, Legs=95, Feet=95, Addons=3 } },
            {580, new LookData() { LookType = LookType.Outfit, LookId=68, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {581, new LookData() { LookType = LookType.Outfit, LookId=300, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {582, new LookData() { LookType = LookType.Outfit, LookId=322, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {583, new LookData() { LookType = LookType.Outfit, LookId=194, Head=95, Body=76, Legs=95, Feet=76, Addons=0 } },
            {584, new LookData() { LookType = LookType.Outfit, LookId=315, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {585, new LookData() { LookType = LookType.Outfit, LookId=268, Head=95, Body=95, Legs=95, Feet=95, Addons=1 } },
            {586, new LookData() { LookType = LookType.Outfit, LookId=320, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {587, new LookData() { LookType = LookType.Outfit, LookId=58, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {594, new LookData() { LookType = LookType.Outfit, LookId=33, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {595, new LookData() { LookType = LookType.Outfit, LookId=18, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {614, new LookData() { LookType = LookType.Outfit, LookId=342, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {615, new LookData() { LookType = LookType.Outfit, LookId=345, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {616, new LookData() { LookType = LookType.Outfit, LookId=343, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {617, new LookData() { LookType = LookType.Outfit, LookId=334, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {618, new LookData() { LookType = LookType.Outfit, LookId=340, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {620, new LookData() { LookType = LookType.Outfit, LookId=344, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {621, new LookData() { LookType = LookType.Outfit, LookId=349, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {623, new LookData() { LookType = LookType.Outfit, LookId=339, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {624, new LookData() { LookType = LookType.Outfit, LookId=338, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {625, new LookData() { LookType = LookType.Outfit, LookId=337, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {627, new LookData() { LookType = LookType.Outfit, LookId=358, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {630, new LookData() { LookType = LookType.Outfit, LookId=341, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {631, new LookData() { LookType = LookType.Outfit, LookId=346, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {632, new LookData() { LookType = LookType.Outfit, LookId=347, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {633, new LookData() { LookType = LookType.Outfit, LookId=348, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {641, new LookData() { LookType = LookType.Outfit, LookId=350, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {643, new LookData() { LookType = LookType.Outfit, LookId=351, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {655, new LookData() { LookType = LookType.Outfit, LookId=115, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {656, new LookData() { LookType = LookType.Outfit, LookId=115, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {672, new LookData() { LookType = LookType.Outfit, LookId=362, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {673, new LookData() { LookType = LookType.Outfit, LookId=357, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {674, new LookData() { LookType = LookType.Outfit, LookId=352, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {675, new LookData() { LookType = LookType.Outfit, LookId=355, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {679, new LookData() { LookType = LookType.Outfit, LookId=310, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {680, new LookData() { LookType = LookType.Outfit, LookId=0, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {691, new LookData() { LookType = LookType.Outfit, LookId=349, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {693, new LookData() { LookType = LookType.Outfit, LookId=380, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {694, new LookData() { LookType = LookType.Outfit, LookId=381, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {695, new LookData() { LookType = LookType.Outfit, LookId=382, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {696, new LookData() { LookType = LookType.Outfit, LookId=384, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {697, new LookData() { LookType = LookType.Outfit, LookId=383, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {698, new LookData() { LookType = LookType.Outfit, LookId=385, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {700, new LookData() { LookType = LookType.Outfit, LookId=395, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {701, new LookData() { LookType = LookType.Outfit, LookId=396, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {702, new LookData() { LookType = LookType.Outfit, LookId=397, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {704, new LookData() { LookType = LookType.Outfit, LookId=94, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {705, new LookData() { LookType = LookType.Outfit, LookId=398, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {706, new LookData() { LookType = LookType.Outfit, LookId=333, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {707, new LookData() { LookType = LookType.Outfit, LookId=234, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {708, new LookData() { LookType = LookType.Outfit, LookId=100, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {709, new LookData() { LookType = LookType.Outfit, LookId=219, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {710, new LookData() { LookType = LookType.Outfit, LookId=99, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {711, new LookData() { LookType = LookType.Outfit, LookId=65, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {712, new LookData() { LookType = LookType.Outfit, LookId=298, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {717, new LookData() { LookType = LookType.Outfit, LookId=408, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {719, new LookData() { LookType = LookType.Outfit, LookId=425, Head=0, Body=0, Legs=0, Feet=0, Addons=2 } },
            {720, new LookData() { LookType = LookType.Outfit, LookId=400, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {723, new LookData() { LookType = LookType.Outfit, LookId=27, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {724, new LookData() { LookType = LookType.Outfit, LookId=417, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {725, new LookData() { LookType = LookType.Outfit, LookId=409, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {726, new LookData() { LookType = LookType.Outfit, LookId=418, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {727, new LookData() { LookType = LookType.Outfit, LookId=420, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {728, new LookData() { LookType = LookType.Outfit, LookId=410, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {729, new LookData() { LookType = LookType.Outfit, LookId=419, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {730, new LookData() { LookType = LookType.Outfit, LookId=393, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {731, new LookData() { LookType = LookType.Outfit, LookId=407, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {732, new LookData() { LookType = LookType.Outfit, LookId=403, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {733, new LookData() { LookType = LookType.Outfit, LookId=404, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {734, new LookData() { LookType = LookType.Outfit, LookId=413, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {735, new LookData() { LookType = LookType.Outfit, LookId=222, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {737, new LookData() { LookType = LookType.Outfit, LookId=159, Head=94, Body=77, Legs=78, Feet=79, Addons=0 } },
            {738, new LookData() { LookType = LookType.Outfit, LookId=412, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {739, new LookData() { LookType = LookType.Outfit, LookId=414, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {740, new LookData() { LookType = LookType.Outfit, LookId=391, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {741, new LookData() { LookType = LookType.Outfit, LookId=159, Head=21, Body=76, Legs=95, Feet=116, Addons=0 } },
            {745, new LookData() { LookType = LookType.Outfit, LookId=281, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {750, new LookData() { LookType = LookType.Outfit, LookId=434, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {751, new LookData() { LookType = LookType.Outfit, LookId=436, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {752, new LookData() { LookType = LookType.Outfit, LookId=435, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {769, new LookData() { LookType = LookType.Outfit, LookId=441, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {770, new LookData() { LookType = LookType.Outfit, LookId=442, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {771, new LookData() { LookType = LookType.Outfit, LookId=211, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {772, new LookData() { LookType = LookType.Outfit, LookId=443, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {776, new LookData() { LookType = LookType.Outfit, LookId=146, Head=104, Body=48, Legs=49, Feet=3, Addons=3 } },
            {777, new LookData() { LookType = LookType.Outfit, LookId=150, Head=96, Body=39, Legs=40, Feet=3, Addons=3 } },
            {778, new LookData() { LookType = LookType.Outfit, LookId=448, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {779, new LookData() { LookType = LookType.Outfit, LookId=449, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {780, new LookData() { LookType = LookType.Outfit, LookId=451, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {781, new LookData() { LookType = LookType.Outfit, LookId=452, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {782, new LookData() { LookType = LookType.Outfit, LookId=453, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {783, new LookData() { LookType = LookType.Outfit, LookId=454, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {784, new LookData() { LookType = LookType.Outfit, LookId=455, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {786, new LookData() { LookType = LookType.Outfit, LookId=456, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {787, new LookData() { LookType = LookType.Outfit, LookId=457, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {788, new LookData() { LookType = LookType.Outfit, LookId=458, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {790, new LookData() { LookType = LookType.Outfit, LookId=460, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {791, new LookData() { LookType = LookType.Outfit, LookId=461, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {792, new LookData() { LookType = LookType.Outfit, LookId=462, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {795, new LookData() { LookType = LookType.Outfit, LookId=470, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {796, new LookData() { LookType = LookType.Outfit, LookId=403, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {797, new LookData() { LookType = LookType.Outfit, LookId=457, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {801, new LookData() { LookType = LookType.Outfit, LookId=458, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {859, new LookData() { LookType = LookType.Outfit, LookId=470, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {860, new LookData() { LookType = LookType.Outfit, LookId=443, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {861, new LookData() { LookType = LookType.Outfit, LookId=442, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {862, new LookData() { LookType = LookType.Outfit, LookId=441, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {867, new LookData() { LookType = LookType.Outfit, LookId=146, Head=57, Body=95, Legs=57, Feet=19, Addons=3 } },
            {868, new LookData() { LookType = LookType.Outfit, LookId=146, Head=62, Body=132, Legs=42, Feet=75, Addons=3 } },
            {869, new LookData() { LookType = LookType.Outfit, LookId=511, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {870, new LookData() { LookType = LookType.Outfit, LookId=60, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {872, new LookData() { LookType = LookType.Outfit, LookId=523, Head=0, Body=0, Legs=0, Feet=0, Addons=3 } },
            {873, new LookData() { LookType = LookType.Outfit, LookId=508, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {874, new LookData() { LookType = LookType.Outfit, LookId=508, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {877, new LookData() { LookType = LookType.Outfit, LookId=515, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {878, new LookData() { LookType = LookType.Outfit, LookId=527, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {879, new LookData() { LookType = LookType.Outfit, LookId=486, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {880, new LookData() { LookType = LookType.Outfit, LookId=487, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {881, new LookData() { LookType = LookType.Outfit, LookId=488, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {882, new LookData() { LookType = LookType.Outfit, LookId=489, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {883, new LookData() { LookType = LookType.Outfit, LookId=490, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {884, new LookData() { LookType = LookType.Outfit, LookId=491, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {885, new LookData() { LookType = LookType.Outfit, LookId=492, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {886, new LookData() { LookType = LookType.Outfit, LookId=494, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {888, new LookData() { LookType = LookType.Outfit, LookId=496, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {889, new LookData() { LookType = LookType.Outfit, LookId=497, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {890, new LookData() { LookType = LookType.Outfit, LookId=498, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {891, new LookData() { LookType = LookType.Outfit, LookId=499, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {894, new LookData() { LookType = LookType.Outfit, LookId=505, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {897, new LookData() { LookType = LookType.Outfit, LookId=489, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {898, new LookData() { LookType = LookType.Outfit, LookId=509, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {899, new LookData() { LookType = LookType.Outfit, LookId=510, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {912, new LookData() { LookType = LookType.Outfit, LookId=528, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {913, new LookData() { LookType = LookType.Outfit, LookId=529, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {914, new LookData() { LookType = LookType.Outfit, LookId=530, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {915, new LookData() { LookType = LookType.Outfit, LookId=531, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {916, new LookData() { LookType = LookType.Outfit, LookId=532, Head=0, Body=78, Legs=59, Feet=0, Addons=0 } },
            {917, new LookData() { LookType = LookType.Outfit, LookId=533, Head=0, Body=76, Legs=83, Feet=0, Addons=0 } },
            {918, new LookData() { LookType = LookType.Outfit, LookId=534, Head=0, Body=19, Legs=121, Feet=0, Addons=0 } },
            {919, new LookData() { LookType = LookType.Outfit, LookId=535, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {920, new LookData() { LookType = LookType.Outfit, LookId=532, Head=0, Body=79, Legs=80, Feet=0, Addons=0 } },
            {922, new LookData() { LookType = LookType.Outfit, LookId=129, Head=93, Body=15, Legs=72, Feet=80, Addons=0 } },
            {924, new LookData() { LookType = LookType.Outfit, LookId=537, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {925, new LookData() { LookType = LookType.Outfit, LookId=538, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {926, new LookData() { LookType = LookType.Outfit, LookId=539, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {958, new LookData() { LookType = LookType.Outfit, LookId=555, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {959, new LookData() { LookType = LookType.Outfit, LookId=554, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {960, new LookData() { LookType = LookType.Outfit, LookId=551, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {961, new LookData() { LookType = LookType.Outfit, LookId=553, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {962, new LookData() { LookType = LookType.Outfit, LookId=560, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {963, new LookData() { LookType = LookType.Outfit, LookId=561, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {973, new LookData() { LookType = LookType.Outfit, LookId=556, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {974, new LookData() { LookType = LookType.Outfit, LookId=552, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {975, new LookData() { LookType = LookType.Outfit, LookId=558, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {976, new LookData() { LookType = LookType.Outfit, LookId=566, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {978, new LookData() { LookType = LookType.Outfit, LookId=550, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {979, new LookData() { LookType = LookType.Outfit, LookId=567, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {980, new LookData() { LookType = LookType.Outfit, LookId=569, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {981, new LookData() { LookType = LookType.Outfit, LookId=570, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {982, new LookData() { LookType = LookType.Outfit, LookId=573, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1000, new LookData() { LookType = LookType.Outfit, LookId=49, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1004, new LookData() { LookType = LookType.Outfit, LookId=579, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1012, new LookData() { LookType = LookType.Outfit, LookId=583, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1013, new LookData() { LookType = LookType.Outfit, LookId=584, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1014, new LookData() { LookType = LookType.Outfit, LookId=585, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1015, new LookData() { LookType = LookType.Outfit, LookId=586, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1016, new LookData() { LookType = LookType.Outfit, LookId=587, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1018, new LookData() { LookType = LookType.Outfit, LookId=588, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1019, new LookData() { LookType = LookType.Outfit, LookId=590, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1021, new LookData() { LookType = LookType.Outfit, LookId=593, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1022, new LookData() { LookType = LookType.Outfit, LookId=594, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1038, new LookData() { LookType = LookType.Outfit, LookId=600, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1039, new LookData() { LookType = LookType.Outfit, LookId=601, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1040, new LookData() { LookType = LookType.Outfit, LookId=602, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1041, new LookData() { LookType = LookType.Outfit, LookId=603, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1042, new LookData() { LookType = LookType.Outfit, LookId=604, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1043, new LookData() { LookType = LookType.Outfit, LookId=605, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1044, new LookData() { LookType = LookType.Outfit, LookId=607, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1045, new LookData() { LookType = LookType.Outfit, LookId=608, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1046, new LookData() { LookType = LookType.Outfit, LookId=609, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1051, new LookData() { LookType = LookType.Outfit, LookId=611, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1052, new LookData() { LookType = LookType.Outfit, LookId=612, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1053, new LookData() { LookType = LookType.Outfit, LookId=613, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1054, new LookData() { LookType = LookType.Outfit, LookId=614, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1055, new LookData() { LookType = LookType.Outfit, LookId=615, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1056, new LookData() { LookType = LookType.Outfit, LookId=617, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1096, new LookData() { LookType = LookType.Outfit, LookId=675, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1097, new LookData() { LookType = LookType.Outfit, LookId=46, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1098, new LookData() { LookType = LookType.Outfit, LookId=47, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1099, new LookData() { LookType = LookType.Outfit, LookId=72, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1100, new LookData() { LookType = LookType.Outfit, LookId=77, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1101, new LookData() { LookType = LookType.Outfit, LookId=20, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1105, new LookData() { LookType = LookType.Outfit, LookId=451, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1109, new LookData() { LookType = LookType.Outfit, LookId=29, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1116, new LookData() { LookType = LookType.Outfit, LookId=293, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1118, new LookData() { LookType = LookType.Outfit, LookId=570, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1119, new LookData() { LookType = LookType.Outfit, LookId=129, Head=115, Body=80, Legs=114, Feet=114, Addons=0 } },
            {1120, new LookData() { LookType = LookType.Outfit, LookId=137, Head=114, Body=114, Legs=110, Feet=114, Addons=0 } },
            {1121, new LookData() { LookType = LookType.Outfit, LookId=242, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1134, new LookData() { LookType = LookType.Outfit, LookId=150, Head=114, Body=94, Legs=78, Feet=79, Addons=1 } },
            {1135, new LookData() { LookType = LookType.Outfit, LookId=150, Head=0, Body=114, Legs=90, Feet=90, Addons=1 } },
            {1137, new LookData() { LookType = LookType.Outfit, LookId=712, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1138, new LookData() { LookType = LookType.Outfit, LookId=714, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1139, new LookData() { LookType = LookType.Outfit, LookId=716, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1141, new LookData() { LookType = LookType.Outfit, LookId=717, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1142, new LookData() { LookType = LookType.Outfit, LookId=720, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1143, new LookData() { LookType = LookType.Outfit, LookId=721, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1144, new LookData() { LookType = LookType.Outfit, LookId=729, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1145, new LookData() { LookType = LookType.Outfit, LookId=131, Head=97, Body=24, Legs=73, Feet=116, Addons=1 } },
            {1146, new LookData() { LookType = LookType.Outfit, LookId=268, Head=97, Body=113, Legs=76, Feet=98, Addons=2 } },
            {1147, new LookData() { LookType = LookType.Outfit, LookId=268, Head=59, Body=19, Legs=95, Feet=94, Addons=1 } },
            {1148, new LookData() { LookType = LookType.Outfit, LookId=730, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1157, new LookData() { LookType = LookType.Outfit, LookId=569, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1161, new LookData() { LookType = LookType.Outfit, LookId=857, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1162, new LookData() { LookType = LookType.Outfit, LookId=858, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1163, new LookData() { LookType = LookType.Outfit, LookId=859, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1174, new LookData() { LookType = LookType.Outfit, LookId=860, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1196, new LookData() { LookType = LookType.Outfit, LookId=855, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {1197, new LookData() { LookType = LookType.Outfit, LookId=854, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1198, new LookData() { LookType = LookType.Outfit, LookId=856, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1224, new LookData() { LookType = LookType.Outfit, LookId=879, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1234, new LookData() { LookType = LookType.Outfit, LookId=877, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1235, new LookData() { LookType = LookType.Outfit, LookId=878, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1260, new LookData() { LookType = LookType.Outfit, LookId=882, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1264, new LookData() { LookType = LookType.Outfit, LookId=877, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1265, new LookData() { LookType = LookType.Outfit, LookId=878, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1266, new LookData() { LookType = LookType.Outfit, LookId=879, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1267, new LookData() { LookType = LookType.Outfit, LookId=882, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1307, new LookData() { LookType = LookType.Outfit, LookId=217, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1314, new LookData() { LookType = LookType.Outfit, LookId=934, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1320, new LookData() { LookType = LookType.Outfit, LookId=935, Head=94, Body=1, Legs=80, Feet=94, Addons=0 } },
            {1321, new LookData() { LookType = LookType.Outfit, LookId=932, Head=94, Body=76, Legs=0, Feet=82, Addons=1 } },
            {1322, new LookData() { LookType = LookType.Outfit, LookId=932, Head=105, Body=0, Legs=0, Feet=94, Addons=1 } },
            {1325, new LookData() { LookType = LookType.Outfit, LookId=395, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1326, new LookData() { LookType = LookType.Outfit, LookId=397, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1327, new LookData() { LookType = LookType.Outfit, LookId=396, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1376, new LookData() { LookType = LookType.Outfit, LookId=231, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1380, new LookData() { LookType = LookType.Outfit, LookId=947, Head=0, Body=9, Legs=0, Feet=0, Addons=0 } },
            {1394, new LookData() { LookType = LookType.Outfit, LookId=933, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1395, new LookData() { LookType = LookType.Outfit, LookId=936, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1412, new LookData() { LookType = LookType.Outfit, LookId=159, Head=58, Body=21, Legs=41, Feet=76, Addons=0 } },
            {1413, new LookData() { LookType = LookType.Outfit, LookId=684, Head=58, Body=40, Legs=60, Feet=116, Addons=0 } },
            {1415, new LookData() { LookType = LookType.Outfit, LookId=976, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1434, new LookData() { LookType = LookType.Outfit, LookId=980, Head=61, Body=96, Legs=95, Feet=62, Addons=0 } },
            {1435, new LookData() { LookType = LookType.Outfit, LookId=977, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1436, new LookData() { LookType = LookType.Outfit, LookId=978, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1437, new LookData() { LookType = LookType.Outfit, LookId=138, Head=0, Body=0, Legs=114, Feet=78, Addons=0 } },
            {1438, new LookData() { LookType = LookType.Outfit, LookId=982, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1439, new LookData() { LookType = LookType.Outfit, LookId=981, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1442, new LookData() { LookType = LookType.Outfit, LookId=594, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1443, new LookData() { LookType = LookType.Outfit, LookId=585, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1481, new LookData() { LookType = LookType.Outfit, LookId=132, Head=114, Body=79, Legs=62, Feet=94, Addons=2 } },
            {1482, new LookData() { LookType = LookType.Outfit, LookId=140, Head=114, Body=79, Legs=62, Feet=94, Addons=2 } },
            {1485, new LookData() { LookType = LookType.Outfit, LookId=989, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1486, new LookData() { LookType = LookType.Outfit, LookId=990, Head=0, Body=0, Legs=0, Feet=0, Addons=2 } },
            {1488, new LookData() { LookType = LookType.Outfit, LookId=990, Head=0, Body=0, Legs=0, Feet=0, Addons=2 } },
            {1496, new LookData() { LookType = LookType.Outfit, LookId=980, Head=94, Body=95, Legs=0, Feet=94, Addons=0 } },
            {1503, new LookData() { LookType = LookType.Outfit, LookId=7, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1504, new LookData() { LookType = LookType.Outfit, LookId=6, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1505, new LookData() { LookType = LookType.Outfit, LookId=8, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1506, new LookData() { LookType = LookType.Outfit, LookId=59, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1507, new LookData() { LookType = LookType.Outfit, LookId=50, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1508, new LookData() { LookType = LookType.Outfit, LookId=25, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1509, new LookData() { LookType = LookType.Outfit, LookId=23, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1510, new LookData() { LookType = LookType.Outfit, LookId=29, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1512, new LookData() { LookType = LookType.Outfit, LookId=132, Head=98, Body=96, Legs=39, Feet=38, Addons=1 } },
            {1513, new LookData() { LookType = LookType.Outfit, LookId=134, Head=95, Body=19, Legs=57, Feet=76, Addons=1 } },
            {1514, new LookData() { LookType = LookType.Outfit, LookId=145, Head=19, Body=77, Legs=3, Feet=20, Addons=1 } },
            {1525, new LookData() { LookType = LookType.Outfit, LookId=1032, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1529, new LookData() { LookType = LookType.Outfit, LookId=537, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1531, new LookData() { LookType = LookType.Outfit, LookId=1033, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1532, new LookData() { LookType = LookType.Outfit, LookId=1034, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1544, new LookData() { LookType = LookType.Outfit, LookId=1036, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1545, new LookData() { LookType = LookType.Outfit, LookId=1035, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1546, new LookData() { LookType = LookType.Outfit, LookId=1037, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1548, new LookData() { LookType = LookType.Outfit, LookId=1029, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1549, new LookData() { LookType = LookType.Outfit, LookId=1030, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1563, new LookData() { LookType = LookType.Outfit, LookId=1041, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1569, new LookData() { LookType = LookType.Outfit, LookId=1041, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1570, new LookData() { LookType = LookType.Outfit, LookId=1048, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1619, new LookData() { LookType = LookType.Outfit, LookId=150, Head=0, Body=0, Legs=0, Feet=86, Addons=3 } },
            {1620, new LookData() { LookType = LookType.Outfit, LookId=1068, Head=114, Body=94, Legs=79, Feet=121, Addons=1 } },
            {1621, new LookData() { LookType = LookType.Outfit, LookId=1068, Head=0, Body=76, Legs=53, Feet=0, Addons=1 } },
            {1622, new LookData() { LookType = LookType.Outfit, LookId=1068, Head=9, Body=0, Legs=86, Feet=9, Addons=0 } },
            {1626, new LookData() { LookType = LookType.Outfit, LookId=980, Head=85, Body=0, Legs=0, Feet=85, Addons=0 } },
            {1637, new LookData() { LookType = LookType.Outfit, LookId=1063, Head=113, Body=94, Legs=78, Feet=78, Addons=0 } },
            {1646, new LookData() { LookType = LookType.Outfit, LookId=1071, Head=57, Body=96, Legs=38, Feet=105, Addons=1 } },
            {1647, new LookData() { LookType = LookType.Outfit, LookId=1071, Head=57, Body=96, Legs=38, Feet=105, Addons=2 } },
            {1653, new LookData() { LookType = LookType.Outfit, LookId=1059, Head=17, Body=41, Legs=77, Feet=57, Addons=0 } },
            {1654, new LookData() { LookType = LookType.Outfit, LookId=1060, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1655, new LookData() { LookType = LookType.Outfit, LookId=1061, Head=79, Body=81, Legs=93, Feet=0, Addons=0 } },
            {1656, new LookData() { LookType = LookType.Outfit, LookId=1066, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1658, new LookData() { LookType = LookType.Outfit, LookId=1064, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1659, new LookData() { LookType = LookType.Outfit, LookId=1063, Head=92, Body=52, Legs=0, Feet=79, Addons=3 } },
            {1663, new LookData() { LookType = LookType.Outfit, LookId=1061, Head=79, Body=113, Legs=78, Feet=112, Addons=0 } },
            {1664, new LookData() { LookType = LookType.Outfit, LookId=1061, Head=87, Body=85, Legs=79, Feet=0, Addons=0 } },
            {1665, new LookData() { LookType = LookType.Outfit, LookId=1061, Head=15, Body=91, Legs=85, Feet=0, Addons=0 } },
            {1666, new LookData() { LookType = LookType.Outfit, LookId=1063, Head=86, Body=85, Legs=82, Feet=93, Addons=3 } },
            {1667, new LookData() { LookType = LookType.Outfit, LookId=1073, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1668, new LookData() { LookType = LookType.Outfit, LookId=1059, Head=94, Body=78, Legs=79, Feet=57, Addons=0 } },
            {1669, new LookData() { LookType = LookType.Outfit, LookId=1059, Head=9, Body=21, Legs=3, Feet=57, Addons=0 } },
            {1670, new LookData() { LookType = LookType.Outfit, LookId=1065, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1671, new LookData() { LookType = LookType.Outfit, LookId=1058, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1674, new LookData() { LookType = LookType.Outfit, LookId=298, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1675, new LookData() { LookType = LookType.Outfit, LookId=306, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1677, new LookData() { LookType = LookType.Outfit, LookId=1088, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1721, new LookData() { LookType = LookType.Outfit, LookId=1133, Head=0, Body=59, Legs=67, Feet=85, Addons=0 } },
            {1722, new LookData() { LookType = LookType.Outfit, LookId=1139, Head=79, Body=121, Legs=23, Feet=86, Addons=1 } },
            {1723, new LookData() { LookType = LookType.Outfit, LookId=1138, Head=86, Body=51, Legs=83, Feet=91, Addons=3 } },
            {1724, new LookData() { LookType = LookType.Outfit, LookId=1122, Head=81, Body=78, Legs=61, Feet=94, Addons=0 } },
            {1725, new LookData() { LookType = LookType.Outfit, LookId=1122, Head=94, Body=21, Legs=77, Feet=78, Addons=0 } },
            {1726, new LookData() { LookType = LookType.Outfit, LookId=1122, Head=9, Body=10, Legs=86, Feet=79, Addons=0 } },
            {1728, new LookData() { LookType = LookType.Outfit, LookId=1134, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1729, new LookData() { LookType = LookType.Outfit, LookId=1135, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1730, new LookData() { LookType = LookType.Outfit, LookId=1137, Head=8, Body=67, Legs=8, Feet=1, Addons=1 } },
            {1731, new LookData() { LookType = LookType.Outfit, LookId=1136, Head=47, Body=7, Legs=0, Feet=85, Addons=0 } },
            {1732, new LookData() { LookType = LookType.Outfit, LookId=1137, Head=114, Body=93, Legs=3, Feet=83, Addons=1 } },
            {1733, new LookData() { LookType = LookType.Outfit, LookId=1136, Head=114, Body=94, Legs=3, Feet=121, Addons=0 } },
            {1734, new LookData() { LookType = LookType.Outfit, LookId=1137, Head=85, Body=10, Legs=16, Feet=83, Addons=3 } },
            {1735, new LookData() { LookType = LookType.Outfit, LookId=1136, Head=72, Body=94, Legs=79, Feet=4, Addons=3 } },
            {1736, new LookData() { LookType = LookType.Outfit, LookId=1148, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1737, new LookData() { LookType = LookType.Outfit, LookId=1149, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1740, new LookData() { LookType = LookType.Outfit, LookId=1161, Head=95, Body=42, Legs=21, Feet=20, Addons=1 } },
            {1741, new LookData() { LookType = LookType.Outfit, LookId=1162, Head=0, Body=10, Legs=38, Feet=57, Addons=2 } },
            {1742, new LookData() { LookType = LookType.Outfit, LookId=1157, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1751, new LookData() { LookType = LookType.Outfit, LookId=1159, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1775, new LookData() { LookType = LookType.Outfit, LookId=1217, Head=2, Body=2, Legs=77, Feet=19, Addons=1 } },
            {1776, new LookData() { LookType = LookType.Outfit, LookId=1217, Head=1, Body=1, Legs=102, Feet=78, Addons=2 } },
            {1798, new LookData() { LookType = LookType.Outfit, LookId=541, Head=95, Body=113, Legs=3, Feet=3, Addons=1 } },
            {1799, new LookData() { LookType = LookType.Outfit, LookId=1199, Head=95, Body=78, Legs=94, Feet=3, Addons=0 } },
            {1800, new LookData() { LookType = LookType.Outfit, LookId=1200, Head=95, Body=95, Legs=94, Feet=95, Addons=0 } },
            {1805, new LookData() { LookType = LookType.Outfit, LookId=1190, Head=41, Body=38, Legs=0, Feet=0, Addons=0 } },
            {1806, new LookData() { LookType = LookType.Outfit, LookId=1190, Head=50, Body=2, Legs=0, Feet=76, Addons=0 } },
            {1807, new LookData() { LookType = LookType.Outfit, LookId=1188, Head=76, Body=75, Legs=57, Feet=0, Addons=2 } },
            {1808, new LookData() { LookType = LookType.Outfit, LookId=1188, Head=0, Body=39, Legs=0, Feet=3, Addons=1 } },
            {1816, new LookData() { LookType = LookType.Outfit, LookId=1189, Head=116, Body=97, Legs=113, Feet=20, Addons=1 } },
            {1817, new LookData() { LookType = LookType.Outfit, LookId=1196, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {1818, new LookData() { LookType = LookType.Outfit, LookId=1195, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {1819, new LookData() { LookType = LookType.Outfit, LookId=1220, Head=0, Body=0, Legs=0, Feet=0, Addons=1 } },
            {1820, new LookData() { LookType = LookType.Outfit, LookId=1212, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1821, new LookData() { LookType = LookType.Outfit, LookId=1213, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1822, new LookData() { LookType = LookType.Outfit, LookId=1214, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1824, new LookData() { LookType = LookType.Outfit, LookId=1217, Head=19, Body=19, Legs=67, Feet=78, Addons=0 } },
            {1841, new LookData() { LookType = LookType.Outfit, LookId=1255, Head=79, Body=6, Legs=94, Feet=2, Addons=0 } },
            {1855, new LookData() { LookType = LookType.Outfit, LookId=1256, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1856, new LookData() { LookType = LookType.Outfit, LookId=1253, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1857, new LookData() { LookType = LookType.Outfit, LookId=1255, Head=79, Body=6, Legs=94, Feet=2, Addons=1 } },
            {1858, new LookData() { LookType = LookType.Outfit, LookId=1255, Head=79, Body=6, Legs=94, Feet=2, Addons=2 } },
            {1864, new LookData() { LookType = LookType.Outfit, LookId=1268, Head=0, Body=6, Legs=0, Feet=116, Addons=0 } },
            {1865, new LookData() { LookType = LookType.Outfit, LookId=1268, Head=0, Body=14, Legs=0, Feet=83, Addons=0 } },
            {1866, new LookData() { LookType = LookType.Outfit, LookId=1268, Head=0, Body=74, Legs=0, Feet=83, Addons=0 } },
            {1880, new LookData() { LookType = LookType.Outfit, LookId=1268, Head=0, Body=19, Legs=0, Feet=38, Addons=0 } },
            {1885, new LookData() { LookType = LookType.Outfit, LookId=1268, Head=0, Body=14, Legs=0, Feet=34, Addons=0 } },
            {1926, new LookData() { LookType = LookType.Outfit, LookId=1294, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1927, new LookData() { LookType = LookType.Outfit, LookId=1296, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1928, new LookData() { LookType = LookType.Outfit, LookId=1295, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1929, new LookData() { LookType = LookType.Outfit, LookId=1298, Head=85, Body=85, Legs=88, Feet=91, Addons=0 } },
            {1930, new LookData() { LookType = LookType.Outfit, LookId=1299, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1931, new LookData() { LookType = LookType.Outfit, LookId=1297, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1932, new LookData() { LookType = LookType.Outfit, LookId=1298, Head=81, Body=114, Legs=85, Feet=83, Addons=0 } },
            {1933, new LookData() { LookType = LookType.Outfit, LookId=1298, Head=114, Body=80, Legs=94, Feet=78, Addons=0 } },
            {1938, new LookData() { LookType = LookType.Outfit, LookId=1313, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1939, new LookData() { LookType = LookType.Outfit, LookId=1312, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1940, new LookData() { LookType = LookType.Outfit, LookId=1314, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1941, new LookData() { LookType = LookType.Outfit, LookId=1315, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1945, new LookData() { LookType = LookType.Outfit, LookId=1298, Head=106, Body=60, Legs=131, Feet=116, Addons=0 } },
            {1946, new LookData() { LookType = LookType.Outfit, LookId=148, Head=114, Body=48, Legs=114, Feet=95, Addons=0 } },
            {1947, new LookData() { LookType = LookType.Outfit, LookId=131, Head=19, Body=76, Legs=74, Feet=114, Addons=0 } },
            {1948, new LookData() { LookType = LookType.Outfit, LookId=129, Head=57, Body=42, Legs=114, Feet=114, Addons=0 } },
            {1949, new LookData() { LookType = LookType.Outfit, LookId=138, Head=95, Body=114, Legs=52, Feet=76, Addons=0 } },
            {1962, new LookData() { LookType = LookType.Outfit, LookId=1298, Head=113, Body=94, Legs=132, Feet=76, Addons=0 } },
            {1963, new LookData() { LookType = LookType.Outfit, LookId=1300, Head=57, Body=77, Legs=1, Feet=1, Addons=0 } },
            {1964, new LookData() { LookType = LookType.Outfit, LookId=1300, Head=0, Body=0, Legs=94, Feet=95, Addons=0 } },
            {1965, new LookData() { LookType = LookType.Outfit, LookId=1301, Head=58, Body=2, Legs=94, Feet=10, Addons=3 } },
            {1966, new LookData() { LookType = LookType.Outfit, LookId=1301, Head=0, Body=2, Legs=0, Feet=94, Addons=0 } },
            {1967, new LookData() { LookType = LookType.Outfit, LookId=1290, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1972, new LookData() { LookType = LookType.Outfit, LookId=1316, Head=76, Body=57, Legs=76, Feet=95, Addons=1 } },
            {1973, new LookData() { LookType = LookType.Outfit, LookId=1316, Head=76, Body=57, Legs=76, Feet=95, Addons=2 } },
            {1974, new LookData() { LookType = LookType.Outfit, LookId=1316, Head=57, Body=2, Legs=21, Feet=95, Addons=0 } },
            {1979, new LookData() { LookType = LookType.Outfit, LookId=111, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {1995, new LookData() { LookType = LookType.Outfit, LookId=298, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2024, new LookData() { LookType = LookType.Outfit, LookId=1344, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2036, new LookData() { LookType = LookType.Outfit, LookId=1346, Head=2, Body=96, Legs=78, Feet=96, Addons=1 } },
            {2037, new LookData() { LookType = LookType.Outfit, LookId=1346, Head=97, Body=119, Legs=80, Feet=80, Addons=0 } },
            {2038, new LookData() { LookType = LookType.Outfit, LookId=1346, Head=57, Body=125, Legs=86, Feet=67, Addons=2 } },
            {2039, new LookData() { LookType = LookType.Outfit, LookId=1346, Head=0, Body=95, Legs=95, Feet=113, Addons=3 } },
            {2051, new LookData() { LookType = LookType.Outfit, LookId=1373, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2088, new LookData() { LookType = LookType.Outfit, LookId=1394, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2089, new LookData() { LookType = LookType.Outfit, LookId=1396, Head=60, Body=84, Legs=40, Feet=94, Addons=3 } },
            {2090, new LookData() { LookType = LookType.Outfit, LookId=1397, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2091, new LookData() { LookType = LookType.Outfit, LookId=1398, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2092, new LookData() { LookType = LookType.Outfit, LookId=1399, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2093, new LookData() { LookType = LookType.Outfit, LookId=1401, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2094, new LookData() { LookType = LookType.Outfit, LookId=1403, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2095, new LookData() { LookType = LookType.Outfit, LookId=1405, Head=0, Body=0, Legs=0, Feet=0, Addons=0 } },
            {2096, new LookData() { LookType = LookType.Outfit, LookId=1406, Head=60, Body=77, Legs=64, Feet=70, Addons=3 } },
            {2098, new LookData() { LookType = LookType.Outfit, LookId=1407, Head=38, Body=58, Legs=114, Feet=2, Addons=3 } },
            {2099, new LookData() { LookType = LookType.Outfit, LookId=1407, Head=114, Body=39, Legs=113, Feet=114, Addons=1 } },
            {2100, new LookData() { LookType = LookType.Outfit, LookId=1408, Head=0, Body=50, Legs=42, Feet=79, Addons=3 } },
            {2101, new LookData() { LookType = LookType.Outfit, LookId=1408, Head=0, Body=112, Legs=3, Feet=79, Addons=0 } },
            {2107, new LookData() { LookType = LookType.Outfit, LookId=1418, Head=21, Body=3, Legs=20, Feet=57, Addons=0 } },
            {2108, new LookData() { LookType = LookType.Outfit, LookId=1418, Head=23, Body=98, Legs=22, Feet=61, Addons=1 } },
            {2109, new LookData() { LookType = LookType.Outfit, LookId=1418, Head=76, Body=57, Legs=0, Feet=19, Addons=2 } }
        };

        public override ConvertResultEventArgs ReadMonster(string filename, out Monster monster)
        {
            ConvertResultEventArgs result = new ConvertResultEventArgs(filename, ConvertError.Warning, "Corpses id missing, blood type guessed, and limited ability parsing.");

            string monsterurl = $" https://tibia.fandom.com/api.php?action=parse&format=json&page={filename}&prop=wikitext";

            int intVal;
            bool boolVal;

            var monsterPage = RequestData(monsterurl).Result;
            // Replace html on page with nothing, the data inside the tags isn't needed and the template parser can choke on it
            var textWithoutHtml = Regex.Replace(monsterPage.Wikitext.Empty, @"<([A-z0-9]*)\b[^>]*>.*?<\/\1>", "", RegexOptions.Singleline | RegexOptions.Compiled);

            InfoboxCreatureTemplate creature = TemplateParser.Deserialize<InfoboxCreatureTemplate>(textWithoutHtml);
            monster = new Monster();
            monster.FileName = filename;
            if (!string.IsNullOrWhiteSpace(creature.name)) { monster.RegisteredName = creature.name; }
            if (!string.IsNullOrWhiteSpace(creature.actualname)) { monster.Name = creature.actualname; }
            if (!string.IsNullOrWhiteSpace(creature.article)) { ParseArticle(monster, creature.article); }
            if (RobustTryParse(creature.hp, out intVal)) { monster.Health = intVal; }
            if (RobustTryParse(creature.exp, out intVal)) { monster.Experience = intVal; }
            if (RobustTryParse(creature.armor, out intVal)) { monster.TotalArmor = monster.Shielding = intVal; }
            if (RobustTryParse(creature.speed, out intVal)) { monster.Speed = intVal * 2; }
            if (!string.IsNullOrWhiteSpace(creature.runsat)) { ParseRunAt(monster, creature.runsat); }
            if (RobustTryParse(creature.summon, out intVal)) { monster.SummonCost = intVal; }
            if (RobustTryParse(creature.convince, out intVal)) { monster.ConvinceCost = intVal; }
            if (RobustTryParse(creature.illusionable, out boolVal)) { monster.IsIllusionable = boolVal; }
            if (RobustTryParse(creature.isboss, out boolVal)) { monster.IsBoss = boolVal; }
            if (!string.IsNullOrWhiteSpace(creature.primarytype)) { monster.HideHealth = creature.primarytype.ToLower().Contains("trap"); }
            if (!string.IsNullOrWhiteSpace(creature.bestiaryclass)) { monster.Bestiary.Class = creature.bestiaryclass; }
            if (!string.IsNullOrWhiteSpace(creature.bestiarylevel) && !string.IsNullOrWhiteSpace(creature.occurrence))
            {
                ParseBestiaryLevel(monster, creature.bestiarylevel, creature.occurrence);
            }
            if (!string.IsNullOrWhiteSpace(creature.attacktype)) { monster.TargetDistance = creature.attacktype.ToLower().Contains("distance") ? 4 : 1; }
            if (!string.IsNullOrWhiteSpace(creature.spawntype)) { monster.IgnoreSpawnBlock = creature.spawntype.ToLower().Contains("unblockable"); }
            if (RobustTryParse(creature.pushable, out boolVal)) { monster.IsPushable = boolVal; }
            // In cipbia ability to push objects means ability to push creatures too
            if (RobustTryParse(creature.pushobjects, out boolVal)) { monster.PushItems = monster.PushCreatures = boolVal; }
            if (RobustTryParse(creature.senseinvis, out boolVal)) { monster.IgnoreInvisible = boolVal; }
            if (RobustTryParse(creature.paraimmune, out boolVal)) { monster.IgnoreParalyze = boolVal; }
            if (!string.IsNullOrWhiteSpace(creature.walksaround)) { ParseWalksAround(monster, creature.walksaround); }
            if (!string.IsNullOrWhiteSpace(creature.walksthrough)) { ParseWalksThrough(monster, creature.walksthrough); }
            if (RobustTryParse(creature.physicaldmgmod, out intVal)) { monster.PhysicalDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.earthdmgmod, out intVal)) { monster.EarthDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.firedmgmod, out intVal)) { monster.FireDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.deathdmgmod, out intVal)) { monster.DeathDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.energydmgmod, out intVal)) { monster.EnergyDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.holydmgmod, out intVal)) { monster.HolyDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.icedmgmod, out intVal)) { monster.IceDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.healmod, out intVal)) { monster.HealingMod = intVal / 100.0; }
            if (RobustTryParse(creature.lifedraindmgmod, out intVal)) { monster.LifeDrainDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.drowndmgmod, out intVal)) { monster.DrownDmgMod = intVal / 100.0; }
            if (RobustTryParse(creature.raceid, out intVal))
            {
                monster.RaceId = intVal;
                if (hardcodedLooks.ContainsKey(monster.RaceId))
                {
                    monster.Look.CopyFrom(hardcodedLooks[monster.RaceId]);
                }
                else
                {
                    result.IncreaseError(ConvertError.Warning);
                    result.AppendMessage("Look type not found");
                }
            }
            if (!string.IsNullOrWhiteSpace(creature.sounds)) { ParseSoundList(monster, creature.sounds); }
            if (!string.IsNullOrWhiteSpace(creature.abilities)) { ParseAbilities(monster, creature.abilities, result); }
            if (!string.IsNullOrWhiteSpace(creature.location)) { monster.Bestiary.Location = creature.location; }
            if (!string.IsNullOrWhiteSpace(creature.loot)) { ParseLoot(monster, creature.loot, filename, result); }

            if (creature.bestiaryclass != null && creature.bestiaryclass.Equals("undead", StringComparison.OrdinalIgnoreCase))
            {
                monster.Race = Blood.undead;
            }
            else if (creature.bestiaryclass != null && creature.bestiaryclass.Equals("construct", StringComparison.OrdinalIgnoreCase))
            {
                monster.Race = Blood.undead;
            }

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (string.IsNullOrWhiteSpace(monster.Name) && !string.IsNullOrWhiteSpace(monster.FileName))
            {
                // Better then nothing guess
                result.AppendMessage("Guessed creature name");
                monster.Name = monster.FileName;
            }
            monster.Name = textInfo.ToTitleCase(monster.Name);

            return result;
        }

        /// <summary>
        /// Boss monsters and single appear monsters don't use article
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="m"></param>
        private static void ParseArticle(Monster mon, string article)
        {
            if (string.IsNullOrWhiteSpace(article))
            {
                mon.Description = mon.Name;
            }
            else
            {
                mon.Description = string.Format("{0} {1}", article.ToLower(), mon.Name).Trim();
            }
        }

        private static void ParseWalksAround(Monster mon, string s)
        {
            foreach (string field in s.ToLower().Split(","))
            {
                string fieldtrim = field.Trim();
                if (fieldtrim == "fire")
                {
                    mon.AvoidFire = true;
                }
                else if (fieldtrim == "energy")
                {
                    mon.AvoidEnergy = true;
                }
                else if (fieldtrim == "poison")
                {
                    mon.AvoidPoison = true;
                }
            }
        }

        private static void ParseWalksThrough(Monster mon, string s)
        {
            foreach (string field in s.ToLower().Split(","))
            {
                string fieldtrim = field.Trim();
                if (fieldtrim == "fire")
                {
                    mon.AvoidFire = false;
                }
                else if (fieldtrim == "energy")
                {
                    mon.AvoidEnergy = false;
                }
                else if (fieldtrim == "poison")
                {
                    mon.AvoidPoison = false;
                }
            }
        }

        private static void ParseSoundList(Monster mon, string sounds)
        {
            if (TemplateParser.IsTemplateMatch<SoundListTemplate>(sounds))
            {
                var soundTemplated = TemplateParser.Deserialize<SoundListTemplate>(sounds);
                if (soundTemplated.sounds != null)
                {
                    foreach (string sound in soundTemplated.sounds)
                    {
                        // Sometimes sound templates include a single empty sound {{SoundList|}}
                        if (!string.IsNullOrWhiteSpace(sound))
                        {
                            // TibiaWiki doesn't include sound level information, default to say
                            mon.Voices.Add(new Voice(sound, SoundLevel.Say));
                        }
                    }
                }
            }
        }

        private static void ParseAbilities(Monster mon, string abilities, ConvertResultEventArgs result)
        {
            if (TemplateParser.IsTemplateMatch<AbilityListTemplate>(abilities))
                ParseAbilityList(mon, abilities, result);
            else
                ParseLegacyAbilities(mon, abilities, result);
        }

        /// <summary>
        /// Historically abilities were a comma seperate list of losely conforming english
        /// The TibiaWiki admins have replaced this format with a standard template.
        /// Eventually everything will switch to the ability list template and this function can be removed
        /// </summary>
        /// <param name="mon"></param>
        /// <param name="abilities"></param>
        private static void ParseLegacyAbilities(Monster mon, string abilities, ConvertResultEventArgs result)
        {
            abilities = abilities.ToLower().Replace("\r", "").Replace("\n", "").Trim();
            if (string.IsNullOrWhiteSpace(abilities) || abilities.Contains("none") || abilities.Contains("unknown") || abilities == "?")
                return;

            // Generally we find each ability is seperated by a comma expect those inside ()'s
            var splitAbilities = abilities.SplitTopLevel(',');

            // Due our best to parse the none standard ability information
            foreach (string ability in splitAbilities)
            {
                string cleanedAbility = ability.Trim().TrimEnd('.');
                switch (cleanedAbility)
                {
                    case var _ when new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[melee\]\](\s*\((?<damage>[0-9- ]+))?").Match(cleanedAbility);
                            var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                            if (TryParseRange(match.Groups["damage"].Value, out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    // Effect might need to be optional
                    case var _ when new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[distance fighting\|(?<effect>[a-z ]+)\]\]s?\s*\((?<damage>[0-9- ]+)(\+?~)?\)").Match(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Offensive, DamageElement = CombatDamage.Physical, Interval = 2000, Chance = 1, Range = 7, ShootEffect = TibiaWikiToAnimation(match.Groups["effect"].Value) };
                            if (TryParseRange(match.Groups["damage"].Value, out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[haste\]\]").IsMatch(cleanedAbility):
                        {
                            var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = 300, MaxSpeedChange = 300, AreaEffect = Effect.MagicRed, Duration = 7000 };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[strong haste\]\]").IsMatch(cleanedAbility):
                        {
                            var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = 450, MaxSpeedChange = 450, AreaEffect = Effect.MagicRed, Duration = 4000 };
                            mon.Attacks.Add(spell);
                            break;
                        }

                    case var _ when new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").IsMatch(cleanedAbility):
                        {
                            var match = new Regex(@"\[\[(self-? ?healing)\]\](\s*\((?<damage>[0-9- ]+))?").Match(cleanedAbility);
                            var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                            if (TryParseRange(match.Groups["damage"].Value, out int min, out int max))
                            {
                                spell.MinDamage = min;
                                spell.MaxDamage = max;
                            }
                            else
                            {
                                // Guess defaults based on creature HP
                                spell.MinDamage = (int?)(mon.Health * 0.1);
                                spell.MaxDamage = (int?)(mon.Health * 0.25);
                            }
                            mon.Attacks.Add(spell);
                            break;
                        }

                    default:
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} legacy ability not parsed \"{cleanedAbility}\"");
                        break;
                }
            }
        }

        private static void ParseAbilityList(Monster mon, string abilities, ConvertResultEventArgs result)
        {
            var abilityList = TemplateParser.Deserialize<AbilityListTemplate>(abilities);

            if (abilityList.ability != null)
            {
                foreach (string ability in abilityList.ability)
                {
                    if (TemplateParser.IsTemplateMatch<MeleeTemplate>(ability))
                    {
                        var melee = TemplateParser.Deserialize<MeleeTemplate>(ability);
                        var spell = new Spell() { Name = "melee", SpellCategory = SpellCategory.Offensive, Interval = 2000, Chance = 1 };
                        if (TryParseRange(melee.damage, out int min, out int max))
                        {
                            spell.MinDamage = -min;
                            spell.MaxDamage = -max;
                        }
                        else
                        {
                            // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                        }
                        mon.Attacks.Add(spell);
                    }
                    if (TemplateParser.IsTemplateMatch<HealingTemplate>(ability))
                    {
                        var healing = TemplateParser.Deserialize<HealingTemplate>(ability);
                        var spell = new Spell() { Name = "combat", SpellCategory = SpellCategory.Defensive, DamageElement = CombatDamage.Healing, Interval = 2000, Chance = 0.2 };
                        if (TryParseRange(healing.damage, out int min, out int max))
                        {
                            spell.MinDamage = min;
                            spell.MaxDamage = max;
                        }
                        else
                        {
                            // Guess defaults based on creature HP
                            result.AppendMessage($"Guessing health range for ability {ability}");
                            result.IncreaseError(ConvertError.Warning);
                            spell.MinDamage = (int?)(mon.Health * 0.1);
                            spell.MaxDamage = (int?)(mon.Health * 0.25);
                        }
                        mon.Attacks.Add(spell);
                    }
                    if (TemplateParser.IsTemplateMatch<SummonTemplate>(ability))
                    {
                        var summon = TemplateParser.Deserialize<SummonTemplate>(ability);
                        int maxSummons = 1;
                        TryParseRange(summon.amount, out int min, out maxSummons);
                        mon.MaxSummons += maxSummons;
                        string firstSummonName = summon.creature;
                        mon.Summons.Add(new Summon() { Name = firstSummonName });

                        if (summon.creatures != null)
                        {
                            foreach (var name in summon.creatures)
                            {
                                mon.Summons.Add(new Summon() { Name = name });
                            }
                        }
                    }
                    if (TemplateParser.IsTemplateMatch<HasteTemplate>(ability))
                    {
                        var haste = TemplateParser.Deserialize<HasteTemplate>(ability);
                        int MinSpeedChange = 300;
                        int MaxSpeedChange = 300;
                        int Duration = 7000;
                        if ((!string.IsNullOrWhiteSpace(haste.name) && haste.name.Contains("strong")))
                        {
                            MinSpeedChange = 450;
                            MaxSpeedChange = 450;
                            Duration = 4000;
                        }
                        var spell = new Spell() { Name = "speed", SpellCategory = SpellCategory.Defensive, Interval = 2000, Chance = 0.15, MinSpeedChange = MinSpeedChange, MaxSpeedChange = MaxSpeedChange, AreaEffect = Effect.MagicRed, Duration = Duration };
                        mon.Attacks.Add(spell);
                    }
                    if (TemplateParser.IsTemplateMatch<AbilityTemplate>(ability))
                    {
                        // TODO, report errors converting abilities each ability should be a single error entry even if that ability has multiple problems use a single entry
                        var abilityObj = TemplateParser.Deserialize<AbilityTemplate>(ability);
                        if (TryParseScene(abilityObj.scene, out Spell spell))
                        {
                            spell.Name = "combat";
                            spell.SpellCategory = SpellCategory.Offensive;
                            spell.DamageElement = CombatDamage.Physical;
                            if (!string.IsNullOrWhiteSpace(abilityObj.element) && WikiToElements.ContainsKey(abilityObj.element.ToLower()))
                                spell.DamageElement = WikiToElements[abilityObj.element.ToLower()];

                            if (TryParseRange(abilityObj.damage, out int min, out int max))
                            {
                                spell.MinDamage = -min;
                                spell.MaxDamage = -max;
                            }
                            else
                            {
                                // Could guess defaults based on creature HP, EXP, and bestiary difficulty
                            }
                            mon.Attacks.Add(spell);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"{mon.FileName} couldn't parse scene for ability \"{ability}\", likely scene is missing");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"{mon.FileName} ability not parsed \"{ability}\"");
                    }
                }
            }
        }

        /// <summary>
        /// Converts from a TibiaWiki Scene to a generic spell
        /// Information about the spell shape is located at https://tibia.fandom.com/wiki/Module:SceneBuilder/data
        /// </summary>
        /// <param name="input">scene in string form</param>
        /// <param name="spell">generic spell</param>
        /// <returns>True for success</returns>
        private static bool TryParseScene(string input, out Spell spell)
        {
            spell = new Spell() { AreaEffect = Effect.None, ShootEffect = Missile.None, Interval = 2000, Chance = 0.15 };

            if (!TemplateParser.IsTemplateMatch<SceneTemplate>(input))
                return false;

            SceneTemplate scene = TemplateParser.Deserialize<SceneTemplate>(input);
            if ((scene.missile != null) && (missileIds.ContainsKey(scene.missile)))
            {
                spell.ShootEffect = missileIds[scene.missile];
                spell.OnTarget = true;
            }
            if ((scene.effect != null) && (effectIds.ContainsKey(scene.effect)))
            {
                spell.AreaEffect = effectIds[scene.effect];
            }

            switch (scene.spell)
            {
                case "singleeffect":
                    spell.OnTarget = false;
                    spell.Radius = 1;
                    break;
                case "front_sweep":
                    spell.IsDirectional = true;
                    spell.Length = 1;
                    spell.Spread = 1;
                    break;
                case "1sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 1;
                    spell.Radius = 1;
                    break;
                case "2sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 2;
                    spell.Radius = 1;
                    break;
                case "3sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 3;
                    spell.Radius = 1;
                    break;
                case "5sqmstrike":
                    spell.OnTarget = true;
                    spell.Range = 5;
                    spell.Radius = 1;
                    break;
                case "great_explosion":
                    spell.Radius = 5;
                    break;
                case "3x3spell":
                    spell.Radius = 3;
                    break;
                case "xspell":
                    spell.Ring = 2;
                    break;
                case "plusspell":
                    spell.Radius = 2;
                    break;
                case "plusspelltarget":
                    spell.OnTarget = true;
                    spell.Radius = 2;
                    break;
                case "3sqmwave":
                    spell.IsDirectional = true;
                    spell.Length = 3;
                    spell.Spread = 2;
                    break;
                case "3sqmwavewide":
                    spell.IsDirectional = true;
                    spell.Length = 3;
                    spell.Spread = 1;
                    break;
                case "5sqmwavenarrow":
                    spell.IsDirectional = true;
                    spell.Length = 5;
                    spell.Spread = 3;
                    break;
                case "5sqmwavewide":
                    spell.IsDirectional = true;
                    spell.Length = 5;
                    spell.Spread = 2;
                    break;
                case "1sqmballtarget":
                    spell.Radius = 3;
                    spell.OnTarget = true;
                    break;
                case "2sqmballtarget":
                    spell.Radius = 4;
                    spell.OnTarget = true;
                    break;
                case "3sqmballtarget":
                    spell.Radius = 5;
                    spell.OnTarget = true;
                    break;
                case "4sqmballtarget":
                    spell.Radius = 6;
                    spell.OnTarget = true;
                    break;
                case "5sqmballtarget":
                    spell.Radius = 7;
                    spell.OnTarget = true;
                    break;
                case "8sqmwave":
                    spell.IsDirectional = true;
                    spell.Spread = 3;
                    spell.Length = 8;
                    break;
                case "10sqmwave":
                    spell.IsDirectional = true;
                    spell.Spread = 4;
                    spell.Length = 10;
                    break;
                case "2sqmring":
                    spell.Ring = 3;
                    break;
                case "3sqmring":
                    spell.Ring = 4;
                    break;
                case "4sqmring":
                    spell.Ring = 5;
                    break;
                case "2sqmballself":
                    spell.Radius = 4;
                    spell.OnTarget = false;
                    break;
                case "3sqmballself":
                    spell.Radius = 5;
                    spell.OnTarget = false;
                    break;
                case "4sqmballself":
                    spell.Radius = 6;
                    spell.OnTarget = false;
                    break;
                case "5sqmballself":
                    spell.Radius = 7;
                    spell.OnTarget = false;
                    break;
                case "6sqmballself":
                    spell.Radius = 8;
                    spell.OnTarget = false;
                    break;
                case "4sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 4;
                    spell.Spread = 0;
                    break;
                case "5sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 5;
                    spell.Spread = 0;
                    break;
                case "6sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 6;
                    spell.Spread = 0;
                    break;
                case "7sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 7;
                    spell.Spread = 0;
                    break;
                case "8sqmbeam":
                    spell.IsDirectional = true;
                    spell.Length = 8;
                    spell.Spread = 0;
                    break;
                case "energy_wall_north_diag_area":
                    break;
                case "energy_wall_south_diag_area":
                    break;
                case "energy_wall_north_south_area":
                    break;
                case "chivalrous_challenge":
                    break;
            }

            return true;
        }

        private static Missile TibiaWikiToAnimation(string missile)
        {
            if ((missile == "spear") || (missile == "spears"))
            {
                return Missile.Spear;
            }
            else if ((missile == "throwing knives") || (missile == "throwing knife"))
            {
                return Missile.ThrowingKnife;
            }
            else if ((missile == "bolt") || (missile == "bolts"))
            {
                return Missile.Bolt;
            }
            else if ((missile == "arrow") || (missile == "arrows"))
            {
                return Missile.Arrow;
            }
            else if (missile.Contains("boulder"))
            {
                return Missile.LargeRock;
            }
            else if (missile.Contains("stone"))
            {
                return Missile.SmallStone;
            }
            else
            {
                return Missile.None;
            }
        }

        private static void ParseLoot(Monster monster, string lootTable, string filename, ConvertResultEventArgs result)
        {
            var lootTableTemplate = TemplateParser.Deserialize<LootTableTemplate>(lootTable);
            if ((lootTableTemplate.loot != null) && (lootTableTemplate.loot.Length >= 1) && (!string.IsNullOrWhiteSpace(lootTableTemplate.loot[0])))
            {
                // Request for full loot stats now that we are sure monster has loot
                string looturl = $"https://tibia.fandom.com/api.php?action=parse&format=json&page=Loot_Statistics:{filename}&prop=wikitext";
                var lootPage = RequestData(looturl).Result;
                if (lootPage != null)
                {
                    string elements = lootPage.Wikitext.Empty.ToLower();
                    var lootsectionsRegEx = new Regex("{{loot2(?<loots>.*?)}}", RegexOptions.Singleline);
                    if (lootsectionsRegEx.IsMatch(elements))
                    {
                        var lootsection = lootsectionsRegEx.Match(elements);
                        string loots = lootsection.Captures[0].Value;

                        var killsmatch = new Regex(@"\|kills=(?<kills>\d+)").Match(loots);
                        double.TryParse(killsmatch.Groups["kills"].Value, out double kills);
                        // sometimes TibiaWiki doesn't show the amount field
                        var lootregex = new Regex(@"\|\s*(?<itemname>[a-z'.() ]*),\s*times:\s*(?<times>\d+)(, amount:\s*(?<amount>[0-9-]+))?");
                        var matches = lootregex.Matches(loots);
                        foreach (Match loot in matches)
                        {
                            string item = loot.Groups["itemname"].Value;
                            double.TryParse(loot.Groups["times"].Value, out double times);
                            string amount = loot.Groups["amount"].Value;

                            if (item != "empty")
                            {
                                double percent = times / kills;

                                if (!int.TryParse(amount, out int count))
                                {
                                    var amounts = amount.Split("-");
                                    if (amounts.Length >= 2)
                                    {
                                        int.TryParse(amounts[1], out count);
                                    }
                                }
                                count = (count > 0) ? count : 1;

                                // Two items have redirects, which can be verified by checking the source at the link below
                                // https://tibia.fandom.com/wiki/Template:Loot2/List?veaction=editsource
                                // Parsing the html output is a pain so for now we can map those two items here
                                if (item == "skull")
                                    item = "skull (item)";
                                if (item == "black skull")
                                    item = "black skull (item)";

                                LootItem lootItem = new LootItem()
                                {
                                    Name = item,
                                    Chance = (decimal)percent,
                                    Count = count
                                };
                                SetItemId(ref lootItem, ref result);

                                monster.Items.Add(lootItem);
                            }
                        }
                    }
                }
                else
                {
                    // Creature has loot but no loot statistics. Use information from loot table to generate the loot
                    // Could be loot item template or just a list of items....
                    foreach (string loot in lootTableTemplate.loot)
                    {
                        if (TemplateParser.IsTemplateMatch<LootItemTemplate>(loot))
                        {
                            LootItemTemplate lootItem = TemplateParser.Deserialize<LootItemTemplate>(loot);
                            if (lootItem.parts != null)
                            {
                                LootItem genericLootItem = null;
                                if (lootItem.parts.Length == 1)
                                {
                                    // template name only
                                    genericLootItem = new LootItem() { Name = lootItem.parts[0], Chance = DEFAULT_LOOT_CHANCE, Count = DEFAULT_LOOT_COUNT };
                                }
                                else if (lootItem.parts.Length == 2)
                                {
                                    // template name + rarity OR count + name
                                    // Assumes first combination if parts[1] matches a rarity description
                                    if (TryParseTibiaWikiRarity(lootItem.parts[1], out decimal chance))
                                    {
                                        genericLootItem = new LootItem() { Name = lootItem.parts[0], Chance = chance, Count = DEFAULT_LOOT_COUNT };
                                    }
                                    else
                                    {
                                        if (!TryParseRange(lootItem.parts[0], out int min, out int max))
                                            max = DEFAULT_LOOT_COUNT;
                                        genericLootItem = new LootItem() { Name = lootItem.parts[1], Chance = DEFAULT_LOOT_CHANCE, Count = max };
                                    }
                                }
                                else if (lootItem.parts.Length == 3)
                                {
                                    // template name + rarity + count
                                    if (!TryParseRange(lootItem.parts[0], out int min, out int max))
                                        max = DEFAULT_LOOT_COUNT;
                                    TryParseTibiaWikiRarity(lootItem.parts[2], out decimal chance);
                                    genericLootItem = new LootItem() { Name = lootItem.parts[1], Chance = chance, Count = max };
                                }

                                if (genericLootItem != null)
                                {
                                    SetItemId(ref genericLootItem, ref result);

                                    monster.Items.Add(genericLootItem);
                                }
                            }

                        }
                    }
                }
            }
        }

        private static void SetItemId(ref LootItem item, ref ConvertResultEventArgs result)
        {
            string loweredName = item.Name.ToLower();
            if (itemsByName.ContainsKey(loweredName))
            {
                if (ushort.TryParse(itemsByName[loweredName].Ids, out ushort _))
                {
                    item.Id = ushort.Parse(itemsByName[loweredName].Ids);
                }
                else if (string.IsNullOrWhiteSpace(itemsByName[loweredName].Ids))
                {
                    string message = $"TibiaWiki is missing item id for item {loweredName}";
                    result.AppendMessage(message);
                }
                else
                {
                    string message = $"TibiaWiki has malformatted or multiple ids {itemsByName[loweredName].Ids} for item {loweredName}";
                    result.AppendMessage(message);
                }
            }
            else
            {
                string message = $"TibiaWiki has no data for item name {loweredName}";
                result.AppendMessage(message);
            }
        }

        private static bool TryParseTibiaWikiRarity(string input, out decimal chance)
        {
            chance = DEFAULT_LOOT_CHANCE;
            if (input == null)
                return false;

            input = input.ToLower();
            if (input == "always")
            {
                chance = 1.0M;
                return true;
            }
            else if (input == "common")
            {
                chance = 0.35M;
                return true;
            }
            else if (input == "uncommon")
            {
                chance = 0.15M;
                return true;
            }
            else if (input == "semi-rare")
            {
                chance = 0.025M;
                return true;
            }
            else if (input == "rare")
            {
                chance = 0.075M;
                return true;
            }
            else if (input == "very rare")
            {
                chance = 0.04M;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creature run at can be number or color based
        /// </summary>
        /// <param name="monster"></param>
        /// <param name="runsAt"></param>
        private static void ParseRunAt(Monster monster, string runsAt)
        {
            if (TryParseRange(runsAt, out int min, out int max))
            {
            }
            else if (runsAt == "deep red")
            {
                max = (int)((monster.Health * 0.04) - 1);
                min = 1;
            }
            else if (runsAt == "red")
            {
                max = (int)((monster.Health * 0.3) - 1);
                min = (int)(monster.Health * 0.04);
            }
            else if (runsAt == "yellow")
            {
                max = (int)((monster.Health * 0.6) - 1);
                min = (int)(monster.Health * 0.3);
            }
            else if (runsAt == "light green")
            {
                max = (int)((monster.Health * 0.95) - 1);
                min = (int)(monster.Health * 0.6);
            }
            else if (runsAt == "green")
            {
                max = (int)(monster.Health - 1);
                min = (int)(monster.Health * 0.95);
            }

            if (min == 0)
                monster.RunOnHealth = max;
            else
                monster.RunOnHealth = (max + min) / 2;
        }

        private void ParseBestiaryLevel(Monster monster, string difficultly, string occurence)
        {
            difficultly = difficultly.ToLower();
            if (difficultly == "harmless")
            {
                monster.Bestiary.DifficultlyStarCount = 0;
            }
            else if (difficultly == "trivial")
            {
                monster.Bestiary.DifficultlyStarCount = 1;
            }
            else if (difficultly == "easy")
            {
                monster.Bestiary.DifficultlyStarCount = 2;
            }
            else if (difficultly == "medium")
            {
                monster.Bestiary.DifficultlyStarCount = 3;
            }
            else if (difficultly == "hard")
            {
                monster.Bestiary.DifficultlyStarCount = 4;
            }
            else if (difficultly == "challenging")
            {
                monster.Bestiary.DifficultlyStarCount = 5;
            }

            occurence = occurence.ToLower();
            if (occurence == "common")
            {
                monster.Bestiary.OccuranceDiamondCount = 0;
            }
            else if (occurence == "uncommon")
            {
                monster.Bestiary.OccuranceDiamondCount = 1;
            }
            else if (occurence == "rare")
            {
                monster.Bestiary.OccuranceDiamondCount = 2;
            }
            else if (occurence == "very rare")
            {
                monster.Bestiary.OccuranceDiamondCount = 3;
            }

            if ((difficultly == "harmless") && (occurence != "very rare"))
            {
                monster.Bestiary.CharmPoints = 1;
                monster.Bestiary.FirstDetailStage = 5;
                monster.Bestiary.SecondDetailStage = 10;
                monster.Bestiary.FinalDetailStage = 25;
            }
            else if ((difficultly == "harmless") && (occurence == "very rare"))
            {
                monster.Bestiary.CharmPoints = 5;
                monster.Bestiary.FirstDetailStage = 2;
                monster.Bestiary.SecondDetailStage = 3;
                monster.Bestiary.FinalDetailStage = 5;
            }
            else if ((difficultly == "trivial") && (occurence != "very rare"))
            {
                monster.Bestiary.CharmPoints = 5;
                monster.Bestiary.FirstDetailStage = 10;
                monster.Bestiary.SecondDetailStage = 100;
                monster.Bestiary.FinalDetailStage = 250;
            }
            else if ((difficultly == "trivial") && (occurence == "very rare"))
            {
                monster.Bestiary.CharmPoints = 10;
                monster.Bestiary.FirstDetailStage = 2;
                monster.Bestiary.SecondDetailStage = 3;
                monster.Bestiary.FinalDetailStage = 5;
            }
            else if ((difficultly == "easy") && (occurence != "very rare"))
            {
                monster.Bestiary.CharmPoints = 15;
                monster.Bestiary.FirstDetailStage = 25;
                monster.Bestiary.SecondDetailStage = 250;
                monster.Bestiary.FinalDetailStage = 500;
            }
            else if ((difficultly == "easy") && (occurence == "very rare"))
            {
                monster.Bestiary.CharmPoints = 30;
                monster.Bestiary.FirstDetailStage = 2;
                monster.Bestiary.SecondDetailStage = 3;
                monster.Bestiary.FinalDetailStage = 5;
            }
            else if ((difficultly == "medium") && (occurence != "very rare"))
            {
                monster.Bestiary.CharmPoints = 25;
                monster.Bestiary.FirstDetailStage = 50;
                monster.Bestiary.SecondDetailStage = 500;
                monster.Bestiary.FinalDetailStage = 1000;
            }
            else if ((difficultly == "medium") && (occurence == "very rare"))
            {
                monster.Bestiary.CharmPoints = 50;
                monster.Bestiary.FirstDetailStage = 2;
                monster.Bestiary.SecondDetailStage = 3;
                monster.Bestiary.FinalDetailStage = 5;
            }
            else if ((difficultly == "hard") && (occurence != "very rare"))
            {
                monster.Bestiary.CharmPoints = 50;
                monster.Bestiary.FirstDetailStage = 100;
                monster.Bestiary.SecondDetailStage = 1000;
                monster.Bestiary.FinalDetailStage = 2500;
            }
            else if ((difficultly == "hard") && (occurence == "very rare"))
            {
                monster.Bestiary.CharmPoints = 100;
                monster.Bestiary.FirstDetailStage = 2;
                monster.Bestiary.SecondDetailStage = 3;
                monster.Bestiary.FinalDetailStage = 5;
            }
            else if ((difficultly == "challenging") && (occurence != "very rare"))
            {
                monster.Bestiary.CharmPoints = 100;
                monster.Bestiary.FirstDetailStage = 200;
                monster.Bestiary.SecondDetailStage = 2000;
                monster.Bestiary.FinalDetailStage = 5000;
            }
            else if ((difficultly == "challenging") && (occurence == "very rare"))
            {
                monster.Bestiary.CharmPoints = 200;
                monster.Bestiary.FirstDetailStage = 2;
                monster.Bestiary.SecondDetailStage = 3;
                monster.Bestiary.FinalDetailStage = 5;
            }
        }

        /// <summary>
        /// Converts a string representing a numeric range to two intergers
        /// Example numeric ranges which can be parsed are "500", "0-500", and "0-500?"
        /// </summary>
        /// <param name="range">String to parse</param>
        /// <param name="min">lower bound value in range, defaults to 0</param>
        /// <param name="max">high bonund value in the range, will be set when the range only has a single number</param>
        /// <returns>returns false when no numeric values can be parsed</returns>
        private static bool TryParseRange(string range, out int min, out int max)
        {
            min = 0;
            max = 0;

            if (range == null)
                return false;

            Regex rgx = new Regex(@"(?<first>\d+)(([ -]?)(?<second>\d+))?");
            var match = rgx.Match(range);
            if (int.TryParse(match.Groups["second"].Value, out max))
            {
                int.TryParse(match.Groups["first"].Value, out min);
                return true;
            }
            else if (int.TryParse(match.Groups["first"].Value, out max))
            {
                return true;
            }
            return false;
        }

        private static bool RobustTryParse(string input, out int value)
        {
            value = 0;
            if (input == null)
                return false;

            Regex rgx = new Regex(@"(?<value>\d+)");
            var match = rgx.Match(input);

            return int.TryParse(match.Groups["value"].Value, out value);
        }

        private static bool RobustTryParse(string input, out bool value)
        {
            value = false;
            if (input == null)
            {
                return false;
            }
            input = input.Trim().ToLower();

            if ((input == "yes") || (input == "y") || (input == "true") || (input == "t") || (input == "1"))
            {
                value = true;
                return true;
            }
            else if ((input == "no") || (input == "n") || (input == "false") || (input == "f") || (input == "0"))
            {
                value = false;
                return true;
            }

            return false;
        }
    }
}
