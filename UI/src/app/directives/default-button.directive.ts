import { Directive, Input, ElementRef, HostListener, HostBinding } from '@angular/core'

@Directive({
    selector: '[appDefaultButton]'
})
export class DefaultButtonDirective {

    @Input() defaultButton: ElementRef

    @HostListener("keydown", ['$event.which']) 
    onKeyDown(which: number) {
        if (which == 13)
            this.defaultButton.nativeElement.click()
    }

    @HostListener("focus")
    onFocus() { setTimeout(() => this.setDefault(true), 0) }

    @HostListener("focusout")
    onFocusOut() {
        this.setDefault(false)
    }

    private setDefault(set: boolean) {
        set 
            ? this.defaultButton.nativeElement.classList.add("isDefaultButton") 
            : this.defaultButton.nativeElement.classList.remove("isDefaultButton") 
    }

    constructor(private el: ElementRef) { }
}
