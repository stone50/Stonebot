namespace Stonebot.UI {
    using Avalonia.Controls;
    using System.Linq;

    internal class SGrid : Grid {
        public SGrid(GridLength[] rowFormats, GridLength[] columnFormats, Controls children) {
            RowDefinitions.AddRange(rowFormats.Select(rowFormat => new RowDefinition(rowFormat)));
            ColumnDefinitions.AddRange(columnFormats.Select(columnFormat => new ColumnDefinition(columnFormat)));
            Children.EnsureCapacity(children.Count);
            for (var i = 0; i < children.Count; ++i) {
                var child = children[i];
                Children.Add(child);
                SetColumn(child, i % ColumnDefinitions.Count);
                SetRow(child, i / ColumnDefinitions.Count);
            }
        }
    }
}
