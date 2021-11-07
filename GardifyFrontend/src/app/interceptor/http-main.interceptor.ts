import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';

import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { retryBackoff } from 'backoff-rxjs';
import { AuthService } from '../services/auth.service';
import { AlertService } from '../services/alert.service';

@Injectable() 
export class HttpMainInterceptor implements HttpInterceptor { 

    constructor(private auth: AuthService, private alertService: AlertService) {}
    
    intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
        request = request.clone({
            setHeaders: {
              Authorization: `Bearer ${this.auth.getToken()}`
            }
        });
        
        return next.handle(request).pipe(
                retryBackoff({
                    initialInterval: 15000, 
                    maxRetries: 5,
                    // retry only if error isnt 400/401/403/404/200/500
                    shouldRetry: (error) => !/200|400|401|403|404|500/.test(error.status)
                }),
                catchError((error) => {
                    switch (error.status) {
                        case 401: this.alertService.error('Du bist leider nicht authorisiert.'); break;
                        case 500: break;
                        default: this.alertService.error(error.error.Message || error.message); break;
                    }
                    return throwError(error);
                })
            );
    }
}
