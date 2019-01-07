import { Directive, ElementRef, HostListener, Input } from '@angular/core'

@Directive({
    selector: '[appSelectAll]'
})
export class SelectAllDirective {

    @Input() selectNameOnly = false

    @HostListener("focus") 
    @HostListener("click") 
    onFocus() {
        if (this.initial) {
            this.initial = false
            if (this.selectNameOnly) {
                const input = this.el.nativeElement as HTMLInputElement
                var pos = input.value.lastIndexOf('.');
                const name = pos != -1 ? input.value.substring(0, pos) : input.value
                input.setSelectionRange(0, name.length)
                return
            }
        }

        (this.el.nativeElement as HTMLInputElement).select()
    }

    constructor(private el: ElementRef) { }

    private initial = true
}
