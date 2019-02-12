namespace Commander

open System.Windows.Forms
open Resources
open Menu

type MainForm () as this = 
    inherit Form()

    do 
        this.SuspendLayout()
        
        this.Menu <- createMenu this

        if Settings.Default.WindowLocation.X <> -1 && Settings.Default.WindowLocation.Y <> -1 then
            this.StartPosition <- FormStartPosition.Manual
            this.Location <- Settings.Default.WindowLocation
        this.Size <- Settings.Default.WindowSize
        if Settings.Default.WindowState <> FormWindowState.Minimized then
            this.WindowState <- Settings.Default.WindowState

        this.KeyPreview <- true

        //brauser.Location <- System.Drawing.Point(0, 0)
        //brauser.Size <- Size(base.ClientSize.Width, base.ClientSize.Height)
        //brauser.TabIndex <- 0
        //brauser.Anchor <- AnchorStyles.Top ||| AnchorStyles.Bottom ||| AnchorStyles.Left ||| AnchorStyles.Right
        this.AutoScaleDimensions <- System.Drawing.SizeF(6.0f, 13.0f);
        this.AutoScaleMode <- AutoScaleMode.Font
        this.Name <- "MainForm"
        this.Text <- "Commander"
        this.ResumeLayout false
        //base.Controls.Add brauser
        //brauser.Focus() |> ignore
        //brauser.Load("https://www.caseris.de")

        let formClosing s e = 
            Settings.Default.WindowLocation <- 
                if this.WindowState = FormWindowState.Normal then 
                    this.Location 
                else 
                    this.RestoreBounds.Location
            Settings.Default.WindowSize <-
                if this.WindowState = FormWindowState.Normal then 
                    this.Size
                else
                    this.RestoreBounds.Size

            Settings.Default.Save()
            Settings.Default.WindowState <- this.WindowState

        this.FormClosing.AddHandler(FormClosingEventHandler(formClosing))