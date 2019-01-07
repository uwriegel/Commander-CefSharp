import { Component, OnInit, Input } from '@angular/core'
import { DialogComponent as Dialog } from '../../dialog/dialog.component'
import { Buttons } from '../../enums/buttons.enum'

@Component({
    selector: 'test-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.css']
})
export class DialogComponent implements OnInit {

    @Input() 
    dialog: Dialog

    ngOnInit() { }

    onOk() { 
        this.dialog.buttons = Buttons.Ok
        this.dialog.text = "Das ist der OK-Dialog"
        const obs = this.dialog.show()
        obs.subscribe(result => {
            this.dialog.buttons = Buttons.OkCancel
            this.dialog.text = "Ja Wahnsinn, jetzt tut er"
            this.dialog.show()
        })
    }

    onOkCancel() { 
        this.dialog.buttons = Buttons.OkCancel
        this.dialog.text = "Das ist der OK-Cancel-Dialog"
        const obs = this.dialog.show()
        obs.subscribe(result => console.log(result))
    }

    onYesNoCancel() { 
        this.dialog.buttons = Buttons.YesNoCancel
        this.dialog.text = "Das ist der JaNeinCancel-Dialog"
        const obs = this.dialog.show()
        obs.subscribe(result => console.log(result))
    }
    
    onInputYesNoCancel() { 
        this.dialog.buttons = Buttons.YesNoCancel
        this.dialog.text = "Das ist der Input-JaNeinCancel-Dialog"
        this.dialog.withInput = true
        this.dialog.inputText = "Dateiname.ext"
        this.dialog.noIsDefault = true
        const obs = this.dialog.show()
        obs.subscribe(result => console.log(result))
    }

    onRename() { 
        this.dialog.buttons = Buttons.OkCancel
        this.dialog.text = "Möchtest Du die Datei umbenennen?"
        this.dialog.withInput = true
        this.dialog.inputText = "Dateiname.ext"
        this.dialog.selectNameOnly = true
        const obs = this.dialog.show()
        obs.subscribe(result => console.log(result))
    }
    

    onlongText() { 
        this.dialog.buttons = Buttons.YesNoCancel
        this.dialog.text = "Mitte des Jahres 1992 lernte Nas MC Serch von 3rd Bass kennen, der sein Manager wurde und ihm noch im selben Jahr einen Plattendeal bei Columbia Records verschaffen konnte. Nas machte sein Solodebüt mit der Single Halftime zum Soundtrack des Films Zebrahead."
        this.dialog.withInput = true
        const obs = this.dialog.show()
        obs.subscribe(result => console.log(result))
    }
}
