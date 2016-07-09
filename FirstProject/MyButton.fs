module MyButton

open LWC
open System.Drawing
open System.Windows.Forms

type MyButton () as this =
    inherit LWC()
    let mutable text = ""
    let mutable selected = false
    let mutable textColor = Brushes.White
    let mutable color : Color = Color.White
    let mutable location = PointF(0.f,0.f)
    let mutable border = Pens.Green
    let mutable font = new Font("Arial", 13.f)
    let mutable onMouseDownListener = (fun _ -> ())
    
    member this.OnMouseDownListener
        with get() = onMouseDownListener
        and set(v) = onMouseDownListener <- v
    member this.Text 
        with get() = text
        and set(v) = text <- v; this.Invalidate()
    member this.Color 
        with get() = color
        and set(v) = color <- v; this.Invalidate()
    member this.Location
        with get() = location
        and set(v) = location <- v; this.Invalidate()
    member this.Border
        with get() = border
        and set(v) = border <- v; this.Invalidate()
    member this.Font
        with get() = font
        and set(v) = font <- v; this.Invalidate()
    member this.TextColor 
        with get() = textColor
        and set(v) = textColor <- v; this.Invalidate()
    member this.Selected
        with get() = selected

    override this.OnMouseDown (e) = 
        this.OnMouseDownListener()
        
    override this.HitTest (p:PointF) =
        let rect = RectangleF(this.Location.X, this.Location.Y, float32 base.Size.Width , float32 base.Size.Height)
        let click = rect.Contains(p)
        if click then
            selected <- true
            this.Invalidate()
        else selected <- false
        click

    override this.OnPaint(e:PaintEventArgs) =
        let g = e.Graphics
        let rect = Rectangle(int location.X, int location.Y, int this.Size.Width, int this.Size.Height)
        g.DrawRectangle(border, rect)
        if selected then
            g.FillRectangle(new SolidBrush(Color.Green), rect)
        else
            g.FillRectangle(new SolidBrush(Color.Black), rect)
        let fs = g.MeasureString(text, font)
        let txShift = PointF((this.Size.Width-fs.Width) / 2.f, (this.Size.Height - fs.Height) / 2.f)
        g.DrawString(text, font, textColor, txShift.X + location.X, txShift.Y + location.Y)


        

