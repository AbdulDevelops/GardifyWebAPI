import { Component, OnInit, OnDestroy } from '@angular/core';
import { ThemeProviderService } from 'src/app/services/themeProvider.service';
import { Subscription } from 'rxjs';
import { ActivatedRoute } from '@angular/router';

@Component({
  selector: 'app-unauthorized',
  templateUrl: './unauthorized.component.html',
  styleUrls: ['./unauthorized.component.css']
})
export class UnauthorizedComponent implements OnInit,OnDestroy {
  mode: string;
  subs= new Subscription();
  origin = '';
  customIntros = {
    'meingarten': 'Hier speichert Gardify die Pflanzen, die du als "deine" markierst. Daraus wird automatisch dein ganzjähriger To-Do-Kalender mit allen wichtigen Pflegetipps erstellt. Gleichzeitig siehst du, wie viele, welche und warum bestimmte Pflanzen in deinem Garten ökologisch wertvoll sind. Deine Daten speichern wir für dich, deswegen ist eine Anmeldung technisch erforderlich.',
    'wetter': 'Hier bekommst du standortgenaue Wetter-Infos, sowie Frost- und Sturmwarnungen für deine empfindlichen Pflanzen. Um diese Funktionen zu nutzen, ist eine Anmeldung technisch erforderlich, damit du die Wetterdaten für deinen Standort siehst.',
  };

  constructor(private tp: ThemeProviderService, private route: ActivatedRoute) { }

  ngOnDestroy(): void {
   this.subs.unsubscribe();
  }

  ngOnInit() {
    this.subs.add(this.tp.getTheme().subscribe(t => this.mode = t));
    this.subs.add(this.route.queryParams.subscribe(params => {
      this.origin = Object.keys(this.customIntros).includes(params.origin) ? params.origin : '';
    }));
  }

}
