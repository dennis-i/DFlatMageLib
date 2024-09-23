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

        if (col2 >= col1)
        {
            if (row2 >= row1)
            {
                if (col2 - col1 >= row2 - row1)
                    MidPointLineSe8(val, plane, col1, row1, col2, row2);
                else
                    MidPointLineSe7(val, plane, row1, col1, row2, col2);
            }
            else
            {
                if (col2 - col1 >= row1 - row2)
                    MidPointLineSe1(val, plane, col1, row1, col2, 2 * row1 - row2);
                else
                    MidPointLineSe2(val, plane, row1, col1, row1 + (row1 - row2), col2);
            }
        }
        else
            DrawLine(plane, row2, col2, row1, col1, val);
    }

    private void MidPointLineSe8(int val, int plane, int x0, int y0, int x1, int y1)
    {


        int dx = x1 - x0;
        int dy = y1 - y0;
        int d = 2 * dy - dx;
        int incrE = 2 * dy;
        int incrSE = 2 * (dy - dx);

        SetPixUnsafe(plane, y0, x0, val);


        while (x0 < x1)
        {
            if (d <= 0)
            {
                d += incrE;
            }
            else
            {
                d += incrSE;
                y0++;
            }
            SetPixUnsafe(plane, y0, ++x0, val);
        }
    }

    private void MidPointLineSe2(int val, int plane, int x0, int y0, int x1, int y1)
    {


        int dx = x1 - x0;
        int dy = y1 - y0;
        int d = 2 * dy - dx;
        int incrE = 2 * dy;
        int incrSE = 2 * (dy - dx);

        SetPixUnsafe(plane, x0, y0, val);

        while (dx > 0)
        {
            if (d <= 0)
            {
                d += incrE;
            }
            else
            {
                d += incrSE;
                y0++;
            }
            dx--;
            SetPixUnsafe(plane, --x0, y0, val);
        }
    }

    private void MidPointLineSe7(int val, int plane, int x0, int y0, int x1, int y1)
    {


        int dx = x1 - x0;
        int dy = y1 - y0;
        int d = 2 * dy - dx;
        int incrE = 2 * dy;
        int incrSE = 2 * (dy - dx);

        SetPixUnsafe(plane, x0, y0, val);

        while (x0 < x1)
        {
            if (d <= 0)
            {
                d += incrE;
            }
            else
            {
                d += incrSE;
                y0++;
            }
            SetPixUnsafe(plane, ++x0, y0, val);
        }
    }

    private void MidPointLineSe1(int val, int plane, int x0, int y0, int x1, int y1)
    {


        int dx = x1 - x0;
        int dy = y1 - y0;
        int d = 2 * dy - dx;
        int incrE = 2 * dy;
        int incrSE = 2 * (dy - dx);



        SetPixUnsafe(plane, y0, x0, val);

        while (x0 < x1)
        {
            if (d <= 0)
            {
                d += incrE;
            }
            else
            {
                d += incrSE;
                y0--;
            }
            SetPixUnsafe(plane, y0, ++x0, val);
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

            SetPixUnsafe(plane, newx, newy, val);

        }
    }

    public void DrawPath(int plane, IReadOnlyList<Point> path, int val)
    {
        if (path.Count < 2)
            throw new ArgumentException($"path should have more than one point,got:{path.Count}");

        for (int i = 1; i < path.Count; ++i)
            DrawLine(plane, path[i - 1].Y, path[i - 1].X, path[i].Y, path[i].X, val);
    }
}


