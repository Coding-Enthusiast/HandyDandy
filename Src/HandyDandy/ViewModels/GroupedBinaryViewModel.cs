// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Hashing;
using Autarkysoft.Bitcoin.ImprovementProposals;
using HandyDandy.Models;
using System;
using System.ComponentModel;
using System.Linq;

namespace HandyDandy.ViewModels
{
    public class GroupedBinaryViewModel : ViewModelBase
    {
        public GroupedBinaryViewModel()
        {
            Items = Enumerable.Range(0, 4).Select(i => new LinkedValues()).ToArray();
        }

        public GroupedBinaryViewModel(int len, OutputType ot, MnemonicLength mnLen)
        {
            if (ot == OutputType.PrivateKey)
            {
                Items = Enumerable.Range(0, 32).Select(i => new LinkedValues(8)).ToArray();
            }
            else if (ot == OutputType.Bip39Mnemonic)
            {
                string[] allWords = BIP0039.GetAllWords(BIP0039.WordLists.English);
                int wordCount;
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

                Items = new LinkedValues[wordCount];
                for (int i = 0; i < Items.Length - 1; i++)
                {
                    Items[i] = new LinkedValues(11, allWords, 0);
                    Items[i].PropertyChanged += GroupedBinaryViewModel_PropertyChanged;
                }
                Items[^1] = new LinkedValues(11, allWords, checksumSize);
            }
            else if (ot == OutputType.ElectrumMnemonic)
            {
                string[] allWords = BIP0039.GetAllWords(BIP0039.WordLists.English);
                Items = new LinkedValues[12];
                for (int i = 0; i < Items.Length; i++)
                {
                    Items[i] = new LinkedValues(11, allWords, 0);
                }
            }
        }


        private readonly int entropySize, checksumSize;

        private void GroupedBinaryViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            // Compute checksum
            using Sha256 sha = new();
            byte[] entropy = new byte[entropySize / 8];
            
            int itemIndex = 0;
            int bitIndex = 0;
            int toTake = 8;
            int maxBits = 11;
            for (int i = 0; i < entropy.Length; i++)
            {
                if (bitIndex + toTake <= maxBits)
                {
                    entropy[i] = (byte)(Items[itemIndex].Value >> (3 - bitIndex));
                }
                else
                {
                    entropy[i] = (byte)(((Items[itemIndex].Value << (bitIndex - 3)) & 0xff) |
                                         (Items[itemIndex + 1].Value >> (14 - bitIndex)));
                }

                bitIndex += toTake;
                if (bitIndex >= maxBits)
                {
                    bitIndex -= maxBits;
                    itemIndex++;
                }
            }

            byte[] hash = sha.ComputeHash(entropy);
            byte CS = (byte)(hash[0] >> (8 - checksumSize));
            for (int i = 0; i < checksumSize; i++)
            {
                int bit = (CS >> i) & 1;
                Items[^1].Buttons[^(i + 1)].SetState(bit);
            }
        }

        public LinkedValues[] Items { get; private set; }
    }
}
