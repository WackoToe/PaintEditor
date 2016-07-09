﻿module LWC
open System.Windows.Forms
open System.Drawing


// Lightweight controls: astrazione programmativa che imita i controlli grafici
type LWC() =
  let mutable parent : Control = null
  let mutable location = PointF()
  let mutable size = SizeF()
  
  abstract OnMouseDown : MouseEventArgs -> unit
  default this.OnMouseDown _ = ()

  abstract OnMouseMove : MouseEventArgs -> unit
  default this.OnMouseMove _ = ()

  abstract OnMouseUp : MouseEventArgs -> unit
  default this.OnMouseUp _ = ()

  abstract OnPaint : PaintEventArgs -> unit
  default this.OnPaint _ = ()

  abstract HitTest : PointF -> bool
  default this.HitTest p =
    (RectangleF(PointF(), size)).Contains(p)


  member this.Invalidate() =
    if parent <> null then parent.Invalidate();

  member this.Location
    with get() = location
    and set(v) = location <- v; this.Invalidate()

  member this.Size
    with get() = size
    and set(v) = size <- v; this.Invalidate()

  member this.Parent
    with get() = parent
    and set(v) = parent <- v


