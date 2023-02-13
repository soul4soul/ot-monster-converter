using MonsterConverterInterface;
using MonsterConverterInterface.MonsterTypes;
using MoonSharp.Interpreter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterConverterTfsRevScriptSys
{
    /// <summary>
    /// The purpose of this class is to immitate the `Game` TFS class used to create monsterTypes that are used to register monsters
    /// </summary>
    [MoonSharpUserData]
    class MockTfsGame
    {
        static Queue<Tuple<Monster, ConvertResultEventArgs>> convertedMonsters = new Queue<Tuple<Monster, ConvertResultEventArgs>>(1);

        public static Queue<Tuple<Monster, ConvertResultEventArgs>> ConvertedMonsters
        {
            private set { ; }
            get { return convertedMonsters; }
        }

        public static MockTfsMonsterType createMonsterType(string name)
        {
            return new MockTfsMonsterType(name);
        }
    }
}
