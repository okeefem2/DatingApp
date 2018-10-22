import { Injectable } from '@angular/core';
declare let alertify: any;

@Injectable({
  providedIn: 'root'
})
export class AlertService {

  public constructor() { }

  public confirm(message: string, okCallback: (choice: any) => {}): any {
    return alertify.confirm(message, okCallback);
  }

  public success(message: string): any {
    alertify.success(message);
  }

  public error(message: string): any {
    alertify.error(message);
  }

  public warning(message: string): any {
    alertify.warning(message);
  }

  public message(message: string): any {
    alertify.message(message);
  }
}
