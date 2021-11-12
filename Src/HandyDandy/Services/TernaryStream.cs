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
using System.Linq;

namespace HandyDandy.Services
{
    public class TernaryStream : InpcBase
    {
        public TernaryStream(int binarySize, OutputType ot) : this(binarySize, 0, null, false, ot)
        {
        }

        public TernaryStream(int binarySize, int disabledCount, IChecksum cs, bool dynamicChecksum, OutputType ot)
        {
            DataSize = (binarySize - disabledCount) / 8;
            outputType = ot;
            checksum = cs;
            dynamicCS = dynamicChecksum;
            Items = new Ternary[binarySize];

            for (int i = 0; i < binarySize; i++)
            {
                bool dis = i < binarySize - disabledCount;
                Items[i] = new Ternary(dis);
                Items[i].PropertyChanged += Item_PropertyChanged;
            }
        }


        private readonly OutputType outputType;
        private readonly IChecksum checksum;
        private int position;
        private readonly bool dynamicCS;

        public Ternary[] Items { get; }
        public int Size => Items.Length;
        public int DataSize { get; }

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
            Span<byte> ba = ToBytes();
            if (e.PropertyName != nameof(Ternary.State) || sender is not Ternary temp || !temp.IsEnabled || !dynamicCS)
            {
                IsAllSet = Items.All(x => x.State != TernaryState.Unset);
                SetResult(ba);
                return;
            }

            byte[] cs = checksum.Compute(ba.Slice(0, DataSize));
            for (int i = 0; i < cs.Length; i++)
            {
                Items[^(cs.Length - i)].SetState(cs[i]);
            }

            IsAllSet = Items.All(x => x.State != TernaryState.Unset);
            SetResult(ba);
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
                    using ElectrumMnemonic temp = new(bytes.ToArray(), ElectrumMnemonic.MnemonicType.SegWit);
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

        private string _res = string.Empty;
        public string Result
        {
            get => _res;
            set => SetField(ref _res, value);
        }

        public byte[] ToBytes()
        {
            int x = (outputType == OutputType.ElectrumMnemonic && Items.Length % 8 == 4) ? 1 : 0;
            byte[] ba = new byte[Items.Length / 8 + x];
            for (int i = 0, j = 0; i < ba.Length - x; i++, j += 8)
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
            if (x != 0)
            {
                ba[^1] = (byte)(Items[128].ToBit() << 7 |
                                Items[129].ToBit() << 6 |
                                Items[130].ToBit() << 5 |
                                Items[131].ToBit() << 4);
            }
            return ba;
        }

        public Ternary[] Next(int count)
        {
            Ternary[] result = new Ternary[count];
            for (int i = 0; i < result.Length; i++)
            {
                result[i] = Items[i + position];
            }
            position += count;
            return result;
        }
    }
}
