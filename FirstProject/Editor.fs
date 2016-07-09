module Editor
open LWContainer
open System.Windows.Forms
open System.Drawing
open LWC

type Editor() as this = 
    inherit LWContainer()

    let bs = new ButtonScheme(Size = SizeF(900.f, 30.f))
    let mutable startPoint : Point option = None
    let mutable lastRectangle : Rectangle option = None
    let mutable rectangleList = []
   
    do bs.setParentToButton this
    do base.LWControls.Add(bs)

    override this.OnMouseDown e = 
        base.OnMouseDown e 
        if not(bs.HitTest(PointF (float32 e.X, float32 e.Y))) then
            match bs.ToolSelected with
                | Tool.None -> ()
                | Tool.Line -> startPoint <- Some(Point(e.X, e.Y-30))
                | Tool.Rectangle -> startPoint <- Some(Point (e.X, e.Y-30))
                | _ -> ()
        this.Invalidate()


    override this.OnMouseMove e = 
        if not(bs.HitTest(PointF (float32 e.X, float32 e.Y))) then
            match bs.ToolSelected with
                | Tool.None -> ()
                | Tool.Rectangle -> (match (startPoint) with
                                        |(Some(p0)) -> let xMin = System.Math.Min(p0.X, e.X)
                                                       let yMin = System.Math.Min(p0.Y, e.Y-30)
                                                       let xMax = System.Math.Max(p0.X, e.X)
                                                       let yMax = System.Math.Max(p0.Y, e.Y-30)
                                                       do lastRectangle <- Some(Rectangle(xMin, yMin, xMax-xMin, yMax-yMin))
                                        | _ -> ())
                | _ -> ()
        this.Invalidate()

    override this.OnMouseUp e =
        if lastRectangle.IsSome then 
            rectangleList <- lastRectangle.Value :: rectangleList
        startPoint <- None
        lastRectangle <- None
        this.Invalidate()

    override this.OnPaint g =
        base.OnPaint(g) 
        g.Graphics.TranslateTransform(0.f, 30.f)
        for r in rectangleList do
            g.Graphics.DrawRectangle(Pens.Coral, r)
        if(lastRectangle.IsSome) then g.Graphics.DrawRectangle(Pens.Orchid, lastRectangle.Value)     