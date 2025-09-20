using System.Collections.Generic;
using WingPanel.Core.Models;

namespace WingPanel.Core.Utilities;

public sealed class WindowInfoHandleComparer : IEqualityComparer<WindowInfo>
{
    public bool Equals(WindowInfo? x, WindowInfo? y)
    {
        if (ReferenceEquals(x, y))
        {
            return true;
        }

        if (x is null || y is null)
        {
            return false;
        }

        return x.Handle == y.Handle;
    }

    public int GetHashCode(WindowInfo obj) => obj.Handle.GetHashCode();
}
