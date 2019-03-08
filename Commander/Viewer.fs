namespace Commander

open System.Windows.Forms
open System.Drawing
open System

type Viewer(parent: Control) =
    let mutable viewer: Control option = None
    let mutable statusHeight = 0

    member this.SetViewerRatio(ratio: double) = 
        let setRation () = 
            match viewer with 
            | Some viewer ->
                let size = parent.ClientSize
                viewer.Height <- int (double size.Height * ratio)
                viewer.Location <- new Point(0, size.Height - viewer.Height - statusHeight)
            | None -> ()
        parent.Invoke(Action(setRation)) 

    member this.SetStatusRatio(ratio: double) = 
        let size = parent.ClientSize
        parent.Invoke(Action(fun () -> statusHeight <- int (double size.Height * ratio)))

    member this.SetFile(file: string) =
        let setFile (viewer: Control) = 
            if file = null && viewer.Visible then
                viewer.Visible <- false
            elif file <> null && not viewer.Visible then
                viewer.Visible <- true
            if file <> null && file.EndsWith(".pdf", StringComparison.InvariantCultureIgnoreCase) then
                //let dateTime = DateTime.Now
                //(parent as MainForm).Browser.LostFocus += FocusControl;

                //void FocusControl(object s, EventArgs e)
                //{
                //    if (DateTime.Now - dateTime > TimeSpan.FromSeconds(3))
                //        (parent as MainForm).Browser.LostFocus -= FocusControl;
                //    parent.BeginInvoke((Action)(() => (parent as MainForm).Browser.Focus()));
                //}
                       
                //viewer.Controls.Clear();
                //var browser = new ChromiumWebBrowser("");
                //viewer.Controls.Add(browser);
                ////browser.Load($"file:///{file}");
                //browser.Load($"https://www.caseris.de");
                ()

        match viewer with
        | Some viewer -> parent.Invoke(Action(fun () -> setFile viewer)) |> ignore
        | None -> ()

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


