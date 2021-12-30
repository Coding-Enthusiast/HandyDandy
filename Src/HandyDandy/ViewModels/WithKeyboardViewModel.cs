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
            Stream = new TernaryStream(ot, mnLen);
            if (ot == OutputType.PrivateKey)
            {
                CollumnCount = 3;

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
                var wordCount = mnLen switch
                {
                    MnemonicLength.Twelve => 12,
                    MnemonicLength.Fifteen => 15,
                    MnemonicLength.Eighteen => 18,
                    MnemonicLength.TwentyOne => 21,
                    MnemonicLength.TwentyFour => 24,
                    _ => throw new ArgumentException("Mnemonic length is not defined."),
                };
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
                Items = new LinkedValues[12];
                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i] = new LinkedValues(Stream, allWords, 11);
                }
            }
            else
            {
                throw new ArgumentException("Output type is not defined.");
            }
        }


        public override TernaryStream Stream { get; }
        public int CollumnCount { get; }
        public LinkedValues[] Items { get; private set; }

        private bool _canSetNext = true;
        public bool CanSetNext
        {
            get => _canSetNext;
            set => SetField(ref _canSetNext, value);
        }

        public void SetNextBit(bool b)
        {
            if (CanSetNext)
            {
                CanSetNext = !Stream.SetNext(b);
            }
        }
    }
}
