module Editor
open LWContainer
open System.Windows.Forms
open System.Drawing
open LWC

type Editor() as this = 
    inherit LWContainer()

    let bs = new ButtonScheme(Size = SizeF(900.f, 30.f))
    let mutable startPoint : Point option = None
    let mutable currentPoint : Point option = None
    let mutable lastRectangle : Rectangle option = None
    let mutable lastLine = [|None; None|]
    let mutable lastBezier = [|None; None|]
    let mutable rectangleList = []
    let mutable lineList = []
    let mutable bezierList = []
   
    do bs.setParentToButton this
    do base.LWControls.Add(bs)

    override this.OnMouseDown e = 
        base.OnMouseDown e 
        if not(bs.HitTest(PointF (float32 e.X, float32 e.Y))) then
            match bs.ToolSelected with
                | Tool.None -> ()
                | Tool.Line -> startPoint <- Some(Point(e.X, e.Y-30)); lastLine.[0] <- startPoint
                | Tool.Rectangle -> startPoint <- Some(Point (e.X, e.Y-30))
                | Tool.Bezier -> startPoint <- Some(Point(e.X, e.Y-30)); lastBezier.[0] <- startPoint
                | _ -> ()
        this.Invalidate()


    override this.OnMouseMove e = 
        if not(bs.HitTest(PointF (float32 e.X, float32 e.Y))) then
            match bs.ToolSelected with
                | Tool.None -> ()
                | Tool.Line -> lastLine.[1] <- Some(Point(e.X, e.Y-30));
                | Tool.Rectangle -> (match (startPoint) with 
                                                        |(Some(p0)) ->  let xMin = System.Math.Min(p0.X, e.X)
                                                                        let yMin = System.Math.Min(p0.Y, e.Y-30)
                                                                        let xMax = System.Math.Max(p0.X, e.X)
                                                                        let yMax = System.Math.Max(p0.Y, e.Y-30)
                                                                        do lastRectangle <- Some(Rectangle(xMin, yMin, xMax-xMin, yMax-yMin))
                                                        | _ -> ())
                | Tool.Bezier -> lastBezier.[1] <- Some(Point(e.X, e.Y-30));
                | _ -> ()
        this.Invalidate()

    override this.OnMouseUp e =
        if lastRectangle.IsSome then 
            rectangleList <- lastRectangle.Value :: rectangleList
        if(lastLine.[0].IsSome && lastLine.[1].IsSome) then
            lineList <- (lastLine.[0].Value, lastLine.[1].Value) :: lineList
        if(lastBezier.[0].IsSome && lastBezier.[1].IsSome) then
            bezierList <- (lastBezier.[0].Value, lastBezier.[1].Value) :: bezierList
            
        this.Invalidate()
        startPoint <- None
        lastLine <- [|None; None|]
        lastBezier <- [|None; None|]
        currentPoint <- None
        lastRectangle <- None
        

    override this.OnPaint g =
        base.OnPaint(g) 
        g.Graphics.TranslateTransform(0.f, 30.f)
        for r in rectangleList do
            g.Graphics.DrawRectangle(Pens.Coral, r)
        for l in lineList do
            let (first, last) = l in
                g.Graphics.DrawLine(Pens.Coral, first, last)
        for b in bezierList do
            let (first, last) = b in
                g.Graphics.DrawBezier(Pens.Coral, first, Point((first.X+last.X)/3, (first.Y+last.Y)/3), Point((first.X+last.X)*2/3, (first.Y+last.Y)*2/3), last)
                //g.Graphics.DrawBezier(Pens.Coral, first, Point(100, 100), Point(200, 200), last)
        if(lastRectangle.IsSome) then g.Graphics.DrawRectangle(Pens.Orchid, lastRectangle.Value)
        if(lastLine.[0].IsSome && lastLine.[1].IsSome) then g.Graphics.DrawLine(Pens.Orchid, lastLine.[0].Value, lastLine.[1].Value)
        if(lastBezier.[0].IsSome && lastBezier.[1].IsSome) then
            let firstControlPoint = Point((lastBezier.[0].Value.X + lastBezier.[1].Value.X)/3, (lastBezier.[0].Value.Y + lastBezier.[1].Value.Y)/3)
            let secondControlPoint = Point((lastBezier.[0].Value.X + lastBezier.[1].Value.X)*2/3, (lastBezier.[0].Value.Y + lastBezier.[1].Value.Y)*2/3)
            g.Graphics.DrawBezier(Pens.Orchid, lastBezier.[0].Value, firstControlPoint, secondControlPoint,lastBezier.[1].Value)