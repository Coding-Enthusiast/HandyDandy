// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;
using Autarkysoft.Bitcoin.ImprovementProposals;
using HandyDandy.Models;
using HandyDandy.MVVM;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

namespace HandyDandy.Services
{
    public class TernaryStream : InpcBase
    {
        /// <summary>
        /// Only used for testing
        /// </summary>
        public TernaryStream(int binarySize, int disabledCount)
        {
            DataBitSize = binarySize - disabledCount;
            Items = new Ternary[binarySize];
            for (int i = 0; i < binarySize; i++)
            {
                bool isEnabled = i < DataBitSize;
                Items[i] = new Ternary(isEnabled);
                Items[i].PropertyChanged += Item_PropertyChanged;
            }
        }

        public TernaryStream(OutputType ot, MnemonicLength mnLen = MnemonicLength.Twelve)
        {
            OutType = ot;
            int disabledCount = 0;
            if (ot == OutputType.PrivateKey)
            {
                DataBitSize = 256;
                DataByteSize = 32;
                disabledCount = 0;
                checksum = new WifChecksum();
            }
            else if (ot == OutputType.Bip39Mnemonic)
            {
                if (mnLen == MnemonicLength.Twelve)
                {
                    DataBitSize = 128;
                    DataByteSize = 16;
                    disabledCount = 4;
                }
                else if (mnLen == MnemonicLength.Fifteen)
                {
                    DataBitSize = 160;
                    DataByteSize = 20;
                    disabledCount = 5;
                }
                else if (mnLen == MnemonicLength.Eighteen)
                {
                    DataBitSize = 192;
                    DataByteSize = 24;
                    disabledCount = 6;
                }
                else if (mnLen == MnemonicLength.TwentyOne)
                {
                    DataBitSize = 224;
                    DataByteSize = 28;
                    disabledCount = 7;
                }
                else if (mnLen == MnemonicLength.TwentyFour)
                {
                    DataBitSize = 256;
                    DataByteSize = 32;
                    disabledCount = 8;
                }
                else
                {
                    throw new ArgumentException("Mnemonic length is not defined.");
                }

                checksum = new Bip39Checksum(disabledCount);
            }
            else if (ot == OutputType.ElectrumMnemonic)
            {
                DataBitSize = 132;
                DataByteSize = 16;
                disabledCount = 0;

                checksum = new ElectrumChecksum();
            }
            else
            {
                throw new ArgumentException("Output type is not defined.");
            }

            Items = new Ternary[DataBitSize + disabledCount];

            for (int i = 0; i < Items.Length; i++)
            {
                bool isEnabled = i < DataBitSize;
                Items[i] = new Ternary(isEnabled);
                Items[i].PropertyChanged += Item_PropertyChanged;
            }
        }


        private readonly IChecksum? checksum;

        public OutputType OutType { get; private set; }
        public int ReadPosition { get; private set; }
        public int SetPosition { get; private set; }

        public Ternary[] Items { get; }
        public int TotalBitSize => Items.Length;
        public int DataBitSize { get; }
        public int DataByteSize { get; }

        private int _changed;
        public int SetBitCount
        {
            get => _changed;
            set => SetField(ref _changed, value);
        }

        private bool _allSet;
        public bool IsAllSet
        {
            get => _allSet;
            set => SetField(ref _allSet, value);
        }


        private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            SetBitCount = Items.Count(x => x.State != TernaryState.Unset);
            //byte[] cs = checksum.Compute(ba.Slice(0, DataSize));
            //for (int i = 0; i < cs.Length; i++)
            //{
            //    Items[^(cs.Length - i)].SetState(cs[i]);
            //}
            for (int i = 0; i < DataBitSize; i++)
            {
                if (Items[i].State == TernaryState.Unset)
                {
                    IsAllSet = false;
                    return;
                }
            }
            IsAllSet = true;
            // Don't put anything after this!
        }

