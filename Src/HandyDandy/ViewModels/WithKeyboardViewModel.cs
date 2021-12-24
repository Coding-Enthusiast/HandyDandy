// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.ImprovementProposals;
using HandyDandy.Models;
using HandyDandy.Services;
using System;

namespace HandyDandy.ViewModels
{
    public class WithKeyboardViewModel : GeneratorVM
    {
        public WithKeyboardViewModel() : this(OutputType.Bip39Mnemonic, MnemonicLength.Twelve)
        {

        }

        public WithKeyboardViewModel(OutputType ot, MnemonicLength mnLen)
        {
            if (ot == OutputType.PrivateKey)
            {
                CollumnCount = 3;

                Stream = new TernaryStream(256, 0, new WifChecksum(), ot);
                Items = new LinkedValues[32];
                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i] = new LinkedValues(Stream, null, 8);
                }
            }
            else if (ot == OutputType.Bip39Mnemonic)
            {
                CollumnCount = 2;

                string[] allWords = BIP0039.GetAllWords(BIP0039.WordLists.English);
                int wordCount, entropySize, checksumSize;
                if (mnLen == MnemonicLength.Twelve)
                {
                    wordCount = 12;
                    entropySize = 128;
                    checksumSize = 4;
                }
                else if (mnLen == MnemonicLength.Fifteen)
                {
                    wordCount = 15;
                    entropySize = 160;
                    checksumSize = 5;
                }
                else if (mnLen == MnemonicLength.Eighteen)
                {
                    wordCount = 18;
                    entropySize = 192;
                    checksumSize = 6;
                }
                else if (mnLen == MnemonicLength.TwentyOne)
                {
                    wordCount = 21;
                    entropySize = 224;
                    checksumSize = 7;
                }
                else if (mnLen == MnemonicLength.TwentyFour)
                {
                    wordCount = 24;
                    entropySize = 256;
                    checksumSize = 8;
                }
                else
                {
                    throw new NotImplementedException();
                }

                Stream = new TernaryStream(entropySize + checksumSize, checksumSize, new Bip39Checksum(checksumSize), ot);
                Items = new LinkedValues[wordCount];
                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i] = new LinkedValues(Stream, allWords, 11);
                }
            }
            else if (ot == OutputType.ElectrumMnemonic)
            {
                CollumnCount = 2;

                string[] allWords = BIP0039.GetAllWords(BIP0039.WordLists.English);
                Stream = new TernaryStream(132, 0, new ElectrumChecksum(), ot);
                Items = new LinkedValues[12];
                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i] = new LinkedValues(Stream, allWords, 11);
                }
            }
            else
            {
                throw new NotImplementedException();
            }
        }


        public override TernaryStream Stream { get; }
        public int CollumnCount { get; }
        public LinkedValues[] Items { get; private set; }

        public void OnKeyPressed()
        {
            Stream.SetNext(true);
        }

        public void SetNextBit(bool b)
        {
            Stream.SetNext(b);
        }
    }
}
