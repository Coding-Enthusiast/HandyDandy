// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using HandyDandy.Models;
using System.Linq;

namespace HandyDandy.ViewModels
{
    public class TextGridViewModel : ViewModelBase
    {
        public TextGridViewModel()
        {
            Items = Enumerable.Range(0, 4).Select(i => new Quad()).ToArray();
        }

        public TextGridViewModel(int len, OutputType ot)
        {
            int bitLen = ot == OutputType.PrivateKey ? 8 : 11;
            len /= bitLen;
            Items = Enumerable.Range(0, len).Select(i => new Quad(bitLen)).ToArray();
        }


        public Quad[] Items { get; private set; }
    }
}
