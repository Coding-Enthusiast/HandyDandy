// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.ImprovementProposals;
using HandyDandy.Models;
using HandyDandy.Services;
using System;
using System.Linq;

namespace HandyDandy.ViewModels
{
    public class GroupedBinaryViewModel : GeneratorVM
    {
        public GroupedBinaryViewModel()
        {
            Items = Enumerable.Range(0, 4).Select(i => new LinkedValues()).ToArray();
        }

        public GroupedBinaryViewModel(OutputType ot, MnemonicLength mnLen)
        {
            Stream = new TernaryStream(ot, mnLen);
            int itemCount, chunkSize;
            string[]? allWords = BIP0039.GetAllWords(BIP0039.WordLists.English);
            if (ot == OutputType.PrivateKey)
            {
                CollumnCount = 3;
                itemCount = 32;
                chunkSize = 8;
                allWords = null;
            }
            else if (ot == OutputType.Bip39Mnemonic)
            {
                CollumnCount = 2;
                chunkSize = 11;
                itemCount = mnLen switch
                {
                    MnemonicLength.Twelve => 12,
                    MnemonicLength.Fifteen => 15,
                    MnemonicLength.Eighteen => 18,
                    MnemonicLength.TwentyOne => 21,
                    MnemonicLength.TwentyFour => 24,
                    _ => throw new ArgumentException("Mnemonic length is not defined."),
                };
            }
            else if (ot == OutputType.ElectrumMnemonic)
            {
                CollumnCount = 2;
                itemCount = 12;
                chunkSize = 11;
            }
            else
            {
                throw new ArgumentException("Output type is not defined.");
            }

            Items = new LinkedValues[itemCount];
            for (int i = 0; i < Items.Length; i++)
            {
                Items[i] = new LinkedValues(Stream, allWords, chunkSize);
            }
        }


        public override TernaryStream Stream { get; }
        public int CollumnCount { get; }
        public LinkedValues[] Items { get; private set; }
    }
}
