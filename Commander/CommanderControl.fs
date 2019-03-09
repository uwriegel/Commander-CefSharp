namespace Commander

type CommanderControl (leftView: CommanderView, rightView: CommanderView)  =
    let mutable focusedView: CommanderView = leftView
    
    let getOther () =
        if focusedView = leftView then rightView else leftView

    member this.OnFocus (id: string) = 
        focusedView <- if id = "left" then leftView else rightView

    member this.Copy () =
        focusedView.StartCopy <| getOther ()

    member this.CreateFolder () = focusedView.StartCreateFolder ()
    member this.AdaptPath () = 
        (getOther ()).ChangePath focusedView.Path
    member this.Refresh () = focusedView.Refresh ()
    