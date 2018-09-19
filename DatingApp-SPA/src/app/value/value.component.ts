import { Component, OnInit, OnDestroy } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-value',
  templateUrl: './value.component.html',
  styleUrls: ['./value.component.css']
})
export class ValueComponent implements OnInit, OnDestroy {
  public values: any[];
  private valuesObs: Subscription;
  public constructor(private http: HttpClient) { }

  public ngOnInit(): void {
    this.getValues();
  }

  public ngOnDestroy(): void {
    if (this.valuesObs) {
      this.valuesObs.unsubscribe();
    }
  }

  public getValues(): void {
    this.valuesObs = this.http.get('http://localhost:5000/api/values')
              .subscribe(
                (values: any[]) => {
                  this.values = values;
                },
                (error: any) => console.log(error));
  }

}
