module LWContainer
open System.Windows.Forms
open System.Drawing
open LWC
open MyButton

type Tool =
    | None = -1
    | Pan = 0
    | ZoomIn = 1
    | ZoomOut = 2
    | Line = 3
    | Rectangle = 4
    | Ellipse = 5
    | Bezier = 6
    | Simulator = 7
    | CleanAll = 8  

type ButtonScheme() as this =
    inherit LWC()
    
    let mutable toolSelected : Tool = Tool.None
    let buttons = ResizeArray<MyButton>()

    let bt0 = new MyButton(Location=PointF(0.f,0.f), Size=SizeF(99.f, 30.f), Text = "Pan", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.Pan ))
    do buttons.Add(bt0)
    let bt1 = new MyButton(Location=PointF(100.f, 0.f), Size=SizeF(99.f,30.f), Text = "Zoom In", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.ZoomIn ))
    do buttons.Add(bt1)
    let bt2 = new MyButton(Location=PointF(200.f, 0.f), Size=SizeF(99.f,30.f), Text = "Zoom Out", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.ZoomOut ))
    do buttons.Add(bt2)
    let bt3 = new MyButton(Location=PointF(300.f, 0.f), Size=SizeF(99.f,30.f), Text = "Line", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.Line ))
    do buttons.Add(bt3)
    let bt4 = new MyButton(Location=PointF(400.f, 0.f), Size=SizeF(99.f,30.f), Text = "Rectangle", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.Rectangle ))
    do buttons.Add(bt4)
    let bt5 = new MyButton(Location=PointF(500.f, 0.f), Size=SizeF(99.f,30.f), Text = "Ellipse", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.Ellipse ))
    do buttons.Add(bt5)
    let bt6 = new MyButton(Location=PointF(600.f, 0.f), Size=SizeF(99.f,30.f), Text = "Bezier", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.Bezier ))
    do buttons.Add(bt6)
    let bt7 = new MyButton(Location=PointF(700.f, 0.f), Size=SizeF(99.f,30.f), Text = "Simulator", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.Simulator ) )
    do buttons.Add(bt7)
    let bt8 = new MyButton(Location=PointF(800.f, 0.f), Size=SizeF(99.f,30.f), Text = "Clean All", OnMouseDownListener = ( fun _ -> toolSelected<-Tool.CleanAll))
    do buttons.Add(bt8)
    
    let mutable record = 0
    let mutable count = 0

    override this.OnMouseDown e =
        let mutable clicked = false
        for b in buttons do
            if b.HitTest (PointF (float32 e.X, float32 e.Y)) then
                b.OnMouseDown(e)      
        printfn "%s" (toolSelected.ToString())

    member this.setParentToButton x = 
        this.Parent <- x
        for b in buttons do
            b.Parent <- x

    member this.ToolSelected
        with get() = toolSelected

    override this.OnPaint(e) =
        bt0.OnPaint e
        bt1.OnPaint e
        bt2.OnPaint e
        bt3.OnPaint e
        bt4.OnPaint e
        bt5.OnPaint e
        bt6.OnPaint e
        bt7.OnPaint e
        bt8.OnPaint e


type LWContainer() as this =
  inherit UserControl()

  
  let controls = ResizeArray<LWC>()


  let cloneMouseEvent (c:LWC) (e:MouseEventArgs) =
    new MouseEventArgs(e.Button, e.Clicks, e.X - int(c.Location.X), e.Y - int(c.Location.Y), e.Delta)

  let correlate (e:MouseEventArgs) (f:LWC->MouseEventArgs->unit) =
    let mutable found = false
    for i in { (controls.Count - 1) .. -1 .. 0 } do
      if not found then
        let c = controls.[i]
        if c.HitTest(PointF(single(e.X) - c.Location.X, single(e.Y) - c.Location.Y)) then
          found <- true
          f c (cloneMouseEvent c e)

  let mutable captured : LWC option = None

  do this.DoubleBuffered <- true

  member this.LWControls
    with get() = controls

  override this.OnMouseDown e =
    correlate e (fun c ev -> captured <- Some(c); c.OnMouseDown(ev))
    base.OnMouseDown e

  override this.OnMouseUp e =
    correlate e (fun c ev -> c.OnMouseUp(ev))
    match captured with
    | Some c -> c.OnMouseUp(cloneMouseEvent c e); captured <- None
    | None  -> ()
    base.OnMouseUp e

  override this.OnMouseMove e =
    correlate e (fun c ev -> c.OnMouseMove(ev))
    match captured with
    | Some c -> c.OnMouseMove(cloneMouseEvent c e)
    | None  -> ()
    base.OnMouseMove e

  override this.OnPaint e =
    controls |> Seq.iter (fun c ->
      let s = e.Graphics.Save()
      e.Graphics.TranslateTransform(c.Location.X, c.Location.Y)
      e.Graphics.Clip <- new Region(RectangleF(0.f, 0.f, c.Size.Width, c.Size.Height))
      let r = e.Graphics.ClipBounds
      let evt = new PaintEventArgs(e.Graphics, new Rectangle(int(r.Left), int(r.Top), int(r.Width), int(r.Height)))
      c.OnPaint evt
      e.Graphics.Restore(s)
    )
    base.OnPaint(e)