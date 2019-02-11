namespace Commander

open System.Windows.Forms
open Resources
open Menu

type MainForm () = 
    inherit Form()

    do 
        base.SuspendLayout()
        base.Menu <- createMenu ()
        base.StartPosition <- FormStartPosition.Manual
        //brauser.Location <- System.Drawing.Point(0, 0)
        //brauser.Size <- Size(base.ClientSize.Width, base.ClientSize.Height)
        //brauser.TabIndex <- 0
        //brauser.Anchor <- AnchorStyles.Top ||| AnchorStyles.Bottom ||| AnchorStyles.Left ||| AnchorStyles.Right
        base.AutoScaleDimensions <- System.Drawing.SizeF(6.0f, 13.0f);
        base.AutoScaleMode <- AutoScaleMode.Font
        base.Name <- "MainForm"
        base.Text <- "Commander"
        base.ResumeLayout false
        //base.Controls.Add brauser
        //brauser.Focus() |> ignore
        //brauser.Load("https://www.caseris.de")



