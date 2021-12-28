// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using HandyDandy.ViewModels;

namespace HandyDandy.Views
{
    public partial class WithKeyboardView : UserControl
    {
        public WithKeyboardView()
        {
            InitializeComponent();
            KeyDown += WithKeyboardView_KeyDown;
        }

        private void WithKeyboardView_KeyDown(object? sender, KeyEventArgs e)
        {
            var ctx = DataContext as WithKeyboardViewModel;
            if (ctx is not null)
            {
                if (ctx.CanSetNext)
                {
                    if (e.Key == Key.D0 || e.Key == Key.NumPad0 || e.Key == Key.Space || e.Key == Key.Escape)
                    {
                        ctx.SetNextBit(false);
                    }
                    else
                    {
                        ctx.SetNextBit(true);
                    }
                }
            }
        }

        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
