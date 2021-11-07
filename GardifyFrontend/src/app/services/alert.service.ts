import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { AlertType, Alert } from '../models/models';
import { Router, NavigationStart } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class AlertService {
  test = 'test alert';
  private subject = new Subject<Alert>();
    private keepAfterRouteChange = false;

    constructor() {
        // clear alert messages on route change unless 'keepAfterRouteChange' flag is true
        // router.events.subscribe(event => {
        //     if (event instanceof NavigationStart) {
        //         if (this.keepAfterRouteChange) {
        //             // only keep for a single route change
        //             this.keepAfterRouteChange = false;
        //         } else {
        //             // clear alert messages
        //             this.clear();
        //         }
        //     }
        // });
    }

    getAlert(): Observable<any> {
        return this.subject.asObservable();
    }

    success(message: string, keepAfterRouteChange = false) {
        this.alert(AlertType.Success, message, keepAfterRouteChange);
    }

    error(message: string, keepAfterRouteChange = false) {
        this.alert(AlertType.Error, message, keepAfterRouteChange);
    }

    alert(type: AlertType, message: string, keepAfterRouteChange = false) {
        if (message) {
            this.keepAfterRouteChange = keepAfterRouteChange;
            this.subject.next(<Alert>{ type: type, message: message });
        }
    }

    clear() {
        // clear alerts
        this.subject.next();
    }
}
