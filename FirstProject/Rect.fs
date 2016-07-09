module Rect

open System.Windows.Forms
open System.Drawing

type Rect() =

    let makeRectangle (center:PointF) (size:single) (g:Graphics) =
        let rectangle = Rectangle(int (center.X-size/2.f), int(center.Y-size/2.f), int size, int size)
        g.DrawRectangle(Pens.Black, rectangle)