        private void SetResult(ReadOnlySpan<byte> bytes)
        {
            if (!IsAllSet)
            {
                Result = "Not all bits are set. The result is still weak and can not be copied.";
                return;
            }

            if (OutType == OutputType.PrivateKey)
            {
                try
                {
                    using PrivateKey temp = new(bytes.ToArray());
                    Result = temp.ToWif(true);
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Result = $"The current bit stream is creating an out of range key.{Environment.NewLine}" +
                             $"{ex.Message}";
                }
                catch (Exception ex)
                {
                    Result = $"An error occurred while converting the bit stream to a private key.{Environment.NewLine}" +
                             $"Error message: {ex.Message}";
                }
            }
            else if (OutType == OutputType.Bip39Mnemonic)
            {
                try
                {
                    using BIP0039 temp = new(bytes.ToArray());
                    Result = temp.ToMnemonic();
                }
                catch (Exception ex)
                {
                    Result = $"An error occurred while converting the bit stream to a BIP-39 mnemonic.{Environment.NewLine}" +
                             $"Error message: {ex.Message}";
                }
            }
            else if (OutType == OutputType.ElectrumMnemonic)
            {
                try
                {
                    string mn = GetElectrumMnemonic();
                    using ElectrumMnemonic temp = new(mn);
                    Result = temp.ToMnemonic();
                }
                catch (Exception ex)
                {
                    Result = $"An error occurred while converting the bit stream to an Electrum mnemonic.{Environment.NewLine}" +
                             $"Error message: {ex.Message}";
                }
            }
            else
            {
                Result = "Undefined.";
            }
        }

        private string GetElectrumMnemonic()
        {
            Debug.Assert(Items.Length == 132);
            int[] wordIndexes = new int[12];
            for (int i = 0, j = 0; i < wordIndexes.Length; i++, j += 11)
            {
                wordIndexes[i] = Items[j].ToBit() << 10 |
                                 Items[j + 1].ToBit() << 9 |
                                 Items[j + 2].ToBit() << 8 |
                                 Items[j + 3].ToBit() << 7 |
                                 Items[j + 4].ToBit() << 6 |
                                 Items[j + 5].ToBit() << 5 |
                                 Items[j + 6].ToBit() << 4 |
                                 Items[j + 7].ToBit() << 3 |
                                 Items[j + 8].ToBit() << 2 |
                                 Items[j + 9].ToBit() << 1 |
                                 Items[j + 10].ToBit();
            }


            string[] allWords = ElectrumMnemonic.GetAllWords(BIP0039.WordLists.English);
            string res = string.Empty;
            while (true)
            {
                try
                {
                    string[] words = wordIndexes.Select(x => allWords[x]).ToArray();
                    res = string.Join(' ', words);
                    var temp = new ElectrumMnemonic(res);
                    if (temp.MnType == ElectrumMnemonic.MnemonicType.SegWit)
                    {
                        break;
                    }
                }
                catch { }

                for (int i = 0; i < wordIndexes.Length; i++)
                {
                    wordIndexes[i]++;
                    if (wordIndexes[i] == 2048)
                    {
                        wordIndexes[i] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return res;
        }

        private string _res = string.Empty;
        public string Result
        {
            get => _res;
            set => SetField(ref _res, value);
        }

        public byte[] ToBytes()
        {
            byte[] ba = new byte[DataByteSize];
            for (int i = 0, j = 0; i < ba.Length; i++, j += 8)
            {
                ba[i] = (byte)(Items[j].ToBit() << 7 |
                               Items[j + 1].ToBit() << 6 |
                               Items[j + 2].ToBit() << 5 |
                               Items[j + 3].ToBit() << 4 |
                               Items[j + 4].ToBit() << 3 |
                               Items[j + 5].ToBit() << 2 |
                               Items[j + 6].ToBit() << 1 |
                               Items[j + 7].ToBit() << 0);
            }
            return ba;
        }

        public Ternary[] Next(int count)
        {
            Ternary[] result = new Ternary[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Items[i + ReadPosition];
            }
            ReadPosition += count;
            return result;
        }

        /// <summary>
        /// Sets next bit until either reaching the end or a disabled bit.
        /// </summary>
        /// <param name="b">Bit value to set (true=1; false=0)</param>
        /// <returns>True if there is more enabled bits to be set; otherwise false.</returns>
        public bool SetNext(bool b)
        {
            Items[SetPosition++].SetState(b);
            return SetPosition < DataBitSize;
        }
    }
}
