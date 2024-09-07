using DFlatMage.Common;

namespace DFlatMage.Impl.ImageImpl;

internal partial class ImageImpl
{

    public void DrawRect(int plane, Rect rect, int val)
    {
        DrawLine(plane, rect.Y, rect.X, rect.Y, rect.X + rect.Width, val);
        DrawLine(plane, rect.Y, rect.X, rect.Y + rect.Height, rect.X, val);
        DrawLine(plane, rect.Y + rect.Height, rect.X, rect.Y + rect.Height, rect.X + rect.Width, val);
        DrawLine(plane, rect.Y, rect.X + rect.Width, rect.Y + rect.Height, rect.X + rect.Width, val);
    }
    public void DrawLine(int plane, int row1, int col1, int row2, int col2, int val)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        ThrowIfNotInRange(row1, _nRows);
        ThrowIfNotInRange(row2, _nRows);
        ThrowIfNotInRange(col1, _nCols);
        ThrowIfNotInRange(col2, _nCols);

        int xSize = col2 - col1;
        int ySize = row2 - row1;
        int cnt = 0;

        int startY = (row2 > row1) ? row1 : row2;
        int endY = (row2 > row1) ? row2 : row1;

        int startX = (col2 > col1) ? col1 : col2;
        int endX = (col2 > col1) ? col2 : col1;

        if (xSize > ySize)
        {
            double factor = ySize / (double)xSize;
            for (int x = startX; x <= endX; x++)
            {
                int y = (int)Math.Round(startY + cnt * factor);
                SetPix(plane, y, x, val);
                cnt++;
            }
        }
        else
        {
            double factor = xSize / (double)ySize;
            for (int y = startY; y <= endY; y++)
            {
                int x = (int)Math.Round(startX + cnt * factor);
                SetPix(plane, y, x, val);
                cnt++;
            }
        }
    }


    public void DrawCirle(int plane, Point center, int radius, int val)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        ThrowIfNotInRange(center.Y + radius, _nRows);
        ThrowIfNotInRange(center.X + radius, _nCols);



        for (double angle = 0; angle <= 360; angle += 0.1)
        {
            double radians = angle * (Math.PI / 180);
            int newx = (int)(center.X + Math.Cos(radians) * radius);
            int newy = (int)(center.Y + Math.Sin(radians) * radius);

            SetPix(plane, newx, newy, val);

        }
    }
}


