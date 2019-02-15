namespace Commander

type CommanderView ()  =
    member this.Ready () = 
        ()
    
    member this.Copy (otherView: CommanderView) = ()
    member this.CreateFolder () = ()
    member this.AdaptPath (path: string) = ()
    member this.Path with get() = ""
    member this.ShowHidden (showHidden: bool) = ()
    member this.Refresh () = ()