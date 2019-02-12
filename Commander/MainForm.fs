﻿namespace Commander

open System.Windows.Forms
open System.Drawing

open CefSharp.WinForms

open Browser
open Menu
open Resources

type MainForm () as this = 
    inherit Form()

    let browser = new ChromiumWebBrowser("")

    let mutable fullScreenForm: Form = null

    let toFullScreen () = 
        if fullScreenForm = null then
            fullScreenForm <- new Form()
            this.Controls.Remove browser
            fullScreenForm.Controls.Add browser
            fullScreenForm.WindowState <- FormWindowState.Normal
            fullScreenForm.FormBorderStyle <- FormBorderStyle.None
            browser.Size <- fullScreenForm.ClientSize
            fullScreenForm.Bounds <- Screen.PrimaryScreen.Bounds
            fullScreenForm.Show()

    do 
        this.SuspendLayout()

        let browserAccess = Browser.createBrowser browser
        
        this.Menu <- createMenu this browserAccess { ToFullScreen = toFullScreen }

        let location = Settings.Default.WindowLocation
        if location.X <> -1 && location.Y <> -1 then
            this.StartPosition <- FormStartPosition.Manual
            this.Location <- Settings.Default.WindowLocation
        this.Size <- Settings.Default.WindowSize
        if Settings.Default.WindowState <> FormWindowState.Minimized then
            this.WindowState <- Settings.Default.WindowState

        this.KeyPreview <- true

        browser.Location <- System.Drawing.Point(0, 0)
        browser.Size <- this.ClientSize
        browser.TabIndex <- 0
        browser.Anchor <- AnchorStyles.Top ||| AnchorStyles.Bottom ||| AnchorStyles.Left ||| AnchorStyles.Right
        this.AutoScaleDimensions <- System.Drawing.SizeF(6.0f, 13.0f);
        this.AutoScaleMode <- AutoScaleMode.Font
        this.Icon <- Resources.Kirk

        this.Name <- "MainForm"
        this.Text <- "Commander"
               
        this.Controls.Add browser
        browser.Load(Browser.getCommanderUrl ())
        
        this.ResumeLayout false

        let formClosing s e = 
            Settings.Default.WindowLocation <- 
                if this.WindowState = FormWindowState.Normal then 
                    this.Location 
                else 
                    let restoreBounds = this.RestoreBounds
                    restoreBounds.Location
            Settings.Default.WindowSize <-
                if this.WindowState = FormWindowState.Normal then 
                    this.Size
                else
                    let restoreBounds = this.RestoreBounds
                    restoreBounds.Size

            Settings.Default.Save()
            Settings.Default.WindowState <- this.WindowState

        this.FormClosing.AddHandler(FormClosingEventHandler(formClosing))

        browser.Focus() |> ignore


    