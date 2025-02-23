namespace Dynamic_Island.Extensions;

public static class CornerRadiusExtensions
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
}