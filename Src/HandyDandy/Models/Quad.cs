// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.MVVM;
using System.Linq;

namespace HandyDandy.Models
{
    public class Quad : InpcBase
    {
        public Quad()
        {
            Buttons = Enumerable.Range(0, 11).Select(i => new Ternary()).ToArray();
        }

        public Quad(int len)
        {
            Buttons = Enumerable.Range(0, len).Select(i => new Ternary()).ToArray();
        }


        public Ternary[] Buttons { get; private set; }
        public string Hex { get; set; }
        public string Bin { get; set; }
    }
}
