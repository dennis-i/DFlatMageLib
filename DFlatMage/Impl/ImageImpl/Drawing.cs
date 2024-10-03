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


    public void DrawRect(Range planes, Rect rect, int val)
    {
        for (int plane = planes.Start.Value; plane < planes.End.Value; ++plane)
        {
            DrawRect(plane, rect, val);
        }
    }

    public void DrawCircle(int plane, Point center, int radius, int val)
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

    public void DrawLine(int plane, int row1, int col1, int row2, int col2, int val)
    {
        ThrowIfNotInRange(plane, NumPlanes);
        ThrowIfNotInRange(row1, _nRows);
        ThrowIfNotInRange(row2, _nRows);
        ThrowIfNotInRange(col1, _nCols);
        ThrowIfNotInRange(col2, _nCols);


        // Calculate the absolute differences
        int dx = Math.Abs(col2 - col1);
        int dy = Math.Abs(row2 - row1);

        // Determine the direction of the line
        int sx = (col1 < col2) ? 1 : -1;  // Step in the x direction
        int sy = (row1 < row2) ? 1 : -1;  // Step in the y direction

        int err = (dx > dy ? dx : -dy) / 2;  // Initial error value

        while (true)
        {
            SetPixUnsafe(plane, row1, col1, val);  // Plot the current point

            // Check if we have reached the end point
            if (col1 == col2 && row1 == row2) break;

            // Store the error value temporarily
            int e2 = err;

            // Update the error term and coordinates for shallow lines (x-major)
            if (e2 > -dx)
            {
                err -= dy;
                col1 += sx;
            }

            // Update the error term and coordinates for steep lines (y-major)
            if (e2 < dy)
            {
                err += dx;
                row1 += sy;
            }
        }

    }
}


