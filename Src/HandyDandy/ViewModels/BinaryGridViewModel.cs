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

namespace HandyDandy.ViewModels
{
    public class BinaryGridViewModel : ViewModelBase
    {
        public BinaryGridViewModel()
        {
            Buttons = Enumerable.Range(0, 8).Select(i => new Ternary()).ToArray();
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
            Result = new byte[Buttons.Length / 8];

            outputType = ot;
        }

        private void Button_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            Debug.Assert(Buttons is not null && Buttons.Length > 0 && Buttons.Length % 8 == 0);
            Debug.Assert(Result is not null && Result.Length == Buttons.Length / 8);

            byte[] temp = new byte[Result.Length];

            int bitIndex = 0;
            int byteIndex = 0;
            while (bitIndex < Buttons.Length)
            {
                int val = 0;
                for (int i = 0; i < 8; i++)
                {
                    int bit = Buttons[bitIndex + i].State.Value == TernaryState.One ? 1 : 0;
                    val |= bit << (7 - i);
                }
                Debug.Assert(val <= byte.MaxValue);
                temp[byteIndex++] = (byte)val;
                bitIndex += 8;
            }

            Result = temp;
        }

        private readonly OutputType outputType;

        public Ternary[] Buttons { get; private set; }

        private byte[] _res;
        public byte[] Result
        {
            get => _res;
            set => SetField(ref _res, value);
        }

        [DependsOnProperty(nameof(Result))]
        public string ResultHex => Result == null ? string.Empty : Base16.Encode(Result);

        [DependsOnProperty(nameof(Result))]
        public string ResultOutput
        {
            get
            {
                if (Result == null)
                {
                    return string.Empty;
                }
                else if (outputType == OutputType.PrivateKey)
                {
                    try
                    {
                        using PrivateKey temp = new(Result);
                        return temp.ToWif(true);
                    }
                    catch (Exception ex)
                    {
                        return ex.ToString();
                    }
                }
                else if (outputType == OutputType.Mnemonic)
                {
                    try
                    {
                        using BIP0039 temp = new(Result);
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
            }
        }

        public void CopyHex()
        {
            Application.Current.Clipboard.SetTextAsync(ResultHex);
        }

        public void CopyOutput()
        {
            Application.Current.Clipboard.SetTextAsync(ResultOutput);
        }
    }
}
