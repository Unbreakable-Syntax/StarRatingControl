# Description
This is a Star Rating Control implementation for C# WinForms that has been derived from [here](https://www.codeproject.com/articles/9117/csharp-star-rating-control), the original article has a broken link for the source code of the custom control, this repository is a rebuild of this custom control, with some other extra changes.

# Extra changes
As per the author stated, this custom control can be optimized by caching some values to optimize performance. This reimplementation applies that idea by caching the entire array once it has been generated, rather than caching individual data points. The render data for the stars will only be redrawn when any of the fields that are part of the rendering process (such as the margin, and star count) has been changed. Otherwise, calling Invalidate() will simply recolor the stars, making the OnMouseMove() and other methods to be at least somewhat faster.

As per the author, the custom control could also benefit from extra exposed brush style properties, which this reimplementation also does. The new properties are FillBrushStyle, StarColor, and GradientDirection. The FillBrushStyle can either be Solid or Gradient. The GradientDirection determines the gradient color direction application (if I described it correctly, which indicates on what direction should the first color fade to the second color), and the StarColor determines the color of the stars themselves, which is used on GradientDirection to apply the gradient color.

This reimplementation also provides an OnMouseDown() event to allow clicking of the stars, as well as the SelectedStar property, which is used to retrieve the value of the current selected star.

This reimplementation current has a tiny issue of not having any immediate changes if a star has been clicked, and the SelectedStarColor and HoverStarColor are different (I have found a simple fix, but it disables hovering until the mouse cursor has reentered the control border). For now, to avoid this visual issue, please ensure that the SelectedStarColor and HoverStarColor are the same.
