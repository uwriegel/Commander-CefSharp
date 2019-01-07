import { Component, ElementRef } from '@angular/core'
import { ThemesService } from './services/themes.service';

@Component({
    selector: 'app-root',
    host: {
		"[class.blue-theme]": "themes.theme == 'blue'",
        "[class.light-blue-theme]": "themes.theme == 'lightblue'",
        "[class.dark-theme]": "themes.theme == 'dark'",
        "[class.ubuntu-theme]": "themes.theme == 'ubuntu'"
	},
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css']
})
export class AppComponent {
    constructor(private themes: ThemesService, private appElement: ElementRef<HTMLElement>) {
        themes.initialize(appElement.nativeElement)
    }
}
