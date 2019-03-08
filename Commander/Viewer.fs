namespace Commander

open System.Windows.Forms
open System.Drawing

type Viewer(parent: Control) =
    let mutable viewer: Control option = None

    member this.SetViewerRatio(ratio: double) = 
        ()

    member this.Show show =
        match show with
        | true -> 
            let size = parent.ClientSize
            viewer <- Some (new Control(Size = new Size(size.Width, 0),
                                Location = new Point(0, size.Height),
                                Parent = parent,
                                Anchor = (AnchorStyles.Left ||| AnchorStyles.Right ||| AnchorStyles.Bottom)))
            match viewer with 
            | Some viewer ->
                parent.Controls.Add(viewer)
                parent.Controls.SetChildIndex(viewer, 0)
                viewer.Visible <- false
            | None -> ()
        | false ->
            match viewer with 
            | Some viewer ->
                parent.Controls.Remove(viewer)
                viewer.Dispose()
            | None -> ()


