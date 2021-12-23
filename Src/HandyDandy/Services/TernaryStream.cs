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
        public TernaryStream(int binarySize, OutputType ot) : this(binarySize, 0, null, ot)
        {
        }

        public TernaryStream(int binarySize, int disabledCount, IChecksum? cs, OutputType ot)
        {
            DataBitSize = binarySize - disabledCount;
            DataByteSize = (binarySize - disabledCount) / 8;
            outputType = ot;
            checksum = cs;
            Items = new Ternary[binarySize];

            for (int i = 0; i < binarySize; i++)
            {
                bool dis = i < binarySize - disabledCount;
                Items[i] = new Ternary(dis);
                Items[i].PropertyChanged += Item_PropertyChanged;
            }
        }


        private readonly OutputType outputType;
        private readonly IChecksum? checksum;
        private int readPosition, setPosition;

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
            IsAllSet = Items.All(x => x.State != TernaryState.Unset);
        }

        private void SetResult(ReadOnlySpan<byte> bytes)
        {
            if (!IsAllSet)
            {
                Result = "Not all bits are set. The result is still weak and can not be copied.";
                return;
            }

            if (outputType == OutputType.PrivateKey)
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
            else if (outputType == OutputType.Bip39Mnemonic)
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
            else if (outputType == OutputType.ElectrumMnemonic)
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
                result[i] = Items[i + readPosition];
            }
            readPosition += count;
            return result;
        }

        public bool SetNext(bool b)
        {
            Items[setPosition].SetState(b);
            return setPosition == Items.Length;
        }
    }
}
