import { Component, OnInit, Output, EventEmitter, ChangeDetectionStrategy } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  public registerForm: FormGroup;

  @Output() public register = new EventEmitter();
  @Output() public cancel = new EventEmitter();

  constructor(private formBuilder: FormBuilder) { }

  public ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: [
        '',
        [
          Validators.required,
          Validators.maxLength(32),
          Validators.minLength(8)
        ]
      ]
    });
  }

  public onSubmit(): void {
    if (this.registerForm.valid) {
      this.register.emit(this.registerForm.value);
    }
  }
}
