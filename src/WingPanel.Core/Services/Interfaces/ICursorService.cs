using System;
using WingPanel.Core.Models;

namespace WingPanel.Core.Services.Interfaces;

public interface ICursorService
{
    event EventHandler<PixelPoint>? CursorMoved;

    PixelPoint GetCursorPosition();

    void SetCursorPosition(PixelPoint point);
}
