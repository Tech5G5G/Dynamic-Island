namespace Dynamic_Island.Extensions;

/// <summary>Contains extension methods for geometric types and structures, such as <see cref="CornerRadius"/> and <see cref="Rect"/>.</summary>
public static class GeometricExtensions
{
    /// <summary>Subtracts two <see cref="CornerRadius"/> from each other; that is, <paramref name="x"/> - <paramref name="y"/>.</summary>
    /// <param name="x">The <see cref="CornerRadius"/> to subtract from.</param>
    /// <param name="y">The <see cref="CornerRadius"/> to subtract.</param>
    /// <returns>The difference between <paramref name="x"/> and <paramref name="y"/>.</returns>
    public static CornerRadius Subtract(this CornerRadius x, CornerRadius y) => new()
    {
        BottomLeft = x.BottomLeft - y.BottomLeft,
        BottomRight = x.BottomRight - y.BottomRight,
        TopLeft = x.TopLeft - y.TopLeft,
        TopRight = x.TopRight - y.TopRight
    };

    /// <summary>Converts <paramref name="rect"/> to a <see cref="RectInt32"/>, rounding some of the dimensions.</summary>
    /// <param name="rect">The <see cref="Rect"/> to convert.</param>
    /// <returns><paramref name="rect"/> converted to a <see cref="RectInt32"/>.</returns>
    public static RectInt32 ToRectInt32(this Rect rect) => new((int)Math.Round(rect.X), (int)Math.Round(rect.Y), (int)Math.Round(rect.Width), (int)Math.Round(rect.Height));
}