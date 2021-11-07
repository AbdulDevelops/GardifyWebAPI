import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from 'src/app/services/auth.service';

@Component({
  selector: 'app-tour',
  templateUrl: './tour.component.html',
  styleUrls: ['./tour.component.css']
})
export class TourComponent implements OnInit {
  @Input() step: {title: string, text: string, step: number};
  @Input() totalStep:number
  @Output() changeStep: EventEmitter<any> = new EventEmitter();
  @Output() closeTour: EventEmitter<any> = new EventEmitter();

  constructor(private auth: AuthService,private router: Router,) { }

  ngOnInit(): void {
  }

  onChangeStep(step) {
    this.changeStep.emit(step);
  }

  onCloseTour() {
    this.closeTour.emit();
  }

  // even steps and pflanze vorschlagen are offset on mobile
  get offset() {
    return this.step.step % 2 === 0 || this.step.step === 9;
  }

  get isSettings() {
    return this.step.step === 10;
  }

  get loggedIn() {
    return this.auth.isLoggedIn();
  }
  get isSocialPage() {
    return this.router.url.includes('/social') || this.router.url.includes('/gartenflora') ;
  }
}
