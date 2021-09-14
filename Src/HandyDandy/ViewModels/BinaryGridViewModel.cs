// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Autarkysoft.Bitcoin.Cryptography.Asymmetric.KeyPairs;
using Autarkysoft.Bitcoin.Encoders;
using Autarkysoft.Bitcoin.ImprovementProposals;
using Avalonia;
using HandyDandy.Models;
using HandyDandy.MVVM;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace HandyDandy.ViewModels
{
    public class BinaryGridViewModel : ViewModelBase
    {
        public BinaryGridViewModel() : this(8, OutputType.PrivateKey)
        {
        }

        public BinaryGridViewModel(int len, OutputType ot)
        {
            if (len <= 0 || len % 8 != 0)
                throw new ArgumentException("Bit length must be divisible by 8.", nameof(len));

            Buttons = Enumerable.Range(0, len).Select(i => new Ternary()).ToArray();
            foreach (var item in Buttons)
            {
                item.PropertyChanged += Button_PropertyChanged;
            }
            Bytes = new byte[Buttons.Length / 8];

            outputType = ot;
            CopyButtonName = ot switch
            {
                OutputType.PrivateKey => "Copy WIF",
                OutputType.Bip39Mnemonic => "Copy mnemonic",
                OutputType.ElectrumMnemonic => "Copy mnemonic",
                _ => throw new NotImplementedException(),
            };
        }

        private void Button_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Debug.Assert(Buttons is not null && Buttons.Length > 0 && Buttons.Length % 8 == 0);
            Debug.Assert(Bytes is not null && Bytes.Length == Buttons.Length / 8);

            // TODO: this could be optimized by getting the index of the item that is changed and changing the corresponding
            //       byte instead of the whole array.

            byte[] temp = new byte[Bytes.Length];

            int bitIndex = 0;
            int byteIndex = 0;
            bool b = true;
            while (bitIndex < Buttons.Length)
            {
                int val = 0;
                for (int i = 0; i < 8; i++)
                {
                    if (Buttons[bitIndex + i].State.Value == TernaryState.Unset)
                    {
                        b = false;
                    }
                    val |= Buttons[bitIndex + i].ToBit() << (7 - i);
                }
                Debug.Assert(val <= byte.MaxValue);
                temp[byteIndex++] = (byte)val;
                bitIndex += 8;
            }

            Bytes = temp;
            IsComplete = b;
        }

        private readonly OutputType outputType;


        public string CopyButtonName { get; }

        private bool _isComplete;
        public bool IsComplete
        {
            get => _isComplete;
            set => SetField(ref _isComplete, value);
        }

        public Ternary[] Buttons { get; private set; }

        private byte[] _ba = Array.Empty<byte>();
        public byte[] Bytes
        {
            get => _ba;
            set => SetField(ref _ba, value);
        }

        [DependsOnProperty(nameof(Bytes))]
        public string Result
        {
            get
            {
                StringBuilder sb = new();
                if (!IsComplete)
                {
                    sb.AppendLine("Not all bits are set. The result is still weak and can not be copied.");
                }

                if (outputType == OutputType.PrivateKey)
                {
                    try
                    {
                        using PrivateKey temp = new(Bytes);
                        return temp.ToWif(true);
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        sb.AppendLine("The current bit stream is creating an out of range key.");
                        sb.AppendLine($"{ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        sb.AppendLine("An error occurred while converting the bit stream to a private key.");
                        sb.AppendLine($"Error message: {ex.Message}");
                    }
                }
                else if (outputType == OutputType.Bip39Mnemonic)
                {
                    try
                    {
                        using BIP0039 temp = new(Bytes);
                        return temp.ToMnemonic();
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }
                else if (outputType == OutputType.ElectrumMnemonic)
                {
                    try
                    {
                        using ElectrumMnemonic temp = new(Bytes, ElectrumMnemonic.MnemonicType.SegWit);
                        return temp.ToMnemonic();
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }
                else
                {
                    return "Undefined.";
                }

                return sb.ToString();
            }
        }

        public void CopyHex()
        {
            Debug.Assert(Bytes is not null && Bytes.Length != 0);
            Debug.Assert(IsComplete);

            string hex = Base16.Encode(Bytes);
            Application.Current.Clipboard.SetTextAsync(hex);
        }

        public void CopyOutput()
        {
            try
            {
                string res = outputType switch
                {
                    OutputType.PrivateKey => new PrivateKey(Bytes).ToWif(true),
                    OutputType.Bip39Mnemonic => new BIP0039(Bytes).ToMnemonic(),
                    OutputType.ElectrumMnemonic => new ElectrumMnemonic(Bytes, ElectrumMnemonic.MnemonicType.SegWit).ToMnemonic(),
                    _ => throw new NotImplementedException()
                };

                Application.Current.Clipboard.SetTextAsync(res);
            }
            catch
            {
            }
        }
    }
}
