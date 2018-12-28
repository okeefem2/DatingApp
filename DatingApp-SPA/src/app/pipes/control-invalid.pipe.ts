import { Pipe, PipeTransform } from '@angular/core';
import { FormControl, ValidationErrors } from '@angular/forms';

@Pipe({
  name: 'controlInvalid'
})
export class ControlInvalidPipe implements PipeTransform {

  transform(errors: ValidationErrors, args?: {touched: string, errors: string[]}): boolean {
    debugger;
    if (args.touched && !!errors) {
      for (const error of args.errors) {
        if (!!errors[error]) {
          return true;
        }
      }
    }
    return false;
  }

}
