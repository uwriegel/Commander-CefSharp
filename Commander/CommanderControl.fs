namespace Commander

type CommanderControl (leftView: CommanderView, rightView: CommanderView)  =
    let mutable focusedView: CommanderView = leftView

    member this.OnFocus (id: string) = 
        focusedView <- if id = "left" then leftView else rightView



