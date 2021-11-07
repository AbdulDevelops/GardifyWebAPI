import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, BehaviorSubject, from } from 'rxjs';
import { environment } from '../../environments/environment';
import { Router } from '@angular/router';
import { Role } from '../models/roles';
import { publishReplay, refCount } from 'rxjs/operators';
import { UserActionsFrom } from '../models/models';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-Type':  'application/json',
    'Accept': '*/*'
  })
};

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  redirectUrl: string;
  user = new BehaviorSubject(JSON.parse(localStorage.getItem('currentUser')));
  cookies = new BehaviorSubject(JSON.parse(localStorage.getItem('gardifycc')));
  cookiesAllowed$ = this.cookies.asObservable();
  user$ = this.user.asObservable();
  private userRoles: Observable<Role[]>;

  constructor(private http: HttpClient, private router: Router) { }

    login(creds: any, simpleMode: boolean): Observable<any> {
      return this.http.post<any>(`${environment.loginURL}/${simpleMode}`, creds );
    }

    register(user: any, simpleMode: boolean,newsLetterAbo:boolean): Observable<any> {
      return this.http.post<any>(`${environment.registerURL}/${simpleMode}/${newsLetterAbo}`, user);
    }

    convert(user: any,apiCallType:any): Observable<any> {
      return this.http.post<any>(`${environment.registerURL}/convert`, user);
    }

    registerDemo(user: any,apiCallType:any): Observable<any> {
      return this.http.post<any>(`${environment.registerURL}/registerDemo`, user, httpOptions);
    }
    forgot(email: any): Observable<any> {
      return this.http.post<any>(environment.forgotUrl, email, httpOptions);
    }
    sendConfMail(user:any):Observable<any>{
      return this.http.post<any>(environment.resendconfEmailUrl, user, httpOptions);
    }
    resetPassword(data: any): Observable<any> {
      return this.http.post<any>(`${environment.userURL}reset`, data, httpOptions);
    }

    updateUser(userId: any, data: any): Observable<any> {
      return this.http.put<any>(`${environment.updateUserURL}${userId}`, data, httpOptions);
    }

    confirmUser(userId: string, code: string): Observable<any> {
      return this.http.get<any>(`${environment.userURL}confirm?userId=${userId}&code=${code}`);
      //return this.http.post<any>(`${environment.userURL}confirm`, {userId, code}, httpOptions);
    }

    changeUserPassword(userId: any, data: any): Observable<any> {
      return this.http.post<any>(`${environment.updateUserURL}pass/${userId}`, data, httpOptions);
    }

    deleteUser(userId:any): Observable<any> {
      return this.http.delete<any>(`${environment.deleteUserURL}${userId}`, httpOptions);
    }

    /**
     * IS BEING DEPRECATED, USE getUserObservable()
     */
    getCurrentUser() {
      const user = JSON.parse(localStorage.getItem('currentUser'));
      return user ? user : null;
    }

    /**
     * replaces getCurrentUser()
     */
    getUserObservable(): Observable<any> {
      return this.user$;
    }

    updateUser$(userObj) {
      const newUser = {...this.user.value, ...userObj};
      localStorage.setItem('currentUser', JSON.stringify(newUser));
      this.user.next(newUser);
    }

    cookiesAllowed(): Observable<boolean> {
      return this.cookiesAllowed$;
    }

    allowCookies() {
      localStorage.setItem('gardifycc', 'true');
      this.cookies.next(true);
    }

    initGA() {
      // get GA bundle
      const GA = document.createElement('script');
      GA.src = 'https://www.googletagmanager.com/gtag/js?id=UA-70030524-12';
      document.body.appendChild(GA);
      // call gtag
      const caller = document.createElement('script');
      caller.innerHTML = "window.dataLayer = window.dataLayer || [];function gtag(){dataLayer.push(arguments);}gtag('js', new Date());gtag('config', 'UA-70030524-12');"
      document.body.appendChild(caller);
    }

    getUserName() {
      const user = JSON.parse(localStorage.getItem('currentUser'));
      return user ? user.Email : '';
    }

    getToken() {
      const user = JSON.parse(localStorage.getItem('currentUser'));
      if (user) {
        return user.Token;
      }
      return '';
    }

    logout(redirect: boolean = true) {
      // remove user from local storage to log user out
      localStorage.removeItem('currentUser');
      if (redirect) {
        this.router.navigate(['/']);
        setTimeout(() => {
          window.location.reload();  
        });
      }
    }

    roles(): Observable<Role[]> {
      if (this.userRoles) {
        return this.userRoles;
      }
      this.userRoles = this.http.get<Role[]>(`${environment.userURL}/roles`)
                                .pipe(publishReplay(1), refCount());
      return this.userRoles;
    }

    isAdmin() {
      const user = localStorage.getItem('currentUser');
      return user && JSON.parse(user).Admin;
    }

    isLoggedIn() {
      const user = localStorage.getItem('currentUser');
      if (!user) {
        return false;
      }
      return new Date(JSON.parse(user).ExpiresUtc) > new Date();
    }
    isTestAccount(){
      const user= localStorage.getItem('currentUser');
      if(!user){
        return false
      }else{
        if(this.getUserName().includes('@user.demo'))
        return true
      }
    }
}
