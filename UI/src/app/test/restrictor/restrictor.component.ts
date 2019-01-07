import { Component, OnInit } from '@angular/core'
import { trigger, state, transition, style, animate } from '@angular/animations';

@Component({
    selector: 'test-restrictor',
    templateUrl: './restrictor.component.html',
    styleUrls: ['./restrictor.component.css'],
    animations: [
        trigger('flyInOut', [
            state('in', style({
                opacity: 0,
                width: '0%',
                height: '0px'    
            })),
            transition(":enter", [
                style({
                    opacity: 0,
                    width: '0%',
                    height: '0px'    
                }),
                animate("0.3s ease-out", style({
                    opacity: 1,
                    width: '70%',
                    height: '15px'
                }))
            ]),
            transition(':leave', [
                animate("0.3s ease-in", style({
                    opacity: 0,
                    width: '0%',
                    height: '0px'    
                }))
            ])
        ])
    ]    
})
export class RestrictorComponent implements OnInit {

    restrictMode = false
    items = [...Array(35).keys()]

    constructor() { }

    ngOnInit() {}

    onKeyDown(evt: KeyboardEvent) {
        this.restrictMode = evt.which != 27
    }
}
