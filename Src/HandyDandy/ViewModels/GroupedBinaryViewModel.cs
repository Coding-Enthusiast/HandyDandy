// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.ImprovementProposals;
using HandyDandy.Models;
using System.Linq;

namespace HandyDandy.ViewModels
{
    public class GroupedBinaryViewModel : ViewModelBase
    {
        public GroupedBinaryViewModel()
        {
            Items = Enumerable.Range(0, 4).Select(i => new Quad()).ToArray();
        }

        public GroupedBinaryViewModel(int len, OutputType ot)
        {
            int bitLen = ot == OutputType.PrivateKey ? 8 : 11;
            len /= bitLen;
            string[]? allWords = (ot == OutputType.Bip39Mnemonic || ot == OutputType.ElectrumMnemonic)
                                ? BIP0039.GetAllWords(BIP0039.WordLists.English)
                                : null;
            Items = Enumerable.Range(0, len).Select(i => new Quad(bitLen, allWords)).ToArray();
        }


        public Quad[] Items { get; private set; }
    }
}
