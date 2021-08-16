// HandyDandy
// Copyright (c) 2021 Coding Enthusiast
// Distributed under the MIT software license, see the accompanying
// file LICENCE or http://www.opensource.org/licenses/mit-license.php.

using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace HandyDandy.Views
{
    public partial class TextGridView : UserControl
    {
        public TextGridView() => InitializeComponent();
        private void InitializeComponent() => AvaloniaXamlLoader.Load(this);
    }
}
