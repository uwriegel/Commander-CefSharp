import { Component, ViewChild, ElementRef } from '@angular/core'
import { trigger, transition, style, animate, state } from '@angular/animations'
import { Buttons } from '../enums/buttons.enum'
import { DialogResultValue } from '../enums/dialog-result-value.enum'
import { Subject, Observable } from '../../../node_modules/rxjs';

export interface DialogResult {
    result: DialogResultValue
    text?: string
}

@Component({
    selector: 'app-dialog',
    templateUrl: './dialog.component.html',
    styleUrls: ['./dialog.component.css'],
    animations: [
        trigger('fadeInOut', [
            transition('void => *', [
                style({
                    opacity: 0
                }), //style only for transition transition (after transiton it removes)
                animate("1s ease-out", style({
                    opacity: 1
                })) // the new state of the transition(after transiton it removes)
            ]),
            transition('* => void', [
                animate("300ms ease-in", style({
                    opacity: 0
                })) // the new state of the transition(after transiton it removes)
            ])
        ]),
        trigger('flyInOut', [
            state('in', style({
                opacity: 1,
                transform: 'translateX(0)'}
            )),
            transition('void => *', [
                style({
                    opacity: 0,
                    transform: 'translateX(-50%)'
                }),
                animate("200ms ease-out"),
            ]),
            transition('* => void', [
                animate("200ms ease-in" , style({
                    opacity: 0,
                    transform: 'translateX(50%)'
                }, ))
            ])
        ])            
    ]        
})
export class DialogComponent {

    @ViewChild("ok") ok: ElementRef
    @ViewChild("no") no: ElementRef
    @ViewChild("input") input: ElementRef
    text = ""
    buttons = Buttons.Ok
    withInput = false
    inputText = ""
    selectNameOnly = false
    noIsDefault = false

    show(): Observable<DialogResult> { 
        this.isShowing = true 
        setTimeout(() => {
            this.defaultButton = this.noIsDefault ? this.no : this.ok
            if (this.inputText)
                this.input.nativeElement.value = this.inputText
            this.withInput 
            ? this.input.nativeElement.focus() 
            : this.noIsDefault
                ? this.no.nativeElement.focus()
                : this.ok.nativeElement.focus()
        }, 0)

        this.dialogFinisher = new Subject<DialogResult>()
        return this.dialogFinisher
    }

    private onFocusIn(evt: Event) {
        if (!(evt.target as HTMLElement).closest(".dialogContainer"))
            this.ok.nativeElement.focus()
    }

    private onKeyDown(evt: KeyboardEvent) {
        if (evt.which == 27) // Esc
            this.close(DialogResultValue.Cancelled)
    }    

    private onKeyDownOk(evt: KeyboardEvent) {
        if (evt.which == 13 || evt.which == 32) // Return || space
            this.okClick()
    }

    private onKeyDownCancel(evt: KeyboardEvent) {
        if (evt.which == 13 || evt.which == 32) // Return || space
            this.cancelClick()
    }
    
    private onKeyDownNo(evt: KeyboardEvent) {
        if (evt.which == 13 || evt.which == 32) // Return || space
            this.noClick()
    }

    private okClick() { this.close(DialogResultValue.Ok) }

    private cancelClick() { this.close(DialogResultValue.Cancelled) }

    private noClick() { this.close(DialogResultValue.No) }

    private close(resultValue: DialogResultValue) {
        this.withInput = false
        this.text = ""
        this.inputText = ""
        this.buttons = Buttons.Ok
        this.selectNameOnly = false
        this.noIsDefault = false
        this.isShowing = false 
        const result = { 
            result: resultValue,
            text: this.input ? this.input.nativeElement.value : null
        }
        setTimeout(() => {
            this.dialogFinisher.next(result)
        }, 150)
    }

    private dialogFinisher: Subject<DialogResult>
    isShowing = false
    private defaultButton: ElementRef
}
