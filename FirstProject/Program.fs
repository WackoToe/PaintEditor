// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.
namespace FirstProgram
module FirstProgram =
    open System.Windows.Forms
    open System.Drawing
    open LWC
    open LWContainer
    open Editor

    [<EntryPoint>]
    let main argv = 

        let f = new Form(Text="FirstProject", Size=Size(915, 500), MinimumSize=Size(450, 450))
        
        let e = new Editor(Size = f.Size)
        f.Controls.Add(e)

        f.Show()
        Application.Run(f)
        0 // restituisci un intero come codice di uscita
